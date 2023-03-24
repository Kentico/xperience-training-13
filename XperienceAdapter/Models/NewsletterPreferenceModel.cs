using Microsoft.Build.Framework;

using System;

namespace XperienceAdapter.Models
{
    public class NewsletterPreferenceModel
    {
        [Required]
        public Guid? NewsletterGuid { get; set; }

        public string? NewsletterDisplayName { get; set; }

        [Required]
        public bool Subscribed { get; set; }
    }
}
