using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Personalization
{
    public class ComesFromBigUsCityViewModel
    {
        [Display(Name = "MedioClinic.PersonalizationCondition.ComesFromBigUsCity.IsForBigCities")]
        public bool IsForBigUsCity { get; set; }
    }
}
