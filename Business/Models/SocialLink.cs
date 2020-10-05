using Kentico.Content.Web.Mvc;

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
        /// URL of the service icon file.
        /// </summary>
        public IPageAttachmentUrl? IconUrl { get; set; }
    }
}
