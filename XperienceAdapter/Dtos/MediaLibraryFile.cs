using System;

namespace XperienceAdapter.Dtos
{
    /// <summary>
    /// Media library file.
    /// </summary>
    public class MediaLibraryFile
    {
        public Guid Guid { get; set; }

        public string? Title { get; set; }
        
        public string? DirectUrl { get; set; }
        
        public string? PermanentUrl { get; set; }
        
        public string? Extension { get; set; }
        
        public int Width { get; set; }
        
        public int Height { get; set; }
    }
}
