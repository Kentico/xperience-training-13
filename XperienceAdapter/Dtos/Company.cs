using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace XperienceAdapter.Dtos
{
    public class Company
    {
        public string? Name { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
