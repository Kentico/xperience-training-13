using Business.Models;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Helpers;

using Common.Configuration;
using Common.Extensions;

using MedioClinic.Customizations.Cookies;
using MedioClinic.Customizations.DataProtection.Consent;

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

        private readonly IConsentCookieLevelInfoProvider _consentCookieLevelInfoProvider;

        private readonly ICookieManager _cookieManager;

        private readonly IConsentManager _consentManager;

        public PrivacyController(ILogger<BaseController> logger,
                              IOptionsMonitor<XperienceOptions> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              IConsentInfoProvider consentInfoProvider,
                              IConsentAgreementService consentAgreementService,
                              ICurrentCookieLevelProvider currentCookieLevelProvider,
                              IConsentCookieLevelInfoProvider consentCookieLevelInfoProvider,
                              ICookieManager cookieManager,
                              IConsentManager consentManager)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _consentInfoProvider = consentInfoProvider ?? throw new ArgumentNullException(nameof(consentInfoProvider));
            _consentAgreementService = consentAgreementService ?? throw new ArgumentNullException(nameof(consentAgreementService));
            _currentCookieLevelProvider = currentCookieLevelProvider ?? throw new ArgumentNullException(nameof(currentCookieLevelProvider));
            _consentCookieLevelInfoProvider = consentCookieLevelInfoProvider ?? throw new ArgumentNullException(nameof(consentCookieLevelInfoProvider));
            _cookieManager = cookieManager ?? throw new ArgumentNullException(nameof(cookieManager));
            _consentManager = consentManager ?? throw new ArgumentNullException(nameof(consentManager));
        }

        // GET: Privacy/
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var contact = ContactManagementContext.GetCurrentContact(false);
            var cookieConsents = new List<ConsentViewModel>();
            var otherConsents = new List<ConsentViewModel>();
            var consentsWithCookieLevels = GetConsentCookieLevels();

            // Prepare an in-memory consent representing the default cookie level.
            cookieConsents.Add(new ConsentViewModel
            {
                Agreed = _cookieManager.IsDefaultCookieLevel,
                Name = _stringLocalizer["OnlineMarketing.EssentialCookies"],
                CookieLevel = _currentCookieLevelProvider.GetDefaultCookieLevel(),
                Id = -1
            });

            // Prepare other consents related to cookies.
            foreach (var consentCookieLevel in consentsWithCookieLevels.OrderBy(CookieManager.CookieLevelColumnName))
            {
                cookieConsents.AddIfNotNull(await GetViewModelAsync(consentCookieLevel.ConsentID, consentCookieLevel.CookieLevel, contact, cancellationToken));
            }

            // Prepare consents not related to cookies.
            var consentsWithoutCookieLevels = _consentInfoProvider.Get()
                .WhereNotIn(ConsentManager.ConsentIdColumnName, consentsWithCookieLevels.Select(consent => consent.ConsentID).ToList());

            foreach (var otherConsent in consentsWithoutCookieLevels)
            {
                otherConsents.AddIfNotNull(await GetViewModelAsync(otherConsent.ConsentID, null, contact, cancellationToken));
            }

            var metadata = new Models.PageMetadata
            {
                Title = Localize("OnlineMarketing.Privacy")
            };

            var allConsents = (cookieConsents, otherConsents);

            var viewModel = GetPageViewModel(metadata, allConsents);

            return View(viewModel);
        }

        // POST: Consent/SetCookieLevel/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetCookieLevel(int consentId, string? returnUrl = null)
        {
            var consentCookieLevels = GetConsentCookieLevels();

            // Get the cookie level to be set.
            var cookieLevelToSet = consentCookieLevels?.TypedResult?.FirstOrDefault(cookieLevel => cookieLevel.ConsentID == consentId)?.CookieLevel
                ?? _currentCookieLevelProvider.GetDefaultCookieLevel();

            var existingContact = ContactManagementContext.GetCurrentContact(false);

            // Get ALL consents with SAME or LOWER cookie level and AGREE to them.
            var smallerOrEqualCookieLevelConsents = _consentManager.GetAllConsentsWithSameOrLowerCookieLevel(cookieLevelToSet);

            // Set the cookie level before attempting to get/create the current contact.
            _currentCookieLevelProvider.SetCurrentCookieLevel(cookieLevelToSet);

            // Try to use the existing contact first as the new cookie level may not allow you to create one.
            var contact = existingContact ?? ContactManagementContext.GetCurrentContact(_cookieManager.VisitorCookiesEnabled);

            if (contact != null)
            {
                // First agree to the required consents to allow ConsentManager.RevokeConsentAgreementHandler() find a proper baseline cookie level.
                foreach (var consentToAgree in smallerOrEqualCookieLevelConsents)
                {
                    _consentAgreementService.Agree(contact, consentToAgree);
                }

                // Get AGREED consents with HIGHER cookie level and REVOKE them.
                var agreedConsentsWithHigherCookieLevel = _consentManager.GetAgreedConsentsWithHigherCookieLevel(contact, cookieLevelToSet);

                // When revoking, the handler continuously lowers the cookie level until it eventually finds the baseline level
                // (even through agreedConsentsWithHigherCookieLevel is not ordered by the cookie level).
                foreach (var consentToRevoke in agreedConsentsWithHigherCookieLevel)
                {
                    _consentAgreementService.Revoke(contact, consentToRevoke);
                }
            }

            return RedirectIfPossible(returnUrl);
        }

        // POST: Consent/Agree/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agree(int consentId, string? returnUrl = null)
        {
            var consent = await _consentInfoProvider.GetAsync(consentId);
            var contact = ContactManagementContext.GetCurrentContact(false);

            // The current cookie level may prevent us from getting the contact
            // and no changes to the cookie level will be done,
            // hence only the existing contact can potentially be used.
            if (consent != null)
            {
                if (contact != null)
                {
                    // Agree to the consent that has no cookie level assigned.
                    _consentAgreementService.Agree(contact, consent);
                }

                return RedirectIfPossible(returnUrl);
            }

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        // POST: Consent/Revoke
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(int consentId)
        {
            var consent = await _consentInfoProvider.GetAsync(consentId);
            var contact = ContactManagementContext.GetCurrentContact(false);

            if (consent != null)
            {
                if (contact != null)
                {
                    // Revoke the consent that has no cookie level assigned.
                    _consentAgreementService.Revoke(contact, consent);
                }

                return RedirectToAction(nameof(Index));
            }

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        private IActionResult RedirectIfPossible(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private ObjectQuery<ConsentCookieLevelInfo> GetConsentCookieLevels() =>
            _consentCookieLevelInfoProvider.Get()
                .WhereGreaterThan(CookieManager.CookieLevelColumnName, CookieManager.NullIntegerValue);

        private async Task<ConsentViewModel> GetViewModelAsync(int consentId, int? cookieLevel, ContactInfo? contactInfo, CancellationToken cancellationToken)
        {
            var consentInfo = await _consentInfoProvider.GetAsync(consentId, cancellationToken);

            if (consentInfo != null)
            {
                var text = consentInfo.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name);

                return new ConsentViewModel
                {
                    Id = consentInfo.ConsentID,
                    CodeName = consentInfo.ConsentName,
                    Name = consentInfo.ConsentDisplayName,
                    ShortText = text?.ShortText,
                    FullText = text?.FullText,
                    CookieLevel = cookieLevel,
                    Agreed = contactInfo != null && _consentAgreementService.IsAgreed(contactInfo, consentInfo)
                };
            }

            return null!;
        }
    }
}
