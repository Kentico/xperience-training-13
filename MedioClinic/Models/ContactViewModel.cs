using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XperienceAdapter.Models;

namespace MedioClinic.Models
{
    public class ContactViewModel
    {
        public Company? Company { get; set; }

        public IEnumerable<MapLocation>? OfficeLocations { get; set; }

        public NamePerexText? ContactPage { get; set; }

        public IEnumerable<MediaLibraryFile>? MedicalServices { get; set; }
    }
}
