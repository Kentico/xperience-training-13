using CMS.ContactManagement;
using CMS.Core;
using CMS.CustomTables.Types.MedioClinic;
using CMS.DataEngine;
using CMS.Globalization;

using MedioClinic.Customizations.Helpers;
using MedioClinic.Customizations.Repositories;

using System;

namespace MedioClinic.Customizations.Macros
{
    public class ComesFromBigUsCityTranslator : IMacroRuleInstanceTranslator
    {
        /// <summary>
        /// Returns all contacts who's city is found in the MedioClinic.BigUsCities custom table and who's state is an american one.
        /// </summary>
        /// <param name="macroRuleInstance">Macro rule instance.</param>
        /// <returns>All contacts from big US cities.</returns>
        public ObjectQuery<ContactInfo> Translate(MacroRuleInstance macroRuleInstance)
        {
            if (macroRuleInstance is null)
            {
                throw new ArgumentNullException(nameof(macroRuleInstance));
            }

            if (!macroRuleInstance.MacroRuleName.Equals(ContactInfoMethods.ComesFromBigUsCityName, StringComparison.Ordinal))
            {
                throw new ArgumentException($"[{nameof(ComesFromBigUsCityTranslator)}.{nameof(ComesFromBigUsCityTranslator.Translate)}]: Only macro rule instances of type {nameof(ContactInfoMethods.ComesFromBigUsCity)} can be translated.");
            }

            var bigUsCityRepository = Service.Resolve<IBigUsCityRepository>();

            return ContactInfo.Provider.Get()
                .WhereIn(nameof(ContactInfo.ContactCity), bigUsCityRepository.GetAllItems().Column(nameof(BigUsCitiesItem.CityName)))
                .WhereIn(nameof(ContactInfo.ContactStateID), CountryHelper.GetUsStates().Column(nameof(StateInfo.StateID))
                .WhereEquals(nameof(ContactInfo.ContactCountryID), CountryHelper.GetUsCountry().CountryID));
        }
    }
}
