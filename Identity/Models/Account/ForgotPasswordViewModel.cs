using Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Models.Account
{
    public class ForgotPasswordViewModel
    {
        public EmailViewModel EmailViewModel => new EmailViewModel();

        public string? ResetPasswordAction { get; set; }

        public string? ResetPasswordController { get; set; }
    }
}
