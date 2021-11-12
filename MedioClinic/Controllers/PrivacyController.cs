using Business.Models;

using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;

using Core.Configuration;

using Microsoft.AspNetCore.Http;
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

namespace MedioClinic.Controllers
{
    public class PrivacyController : BaseController
    {
        private readonly IConsentInfoProvider _consentInfoProvider;

        private readonly IConsentAgreementService _consentAgreementService;

        private readonly ICurrentCookieLevelProvider _currentCookieLevelProvider;

        public PrivacyController(ILogger<BaseController> logger,
                              IOptionsMonitor<XperienceOptions> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              IConsentInfoProvider consentInfoProvider,
                              IConsentAgreementService consentAgreementService,
                              ICurrentCookieLevelProvider currentCookieLevelProvider)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _consentInfoProvider = consentInfoProvider ?? throw new ArgumentNullException(nameof(consentInfoProvider));
            _consentAgreementService = consentAgreementService ?? throw new ArgumentNullException(nameof(consentAgreementService));
            _currentCookieLevelProvider = currentCookieLevelProvider ?? throw new ArgumentNullException(nameof(currentCookieLevelProvider));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var marketingOptions = _optionsMonitor.CurrentValue?.OnlineMarketingOptions;
            var contact = ContactManagementContext.GetCurrentContact(false);
            var allConsents = new List<ConsentViewModel>();

            if (marketingOptions != null)
            {
                var cookieConsentName = marketingOptions.AnalyticalCookiesConsentName;

                if (!string.IsNullOrEmpty(cookieConsentName))
                {
                    allConsents.Add(await GetViewModelAsync(cookieConsentName, CookieLevel.Visitor, CookieLevel.Essential, contact, cancellationToken));
                }

                var formsDataConsentName = marketingOptions.FormsDataConsentName;

                if (!string.IsNullOrEmpty(formsDataConsentName))
                {
                    allConsents.Add(await GetViewModelAsync(formsDataConsentName, null, null, contact, cancellationToken));
                }
                var fileDataConsentName = marketingOptions.FileDataConsentName;

                if (!string.IsNullOrEmpty(fileDataConsentName))
                {
                    allConsents.Add(await GetViewModelAsync(fileDataConsentName, null, null, contact, cancellationToken));
                }

                var metadata = new Models.PageMetadata
                {
                    Title = Localize("OnlineMarketing.Privacy")
                };

                var viewModel = GetPageViewModel(metadata, allConsents);

                return View(viewModel);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // POST: Consent/Agree
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agree(string consentName, int? cookieLevel, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(consentName))
            {
                var consent = await _consentInfoProvider.GetAsync(consentName);

                if (consent != null)
                {
                    if (cookieLevel.HasValue)
                    {
                        // Set the cookie level before attempting to get/create the current contact.
                        _currentCookieLevelProvider.SetCurrentCookieLevel(cookieLevel.Value);
                    }

                    var contact = ContactManagementContext.GetCurrentContact(true);
                    _consentAgreementService.Agree(contact, consent);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        // POST: Consent/Revoke
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string consentName, int? revocationCookieLevel)
        {
            if (!string.IsNullOrEmpty(consentName))
            {
                var consent = await _consentInfoProvider.GetAsync(consentName);
                var contact = ContactManagementContext.GetCurrentContact(false);

                if (consent != null)
                {
                    _consentAgreementService.Revoke(contact, consent);
                    _currentCookieLevelProvider.SetCurrentCookieLevel(revocationCookieLevel ?? _currentCookieLevelProvider.GetDefaultCookieLevel());

                    return RedirectToAction(nameof(Index));
                }
            }

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        private async Task<ConsentViewModel> GetViewModelAsync(string codeName, int? cookieLevel, int? revocationCookieLevel, ContactInfo? contactInfo, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(codeName))
            {
                var consentInfo = await _consentInfoProvider.GetAsync(codeName, cancellationToken);

                if (consentInfo != null)
                {
                    var text = consentInfo.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name);

                    return new ConsentViewModel
                    {
                        CodeName = consentInfo.ConsentName,
                        ShortText = text?.ShortText,
                        FullText = text?.FullText,
                        CookieLevel = cookieLevel,
                        RevocationCookieLevel = revocationCookieLevel,
                        Agreed = contactInfo != null && _consentAgreementService.IsAgreed(contactInfo, consentInfo)
                    };
                }
            }

            return null!;
        }
    }
}
