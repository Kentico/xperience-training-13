namespace Business.Configuration
{
    /// <summary>
    /// Xperience-related configuration options.
    /// </summary>
    public class XperienceOptions
    {
        /// <summary>
        /// Code name of the site.
        /// </summary>
        public string? SiteName { get; set; }

        /// <summary>
        /// Friendly name of the company.
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Default site culture, as in RFC 5646 (e.g. "en-US").
        /// </summary>
        public string? DefaultCulture { get; set; }

        /// <summary>
        /// Indicates if content tree-based routing is used in the site.
        /// </summary>
        public bool? UseContentTreeBasedRouting { get; set; }

        public MediaLibraryOptions? MediaLibraryOptions { get; set; }

        public ContentTreeLocationOptions? ContentTreeLocationOptions { get; set; }

        public IdentityOptions? IdentityOptions { get; set; }
    }

    /// <summary>
    /// Media library options.
    /// </summary>
    public class MediaLibraryOptions
    {
        /// <summary>
        /// Image formats allowed in the site.
        /// </summary>
        public string[]? AllowedImageExtensions { get; set; }

        /// <summary>
        /// Code name of the medical centers library.
        /// </summary>
        public string? MedicalCentersLibrary { get; set; }

        /// <summary>
        /// File size limit.
        /// </summary>
        public long FileSizeLimit { get; set; }
    }

    /// <summary>
    /// Locations of specific data in the content tree.
    /// </summary>
    public class ContentTreeLocationOptions
    {
        /// <summary>
        /// Start location of the bottom navigation.
        /// </summary>
        public string? BottomNavigationAliasPath { get; set; }
    }

    /// <summary>
    /// Identity-related options.
    /// </summary>
    public class IdentityOptions
    {
        public bool EmailConfirmedRegistration { get; set; }

        public string? PathToDefaultAvatar { get; set; }
    }
}
