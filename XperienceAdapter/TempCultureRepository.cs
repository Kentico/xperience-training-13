using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Abstractions;
using XperienceAdapter.Dtos;

namespace XperienceAdapter
{
    /// <summary>
    /// Temporary mock used for manual view testing.
    /// </summary>
    public class TempCultureRepository : ICultureRepository
    {
        public IEnumerable<SiteCulture> GetAll() =>
            Cultures;

        public SiteCulture GetByExactIsoCode(string isoCode) =>
            Cultures.First();

        public SiteCulture GetDefault() =>
            Cultures.First();

        private IEnumerable<SiteCulture> Cultures
        {
            get
            {
                return new List<SiteCulture>
                {
                    new SiteCulture
                    {
                        FriendlyName = "English",
                        IsDefault = true,
                        IsoCode = "en-US",
                        ShortName = "EN"
                    },
                    new SiteCulture
                    {
                        FriendlyName = "Czech",
                        IsoCode = "cz-CS",
                        ShortName = "CZ"
                    }
                };
            }
        }
    }
}
