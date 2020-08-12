using Core;

namespace Identity.Models.Account
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public PasswordConfirmationViewModel PasswordConfirmationViewModel { get; set; }
    }
}