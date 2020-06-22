using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter;

namespace Business.Dtos
{
    public class CompanyServiceDto : BasePageDto
    {
        public override IEnumerable<string> SourceColumns =>
            base.SourceColumns.Concat(new[] { "ServiceDescription", "Icon" });

        public string? ServiceDescription { get; set; }

        public string? IconPath { get; set; }
    }
}
