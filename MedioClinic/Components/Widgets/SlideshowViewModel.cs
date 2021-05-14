using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MedioClinic.Models;

using XperienceAdapter.Models;

namespace MedioClinic.Components.Widgets
{
    public class SlideshowViewModel
    {
        /// <summary>
        /// Images from the media library.
        /// </summary>
        public IEnumerable<MediaLibraryFile>? Images { get; set; }

        /// <summary>
        /// Info about the media library in use.
        /// </summary>
        public MediaLibraryViewModel MediaLibraryViewModel { get; set; } = new MediaLibraryViewModel();

        /// <summary>
        /// Slideshow element width.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Slideshow element height.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Indicates if width and height are enforced in the live site.
        /// </summary>
        public bool EnforceDimensions { get; set; }

        /// <summary>
        /// Image transition delay (ms).
        /// </summary>
        public int? TransitionDelay { get; set; }

        /// <summary>
        /// Image transition speed (ms).
        /// </summary>
        public int? TransitionSpeed { get; set; }

        /// <summary>
        /// Indicates if navigation arrow signs are displayed in the live site.
        /// </summary>
        public bool DisplayArrowSigns { get; set; }
    }
}
