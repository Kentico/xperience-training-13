using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Components.Widgets
{
    public class ButtonProperties : IWidgetProperties
    {
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$MedioClinic.Widget.Button.Url$}")]
        [Required]
        public string? Url { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$MedioClinic.LinkTextResourceKey$}")]
        [Required]
        public string? LinkTextResourceKey { get; set; }

        [EditingComponent(ComponentIdentifiers.ColorSelectionFormComponent, Label = "{$" + ComponentIdentifiers.ColorSelectionFormComponent + ".Title$}")]
        public string? ButtonColor { get; set; }
    }
}
