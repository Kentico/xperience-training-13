using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using Business.Models;

namespace Identity.Models.Account
{
    public class ForgotPasswordViewModel
    {
        public EmailViewModel EmailViewModel => new EmailViewModel();

        [HiddenInput]
        public string? ResetPasswordAction { get; set; }

        [HiddenInput]
        public string? ResetPasswordController { get; set; }
    }
}
