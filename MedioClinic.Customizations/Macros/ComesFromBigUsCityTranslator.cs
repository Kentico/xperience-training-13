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
        /// Depending on the {_comes} macro parameter, returns all contacts who's city is found in the MedioClinic.BigUsCities custom table and who's state is an American one.
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
                throw new ArgumentException($"[{nameof(ComesFromBigUsCityTranslator)}.{nameof(ComesFromBigUsCityTranslator.Translate)}]: Only macro rule instances of type {ContactInfoMethods.ComesFromBigUsCityName} can be translated.");
            }

            var bigUsCityRepository = Service.Resolve<IBigUsCityRepository>();
            var comesOrDoesNotComeParameter = macroRuleInstance.Parameters["_comes"];

            if (comesOrDoesNotComeParameter.Value.Equals("!", StringComparison.OrdinalIgnoreCase))
            {
                return ContactInfo.Provider.Get()
                    .WhereNotIn(nameof(ContactInfo.ContactStateID), CountryHelper.GetUsStates().Column(nameof(StateInfo.StateID)))
                    .Or()
                    .WhereNull(nameof(ContactInfo.ContactStateID))
                    .Or()
                    .WhereNotIn(nameof(ContactInfo.ContactCity), bigUsCityRepository.GetAllItems().Column(nameof(BigUsCitiesItem.CityName)))
                    .Or()
                    .WhereNull(nameof(ContactInfo.ContactCity));
            }
            else
            {
                return ContactInfo.Provider.Get()
                    .WhereIn(nameof(ContactInfo.ContactStateID), CountryHelper.GetUsStates().Column(nameof(StateInfo.StateID)))
                    .WhereIn(nameof(ContactInfo.ContactCity), bigUsCityRepository.GetAllItems().Column(nameof(BigUsCitiesItem.CityName)));
            }
        }
    }
}
