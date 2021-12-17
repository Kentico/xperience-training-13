using MedioClinic.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Components.InlineEditors
{
    public class FileDownloadViewModel : InlineEditorViewModel
    {
        public string? MediaLibraryName { get; set; }

        public Guid? FileGuid { get; set; }
    }
}
