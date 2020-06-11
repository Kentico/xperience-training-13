using System;
using System.Collections.Generic;
using System.Text;

using Abstractions;
using XperienceAdapter.Dtos;

namespace XperienceAdapter
{
    public class TempCompanyRepository : IRepository<Company>
    {
        public IEnumerable<Company> GetAll() =>
            new List<Company> { Company };

        private Company Company
        {
            get => new Company
            {
                City = "City",
                Country = "Country",
                EmailAddress = "email@address.com",
                Name = "Company name",
                PhoneNumber = "1234567890",
                PostalCode = "12345",
                Region = "Region",
                Street = "Street"
            };
        }
    }
}
