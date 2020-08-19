using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.Helpers;

using Business.Configuration;
using Identity;
using Identity.Models.Account;
using MedioClinic.Controllers;
using MedioClinic.Models;

namespace MedioClinic.Areas.Identity.Controllers
{
    // In production, use [RequireHttps].
    [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 0, Location = ResponseCacheLocation.None)]
    public class AccountController : BaseController
    {
        private readonly IAccountManager _accountManager;

        private IdentityOptions? IdentityOptions => _optionsMonitor.CurrentValue.IdentityOptions;

        public AccountController(ISiteService siteService, IOptionsMonitor<XperienceOptions> optionsMonitor, IAccountManager accountManager) : base(siteService, optionsMonitor)
        {
            _accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            var model = GetPageViewModel(new RegisterViewModel(), Localize("Controllers.Account.Register.Title"));

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PageViewModel<RegisterViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var emailConfirmedRegistration = IdentityOptions?.EmailConfirmedRegistration ?? default;
                var accountResult = await _accountManager.RegisterAsync(uploadModel.Data, emailConfirmedRegistration, Request);

                if (accountResult.ResultState == RegisterResultState.InvalidInput)
                {
                    AddIdentityErrors(accountResult);

                    return InvalidInput(uploadModel);
                }

                string title = ErrorTitle;
                var message = ConcatenateContactAdmin("Controllers.Account.Register.Failure.Message");
                var messageType = MessageType.Error;

                if (emailConfirmedRegistration)
                {
                    if (accountResult.ResultState == RegisterResultState.EmailSent)
                    {
                        title = Localize("Controllers.Account.Register.ConfirmedSuccess.Title");
                        message = Localize("Controllers.Account.Register.ConfirmedSuccess.Message");
                        messageType = MessageType.Info;
                    }
                }
                else if (accountResult.Success)
                {
                    title = Localize("Controllers.Account.Register.DirectSuccess.Title");
                    message = Localize("Controllers.Account.Register.DirectSuccess.Message");
                    messageType = MessageType.Info;
                }

                var messageViewModel = GetPageViewModel(title, message, false, messageType);

                return View("UserMessage", messageViewModel);
            }

            return InvalidInput(uploadModel);
        }

        // GET: /Account/ConfirmUser
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            var title = ErrorTitle;
            var message = ConcatenateContactAdmin("Error.Message");
            var displayAsRaw = false;
            var messageType = MessageType.Error;

            if (userId.HasValue)
            {
                var accountResult = await _accountManager.ConfirmUserAsync(userId.Value, token, Request);

                switch (accountResult.ResultState)
                {
                    case ConfirmUserResultState.EmailNotConfirmed:
                        message = Localize("Controllers.Account.ConfirmUser.ConfirmationFailure.Message");
                        break;
                    case ConfirmUserResultState.AvatarNotCreated:
                        message = Localize("Controllers.Account.ConfirmUser.AvatarFailure.Message");
                        messageType = MessageType.Warning;
                        break;
                    case ConfirmUserResultState.UserConfirmed:
                        title = Localize("Controllers.Account.ConfirmUser.Success.Title");
                        message = ResHelper.GetStringFormat("Controllers.Account.ConfirmUser.Success.Message", Url.Action("SignIn"));
                        displayAsRaw = true;
                        messageType = MessageType.Info;
                        break;
                }
            }

            return View("UserMessage", GetPageViewModel(title, message, displayAsRaw, messageType));
        }

        // GET: /Account/Signin
        public ActionResult SignIn()
        {
            return View(GetPageViewModel(new SignInViewModel(), Localize("LogonForm.LogonButton")));
        }

        // POST: /Account/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(PageViewModel<SignInViewModel> uploadModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accountResult = await _accountManager.SignInAsync(uploadModel.Data);

                switch (accountResult.ResultState)
                {
                    case SignInResultState.UserNotFound:
                    case SignInResultState.EmailNotConfirmed:
                    case SignInResultState.NotSignedIn:
                    default:
                        return InvalidAttempt(uploadModel);
                    case SignInResultState.SignedIn:
                        return RedirectToLocal(WebUtility.UrlDecode(returnUrl));
                }
            }

            return InvalidAttempt(uploadModel);
        }

        // GET: /Account/Signout
        [Authorize]
        public async Task<ActionResult> SignOut()
        {
            var accountResult = await _accountManager.SignOutAsync();

            if (accountResult.Success)
            {
                return RedirectToAction("Index", "Home");
            }

            var message = ConcatenateContactAdmin("Controllers.Account.SignOut.Failure.Message");

            return View("UserMessage", GetPageViewModel(Localize("General.Error"), message, messageType: MessageType.Error));
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var model = new EmailViewModel();

            return View(GetPageViewModel(model, Localize("PassReset.Title")));
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PageViewModel<EmailViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                // All of the result states should be treated equal (to prevent enumeration attacks), hence discarding the result entirely.
                _ = await _accountManager.ForgotPasswordAsync(uploadModel.Data, Request);

                var title = Localize("Controllers.Account.CheckEmailResetPassword.Title");
                var message = Localize("Controllers.Account.CheckEmailResetPassword.Message");

                return View("UserMessage", GetPageViewModel(title, message, messageType: MessageType.Info));
            }

            return View(GetPageViewModel(uploadModel.Data, Localize("PassReset.Title")));
        }

        // GET: /Account/ResetPassword
        public async Task<ActionResult> ResetPassword(int? userId, string token)
        {
            var message = ConcatenateContactAdmin("Controllers.Account.ResetPassword.Failure.Message");

            if (userId.HasValue && !string.IsNullOrEmpty(token))
            {
                var accountResult = await _accountManager.VerifyResetPasswordTokenAsync(userId.Value, token);

                if (accountResult.Success)
                {
                    return View(GetPageViewModel(accountResult.Data, Localize("PassReset.Title")));
                }
                else
                {
                    message = ConcatenateContactAdmin("Controllers.Account.InvalidToken.Message");
                }
            }

            return View("UserMessage", GetPageViewModel(Localize("General.Error"), message, messageType: MessageType.Error));
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PageViewModel<ResetPasswordViewModel> uploadModel)
        {
            var message = ConcatenateContactAdmin("Error.Message");
            var messageType = MessageType.Error;

            if (ModelState.IsValid)
            {
                var accountResult = await _accountManager.ResetPasswordAsync(uploadModel.Data);

                if (accountResult.Success)
                {
                    message = Localize("Controllers.Account.ResetPassword.Success.Message");
                    messageType = MessageType.Info;

                    if (HttpContext.User.Identity?.IsAuthenticated == false)
                    {
                        var signInAppendix = ResHelper.GetStringFormat("Controllers.Account.ResetPassword.Success.SignInAppendix", Url.Action("Signin"));
                        message = message.Insert(message.Length, $" {signInAppendix}");
                    }
                }
            }

            return View("UserMessage", GetPageViewModel(uploadModel.Data, Localize("PassReset.Title"), message, displayAsRaw: true, messageType: messageType));
        }

        /// <summary>
        /// Redirects to a local URL.
        /// </summary>
        /// <param name="returnUrl">Local URL to redirect to.</param>
        /// <returns>Redirect to a URL.</returns>
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays an invalid sign-in attempt message.
        /// </summary>
        /// <param name="uploadModel">Sign-in model taken from the user.</param>
        /// <returns>The user message.</returns>
        protected ActionResult InvalidAttempt(PageViewModel<SignInViewModel> uploadModel)
        {
            ModelState.AddModelError(string.Empty, Localize("Controllers.Account.InvalidAttempt"));

            return View(GetPageViewModel(uploadModel.Data, Localize("LogonForm.LogonButton")));
        }
    }
}
