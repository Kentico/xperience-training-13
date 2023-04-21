using CMS.Base;
using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Newsletters;

using Common.Configuration;

using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedioClinic.Components.Widgets
{
    public class NewsletterSubscriptionViewComponent : ViewComponent
    {
        private readonly INewsletterInfoProvider _newsletterInfoProvider;

        private readonly ISiteService _siteService;

        private readonly ISubscriptionService _subscriptionService;

        private readonly IConsentInfoProvider _consentInfoProvider;

        private readonly IConsentAgreementService _consentAgreementService;

        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        public NewsletterSubscriptionViewComponent(
            INewsletterInfoProvider newsletterInfoProvider,
            ISiteService siteService,
            ISubscriptionService subscriptionService,
            IConsentInfoProvider consentInfoProvider,
            IConsentAgreementService consentAgreementService,
            IOptionsMonitor<XperienceOptions> optionsMonitor)
        {
            _newsletterInfoProvider = newsletterInfoProvider ?? throw new ArgumentNullException(nameof(newsletterInfoProvider));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _consentInfoProvider = consentInfoProvider ?? throw new ArgumentNullException(nameof(consentInfoProvider));
            _consentAgreementService = consentAgreementService ?? throw new ArgumentNullException(nameof(consentAgreementService));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<NewsletterSubscriptionProperties> componentViewModel)
        {
            var properties = componentViewModel?.Properties;
            var viewName = "~/Components/Widgets/_NewsletterSubscription.cshtml";
            var subscribedViewName = "~/Components/Widgets/_NewsletterSubscriptionSubscribed.cshtml";

            if (!string.IsNullOrEmpty(properties?.NewsletterGuid) && Guid.TryParse(properties.NewsletterGuid, out Guid newsletterGuid))
            {
                var allNewsletters = _newsletterInfoProvider.Get().OnSite(_siteService.CurrentSite.SiteID);
                var newsletter = (await allNewsletters.WithGuid(newsletterGuid).GetEnumerableTypedResultAsync())?.FirstOrDefault();
                var currentContact = ContactManagementContext.GetCurrentContact(false);

                if (newsletter is not null && currentContact is not null && _subscriptionService.IsMarketable(currentContact, newsletter))
                {
                    return View(subscribedViewName, newsletter.NewsletterDisplayName);
                }

                var consentName = _optionsMonitor.CurrentValue?.OnlineMarketingOptions?.NewsletterSubscriptionConsentName;
                var consent = await _consentInfoProvider.GetAsync(consentName);

                if (newsletter is not null && consent is not null)
                {
                    var cacheKeys = (await allNewsletters.GetEnumerableTypedResultAsync())?.Select(newsletter => $"Newsletter.Newsletter|ByGuid|{newsletter.NewsletterGUID}");
                    componentViewModel!.CacheDependencies.CacheKeys = cacheKeys?.ToList();
                    var consentIsAgreed = currentContact is not null ? _consentAgreementService.IsAgreed(currentContact, consent) : false;

                    var model = new NewsletterSubscriptionModel
                    {
                        NewsletterDisplayName = newsletter.NewsletterDisplayName,
                        NewsletterGuid = newsletter.NewsletterGUID,
                        ContactGuid = currentContact?.ContactGUID,
                        ContactEmail = currentContact?.ContactEmail,
                        ContactFirstName = currentContact?.ContactFirstName,
                        ContactLastName = currentContact?.ContactLastName,
                        ConsentIsAgreed = consentIsAgreed,
                        ConsentShortText = consent.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name)?.ShortText,
                        ThankYouMessageResourceKey = properties?.ThankYouMessageResourceKey,
                        RequireDoubleOptIn = newsletter.NewsletterEnableOptIn
                    };

                    return View(viewName, model);
                }
            }

            return View(viewName, new NewsletterSubscriptionModel());
        }
    }
}
