using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// Social media link.
    /// </summary>
    public class SocialLink : BasePage
    {
        /// <summary>
        /// URL to social media.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Server path to the social link icon.
        /// </summary>
        public string? IconServerPath { get; set; }
    }
}
