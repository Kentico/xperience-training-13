using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter.Models;

namespace Business.Models
{
    public class EventLandingPage : BasicPage
    {
        public override IEnumerable<string> SourceColumns =>
            base.SourceColumns.Concat(new[] { nameof(CMS.DocumentEngine.Types.MedioClinic.EventLandingPage.EventDate) });

        public DateTime? EventDate { get; set; }
    }
}
