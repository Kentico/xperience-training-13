using System;
using System.Text;
using System.Linq;

using TinyCsvParser;

using CMS.ContactManagement;
using CMS.Globalization;

namespace XperienceAdapter.Generator
{
    public class Generator : IGenerator
    {
        private const string TestingCenterStartingDateDbName = "TestingCenterStartingDate";

        private static CsvParserOptions _csvParserOptions = new CsvParserOptions(true, ',');

        public void GenerateContacts(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var mapping = new ContactMapping();
            var parser = new CsvParser<Contact>(_csvParserOptions, mapping);
            var contacts = parser?.ReadFromFile(path, Encoding.UTF8).ToList();
            var countryIds = CountryInfo.Provider.Get().OrderBy("CountryID").Select(c => c.CountryID).ToList();
            var random = new Random();
            var max = countryIds?.Count();

            if (contacts?.Any() == true && countryIds?.Any() == true)
            {
                foreach (var contact in contacts)
                {
                    if (contact?.IsValid == true)
                    {
                        var result = contact.Result;
                        var countryIndex = random?.Next(0, max ?? 0) ?? 0;

                        var contactInfo = new ContactInfo()
                        {
                            ContactFirstName = result.FirstName,
                            ContactLastName = result.LastName,
                            ContactEmail = result.EmailAddress,
                            ContactCountryID = countryIds[countryIndex]
                        };

                        contactInfo.SetValue(TestingCenterStartingDateDbName, result.TestingCenterStartingDate);

                        ContactInfo.Provider.Set(contactInfo);
                    }
                }
            }
        }

        public void GenerateActivities()
        {
            throw new NotImplementedException();
        }

        public void GenerateContactGroup()
        {
            throw new NotImplementedException();
        }

        public void GenerateConversions()
        {
            throw new NotImplementedException();
        }

        public void GeneratePersona()
        {
            throw new NotImplementedException();
        }
    }
}
