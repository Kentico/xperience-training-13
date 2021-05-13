using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MedioClinic.Models;

namespace MedioClinic.Components.InlineEditors
{
    public class ImageUploaderViewModel : InlineEditorViewModel
    {
        public int? PageId { get; set; }

        public bool HasImage { get; set; }

        public MediaLibraryViewModel? MediaLibraryViewModel { get; set; }
    }
}
