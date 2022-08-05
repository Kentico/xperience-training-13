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

        public static CountryInfo GetUsCountry() => CountryInfo.Provider.Get("USA");

        public static StateInfo GetContactState(ContactInfo contact, CountryInfo country)
        {
            return contact?.ContactStateID > 0 == true && country != null
                ? StateInfo.Provider.Get(contact.ContactStateID)
                : null;
        }

        private static CacheSettings GetCacheSettings(string cacheKey) =>
            new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);
    }
}
