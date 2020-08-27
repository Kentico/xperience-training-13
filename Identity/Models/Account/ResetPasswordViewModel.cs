using Core;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Models.Account
{
    public class ResetPasswordViewModel
    {
        [HiddenInput]
        public int UserId { get; set; }

        [HiddenInput]
        public string? Token { get; set; }

        public PasswordConfirmationViewModel PasswordConfirmationViewModel { get; set; } = new PasswordConfirmationViewModel();
    }
}