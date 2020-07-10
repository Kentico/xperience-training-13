using System.Collections.Generic;
using System.Linq;

using XperienceAdapter.Dtos;

namespace Business.Dtos
{
    /// <summary>
    /// Basic page that has a URL slug.
    /// </summary>
    public class BasicPageWithUrlSlug : BasePage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "UrlSlug" });

        /// <summary>
        /// URL slug used for conventional routing.
        /// </summary>
        public string? UrlSlug { get; set; }
    }
}
