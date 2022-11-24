using Microsoft.Extensions.Localization;

using CMS.ContactManagement;
using CMS.Core;
using Kentico.PageBuilder.Web.Mvc.Personalization;

using XperienceAdapter.Localization;
using MedioClinic.Customizations.Helpers;

namespace MedioClinic.Personalization
{
    public class ComesFromBigUsCityConditionType : ConditionType
    {
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
            _stringLocalizer = Service.Resolve<IStringLocalizer<SharedResource>>();
        }

        public override bool Evaluate()
        {
            var currentContact = ContactManagementContext.GetCurrentContact(false);

            if (IsForBigCities && currentContact != null)
            {
                return CountryHelper.ContactComesFromBigUsCity(currentContact);
            }

            return false;
        }
    }
}
