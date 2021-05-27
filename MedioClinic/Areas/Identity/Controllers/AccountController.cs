using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;

using Core.Configuration;
using XperienceAdapter.Repositories;
using Business.Models;
using Identity;
using Identity.Models;
using Identity.Models.Account;
using MedioClinic.Controllers;
using MedioClinic.Models;
using Microsoft.Extensions.Localization;
using XperienceAdapter.Localization;

namespace MedioClinic.Areas.Identity.Controllers
{
    // In production, use [RequireHttps].
    public class AccountController : BaseIdentityController
    {
        private readonly IAccountManager _accountManager;

        private Core.Configuration.IdentityOptions? IdentityOptions => _optionsMonitor.CurrentValue.IdentityOptions;

        public AccountController(
            ILogger<AccountController> logger, 
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IAccountManager accountManager,
            IPageUrlRetriever pageUrlRetriever) 
            : base(logger, optionsMonitor, stringLocalizer, pageUrlRetriever)
        {
            _accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            model.PasswordConfirmationViewModel.ConfirmationAction = nameof(ConfirmUser);

            var metadata = new Models.PageMetadata
            {
                Title = Localize("Identity.Account.Register.Title")
            };

            var viewModel = GetPageViewModel(metadata, model);

            return View(viewModel);
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
                var message = ConcatenateContactAdmin("Identity.Account.Register.Failure.Message");
                var messageType = MessageType.Error;

                if (emailConfirmedRegistration)
                {
                    if (accountResult.ResultState == RegisterResultState.EmailSent)
                    {
                        title = Localize("Identity.Account.Register.ConfirmedSuccess.Title");
                        message = Localize("Identity.Account.Register.ConfirmedSuccess.Message");
                        messageType = MessageType.Info;
                    }
                }
                else if (accountResult.Success)
                {
                    title = Localize("Identity.Account.Register.DirectSuccess.Title");
                    message = Localize("Identity.Account.Register.DirectSuccess.Message");
                    messageType = MessageType.Info;
                }

                var metadata = new Models.PageMetadata
                {
                    Title = title
                };

                var messageViewModel = GetPageViewModel(metadata, message, true, false, messageType);

                return View("UserMessage", messageViewModel);
            }

            return InvalidInput(uploadModel);
        }

        // GET: /Account/ConfirmUser
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            var metadata = new Models.PageMetadata
            {
                Title = ErrorTitle
            };

            var message = ConcatenateContactAdmin("General.Error");
            var displayAsRaw = false;
            var messageType = MessageType.Error;

            if (userId.HasValue)
            {
                var accountResult = await _accountManager.ConfirmUserAsync(userId.Value, token);

                switch (accountResult.ResultState)
                {
                    case ConfirmUserResultState.EmailNotConfirmed:
                        message = Localize("Identity.Account.ConfirmUser.ConfirmationFailure.Message");
                        break;
                    case ConfirmUserResultState.UserConfirmed:
                        metadata.Title = Localize("Identity.Account.ConfirmUser.Success.Title");
                        message = Localize("Identity.Account.ConfirmUser.Success.Message", Url.Action(nameof(SignIn)));
                        displayAsRaw = true;
                        messageType = MessageType.Info;
                        break;
                }
            }

            return View("UserMessage", GetPageViewModel(metadata, message, true, displayAsRaw, messageType));
        }

        // GET: /Account/Signin
        public ActionResult SignIn()
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("Identity.Account.SignIn.Title")
            };

            return View(GetPageViewModel(metadata, new SignInViewModel()));
        }

        // POST: /Account/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(PageViewModel<SignInViewModel> uploadModel, string? returnUrl = default)
        {
            if (ModelState.IsValid)
            {
                var url = !string.IsNullOrEmpty(returnUrl) ? WebUtility.UrlDecode(returnUrl) : GetHomeUrl();
                var accountResult = await _accountManager.SignInAsync(uploadModel.Data);

                switch (accountResult.ResultState)
                {
                    case SignInResultState.UserNotFound:
                    case SignInResultState.EmailNotConfirmed:
                    case SignInResultState.NotSignedIn:
                    default:
                        return InvalidAttempt(uploadModel);
                    case SignInResultState.SignedIn:
                        return RedirectToLocal(url);
                }
            }

            return InvalidAttempt(uploadModel);
        }

        // GET: /Account/Signout
        [Authorize]
        public async Task<ActionResult> SignOut()
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("General.Error")
            };

            var accountResult = await _accountManager.SignOutAsync();

            if (accountResult.Success)
            {
                return RedirectToLocal(GetHomeUrl());
            }

            var message = ConcatenateContactAdmin("Identity.Account.SignOut.Failure.Message");

            return View("UserMessage", GetPageViewModel(metadata, message: message, messageType: MessageType.Error));
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("Identity.Account.ResetPassword.Title")
            };

            var model = new ForgotPasswordViewModel();
            model.ResetPasswordController = "Account";
            model.ResetPasswordAction = nameof(ResetPassword);

            return View(GetPageViewModel(metadata, model));
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PageViewModel<ForgotPasswordViewModel> uploadModel)
        {
            var metadata = new Models.PageMetadata();

            if (ModelState.IsValid)
            {
                // All of the result states should be treated equal (to prevent enumeration attacks), hence discarding the result entirely.
                _ = await _accountManager.ForgotPasswordAsync(uploadModel.Data, Request);

                metadata.Title = Localize("Identity.Account.CheckEmailResetPassword.Title");
                var message = Localize("Identity.Account.CheckEmailResetPassword.Message");

                return View("UserMessage", GetPageViewModel(metadata, message: message, displayMessage: false, messageType: MessageType.Info));
            }

            metadata.Title = Localize("Identity.Account.ResetPassword.Title");

            return View(GetPageViewModel(metadata, uploadModel.Data));
        }

        // GET: /Account/ResetPassword
        public async Task<ActionResult> ResetPassword(int? userId, string token)
        {
            var metadata = new Models.PageMetadata();
            var message = ConcatenateContactAdmin("Identity.Account.ResetPassword.Failure.Message");

            if (userId.HasValue && !string.IsNullOrEmpty(token))
            {
                var accountResult = await _accountManager.VerifyResetPasswordTokenAsync(userId.Value, token);

                if (accountResult.Success)
                {
                    metadata.Title = Localize("Identity.Account.ResetPassword.Title");

                    return View(GetPageViewModel(metadata, accountResult.Data));
                }
                else
                {
                    message = ConcatenateContactAdmin("Identity.Account.InvalidToken.Message");
                }
            }

            metadata.Title = Localize("General.Error");

            return View("UserMessage", GetPageViewModel(metadata, message: message, displayMessage: false, messageType: MessageType.Error));
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PageViewModel<ResetPasswordViewModel> uploadModel)
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("Identity.Account.ResetPassword.Title")
            };

            var message = ConcatenateContactAdmin("General.Error");
            var messageType = MessageType.Error;

            if (ModelState.IsValid)
            {
                var accountResult = await _accountManager.ResetPasswordAsync(uploadModel.Data);

                if (accountResult.Success)
                {
                    message = Localize("Identity.Account.ResetPassword.Success.Message");
                    messageType = MessageType.Info;

                    if (HttpContext.User.Identity?.IsAuthenticated == false)
                    {
                        var signInAppendix = Localize("Identity.Account.ResetPassword.Success.SignInAppendix", Url.Action(nameof(SignIn)));
                        message = message.Insert(message.Length, $" {signInAppendix}");
                    }
                }
            }

            return View("UserMessage", GetPageViewModel(metadata, uploadModel.Data, message, false, true, messageType));
        }

        /// <summary>
        /// Redirects authentication requests to an external service.
        /// </summary>
        /// <param name="provider">Name of the authentication middleware.</param>
        /// <param name="returnUrl">Return URL.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestExternalSignIn(string provider, string returnUrl)
        {
            var callbackUrl = Url.Action(nameof(ExternalSignInCallback), new { ReturnUrl = returnUrl });
            var authenticationProperties = _accountManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return Challenge(authenticationProperties, provider);
        }

        /// <summary>
        /// Handles responses from external authentication services.
        /// </summary>
        /// <param name="returnUrl">Return URL.</param>
        /// <param name="remoteError">Error returned by the external identity provider.</param>
        public async Task<IActionResult> ExternalSignInCallback(string returnUrl, string? remoteError = default)
        {
            if (remoteError != null)
            {
                var error = $"External authentication failed: {remoteError}";
                _logger.LogError(error);
                ModelState.AddModelError(string.Empty, error);

                return View(nameof(SignIn));
            }

            var loginInfo = await _accountManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                var error = $"External authentication failed. ExteralLoginInfo must not be null.";
                _logger.LogError(error);
                ModelState.AddModelError(string.Empty, error);

                return View(nameof(SignIn));
            }

            var result = await _accountManager.SignInExternalAsync(loginInfo);

            return result.Success
                ? RedirectToLocal(returnUrl)
                : InvalidAttempt(new PageViewModel<SignInViewModel>());
        }

        /// <summary>
        /// Displays an invalid sign-in attempt message.
        /// </summary>
        /// <param name="uploadModel">Sign-in model taken from the user.</param>
        /// <returns>The user message.</returns>
        private ActionResult InvalidAttempt(PageViewModel<SignInViewModel> uploadModel)
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("Identity.Account.SignIn.Title")
            };

            ModelState.AddModelError(string.Empty, Localize("Identity.Account.InvalidAttempt"));

            return View(GetPageViewModel(metadata, uploadModel.Data));
        }
    }
}
