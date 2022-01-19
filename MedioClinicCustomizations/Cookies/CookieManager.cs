using CMS;
using CMS.Core;
using CMS.Helpers;

using MedioClinicCustomizations.Cookies;
using MedioClinicCustomizations.DataProtection.ConsentCustomizations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: RegisterImplementation(typeof(ICookieManager), typeof(CookieManager))]

namespace MedioClinicCustomizations.Cookies
{
    public class CookieManager : CookieHelper, ICookieManager
    {
        public const string FirstReferrerCookieName = "FirstReferrer";

        public const string GoogleTagManagerId = "GTM-XXXX";

        public const string GoogleAnalyticsPropertyId = "UA-XXXXX-X";

        public const string ExampleAdvertisingCookieName = "ExampleAdvertisingCookie";

        public const string CookieLevelColumnName = nameof(ConsentCookieLevelInfo.CookieLevel);

        public const int NullIntegerValue = -10000;

        private readonly ICurrentCookieLevelProvider _currentCookieLevelProvider;

        /// <summary>
        /// Gets names of Google Analytics cookies with the Google Property ID in them.
        /// </summary>
        /// <param name="gaPropertyId">Google Property ID.</param>
        /// <returns>An array of cookie names.</returns>
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


        public bool VisitorCookiesEnabled => _currentCookieLevelProvider.GetCurrentCookieLevel() >= CMS.Helpers.CookieLevel.Visitor;

        public bool IsDefaultCookieLevel => _currentCookieLevelProvider.GetCurrentCookieLevel() == _currentCookieLevelProvider.GetDefaultCookieLevel();

        public CookieManager()
        {
            _currentCookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();
        }

        public IEnumerable<string> AllCookieNames => GetGoogleAnalyticsCookieNames(GoogleAnalyticsPropertyId)
                .Concat(new[] { FirstReferrerCookieName, ExampleAdvertisingCookieName });


        public IEnumerable<Cookie> GetCookies() => GetCookies(AllCookieNames);


        public IEnumerable<Cookie> GetCookies(IEnumerable<string> cookieNames)
        {
            if (cookieNames != null)
            {
                return cookieNames
                    .Select(cookie => new Cookie
                    {
                        Name = cookie,
                        Level = MapCookieLevel(cookie)
                    });
            }

            return Enumerable.Empty<Cookie>();
        }

        public IEnumerable<CookieLevel> GetCookieLevels() => GetCookieLevels(AllCookieNames);

        public IEnumerable<CookieLevel> GetCookieLevels(IEnumerable<string> cookieNames)
        {
            var levelsOfCookies = GetCookies(cookieNames).Select(cookie => cookie.Level);
            var systemLevels = GetSystemCookieLevels();

            return levelsOfCookies
                .Concat(systemLevels)
                .Distinct()
                .OrderBy(level => level.Level);
        }

        public IEnumerable<CookieLevel> GetSystemCookieLevels() =>
            typeof(CMS.Helpers.CookieLevel)
                .GetFields()
                .Where(
                    field => field.IsLiteral
                    && !field.IsInitOnly
                    && field.FieldType == typeof(int))
                .Select(field => new CookieLevel
                {
                    Name = field.Name,
                    Level = (int)field.GetRawConstantValue()
                })
                .OrderBy(level => level.Level);

        public static void RegisterOnlineMarketingCookies()
        {
            foreach (var cookieName in GetGoogleAnalyticsCookieNames(GoogleAnalyticsPropertyId))
            {
                RegisterCookieAtTheVisitorLevel(cookieName);
            }

            RegisterCookieAtTheEssentialLevel(FirstReferrerCookieName);

            RegisterCookie(ExampleAdvertisingCookieName, 500);
        }

        public static void RegisterCookieAtTheVisitorLevel(string cookieName) =>
            RegisterCookie(cookieName, CMS.Helpers.CookieLevel.Visitor);

        public static void RegisterCookieAtTheEssentialLevel(string cookieName) =>
            RegisterCookie(cookieName, CMS.Helpers.CookieLevel.Essential);

        private CookieLevel MapCookieLevel(string cookieName)
        {
            var retrievedLevel = GetCookieLevelInternal(cookieName);

            return GetSystemCookieLevels()
                        .FirstOrDefault(level => level.Level == retrievedLevel)
                        ?? new CookieLevel { Level = retrievedLevel };
        }
    }
}
