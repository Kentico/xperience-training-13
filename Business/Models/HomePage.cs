using System.Collections.Generic;
using System.Linq;

namespace Business.Models
{
    /// <summary>
    /// A home page.
    /// </summary>
    public class HomePage : BasicPageWithUrlSlug
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "DoctorsLinkButtonText", "ServicesLinkButtonText", "DoctorsLink" });

        /// <summary>
        /// Page perex.
        /// </summary>
        public string? Perex { get; set; }

        /// <summary>
        /// Page text.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Text of a link to a Doctors page.
        /// </summary>
        public string? DoctorsLinkButtonText { get; set; }

        /// <summary>
        /// Text of a link to a Services section.
        /// </summary>
        public string? ServicesLinkButtonText { get; set; }

        /// <summary>
        /// URL of the link to the Doctors page.
        /// </summary>
        public string? DoctorsUrl { get; set; }
    }
}
