using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace MedioClinic.Components.Widgets
{
    public class SlideshowProperties : IWidgetProperties
    {
        /// <summary>
        /// Image GUID identifiers.
        /// </summary>
        public Guid[]? ImageGuids { get; set; }

        /// <summary>
        /// Media library name.
        /// </summary>
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$General.MediaLibraryName$}", Order = 0)]
        public string? MediaLibraryName { get; set; }

        /// <summary>
        /// Image transition delay (ms).
        /// </summary>
        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + ComponentIdentifiers.SlideshowWidget + ".TransitionDelay$}", Order = 1)]
        public int? TransitionDelay { get; set; } = 5000;

        /// <summary>
        /// Image transition speed (ms).
        /// </summary>
        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + ComponentIdentifiers.SlideshowWidget + ".TransitionSpeed$}", Order = 2)]
        public int? TransitionSpeed { get; set; } = 300;

        /// <summary>
        /// Indicates if navigation arrow signs are displayed in the live site.
        /// </summary>
        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label = "{$" + ComponentIdentifiers.SlideshowWidget + ".DisplayArrowSigns$}", Order = 3)]
        public bool DisplayArrowSigns { get; set; } = true;

        /// <summary>
        /// Indicates if width and height are enforced in the live site.
        /// </summary>
        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label = "{$" + ComponentIdentifiers.SlideshowWidget + ".EnforceDimensions$}", Order = 4)]
        public bool EnforceDimensions { get; set; }

        /// <summary>
        /// Slideshow element width.
        /// </summary>
        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$General.Width$}", Order = 5)]
        public int? Width { get; set; }

        /// <summary>
        /// Slideshow element height.
        /// </summary>
        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$General.Height$}", Order = 6)]
        public int? Height { get; set; }
    }
}
