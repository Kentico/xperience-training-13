using CMS.ContactManagement;
using CMS.DataProtection;

using Common.Configuration;

using MedioClinic.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using XperienceAdapter.Localization;
using XperienceAdapter.Models;
using XperienceAdapter.Services;

namespace MedioClinic.Controllers
{
    public class CommunicationsController : BaseController
    {
        private const string MessageViewName = "SubscriptionMessage";

        private readonly IEmailCommunicationService _emailCommunicationService;

        private readonly IConsentInfoProvider _consentInfoProvider;

        private readonly IConsentAgreementService _consentAgreementService;

        public CommunicationsController(ILogger<BaseController> logger,
                                        IOptionsMonitor<XperienceOptions> optionsMonitor,
                                        IStringLocalizer<SharedResource> stringLocalizer,
                                        IEmailCommunicationService emailCommunicationService,
                                        IConsentInfoProvider consentInfoProvider,
                                        IConsentAgreementService consentAgreementService)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _emailCommunicationService = emailCommunicationService ?? throw new ArgumentNullException(nameof(emailCommunicationService));
            _consentInfoProvider = consentInfoProvider ?? throw new ArgumentNullException(nameof(consentInfoProvider));
            _consentAgreementService = consentAgreementService ?? throw new ArgumentNullException(nameof(consentAgreementService));
        }

        public async Task<IActionResult> SubscribeToNewsletter(NewsletterSubscriptionModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"A contact could not subscribe to a newsletter due to missing data. Newsletter GUID: {model.NewsletterGuid}, contact GUID: {model.ContactGuid}, email: {model.Email}.");

                return BadRequest(ModelState);
            }

            var result = await _emailCommunicationService.SubscribeToNewsletterAsync(model, cancellationToken, true);
            var handledResult = HandleSubscriptionResultStates(model, result);

            return StatusCode(handledResult.StatusCode, handledResult.ResponseText);
        }

        // GET: Communications/
        public async Task<IActionResult> Index(CancellationToken cancellationToken) => await GetIndexResultAsync(cancellationToken);

        // POST: Communications/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PageViewModel<NewsletterPreferenceViewModel> uploadModel, CancellationToken cancellationToken)
        {
            var contactGuid = ContactManagementContext.CurrentContact.ContactGUID;

            if (!ModelState.IsValid)
            {
                var contactData = $"Contact: {contactGuid}";
                var emailData = $"email: {uploadModel.Data.EmailViewModel.Email}";
                var consentData = $"consent: {uploadModel.Data.ConsentAgreed}";
                _logger.LogError($"Newsletter preferences could not be updated due to invalid data. {contactData}, {emailData}, {consentData}.");

                return await GetIndexResultAsync(cancellationToken, uploadModel.Data, Localize("General.InvalidInput.Message"), MessageType.Error);
            }

            if (!uploadModel.Data.ConsentAgreed && uploadModel.Data.Newsletters.Any(newsletter => newsletter.Subscribed))
            {
                return await GetIndexResultAsync(cancellationToken, uploadModel.Data, Localize("OnlineMarketing.ConsentRequired"), MessageType.Error);
            }
            else if (!uploadModel.Data.ConsentAgreed)
            {
                var consent = await _consentInfoProvider.GetAsync(_optionsMonitor.CurrentValue.OnlineMarketingOptions.NewsletterSubscriptionConsentName, cancellationToken);

                if (consent is not null)
                {
                    _consentAgreementService.Revoke(ContactManagementContext.CurrentContact, consent); 
                }
            }

            var handledSubscriptionResults = new List<(int StatusCode, string? ResponseText)>();
            var unsubscriptionResults = new List<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>>();
            var message = string.Empty;
            bool otherFailures = false;
            var messageType = MessageType.Info;

            foreach (var newsletter in uploadModel.Data.Newsletters)
            {
                if (contactGuid != Guid.Empty && newsletter.NewsletterGuid.HasValue)
                {
                    var model = new NewsletterSubscriptionModel
                    {
                        NewsletterGuid = newsletter.NewsletterGuid.Value,
                        ContactGuid = contactGuid,
                        Email = uploadModel.Data.EmailViewModel.Email,
                        FirstName = uploadModel.Data.FirstName,
                        LastName = uploadModel.Data.LastName,
                    };

                    if (newsletter.Subscribed)
                    {
                        var result = await _emailCommunicationService.SubscribeToNewsletterAsync(model, cancellationToken, false);
                        handledSubscriptionResults.Add(HandleSubscriptionResultStates(model, result));
                    }
                    else
                    {
                        unsubscriptionResults.Add(await _emailCommunicationService.UnsubscribeAsync(model, cancellationToken));
                    }
                }
                else
                {
                    otherFailures = true;
                    _logger.LogWarning($"Contact or newsletter data is missing when subscribing. Contact: {contactGuid}, newsletter: {newsletter.NewsletterGuid}.");
                }
            }

            if (handledSubscriptionResults.Any(result => result.StatusCode > 200) || unsubscriptionResults.Any(result => !result.Success) || otherFailures)
            {
                messageType = MessageType.Error;
                message = Localize("OnlineMarketing.NewsletterSubscriptionErrors");
            }
            else
            {
                message = Localize("OnlineMarketing.NewsletterSubscriptionSuccessfull");
            }

            return await GetIndexResultAsync(cancellationToken, null, message, messageType);
        }

        public IActionResult ConfirmSubscription(NewsletterSubscriptionConfirmationModel model)
        {
            var outputModel = CreateDefaultOutputModel("OnlineMarketing.SubscriptionConfirmation");

            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model?.SubscriptionHash) && string.IsNullOrEmpty(model?.DateTime))
                {
                    // An expected ping from the administration application, not logged as an error.
                    return Ok();
                }
                else
                {
                    _logger.LogError($"A contact could not confirm their newsletter subscription due to invalid data. Hash: {model.SubscriptionHash}, date: {model.DateTime}.");

                    return View(MessageViewName, outputModel);
                }
            }

            var result = _emailCommunicationService.ConfirmSubscription(model);

            switch (result.ResultState)
            {
                case NewsletterSubscriptionConfirmationResultState.Confirmed:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.SubscriptionConfirmed"];
                        outputModel.UserMessage.MessageType = MessageType.Info;

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.InvalidDateTime:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.InvalidDateTime"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.SubscriptionNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.SubscriptionNotFound"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.TimeExceeded:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.TimeExceeded"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.NotConfirmed:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.NotConfirmed"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.OtherErrors:
                default:
                    break;
            }

            return View(MessageViewName, outputModel);
        }

        public async Task<IActionResult> Unsubscribe(NewsletterUnsubscriptionModel model, CancellationToken cancellationToken)
        {
            var titleResourceKey = "OnlineMarketing.Unsubscription";
            var outputModel = CreateDefaultOutputModel(titleResourceKey);

            if (!ModelState.IsValid)
            {
                if (model?.NewsletterGuid == Guid.Empty
                    && model?.IssueGuid == Guid.Empty
                    && string.IsNullOrEmpty(model?.Email)
                    && model?.UnsubscribeFromAll == false)
                {
                    // An expected ping from the administration application, not logged as an error.
                    return Ok();
                }
                else
                {
                    _logger.LogError($"A contact could not unsubscribe from a newsletter due to invalid data. Email: {model?.Email}, newsletter GUID: {model?.NewsletterGuid}, issue GUID: {model?.IssueGuid}, hash: {model?.Hash}.");

                    return View(MessageViewName, outputModel);
                }
            }

            var result = await _emailCommunicationService.UnsubscribeAsync(model, cancellationToken);

            switch (result.ResultState)
            {
                case NewsletterUnsubscriptionResultState.Unsubscribed:
                case NewsletterUnsubscriptionResultState.AlreadyUnsubscribed:
                case NewsletterUnsubscriptionResultState.AlreadyUnsubscribedFromAll:
                    {
                        var metadata = new PageMetadata
                        {
                            Title = _stringLocalizer[titleResourceKey]
                        };

                        var formName = _optionsMonitor.CurrentValue.OnlineMarketingOptions.NewsletterUnsubscriptionFeedbackFormName;

                        var unsubscriptionModel = new MessageWithFeedbackModel
                        {
                            FormTitle = _stringLocalizer["OnlineMarketing.UnsubscriptionFeedback.Title"],
                            FormWidgetProperties = new Kentico.Forms.Web.Mvc.Widgets.FormWidgetProperties
                            {
                                SelectedForm = formName
                            }
                        };

                        var message = _stringLocalizer["OnlineMarketing.Unsubscribed"];
                        var unsubscriptionViewModel = GetPageViewModel(metadata, unsubscriptionModel, message: message);

                        return View("Unsubscribe", unsubscriptionViewModel);
                    }
                case NewsletterUnsubscriptionResultState.InvalidHash:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.InvalidHash"];

                        break;
                    }
                case NewsletterUnsubscriptionResultState.IssueNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.IssueNotFound"];

                        break;
                    }
                case NewsletterUnsubscriptionResultState.NewsletterNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["OnlineMarketing.NewsletterNotFound"];

                        break;
                    }
                case NewsletterUnsubscriptionResultState.OtherErrors:
                default:
                    break;
            }

            return View(MessageViewName, outputModel);
        }

        /// <summary>
        /// Builds a default view model initialized to a generic error message.
        /// </summary>
        /// <param name="titleResourceKey">Resource key pointing to a page title.</param>
        /// <returns>A page view model.</returns>
        private PageViewModel CreateDefaultOutputModel(string titleResourceKey)
        {
            var metadata = new PageMetadata
            {
                Title = _stringLocalizer[titleResourceKey]
            };

            var outputModel = GetPageViewModel(metadata, message: _stringLocalizer["General.UnknownError"]);
            outputModel.UserMessage.Display = true;
            outputModel.UserMessage.MessageType = MessageType.Error;

            return outputModel;
        }

        /// <summary>
        /// Draws the main page, either with database data, or, with form data (in case of validation errors).
        /// </summary>
        /// <param name="uploadModel">Upload view model.</param>
        /// <param name="message">User message.</param>
        /// <param name="messageType">Message type.</param>
        /// <returns>The implicit Index view.</returns>
        private async Task<IActionResult> GetIndexResultAsync(CancellationToken cancellationToken, NewsletterPreferenceViewModel? uploadModel = default, string? message = default, MessageType messageType = MessageType.Info)
        {
            PageViewModel viewModel;
            var contact = ContactManagementContext.CurrentContact;
            PageMetadata metadata = new PageMetadata
            {
                Title = Localize("MedioClinic.CommunicationPreferences.Title")
            };

            NewsletterPreferenceViewModel outputModel;

            if (uploadModel is not null)
            {
                outputModel = uploadModel;
            }
            else
            {
                if (contact is null)
                {
                    _logger.LogInformation($"A contact was not found when displaying the Communications preferences page.");
                    message = Localize("OnlineMarketing.ContactNotFound");
                    viewModel = GetPageViewModel(metadata, message: message);

                    return View(MessageViewName, viewModel);
                }

                var consentName = _optionsMonitor.CurrentValue.OnlineMarketingOptions.NewsletterSubscriptionConsentName;

                var consent = (await _consentInfoProvider
                    .Get()
                    .WhereEquals(ConsentInfo.TYPEINFO.CodeNameColumn, consentName)
                    .TopN(1)
                    .GetEnumerableTypedResultAsync())
                    .FirstOrDefault();

                if (consent is null)
                {
                    _logger.LogError($"A newsletter consent was not found. Consent name: {consentName}.");
                    message = Localize("OnlineMarketing.ConsentNotFound");
                    viewModel = GetPageViewModel(metadata, message: message);

                    return View(MessageViewName, viewModel);
                }

                var newsletters = await _emailCommunicationService.GetNewslettersForContactAsync(cancellationToken);
                var text = consent.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name);

                outputModel = new NewsletterPreferenceViewModel
                {
                    FirstName = contact.ContactFirstName,
                    LastName = contact.ContactLastName,
                    ConsentAgreed = _consentAgreementService.IsAgreed(contact, consent),
                    ConsentShortText = text?.ShortText,
                    Newsletters = newsletters
                };

                outputModel.EmailViewModel.Email = contact.ContactEmail;
            }

            viewModel = GetPageViewModel(metadata, outputModel, message: message, messageType: messageType);

            return View(viewModel);
        }

        /// <summary>
        /// Logs subscription results and returns status codes along with localized user messages.
        /// </summary>
        /// <param name="model">Subscription model.</param>
        /// <param name="result">Result to be processed.</param>
        /// <returns>A tuple of a status code and the user message.</returns>
        private (int StatusCode, string? ResponseText) HandleSubscriptionResultStates(NewsletterSubscriptionModel model, NewsletterSubscriptionResult<NewsletterSubscriptionResultState> result)
        {
            switch (result.ResultState)
            {
                case NewsletterSubscriptionResultState.Subscribed:
                    return (200, null);
                case NewsletterSubscriptionResultState.NewsletterNotFound:
                    {
                        _logger.LogError($"The newsletter {model.NewsletterGuid} was not found.");

                        return (400, _stringLocalizer["OnlineMarketing.NewsletterNotFound"]?.Value);
                    }
                case NewsletterSubscriptionResultState.ContactNotFound:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} was not found.");

                        return (400, _stringLocalizer["OnlineMarketing.ContactNotFound"]?.Value);
                    }
                case NewsletterSubscriptionResultState.ContactNotSet:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} could not be set.");

                        return (500, _stringLocalizer["OnlineMarketing.ContactNotSet"]?.Value);
                    }
                case NewsletterSubscriptionResultState.NotSubscribed:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} could not be subscribed to the newsletter {model.NewsletterGuid}.");

                        return (500, _stringLocalizer["OnlineMarketing.NotSubscribed"]?.Value);
                    }
                case NewsletterSubscriptionResultState.OtherErrors:
                default:
                    return (500, null);
            }
        }
    }
}
