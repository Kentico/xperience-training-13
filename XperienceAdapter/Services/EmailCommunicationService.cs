using CMS.Base;
using CMS.ContactManagement;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;

using Common.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XperienceAdapter.Models;

namespace XperienceAdapter.Services
{
    public class EmailCommunicationService : IEmailCommunicationService
    {
        private readonly ILogger<EmailCommunicationService> _logger;

        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly IContactInfoProvider _contactInfoProvider;

        private readonly IContactMergeService _contactMergeService;

        private readonly INewsletterInfoProvider _newsletterInfoProvider;

        private readonly ISubscriptionService _subscriptionService;

        private readonly ISubscriptionApprovalService _subscriptionApprovalService;

        private readonly ISiteService _siteService;

        private readonly IEmailHashValidator _emailHashValidator;

        private readonly IUnsubscriptionProvider _unsubscriptionProvider;

        private readonly IIssueInfoProvider _issueInfoProvider;

        public EmailCommunicationService(ILogger<EmailCommunicationService> logger,
                                IOptionsMonitor<XperienceOptions> optionsMonitor,
                                IContactInfoProvider contactInfoProvider,
                                IContactMergeService contactMergeService,
                                INewsletterInfoProvider newsletterInfoProvider,
                                ISubscriptionService subscriptionService,
                                ISubscriptionApprovalService subscriptionApprovalService,
                                ISiteService siteService,
                                IEmailHashValidator emailHashValidator,
                                IUnsubscriptionProvider unsubscriptionProvider,
                                IIssueInfoProvider issueInfoProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _contactInfoProvider = contactInfoProvider ?? throw new ArgumentNullException(nameof(contactInfoProvider));
            _contactMergeService = contactMergeService ?? throw new ArgumentNullException(nameof(contactMergeService));
            _newsletterInfoProvider = newsletterInfoProvider ?? throw new ArgumentNullException(nameof(newsletterInfoProvider));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _subscriptionApprovalService = subscriptionApprovalService ?? throw new ArgumentNullException(nameof(subscriptionApprovalService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _emailHashValidator = emailHashValidator ?? throw new ArgumentNullException(nameof(emailHashValidator));
            _unsubscriptionProvider = unsubscriptionProvider ?? throw new ArgumentNullException(nameof(unsubscriptionProvider));
            _issueInfoProvider = issueInfoProvider ?? throw new ArgumentNullException(nameof(issueInfoProvider));
        }

        public async Task<NewsletterSubscriptionResult<NewsletterSubscriptionResultState>> SubscribeToNewsletterAsync(NewsletterSubscriptionModel model, CancellationToken cancellationToken, bool allowOptIn)
        {
            var result = new NewsletterSubscriptionResult<NewsletterSubscriptionResultState>();
            var newsletter = await _newsletterInfoProvider.GetAsync(model.NewsletterGuid, _siteService.CurrentSite.SiteID, cancellationToken);
            var contact = await _contactInfoProvider.GetAsync(model.ContactGuid, cancellationToken);

            if (newsletter is null)
            {
                result.ResultState = NewsletterSubscriptionResultState.NewsletterNotFound;

                return result;
            }

            if (contact is null)
            {
                result.ResultState = NewsletterSubscriptionResultState.ContactNotFound;

                return result;
            }

            if (newsletter is not null && contact is not null)
            {
                var updateContact = false;

                updateContact = UpdateName(model, contact, updateContact);

                updateContact = await UpdateEmailOrMergeAsync(model, contact, updateContact, cancellationToken);

                if (updateContact)
                {
                    try
                    {
                        _contactInfoProvider.Set(contact);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        result.Errors.Add(ex.Message);
                        result.ResultState = NewsletterSubscriptionResultState.ContactNotSet;

                        return result;
                    }
                }

                var settings = new SubscribeSettings
                {
                    RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    SendConfirmationEmail = _optionsMonitor.CurrentValue?.OnlineMarketingOptions?.SendConfirmationEmails ?? false,
                    AllowOptIn = allowOptIn,
                    RemoveUnsubscriptionFromNewsletter = true
                };

                try
                {
                    _subscriptionService.Subscribe(contact, newsletter, settings);
                    result.Success = true;
                    result.ResultState = NewsletterSubscriptionResultState.Subscribed;

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    result.Errors.Add(ex.Message);
                    result.ResultState = NewsletterSubscriptionResultState.NotSubscribed;
                }
            }

            return result;
        }

        public NewsletterSubscriptionResult<NewsletterSubscriptionConfirmationResultState> ConfirmSubscription(NewsletterSubscriptionConfirmationModel model)
        {
            DateTime parsedDateTime = DateTime.MinValue;
            var approvalResult = ApprovalResult.Failed;
            var confirmationResult = new NewsletterSubscriptionResult<NewsletterSubscriptionConfirmationResultState, string>();

            if (!string.IsNullOrEmpty(model.DateTime) && !DateTimeUrlFormatter.TryParse(model.DateTime, out parsedDateTime))
            {
                confirmationResult.ResultState = NewsletterSubscriptionConfirmationResultState.InvalidDateTime;

                return confirmationResult;
            }

            try
            {
                approvalResult = _subscriptionApprovalService.ApproveSubscription(model.SubscriptionHash,
                    _optionsMonitor.CurrentValue?.OnlineMarketingOptions?.SendConfirmationEmails == true,
                    _siteService.CurrentSite.SiteName, parsedDateTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            switch (approvalResult)
            {
                case ApprovalResult.Success:
                case ApprovalResult.AlreadyApproved:
                    {
                        confirmationResult.Success = true;
                        confirmationResult.ResultState = NewsletterSubscriptionConfirmationResultState.Confirmed;

                        break;
                    }
                case ApprovalResult.NotFound:
                    {
                        confirmationResult.ResultState = NewsletterSubscriptionConfirmationResultState.SubscriptionNotFound;

                        break;
                    }
                case ApprovalResult.TimeExceeded:
                    {
                        confirmationResult.ResultState = NewsletterSubscriptionConfirmationResultState.TimeExceeded;

                        break;
                    }
                case ApprovalResult.Failed:
                default:
                    break;
            }

            return confirmationResult;
        }

        public async Task<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>> UnsubscribeAsync(NewsletterUnsubscriptionModel model, CancellationToken cancellationToken)
        {
            var result = new NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>();
            var email = model?.Email;

            if (string.IsNullOrWhiteSpace(email) || _emailHashValidator.ValidateEmailHash(model!.Hash, email))
            {
                result.ResultState = NewsletterUnsubscriptionResultState.InvalidHash;

                return result;
            }

            var issue = await _issueInfoProvider.GetAsync(model.IssueGuid, _siteService.CurrentSite.SiteID, cancellationToken);

            if (issue is null)
            {
                result.ResultState = NewsletterUnsubscriptionResultState.IssueNotFound;

                return result;
            }

            if (model.UnsubscribeFromAll)
            {
                if (!_unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email))
                {
                    try
                    {
                        _subscriptionService.UnsubscribeFromAllNewsletters(email);
                        result.Success = true;
                        result.ResultState = NewsletterUnsubscriptionResultState.Unsubscribed;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }
            else
            {
                result = await UnsubscribeInternalAsync(model, cancellationToken, issue.IssueID);
            }

            return result;
        }


        public async Task<List<NewsletterPreferenceModel>> GetNewslettersForContact()
        {
            var currentContact = ContactManagementContext.CurrentContact;

            if (currentContact is null)
            {
                return null!;
            }

            var allNewsletters = await _newsletterInfoProvider.Get().OnSite(_siteService.CurrentSite.SiteID).GetEnumerableTypedResultAsync();
            var output = new List<NewsletterPreferenceModel>();

            foreach (var newsletter in allNewsletters)
            {
                var model = new NewsletterPreferenceModel
                {
                    NewsletterGuid = newsletter.NewsletterGUID,
                    NewsletterDisplayName = newsletter.NewsletterDisplayName
                };

                model.Subscribed = _subscriptionService.IsMarketable(currentContact, newsletter) ? true : false;
                output.Add(model);
            }

            return output;
        }

        public async Task<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>> BulkUnsubscribeAsync(NewsletterSubscriptionModel model, CancellationToken cancellationToken)
        {
            var unsubscriptionModel = new NewsletterUnsubscriptionModel
            {
                Email = model.Email,
                NewsletterGuid = model.NewsletterGuid
            };

            return await UnsubscribeInternalAsync(unsubscriptionModel, cancellationToken);
        }

        /// <summary>
        /// Looks up a newsletter and unsubscribes a contact therefrom.
        /// </summary>
        /// <param name="model">Unsubscription model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="issueId">Issue ID.</param>
        /// <returns>A result with two possible states: unsubscribed and newsletter not found.</returns>
        private async Task<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>> UnsubscribeInternalAsync(NewsletterUnsubscriptionModel model,
                                                                                                                       CancellationToken cancellationToken,
                                                                                                                       int? issueId = null)
        {
            var result = new NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>
            {
                ResultState = NewsletterUnsubscriptionResultState.Unsubscribed,
                Success = true
            };

            var newsletter = await _newsletterInfoProvider.GetAsync(model.NewsletterGuid, _siteService.CurrentSite.SiteID, cancellationToken);

            if (newsletter is null)
            {
                result.ResultState = NewsletterUnsubscriptionResultState.NewsletterNotFound;
                result.Success = false;
            }
            else if (!_unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(model.Email, newsletter.NewsletterID))
            {
                try
                {
                    _subscriptionService.UnsubscribeFromSingleNewsletter(model.Email, newsletter.NewsletterID, issueId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if the contact object's first name and last name fields needs to be updated.
        /// </summary>
        /// <param name="model">An upload model.</param>
        /// <param name="contact">The contact.</param>
        /// <param name="updateContact">Result of other update inspections.</param>
        /// <returns>True if the object needs update.</returns>
        private bool UpdateName(NewsletterSubscriptionModel model, ContactInfo contact, bool updateContact)
        {
            if (!string.IsNullOrWhiteSpace(model.FirstName)
                && !contact.ContactFirstName.Equals(model.FirstName, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning($"The contact {model.ContactGuid} with a first name \"{contact.ContactFirstName}\" has subscribed to a newsletter {model.NewsletterGuid} with a first name \"{model.FirstName}\". The contact will be updated.");
                contact.ContactFirstName = model.FirstName;
                updateContact = true;
            }

            if (!string.IsNullOrWhiteSpace(model.LastName)
                && !contact.ContactFirstName.Equals(model.LastName, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning($"The contact {model.ContactGuid} with a last name \"{contact.ContactLastName}\" has subscribed to a newsletter {model.NewsletterGuid} with a last name \"{model.LastName}\". The contact will be updated.");
                contact.ContactLastName = model.LastName;
                updateContact = true;
            }

            return updateContact;
        }

        /// <summary>
        /// Determines if the contact object's email address needs to be updated or if the contact needs to be merged with another one.
        /// </summary>
        /// <param name="model">An upload model.</param>
        /// <param name="contact">The contact.</param>
        /// <param name="updateContact">Result of other update inspections.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the object needs update.</returns>
        private async Task<bool> UpdateEmailOrMergeAsync(NewsletterSubscriptionModel model, ContactInfo contact, bool updateContact, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(model?.Email))
            {
                var collidingContact = (await _contactInfoProvider
                    .Get()
                    .WhereEquals(nameof(ContactInfo.ContactEmail), model.Email)
                    .WhereNotEquals(ContactInfo.TYPEINFO.GUIDColumn, contact.ContactGUID)
                    .TopN(1)
                    .GetEnumerableTypedResultAsync(cancellationToken: cancellationToken))
                    .FirstOrDefault();

                if (collidingContact is not null && !string.IsNullOrEmpty(contact?.ContactEmail))
                {
                    _logger.LogWarning($"The contact {model.ContactGuid} with an email address {contact.ContactEmail} has subscribed to a newsletter {model.NewsletterGuid} with another email address {model.Email} belonging to another contact {collidingContact.ContactGUID}. They will be merged.");
                    _contactMergeService.MergeContacts(contact, collidingContact);
                }
                else if (!string.IsNullOrEmpty(contact?.ContactEmail))
                {
                    _logger.LogWarning($"The contact {model.ContactGuid} with an email address {contact.ContactEmail} has subscribed to a newsletter {model.NewsletterGuid} with another email address {model.Email}. The email address will be set.");
                    contact.ContactEmail = model?.Email;
                    updateContact = true;
                }
                else if (contact is not null && string.IsNullOrEmpty(contact?.ContactEmail))
                {
                    _logger.LogInformation($"The contact {model.ContactGuid} without any email address set subscribed to a newsletter {model.NewsletterGuid} with an email address {model.Email}. Their email address will be set.");
                    contact!.ContactEmail = model.Email;
                    updateContact = true;
                }
            }

            return updateContact;
        }
    }
}
