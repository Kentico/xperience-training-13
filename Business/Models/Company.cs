using System.ComponentModel.DataAnnotations;

using XperienceAdapter.Models;

namespace Business.Models
{
    public class Company : BasePage
    {
        public string? Street { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
