using Common.Configuration;

using MedioClinic.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
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

        public CommunicationsController(ILogger<BaseController> logger,
                                        IOptionsMonitor<XperienceOptions> optionsMonitor,
                                        IStringLocalizer<SharedResource> stringLocalizer,
                                        IEmailCommunicationService emailCommunicationService)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _emailCommunicationService = emailCommunicationService ?? throw new ArgumentNullException(nameof(emailCommunicationService));
        }

        public async Task<IActionResult> SubscribeToNewsletter(NewsletterSubscriptionModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"A contact could not subscribe to a newsletter due to missing data. Newsletter GUID: {model.NewsletterGuid}, contact GUID: {model.ContactGuid}, email: {model.Email}.");

                return BadRequest(ModelState);
            }

            var result = await _emailCommunicationService.SubscribeToNewsletterAsync(model, cancellationToken);

            switch (result.ResultState)
            {
                case NewsletterSubscriptionResultState.Subscribed:
                    return Ok();
                case NewsletterSubscriptionResultState.NewsletterNotFound:
                    {
                        _logger.LogError($"The newsletter {model.NewsletterGuid} was not found.");

                        return BadRequest(_stringLocalizer["MedioClinic.EmailMarketing.NewsletterNotFound"]?.Value);
                    }
                case NewsletterSubscriptionResultState.ContactNotFound:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} was not found.");

                        return BadRequest(_stringLocalizer["MedioClinic.EmailMarketing.ContactNotFound"]?.Value);
                    }
                case NewsletterSubscriptionResultState.ContactNotSet:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} could not be set.");

                        return StatusCode(500, _stringLocalizer["MedioClinic.EmailMarketing.ContactNotSet"]?.Value);
                    }
                case NewsletterSubscriptionResultState.NotSubscribed:
                    {
                        _logger.LogError($"The contact {model.ContactGuid} could not be subscribed to the newsletter {model.NewsletterGuid}.");

                        return StatusCode(500, _stringLocalizer["MedioClinic.EmailMarketing.NotSubscribed"]?.Value);
                    }
                case NewsletterSubscriptionResultState.OtherErrors:
                default:
                    return StatusCode(500);
            }
        }

        public IActionResult ConfirmSubscription(NewsletterSubscriptionConfirmationModel model)
        {
            var outputModel = CreateDefaultOutputModel("MedioClinic.EmailMarketing.SubscriptionConfirmation");

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
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.SubscriptionConfirmed"];
                        outputModel.UserMessage.MessageType = MessageType.Info;

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.InvalidDateTime:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.InvalidDateTime"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.SubscriptionNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.SubscriptionNotFound"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.TimeExceeded:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.TimeExceeded"];

                        break;
                    }
                case NewsletterSubscriptionConfirmationResultState.NotConfirmed:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.NotConfirmed"];

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
            var outputModel = CreateDefaultOutputModel("MedioClinic.EmailMarketing.Unsubscription");

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
                _logger.LogError($"A contact could not unsubscribe from a newsletter due to invalid data. Email: {model.Email}, newsletter GUID: {model.NewsletterGuid}, issue GUID: {model.IssueGuid}, hash: {model.Hash}.");

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
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.Unsubscribed"];
                        outputModel.UserMessage.MessageType = MessageType.Info;

                        break;
                    }
                case NewsletterUnsubscriptionResultState.InvalidHash:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.InvalidHash"];

                        break;
                    }
                case NewsletterUnsubscriptionResultState.IssueNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.IssueNotFound"];

                        break;
                    }
                case NewsletterUnsubscriptionResultState.NewsletterNotFound:
                    {
                        outputModel.UserMessage.Message = _stringLocalizer["MedioClinic.EmailMarketing.NewsletterNotFound"];

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
    }
}
