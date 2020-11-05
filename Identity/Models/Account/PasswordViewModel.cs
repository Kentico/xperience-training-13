using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models.Account
{
    public class PasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Identity.Account.Password")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? Password { get; set; }
    }
}