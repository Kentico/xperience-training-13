using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using CMS.DataEngine;

using Kentico.Forms.Web.Mvc;

namespace MedioClinic.Components.FormComponents
{
    public class MediaLibraryUploaderProperties : FormComponentProperties<string>
    {
        public MediaLibraryUploaderProperties() : base(FieldDataType.Text, 400)
        {
        }

        [DefaultValueEditingComponent(TextInputComponent.IDENTIFIER)]
        public override string? DefaultValue { get; set; }

        [EditingComponent(ComponentIdentifiers.MediaLibrarySelectionFormComponent,
            Label = "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Title$}",
            Tooltip = "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Description$}",
            Order = 0)]
        [Required]
        public string MediaLibraryId { get; set; }
    }
}
