using System.Collections.Generic;
using System.Linq;

namespace Business.Models
{
    /// <summary>
    /// A home page.
    /// </summary>
    public class HomePage : BasicPageWithUrlSlug
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "LinkButtonText", "DoctorsLink" });

        /// <summary>
        /// Text of a link to a Doctors page.
        /// </summary>
        public string? LinkButtonText { get; set; }

        /// <summary>
        /// URL of the link to the Doctors page.
        /// </summary>
        public string? DoctorsUrl { get; set; }
    }
}
