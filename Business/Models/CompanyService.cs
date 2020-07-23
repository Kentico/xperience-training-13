using System.Collections.Generic;
using System.Linq;

using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// Company service.
    /// </summary>
    public class CompanyService : BasePage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "ServiceDescription", "Icon" });

        /// <summary>
        /// Textual service description.
        /// </summary>
        public string? ServiceDescription { get; set; }

        /// <summary>
        /// Server path to the service icon file.
        /// </summary>
        public string? IconServerPath { get; set; }
    }
}
