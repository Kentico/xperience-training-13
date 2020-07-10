namespace XperienceAdapter.Dtos
{
    /// <summary>
    /// Site culture.
    /// </summary>
    public class SiteCulture
    {
        public string? FriendlyName { get; set; }

        public string? ShortName { get; set; }

        /// <summary>
        /// In the form of RFC 5646 (e.g. "en-US").
        /// </summary>
        public string? IsoCode { get; set; }

        public bool IsDefault { get; set; }
    }
}