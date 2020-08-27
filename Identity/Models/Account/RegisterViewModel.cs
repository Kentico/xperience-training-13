using System.ComponentModel.DataAnnotations;

using Business.Models;

namespace Identity.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "General.FirstName")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "General.LastName")]
        public string? LastName { get; set; }

        public EmailViewModel EmailViewModel { get; set; } = new EmailViewModel();

        public PasswordConfirmationViewModel PasswordConfirmationViewModel { get; set; } = new PasswordConfirmationViewModel();
    }
}