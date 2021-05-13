using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Models
{
    /// <summary>
    /// Wraps information about a media library.
    /// </summary>
    public class MediaLibraryViewModel
    {
        /// <summary>
        /// Library name.
        /// </summary>
        public string? LibraryName { get; set; }

        /// <summary>
        /// Library site name.
        /// </summary>
        public string? LibrarySiteName { get; set; }

        /// <summary>
        /// Allowed file extensions.
        /// </summary>
        public HashSet<string>? AllowedFileExtensions { get; set; }
    }
}
