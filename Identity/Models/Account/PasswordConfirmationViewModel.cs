using System.ComponentModel.DataAnnotations;

namespace Identity.Models.Account
{
    public class PasswordConfirmationViewModel : PasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Identity.Account.ConfirmPassword")]
        [Compare(nameof(Password), ErrorMessage = "Identity.Account.Password.DoesntMatch")]
        public string? ConfirmPassword { get; set; }
    }
}