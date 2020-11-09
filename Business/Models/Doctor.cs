using System;
using System.Collections.Generic;
using System.Linq;

using XperienceAdapter.Models;

namespace Business.Models
{
    /// <summary>
    /// Information about a doctor
    /// </summary>
    public class Doctor : User
    {
        public override IEnumerable<string> SourceColumns => base.SourceColumns.Concat(new[] 
        { 
            "Degree",
            "EmergencyShift",
            "Biography",
            "Specialty"
        });

        /// <summary>
        /// Academic degree.
        /// </summary>
        public string? Degree { get; set; }

        /// <summary>
        /// Day of the doctor's emergency shift.
        /// </summary>
        public DayOfWeek? EmergencyShift { get; set; }

        /// <summary>
        /// Doctor's biography.
        /// </summary>
        public string? Biography { get; set; }

        /// <summary>
        /// Doctor's specialty.
        /// </summary>
        public string? Specialty { get; set; }
    }
}