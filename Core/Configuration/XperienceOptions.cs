using System;

namespace Core.Configuration
{
    /// <summary>
    /// Xperience-related configuration options.
    /// </summary>
    public class XperienceOptions
    {
        /// <summary>
        /// Friendly name of the company.
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Site code name.
        /// </summary>
        public string? SiteCodeName { get; set; }

        public MediaLibraryOptions? MediaLibraryOptions { get; set; }

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
        /// File size limit.
        /// </summary>
        public long FileSizeLimit { get; set; }
    }

    /// <summary>
    /// Identity-related options.
    /// </summary>
    public class IdentityOptions
    {
        public bool EmailConfirmedRegistration { get; set; }

        public Guid DefaultAvatarGuid { get; set; }

        public FacebookAuthenticationOptions? FacebookOptions { get; set; }

        public GoogleAuthenticationOptions? GoogleOptions { get; set; }

        public MicrosoftAuthenticationOptions? MicrosoftOptions { get; set; }

        public TwitterAuthenticationOptions? TwitterOptions { get; set; }
    }

    public class FacebookAuthenticationOptions
    {
        public bool UseFacebookAuth { get; set; }

        public string? AppId { get; set; }

        public string? AppSecret { get; set; }
    }

    public class GoogleAuthenticationOptions
    {
        public bool UseGoogleAuth { get; set; }

        public string? CallbackPath { get; set; }

        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }
    }

    public class MicrosoftAuthenticationOptions
    {
        public bool UseMicrosoftAuth { get; set; }

        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }
    }

    public class TwitterAuthenticationOptions
    {
        public bool UseTwitterAuth { get; set; }

        public string? ConsumerKey { get; set; }

        public string? ConsumerSecret { get; set; }
    }
}
