using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Personalization
{
    public class IsInPersonaViewModel
    {
        [Required]
        [Display(Name = "OnlineMarketing.Persona.CodeName")]
        public string? PersonaCodeName { get; set; }


        public List<IsInPersonaListItemViewModel>? AllPersonas { get; set; }
    }
}