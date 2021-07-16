using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// A general page with name, perex, and text.
    /// </summary>
    public class NamePerexText : BasicPage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "Perex", "Text" });

        /// <summary>
        /// Page perex.
        /// </summary>
        public string? Perex { get; set; }

        /// <summary>
        /// Page text.
        /// </summary>
        public string? Text { get; set; }
    }
}
