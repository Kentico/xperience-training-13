using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using XperienceAdapter.Models;

namespace Business.Models
{
    public class Company : BasicPage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[]
        {
            "Street",
            "City",
            "Country",
            "PostalCode",
            "EmailAddress",
            "PhoneNumber"
        });

        public string? Street { get; set; }

        public string? City { get; set; }

        [UIHint("Country")]
        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
