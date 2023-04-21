using System;
using System.ComponentModel.DataAnnotations;

namespace XperienceAdapter.Models
{
    public class NewsletterSubscriptionModel
    {
        [Required]
        public Guid NewsletterGuid { get; set; }

        [Required]
        public Guid ContactGuid { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
