using System.Collections.Generic;
using System.Linq;

using Kentico.Content.Web.Mvc;

using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// Social media link.
    /// </summary>
    public class SocialLink : BasicPage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[]
        {
            "Url",
            "Icon"
        });

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
