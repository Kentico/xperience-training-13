using System;
using System.Collections.Generic;
using System.Text;

using CMS.Helpers;

namespace XperienceAdapter.Cookies
{
    public static class CookieManager
    {
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
    }
}
