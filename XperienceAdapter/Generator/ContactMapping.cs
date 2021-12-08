using System;
using System.Collections.Generic;
using System.Text;

using TinyCsvParser.Mapping;

namespace XperienceAdapter.Generator
{
    internal class ContactMapping : CsvMapping<Contact>
    {
        public ContactMapping() : base()
        {
            MapProperty(0, m => m.FirstName);
            MapProperty(1, m => m.LastName);
            MapProperty(2, m => m.EmailAddress);
            MapProperty(3, m => m.UrlReferrer);
            MapProperty(4, m => m.TestingCenterStartingDate);
        }
    }
}
