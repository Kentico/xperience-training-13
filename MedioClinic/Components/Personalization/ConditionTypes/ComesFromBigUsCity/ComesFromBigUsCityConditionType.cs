using System;
using System.Linq;
using Microsoft.Extensions.Localization;

using CMS.ContactManagement;
using CMS.Core;
using CMS.Globalization;
using CMS.Helpers;
using Kentico.PageBuilder.Web.Mvc.Personalization;

using XperienceAdapter.Localization;
using Business.Repositories;

namespace MedioClinic.Personalization
{
    public class ComesFromBigUsCityConditionType : ConditionType
    {
        private const string CountryCacheDependencyStub = "CMS.Country";

        private const string StateCacheDependencyStub = "CMS.State";

        private static string CacheKeyStub = $"{nameof(ComesFromBigUsCityConditionType)}|{nameof(Evaluate)}";

        private readonly IProgressiveCache _progressiveCache;

        private readonly IBigUsCityRepository _bigUsCityRepository;

        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public bool IsForBigCities { get; set; }

        public override string VariantName
        {
            get => IsForBigCities 
                ? _stringLocalizer["MedioClinic.PersonalizationCondition.ComesFromBigUsCity.IsForBigCities"] 
                : _stringLocalizer["MedioClinic.PersonalizationCondition.ComesFromBigUsCity.IsForOtherCities"];
            set
            {
                // No need to set automatically generated variant name
            }
        }

        public ComesFromBigUsCityConditionType()
        {
            _progressiveCache = Service.Resolve<IProgressiveCache>();
            _bigUsCityRepository = Service.Resolve<IBigUsCityRepository>();
            _stringLocalizer = Service.Resolve<IStringLocalizer<SharedResource>>();
        }

        public override bool Evaluate()
        {
            var currentContact = ContactManagementContext.GetCurrentContact(false);

            if (IsForBigCities && currentContact != null)
            {
                var usCountry = GetUsCountry();
                var contactState = GetContactState(currentContact, usCountry);
                var contactCity = currentContact?.ContactCity;

                if (!string.IsNullOrEmpty(contactCity) && !string.IsNullOrEmpty(contactState?.StateCode))
                {
                    var matchingBigUsCities = _bigUsCityRepository.GetByNameAndStateCode(contactCity, contactState.StateCode);

                    return matchingBigUsCities?.Any() == true;
                }
            }

            return false;
        }

        private CountryInfo GetUsCountry() =>
            _progressiveCache.Load(cacheSettings =>
                {
                    var result = CountryInfo.Provider.Get()
                        .WhereEquals(nameof(CountryInfo.CountryThreeLetterCode), "USA")
                        .TopN(1)
                        .FirstOrDefault();

                    cacheSettings.CacheDependency =
                        CacheHelper.GetCacheDependency($"{CountryCacheDependencyStub}|ById|{result.CountryID}");

                    return result;
                }, GetCacheSettings($"{CacheKeyStub}|UsCountry"));

        private StateInfo GetContactState(ContactInfo contact, CountryInfo country)
        {
            if (contact?.ContactStateID > 0 == true && country != null)
            {
                var data = _progressiveCache.Load(cacheSettings =>
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

            return null!;
        }

        private static CacheSettings GetCacheSettings(string cacheKey) =>
            new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);
    }
}
