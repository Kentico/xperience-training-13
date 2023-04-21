using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Components.Widgets
{
    public class NewsletterSubscriptionProperties : IWidgetProperties
    {
        [EditingComponent(ComponentIdentifiers.NewsletterSelectionFormComponent, Label = "{$" + ComponentIdentifiers.NewsletterSelectionFormComponent + ".Title$}")]
        [Required]
        public string? NewsletterGuid { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$" + ComponentIdentifiers.NewsletterSelectionFormComponent + ".ThankYouMessageInput$}")]
        public string? ThankYouMessageResourceKey { get; set; }
    }
}
