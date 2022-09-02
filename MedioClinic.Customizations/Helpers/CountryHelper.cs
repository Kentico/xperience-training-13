using System.Linq;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;

using MedioClinic.Customizations.Repositories;

namespace MedioClinic.Customizations.Helpers
{
    public class CountryHelper
    {
        private const string CountryCacheDependencyStub = "CMS.Country";

        private const string StateCacheDependencyStub = "CMS.State";

        public static bool ContactComesFromBigUsCity(ContactInfo contact)
        {
            if (contact is null)
            {
                throw new System.ArgumentNullException(nameof(contact));
            }

            if (!IsUsState(contact.ContactStateID))
            {
                return false;
            }

            var bigUsCityRepository = Service.Resolve<IBigUsCityRepository>();
            var contactState = GetContactState(contact);
            var contactCity = contact?.ContactCity;

            if (!string.IsNullOrEmpty(contactCity) && !string.IsNullOrEmpty(contactState?.StateCode))
            {
                var matchingBigUsCities = bigUsCityRepository.GetByNameAndStateCode(contactCity, contactState.StateCode);

                return matchingBigUsCities?.Any() == true;
            }

            return false;
        }

        public static CountryInfo GetUsCountry() => CountryInfo.Provider.Get("USA");

        public static StateInfo GetContactState(ContactInfo contact)
        {
            return contact?.ContactStateID > 0 == true
                ? StateInfo.Provider.Get(contact.ContactStateID)
                : null;
        }

        public static ObjectQuery<StateInfo> GetUsStates()
        {
            var usCountry = GetUsCountry();

            var usStates = CacheHelper.Cache(cs =>
            { 
                var result = StateInfo.Provider.Get().WhereEquals(nameof(StateInfo.CountryID), usCountry.CountryID);
                cs.CacheDependency = CacheHelper.GetCacheDependency("CMS.State|All");

                return result;
            },
            new CacheSettings(10, $"{nameof(CountryHelper)}|{nameof(GetUsStates)}"));

            return usStates;
        }

        public static bool IsUsState(int stateId) =>
            StateInfo.Provider.Get(stateId)?.CountryID == GetUsCountry().CountryID;
    }
}
