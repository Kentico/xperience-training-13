using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Dtos
{
    public class HomePageDto : BasicPageWithTitleDto
    {
        public override IEnumerable<string> SourceColumns =>
            base.SourceColumns.Concat(new[] { "LinkButtonText" });

        public string? LinkButtonText { get; set; }
    }
}
