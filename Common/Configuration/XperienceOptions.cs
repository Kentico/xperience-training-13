using System;

namespace Common.Configuration
{
    /// <summary>
    /// Xperience-related configuration options.
    /// </summary>
    public class XperienceOptions
    {
        /// <summary>
        /// Friendly name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Site code name.
        /// </summary>
        public string SiteCodeName { get; set; }
    }
}
