using CMS.Globalization;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    public class CountryRepository : IRepository<Country>
    {
        protected readonly ICountryInfoProvider _countryInfoProvider;

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

        public IEnumerable<Country> GetAll()
        {
            var test01 = _countryInfoProvider
            .Get();

            var test02 = test01
            .AsEnumerable();

            var test03 = test02
            .Select(countryInfo => MapDtoProperties(countryInfo));

            return test03;
        }

        public Task<IEnumerable<Country>> GetAllAsync() => Task.FromResult(GetAll());
    }
}
