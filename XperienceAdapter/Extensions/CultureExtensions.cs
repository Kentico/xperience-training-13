using System;
using System.Globalization;
using System.Linq;

using CMS.SiteProvider;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

namespace XperienceAdapter.Extensions
{
    public static class CultureExtensions
    {
        public static SiteCulture? ToSiteCulture(this CultureInfo cultureInfo)
        {
            var siteName = SiteContext.CurrentSiteName;

            if (cultureInfo != null && CultureSiteInfoProvider.IsCultureOnSite(cultureInfo.Name, siteName))
            {
                var culture = CultureSiteInfoProvider
                    .GetSiteCultures(siteName)
                    .FirstOrDefault(culture => culture.CultureCode.Equals(cultureInfo.Name, StringComparison.InvariantCulture));

                if (culture != null)
                {
                    return SiteCultureRepository.MapDtoProperties(culture, siteName);
                }
            }

            return null;
        }
    }
}
