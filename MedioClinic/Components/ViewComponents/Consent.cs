#define no_suffix

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.DataProtection;
using Microsoft.Extensions.Options;
using Common.Configuration;
using CMS.ContactManagement;
using Business.Models;
using CMS.Helpers;

namespace MedioClinic.Components.ViewComponents
{
    public class Consent : ViewComponent
    {
        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly IConsentInfoProvider _consentInfoProvider;

        private readonly IConsentAgreementService _consentAgreementService;

        public Consent(IOptionsMonitor<XperienceOptions> optionsMonitor, IConsentInfoProvider consentInfoProvider, IConsentAgreementService consentAgreementService)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _consentInfoProvider = consentInfoProvider ?? throw new ArgumentNullException(nameof(consentInfoProvider));
            _consentAgreementService = consentAgreementService ?? throw new ArgumentNullException(nameof(consentAgreementService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var consentName = _optionsMonitor.CurrentValue?.OnlineMarketingOptions?.AnalyticalCookiesConsentName;

            if (!string.IsNullOrEmpty(consentName))
            {
                var consent = await _consentInfoProvider.GetAsync(consentName);
                var contact = ContactManagementContext.GetCurrentContact(false);

                // A missing consent is better to be logged at application startup, not during each request.
                if (consent != null)
                {
                    if (contact is null || !_consentAgreementService.IsAgreed(contact, consent))
                    {
                        var text = consent.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name);

                        var consentViewModel = new ConsentViewModel
                        {
                            Id = consent.ConsentID,
                            ShortText = text?.ShortText,
                        };

                        // Display the consent overlay bar.
                        return View("CookieBar", consentViewModel);
                    }
                }
            }

            return Content(string.Empty);
        }
    }
}
