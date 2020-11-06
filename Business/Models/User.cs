using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Models
{
    public class User : BasicPageWithUrlSlug
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "UserAccount" });

        /// <summary>
        /// User name.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public int? UserId { get; set; }
    }
}
