using Business.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using XperienceAdapter.Models;

namespace MedioClinic.Models
{
    public class NewsletterPreferenceViewModel
    {
        [Display(Prompt = "General.FirstName")]
        public string? FirstName { get; set; }

        [Display(Prompt = "General.LastName")]
        public string? LastName { get; set; }

        [Required]
        public EmailViewModel EmailViewModel { get; set; } = new EmailViewModel();

        public string? ConsentShortText { get; set; }

        [Required]
        public bool ConsentAgreed { get; set; }

        [Required]
        public List<NewsletterPreferenceModel> Newsletters  { get; set; } = new List<NewsletterPreferenceModel>();
    }
}
