using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Content.Web.Mvc;

using MedioClinic.Models;

using XperienceAdapter.Models;

namespace MedioClinic.Components.Widgets
{
    public class ImageViewModel
    {
        /// <summary>
        /// Page ID.
        /// </summary>
        public int? PageId { get; set; }

        /// <summary>
        /// Indicates if the model contains an image.
        /// </summary>
        public bool HasImage { get; set; }

        /// <summary>
        /// Media library file.
        /// </summary>
        public MediaLibraryFile? MediaLibraryFile { get; set; }

        /// <summary>
        /// Info about the media library in use.
        /// </summary>
        public MediaLibraryViewModel MediaLibraryViewModel { get; set; } = new MediaLibraryViewModel();
    }
}
