using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Core;
using Business.Models;
using Identity.Models;
using Identity.Models.Account;
using Microsoft.AspNetCore.Authentication;
using CMS.Membership;
using Microsoft.AspNetCore.Identity;

namespace Identity
{
    /// <summary>
    /// Manager of user account operations.
    /// </summary>
    public interface IAccountManager : IService
    {
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="uploadModel">View model taken from the user.</param>
        /// <param name="emailConfirmed">Signals if email confirmation is required.</param>
        /// <param name="requestContext">Request context.</param>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<RegisterResultState>> RegisterAsync(RegisterViewModel uploadModel, bool emailConfirmed, HttpRequest request);

        /// <summary>
        /// Confirms the user account creation via email.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Confirmation token.</param>
        /// <param name="requestContext">Request context.</param>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<ConfirmUserResultState>> ConfirmUserAsync(int userId, string token, HttpRequest request);

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="uploadModel">Credentials view model taken from the user.</param>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<SignInResultState>> SignInAsync(SignInViewModel uploadModel);

        /// <summary>
        /// Signs the user out.
        /// </summary>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<SignOutResultState>> SignOutAsync();

        /// <summary>
        /// Sends a unique URL with a reset token to an email address.
        /// </summary>
        /// <param name="uploadModel">Email address taken from the user.</param>
        /// <param name="requestContext">Request context.</param>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<ForgotPasswordResultState>> ForgotPasswordAsync(EmailViewModel uploadModel, HttpRequest request);

        /// <summary>
        /// Verifies the token sent via <see cref="ForgotPasswordAsync(EmailViewModel, RequestContext)"/>.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Verification token.</param>
        /// <returns>A view model to reset the password.</returns>
        Task<IdentityManagerResult<ResetPasswordResultState, ResetPasswordViewModel>> VerifyResetPasswordTokenAsync(int userId, string token);

        /// <summary>
        /// Resets the user password.
        /// </summary>
        /// <param name="uploadModel">New passwords taken from the user.</param>
        /// <returns>An operation result.</returns>
        Task<IdentityManagerResult<ResetPasswordResultState>> ResetPasswordAsync(ResetPasswordViewModel uploadModel);

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string returnUrl);

        Task<Microsoft.AspNetCore.Identity.ExternalLoginInfo> GetExternalLoginInfoAsync();

        Task<IdentityManagerResult<SignInResultState, SignInViewModel>> SignInExternalAsync(Microsoft.AspNetCore.Identity.ExternalLoginInfo loginInfo);
    }
}