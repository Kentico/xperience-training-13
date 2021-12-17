using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Components.Widgets
{
    public class FileDownloadProperties : IWidgetProperties
    {
        [EditingComponent(ComponentIdentifiers.MediaLibrarySelectionFormComponent,
            Label = "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Title$}",
            Tooltip = "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Description$}",
            Order = 0)]
        [Required]
        public string? MediLibraryId { get; set; }

        public Guid? FileGuid { get; set; }

        public bool SecuredDownload { get; set; }
    }
}