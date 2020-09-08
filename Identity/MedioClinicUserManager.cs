using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Kentico.Membership;

using Identity.Models;
using CMS.Helpers;
using System.Security.Claims;
using CMS.Base;
using System.Threading.Tasks;

namespace Identity
{
    public class MedioClinicUserManager : ApplicationUserManager<MedioClinicUser>, IMedioClinicUserManager<MedioClinicUser>
    {
        private ISiteService _siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MedioClinicUserManager"/> class.
        /// </summary>
        /// <param name="store">The persistence store the manager will operate over.</param>
        /// <param name="optionsAccessor">The accessor used to access the <see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />.</param>
        /// <param name="passwordHasher">The password hashing implementation to use when saving passwords.</param>
        /// <param name="userValidators">A collection of <see cref="T:Microsoft.AspNetCore.Identity.IUserValidator`1" /> to validate users against.</param>
        /// <param name="passwordValidators">A collection of <see cref="T:Microsoft.AspNetCore.Identity.IPasswordValidator`1" /> to validate passwords against.</param>
        /// <param name="keyNormalizer">The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> to use when generating index keys for users.</param>
        /// <param name="errors">The <see cref="T:Microsoft.AspNetCore.Identity.IdentityErrorDescriber" /> used to provider error messages.</param>
        /// <param name="services">The <see cref="T:System.IServiceProvider" /> used to resolve services.</param>
        /// <param name="logger">The logger used to log messages, warnings and errors.</param>
        public MedioClinicUserManager(IUserStore<MedioClinicUser> store,
            IOptionsSnapshot<IdentityOptions> optionsAccessor,
            IPasswordHasher<MedioClinicUser> passwordHasher,
            IEnumerable<IUserValidator<MedioClinicUser>> userValidators,
            IEnumerable<IPasswordValidator<MedioClinicUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<MedioClinicUserManager> logger,
            ISiteService siteService
            ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _siteService = siteService;
        }

        public IUserStore<MedioClinicUser> UserStore => Store;

        public async Task<IdentityResult> CreateExternalUser(ExternalLoginInfo loginInfo)
        {
            // Prepares a new user entity based on the external login data
            MedioClinicUser user = new MedioClinicUser
            {
                UserName = ValidationHelper.GetSafeUserName(loginInfo.Principal.FindFirstValue(ClaimTypes.Name) ??
                                                            loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                                                            _siteService.CurrentSite.SiteName),
                Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                Enabled = true, // The user is enabled by default
                IsExternal = true // IsExternal must always be true for users created via external authentication
                                  // Set any other required user properties using the data available in loginInfo
            };

            // Attempts to create the user in the Xperience database
            IdentityResult result = await CreateAsync(user);
            if (result.Succeeded)
            {
                // If the user was created successfully, creates a mapping between the user and the given authentication provider
                result = await AddLoginAsync(user, loginInfo);
            }

            return result;
        }
    }
}
