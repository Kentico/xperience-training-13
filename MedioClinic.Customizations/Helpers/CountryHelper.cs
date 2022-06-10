using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.ContactManagement;
using CMS.Core;
using CMS.Globalization;
using CMS.Helpers;

using MedioClinic.Customizations.Repositories;

namespace MedioClinic.Customizations.Helpers
{
    public class CountryHelper
    {
        private const string CountryCacheDependencyStub = "CMS.Country";

        private const string StateCacheDependencyStub = "CMS.State";

        private static string CacheKeyStub = nameof(CountryHelper);

        public static bool ContactComesFromBigUsCity(ContactInfo contact)
        {
            var bigUsCityRepository = Service.Resolve<IBigUsCityRepository>();
            var usCountry = GetUsCountry();
            var contactState = GetContactState(contact, usCountry);
            var contactCity = contact?.ContactCity;

            if (!string.IsNullOrEmpty(contactCity) && !string.IsNullOrEmpty(contactState?.StateCode))
            {
                var matchingBigUsCities = bigUsCityRepository.GetByNameAndStateCode(contactCity, contactState.StateCode);

                return matchingBigUsCities?.Any() == true;
            }

            return false;
        }

        public static CountryInfo GetUsCountry()
        {
            var progressiveCache = Service.Resolve<IProgressiveCache>();

            return progressiveCache.Load(cacheSettings =>
            {
                var result = CountryInfo.Provider.Get()
                    .WhereEquals(nameof(CountryInfo.CountryThreeLetterCode), "USA")
                    .TopN(1)
                    .FirstOrDefault();

                cacheSettings.CacheDependency =
                    CacheHelper.GetCacheDependency($"{CountryCacheDependencyStub}|ById|{result.CountryID}");

                return result;
            }, GetCacheSettings($"{CacheKeyStub}|UsCountry"));
        }

        public static StateInfo GetContactState(ContactInfo contact, CountryInfo country)
        {
            var progressiveCache = Service.Resolve<IProgressiveCache>();

            if (contact?.ContactStateID > 0 == true && country != null)
            {
                var data = progressiveCache.Load(cacheSettings =>
                {
                    var result = StateInfo.Provider.Get()
                        .WhereEquals(nameof(StateInfo.StateID), contact?.ContactStateID)
                        .WhereEquals(nameof(StateInfo.CountryID), country?.CountryID)
                        .TopN(1)
                        .FirstOrDefault();

                    cacheSettings.CacheDependency =
                        CacheHelper.GetCacheDependency($"{StateCacheDependencyStub}|ById|{result.StateID}");

                    return result;
                }, GetCacheSettings($"{CacheKeyStub}|ContactState"));

                return data;
            }

            return null;
        }

        private static CacheSettings GetCacheSettings(string cacheKey) =>
            new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);
    }
}
