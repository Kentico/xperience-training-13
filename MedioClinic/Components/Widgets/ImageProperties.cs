using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace MedioClinic.Components.Widgets
{
    public class ImageProperties : IWidgetProperties
    {
        /// <summary>
        /// GUID of the media library image file.
        /// </summary>
        public Guid? ImageGuid { get; set; }

        /// <summary>
        /// Name of the target media library.
        /// </summary>
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$General.MediaLibraryName$}", Order = 0)]
        public string? MediaLibraryName { get; set; }
    }
}
