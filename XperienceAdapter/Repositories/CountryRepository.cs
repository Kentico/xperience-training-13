using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.Globalization;

using Common;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    public class CountryRepository : IRepository<Country>
    {
        private readonly ICountryInfoProvider _countryInfoProvider;

        public CountryRepository(ICountryInfoProvider countryInfoProvider)
        {
            _countryInfoProvider = countryInfoProvider ?? throw new ArgumentNullException(nameof(countryInfoProvider));
        }

        public static Country MapDtoProperties(CountryInfo countryInfo) => new Country
        {
            CodeName = countryInfo.CountryName,
            Guid = countryInfo.CountryGUID,
            Name = countryInfo.CountryDisplayName
        };

        public IEnumerable<Country> GetAll() => _countryInfoProvider
                .Get()
                .AsEnumerable()
                .Select(countryInfo => MapDtoProperties(countryInfo));

        public Task<IEnumerable<Country>> GetAllAsync(CancellationToken? cancellationToken = default) => Task.FromResult(GetAll());
    }
}
