#define no_suffix

using Business.Models;

using CMS.Helpers;

using Core.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Components.ViewComponents
{
    public class ExternalAnalyticalTools : ViewComponent
    {
        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly ICurrentCookieLevelProvider _currentCookieLevelProvider;

        public bool IsDefaultCookieLevel => _currentCookieLevelProvider.GetCurrentCookieLevel() == _currentCookieLevelProvider.GetDefaultCookieLevel();

        public bool VisitorCookiesEnabled => _currentCookieLevelProvider.GetCurrentCookieLevel() == CookieLevel.Visitor;

        public ExternalAnalyticalTools(IOptionsMonitor<XperienceOptions> optionsMonitor, ICurrentCookieLevelProvider currentCookieLevelProvider)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _currentCookieLevelProvider = currentCookieLevelProvider ?? throw new ArgumentNullException(nameof(currentCookieLevelProvider));
        }

        public IViewComponentResult Invoke(TagKind tagKind)
        {
            var marketingOptions = _optionsMonitor.CurrentValue?.OnlineMarketingOptions;
            var googleTagManagerId = marketingOptions?.GoogleTagManagerId;
            var googleAnalyticsId = marketingOptions?.GoogleAnalyticsPropertyId;

            if (!string.IsNullOrEmpty(googleTagManagerId) && !string.IsNullOrEmpty(googleAnalyticsId))
            {
                switch (tagKind)
                {
                    case TagKind.HeadScript:
                        if (VisitorCookiesEnabled)
                        {
                            return View("HeadScript", googleTagManagerId);
                        }
                        break;
                    case TagKind.Noscript:
                        if (VisitorCookiesEnabled)
                        {
                            return View("Noscript", googleTagManagerId);
                        }
                        break;
                    case TagKind.Disabler:
                        if (IsDefaultCookieLevel)
                        {
                            return View("Disabler", googleAnalyticsId);
                        }
                        break;
                    default:
                        return Content(string.Empty);
                }
            }

            return Content(string.Empty);
        }

        public enum TagKind
        {
            HeadScript,
            Noscript,
            Disabler
        }
    }
}
