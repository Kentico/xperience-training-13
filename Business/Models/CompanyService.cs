using Kentico.Content.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// Company service.
    /// </summary>
    public class CompanyService : BasicPage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "ServiceDescription", "Icon" });

        /// <summary>
        /// Textual service description.
        /// </summary>
        public string? ServiceDescription { get; set; }

        /// <summary>
        /// URL of the service icon file.
        /// </summary>
        public IPageAttachmentUrl? IconUrl { get; set; }
    }
}
