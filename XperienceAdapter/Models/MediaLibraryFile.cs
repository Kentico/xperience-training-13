using Kentico.Content.Web.Mvc;
using System;

namespace XperienceAdapter.Models
{
    /// <summary>
    /// Media library file.
    /// </summary>
    public class MediaLibraryFile
    {
        public Guid Guid { get; set; }

        public string? Name { get; set; }
        
        public IMediaFileUrl? MediaFileUrl { get; set; }
        
        public string? Extension { get; set; }

        public int Width { get; set; }
        
        public int Height { get; set; }

        public string? MimeType { get; set; }

        public DateTimeOffset? LastModified { get; set; }
    }
}
