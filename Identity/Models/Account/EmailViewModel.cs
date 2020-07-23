using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Abstractions;

namespace Identity.Models.Account
{
    public class EmailViewModel : IViewModel
    {
        [Required(ErrorMessage = "General.RequireEmail")]
        [DisplayName("General.EmailAddress")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Models.EmailFormat")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string Email { get; set; }
    }
}