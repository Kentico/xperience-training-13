using System;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Components.Widgets
{
    public class NewsletterSubscriptionModel
    {
        public string? NewsletterDisplayName { get; set; }

        public Guid? NewsletterGuid { get; set; }

        public Guid? ContactGuid { get; set; }

        [EmailAddress]
        public string? ContactEmail { get; set; }

        public string? ContactFirstName { get; set; }

        public string? ContactLastName { get; set; }

        public bool ConsentIsAgreed { get; set; }

        public string? ConsentShortText { get; set; }

        public string? ThankYouMessageResourceKey { get; set; }

        public bool RequireDoubleOptIn { get; set; }
    }
}
