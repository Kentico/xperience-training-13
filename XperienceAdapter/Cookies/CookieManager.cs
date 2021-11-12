using System;
using System.Collections.Generic;
using System.Text;

using CMS.Helpers;

namespace XperienceAdapter.Cookies
{
    public class CookieManager : ICookieManager
    {
        public const string FirstReferrerCookieName = "FirstReferrer";

        private readonly ICurrentCookieLevelProvider _currentCookieLevelProvider;

        public bool VisitorCookiesEnabled => _currentCookieLevelProvider.GetCurrentCookieLevel() >= CookieLevel.Visitor;

        public bool IsDefaultCookieLevel => _currentCookieLevelProvider.GetCurrentCookieLevel() == _currentCookieLevelProvider.GetDefaultCookieLevel();

        public CookieManager(ICurrentCookieLevelProvider currentCookieLevelProvider)
        {
            _currentCookieLevelProvider = currentCookieLevelProvider ?? throw new ArgumentNullException(nameof(currentCookieLevelProvider));
        }

        public static string[] GetGoogleAnalyticsCookieNames(string gaPropertyId) => new string[14]
            {
                "_ga",
                "_gid",
                $"_gat_{gaPropertyId}",
                $"_dc_gtm_{gaPropertyId}",
                "AMP_TOKEN",
                $"_gac_{gaPropertyId}",
                "__utma",
                "__utmt",
                "__utmb",
                "__utmc",
                "__utmz",
                "__utmv",
                "__utmx",
                "__utmxx"
            };

        public static void RegisterCookieAtTheVisitorLevel(string cookieName) =>
            CookieHelper.RegisterCookie(cookieName, CookieLevel.Visitor);

        public static void RegisterCookieAtTheEssentialLevel(string cookieName) =>
            CookieHelper.RegisterCookie(cookieName, CookieLevel.Essential);
    }
}
