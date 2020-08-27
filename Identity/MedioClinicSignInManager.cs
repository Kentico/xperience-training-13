using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Identity
{
    public class MedioClinicSignInManager : SignInManager<MedioClinicUser>, IMedioClinicSignInManager<MedioClinicUser>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MedioClinicSignInManager"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager"/> used to retrieve users from and persist users.</param>
        /// <param name="contextAccessor">The accessor used to access the <see cref="HttpContext"/>.</param>
        /// <param name="claimsFactory">The factory to use to create claims principals for a user.</param>
        /// <param name="optionsAccessor">The accessor used to access the <see cref="IdentityOptions"/>.</param>
        /// <param name="logger">The logger used to log messages, warnings and errors.</param>
        /// <param name="schemes">The scheme provider that is used enumerate the authentication schemes.</param>
        /// <param name="confirmation">The <see cref="IUserConfirmation{MedioClinicUser}"/> used check whether a user account is confirmed.</param>
        public MedioClinicSignInManager(IMedioClinicUserManager<MedioClinicUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<MedioClinicUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<MedioClinicSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<MedioClinicUser> confirmation)
            : base((UserManager<MedioClinicUser>)userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}
