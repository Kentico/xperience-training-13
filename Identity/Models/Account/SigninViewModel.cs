using System.ComponentModel;

using Core;

namespace Identity.Models.Account
{
    public class SignInViewModel
    {
        public EmailViewModel EmailViewModel { get; set; }

        public PasswordViewModel PasswordViewModel { get; set; }

        [DisplayName("Models.Account.StaySignedIn")]
        public bool StaySignedIn { get; set; }
    }
}