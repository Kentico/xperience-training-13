using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter;

namespace Business.Dtos
{
    public class BasicPageWithTitleDto : BasePageDto
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "PageTitle" });

        public string? PageTitle { get; set; }
    }
}
