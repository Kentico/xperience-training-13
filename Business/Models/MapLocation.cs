using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XperienceAdapter.Models;

namespace Business.Models
{
    public class MapLocation : BasePage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "Latitude", "Longitude", "Tooltip" });

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string? Tooltip { get; set; }

        
    }
}
