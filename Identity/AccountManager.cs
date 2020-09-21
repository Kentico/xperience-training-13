using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

using CMS.Helpers;
using Kentico.Membership;

using Business.Extensions;
using Business.Models;
using Identity.Models;
using Identity.Models.Account;

namespace Identity
{
    public class AccountManager : BaseIdentityManager, IAccountManager
    {
        protected readonly IUrlHelperFactory _urlHelperFactory;

        protected readonly IActionContextAccessor _actionContextAccessor;

        protected readonly IMessageService _messageService;

        protected readonly IMedioClinicSignInManager<MedioClinicUser> _signInManager;

        //public IAvatarRepository AvatarRepository { get; set; }


        public AccountManager(
            ILogger<AccountManager> logger,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IMessageService messageService,
            IMedioClinicUserManager<MedioClinicUser> userManager,
            IMedioClinicSignInManager<MedioClinicUser> signInManager
            )
            : base(logger, userManager)
        {
            _urlHelperFactory = urlHelperFactory ?? throw new ArgumentNullException(nameof(urlHelperFactory));
            _actionContextAccessor = actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async Task<IdentityManagerResult<RegisterResultState>> RegisterAsync(RegisterViewModel uploadModel, bool emailConfirmed, HttpRequest request)
        {
            var user = new MedioClinicUser
            {
                UserName = uploadModel.EmailViewModel.Email,
                Email = uploadModel.EmailViewModel.Email,
                FirstName = uploadModel.FirstName,
                LastName = uploadModel.LastName,
                Enabled = !emailConfirmed
            };

            var accountResult = new IdentityManagerResult<RegisterResultState>();
            IdentityResult? identityResult = default;

            try
            {
                identityResult = await _userManager.CreateAsync(user, uploadModel.PasswordConfirmationViewModel.Password!);
            }
            catch (Exception ex)
            {
                HandleException(nameof(RegisterAsync), ex, ref accountResult);

                return accountResult;
            }

            if (identityResult?.Succeeded == true)
            {
                // Registration: Confirmed registration (begin)
                if (emailConfirmed)
                {
                    string? token = default;

                    try
                    {
                        token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    }
                    catch (Exception ex)
                    {
                        accountResult.ResultState = RegisterResultState.TokenNotCreated;
                        HandleException(nameof(RegisterAsync), ex, ref accountResult);

                        return accountResult;
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        // TODO: use nameof (via input params).
                        var confirmationUrl = _urlHelperFactory
                            .GetUrlHelper(_actionContextAccessor.ActionContext)
                            .AbsoluteUrl(request, "ConfirmUser", routeValues: new { userId = user.Id, token });

                        var subject = ResHelper.GetString("Identity.Account.Register.Email.Confirm.Subject");
                        var body = ResHelper.GetStringFormat("Identity.Account.Register.Email.Confirm.Body", confirmationUrl);

                        await _messageService.SendEmailAsync(user.Email, subject, body);

                        accountResult.Success = true;
                        accountResult.ResultState = RegisterResultState.EmailSent;
                    }
                }
                // Registration: Confirmed registration (end)

                // Registration: Direct sign in (begin)
                else
                {
                    identityResult = await AddToPatientRoleAsync(user.Id);

                    try
                    {
                        //await CreateNewAvatarAsync(user, requestContext.HttpContext.Server);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        accountResult.ResultState = RegisterResultState.SignedIn;
                        accountResult.Success = true;
                    }
                    catch (Exception ex)
                    {
                        accountResult.ResultState = RegisterResultState.NotSignedIn;
                        HandleException(nameof(RegisterAsync), ex, ref accountResult);

                        return accountResult;
                    }
                }
                // Registration: Direct sign in (end)
            }

            accountResult.Errors.AddNonNullRange(identityResult?.Errors.Select(error => error.Description));

            return accountResult;
        }

        public async Task<IdentityManagerResult<ConfirmUserResultState>> ConfirmUserAsync(int userId, string token, HttpRequest request)
        {
            var accountResult = new IdentityManagerResult<ConfirmUserResultState>();
            IdentityResult identityResult = IdentityResult.Failed();
            MedioClinicUser user;

            try
            {
                user = await _userManager.FindByIdAsync(userId.ToString());
                identityResult = await _userManager.ConfirmEmailAsync(user, token);
            }
            catch (Exception ex)
            {
                accountResult.ResultState = ConfirmUserResultState.EmailNotConfirmed;
                HandleException(nameof(ConfirmUserAsync), ex, ref accountResult);

                return accountResult;
            }

            if (identityResult.Succeeded && (await AddToPatientRoleAsync(userId)).Succeeded)
            {
                try
                {
                    //var user = await UserManager.FindByIdAsync(userId);
                    //await CreateNewAvatarAsync(user, requestContext.HttpContext.Server);
                    accountResult.Success = true;
                    accountResult.ResultState = ConfirmUserResultState.UserConfirmed;
                }
                catch (Exception ex)
                {
                    accountResult.ResultState = ConfirmUserResultState.AvatarNotCreated;
                    HandleException(nameof(ConfirmUserAsync), ex, ref accountResult);

                    return accountResult;
                }
            }

            accountResult.Errors.AddNonNullRange(identityResult.Errors.Select(error => error.Description));

            return accountResult;
        }

        public async Task<IdentityManagerResult<SignInResultState>> SignInAsync(SignInViewModel uploadModel)
        {
            var accountResult = new IdentityManagerResult<SignInResultState, SignInViewModel>();
            MedioClinicUser? user = default;

            try
            {
                user = await _userManager.FindByNameAsync(uploadModel.EmailViewModel.Email!);
            }
            catch (Exception ex)
            {
                var ar = accountResult as IdentityManagerResult<SignInResultState>;
                accountResult.ResultState = SignInResultState.UserNotFound;
                HandleException(nameof(SignInAsync), ex, ref ar);

                return accountResult;
            }

            // Registration: Confirmed registration (begin)
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                accountResult.ResultState = SignInResultState.EmailNotConfirmed;

                return accountResult;
            }
            // Registration: Confirmed registration (end)

            Microsoft.AspNetCore.Identity.SignInResult signInResult;

            try
            {
                signInResult = await _signInManager.PasswordSignInAsync(uploadModel.EmailViewModel.Email!, uploadModel.PasswordViewModel.Password!, uploadModel.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                var ar = accountResult as IdentityManagerResult<SignInResultState>;
                accountResult.ResultState = SignInResultState.NotSignedIn;
                HandleException(nameof(SignInAsync), ex, ref ar);

                return accountResult;
            }

            if (signInResult.Succeeded)
            {
                accountResult.Success = true;
                accountResult.ResultState = SignInResultState.SignedIn;
            }

            return accountResult;
        }

        public async Task<IdentityManagerResult<SignOutResultState>> SignOutAsync()
        {
            var accountResult = new IdentityManagerResult<SignOutResultState>();

            try
            {
                await _signInManager.SignOutAsync();
                accountResult.Success = true;
                accountResult.ResultState = SignOutResultState.SignedOut;
            }
            catch (Exception ex)
            {
                accountResult.ResultState = SignOutResultState.NotSignedOut;
                HandleException(nameof(SignOutAsync), ex, ref accountResult);
            }

            return accountResult;
        }

        public async Task<IdentityManagerResult<ForgotPasswordResultState>> ForgotPasswordAsync(EmailViewModel uploadModel, HttpRequest request)
        {
            var accountResult = new IdentityManagerResult<ForgotPasswordResultState>();
            MedioClinicUser? user = default;

            try
            {
                user = await _userManager.FindByEmailAsync(uploadModel.Email!);
            }
            catch (Exception ex)
            {
                accountResult.ResultState = ForgotPasswordResultState.UserNotFound;
                HandleException(nameof(ForgotPasswordAsync), ex, ref accountResult);

                return accountResult;
            }

            // Registration: Confirmed registration (begin)
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                accountResult.ResultState = ForgotPasswordResultState.EmailNotConfirmed;

                return accountResult;
            }
            // Registration: Confirmed registration (end)

            string? token = default;

            try
            {
                token = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch (Exception ex)
            {
                accountResult.ResultState = ForgotPasswordResultState.TokenNotCreated;
                HandleException(nameof(ForgotPasswordAsync), ex, ref accountResult);

                return accountResult;
            }

            // TODO: Use nameof.
            var resetUrl = _urlHelperFactory
                .GetUrlHelper(_actionContextAccessor.ActionContext)
                .AbsoluteUrl(request, "ResetPassword", "Account", new { userId = user.Id, token });

            var subject = ResHelper.GetString("Identity.Account.ResetPassword.Title");
            var body = ResHelper.GetStringFormat("Identity.Account.ForgotPassword.Email.Body", resetUrl);

            try
            {
                await _messageService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                accountResult.ResultState = ForgotPasswordResultState.EmailNotSent;
                HandleException(nameof(ForgotPasswordAsync), ex, ref accountResult);

                return accountResult;
            }

            accountResult.Success = true;
            accountResult.ResultState = ForgotPasswordResultState.EmailSent;

            return accountResult;
        }

        public async Task<IdentityManagerResult<ResetPasswordResultState, ResetPasswordViewModel>> VerifyResetPasswordTokenAsync(int userId, string token)
        {
            var accountResult = new IdentityManagerResult<ResetPasswordResultState, ResetPasswordViewModel>();
            var tokenVerified = false;

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                tokenVerified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);
            }
            catch (Exception ex)
            {
                var ar = accountResult as IdentityManagerResult<ResetPasswordResultState>;
                accountResult.ResultState = ResetPasswordResultState.InvalidToken;
                HandleException(nameof(VerifyResetPasswordTokenAsync), ex, ref ar);

                return accountResult;
            }

            if (tokenVerified)
            {
                accountResult.Success = true;
                accountResult.ResultState = ResetPasswordResultState.TokenVerified;

                accountResult.Data = new ResetPasswordViewModel
                {
                    UserId = userId,
                    Token = token
                };
            }

            return accountResult;
        }

        public async Task<IdentityManagerResult<ResetPasswordResultState>> ResetPasswordAsync(ResetPasswordViewModel uploadModel)
        {
            var accountResult = new IdentityManagerResult<ResetPasswordResultState>();
            var identityResult = IdentityResult.Failed();

            try
            {
                var user = await _userManager.FindByIdAsync(uploadModel.UserId.ToString());
                identityResult = await _userManager.ResetPasswordAsync(
                    user,
                    uploadModel.Token!,
                    uploadModel.PasswordConfirmationViewModel.Password!);
            }
            catch (Exception ex)
            {
                accountResult.ResultState = ResetPasswordResultState.PasswordNotReset;
                HandleException(nameof(ResetPasswordAsync), ex, ref accountResult);

                return accountResult;
            }

            if (identityResult.Succeeded)
            {
                accountResult.Success = true;
                accountResult.ResultState = ResetPasswordResultState.PasswordReset;
            }

            return accountResult;
        }

        /// <summary>
        /// Adds a user to the patient role.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>An identity result.</returns>
        protected async Task<IdentityResult> AddToPatientRoleAsync(int userId)
        {
            var patientRole = Roles.Patient.ToString();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return await _userManager.AddToRolesAsync(user, new[] { patientRole });
        }
    }
}