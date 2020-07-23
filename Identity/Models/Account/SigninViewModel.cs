using System.ComponentModel;

using Abstractions;

namespace Identity.Models.Account
{
    public class SignInViewModel : IViewModel
    {
        public EmailViewModel EmailViewModel { get; set; }

        public PasswordViewModel PasswordViewModel { get; set; }

        [DisplayName("Models.Account.StaySignedIn")]
        public bool StaySignedIn { get; set; }
    }
}