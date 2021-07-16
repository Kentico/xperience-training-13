using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XperienceAdapter.Models;

namespace Business.Models
{
    public class MapLocation : BasicPage
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] { "Latitude", "Longitude" });

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
