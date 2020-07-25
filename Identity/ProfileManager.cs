using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EnumsNET;

using CMS.Helpers;

using Abstractions;
using XperienceAdapter.Logging;
using Identity.Extensions;
using Identity.Models;
using Identity.Models.Account;
using Identity.Models.Profile;
using Identity.Services;

namespace Identity
{
    public class ProfileManager : BaseIdentityManager, IProfileManager
    {
        //public IAvatarRepository AvatarRepository { get; }

        protected readonly IFileService _fileService;

        protected readonly IUserModelService _userModelService;

        public ProfileManager(
            ILogger<ProfileManager> logger,
            //IAvatarRepository avatarRepository,
            IFileService fileService,
            IUserModelService userModelService,
            IMedioClinicUserManager<MedioClinicUser> userManager
            //IMedioClinicUserStore userStore,
            )
                : base(logger, userManager)
        {
            //AvatarRepository = avatarRepository ?? throw new ArgumentNullException(nameof(avatarRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _userModelService = userModelService ?? throw new ArgumentNullException(nameof(userModelService));
        }

        public async Task<IdentityManagerResult<GetProfileResultState, (IUserViewModel UserViewModel, string PageTitle)>>
            GetProfileAsync(string userName)
        {
            var profileResult = new IdentityManagerResult<GetProfileResultState, (IUserViewModel, string)>();
            MedioClinicUser user = null;

            try
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            catch (Exception ex)
            {
                var pr = profileResult as IdentityManagerResult<GetProfileResultState>;
                HandleException(nameof(GetProfileAsync), ex, ref pr);
                profileResult.ResultState = GetProfileResultState.UserNotFound;

                return profileResult;
            }

            (var model, var title) = GetViewModelByUserRoles(user);

            if (model != null)
            {
                profileResult.Success = true;
                profileResult.ResultState = GetProfileResultState.UserFound;
                profileResult.Data = (model, title);
            }

            return profileResult;
        }

        public async Task<IdentityManagerResult<PostProfileResultState, (IUserViewModel UserViewModel, string PageTitle)>>
            PostProfileAsync(IUserViewModel uploadModel)
        {
            var profileResult = new IdentityManagerResult<PostProfileResultState, (IUserViewModel, string)>();
            var userTitle = ResHelper.GetString("General.User");
            var userDoesntExistTitle = ResHelper.GetString("Adm.User.NotExist");
            profileResult.Data = (uploadModel, userTitle);
            MedioClinicUser user = null;

            try
            {
                user = await _userManager.FindByIdAsync(uploadModel.CommonUserViewModel.Id.ToString());
            }
            catch (Exception ex)
            {
                HandlePostProfileException(ref profileResult, ex, PostProfileResultState.UserNotFound);
                profileResult.Data = (uploadModel, userDoesntExistTitle);

                return profileResult;
            }

            var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
            {
                { (nameof(MedioClinicUser.Email), typeof(string)), uploadModel.CommonUserViewModel.EmailViewModel.Email },
            };

            try
            {
                // Map the common user properties.
                user = _userModelService.MapToMedioClinicUser(uploadModel.CommonUserViewModel, user, commonUserModelCustomMappings);

                // Map all other potential properties of specific models (patient, doctor, etc.)
                user = _userModelService.MapToMedioClinicUser(uploadModel, user);
            }
            catch (Exception ex)
            {
                HandlePostProfileException(ref profileResult, ex, PostProfileResultState.UserNotMapped);

                return profileResult;
            }

            try
            {
                // We need to use the user store directly due to the design of Microsoft.AspNet.Identity.Core.UserManager.UpdateAsync().
                await _userManager.UserStore.UpdateAsync(user, CancellationToken.None);

                //var avatarFile = uploadModel.CommonUserViewModel.AvatarFile;

                //if (avatarFile != null)
                //{
                //    var avatarBinary = _fileService.GetPostedFileBinary(avatarFile);
                //    AvatarRepository.UploadUserAvatar(user, avatarBinary);
                //}
            }
            catch (Exception ex)
            {
                HandlePostProfileException(ref profileResult, ex, PostProfileResultState.UserNotUpdated);

                return profileResult;
            }

            (var model, var title) = GetViewModelByUserRoles(user);

            if (model != null)
            {
                profileResult.Success = true;
                profileResult.ResultState = PostProfileResultState.UserUpdated;
                profileResult.Data = (model, title);
            }

            return profileResult;
        }

        /// <summary>
        /// Computes the user view model, based on roles.
        /// </summary>
        /// <param name="user">User to compute the view model by.</param>
        /// <param name="requestContext">Request context.</param>
        /// <param name="forceAvatarFileOverwrite">Flag that signals the need to update the app-local physical avatar file.</param>
        /// <returns>The view model and a page title.</returns>
        protected (IUserViewModel UserViewModel, string PageTitle) GetViewModelByUserRoles(MedioClinicUser user)
        {
            if (user != null)
            {
                var roles = user.Roles.ToMedioClinicRoles();
                //string avatarPhysicalPath = EnsureAvatarPhysicalPath(user, requestContext, forceAvatarFileOverwrite);

                //var avatarRelativePath = avatarPhysicalPath != null
                //        ? _fileService.GetServerRelativePath(requestContext.HttpContext.Request, avatarPhysicalPath)
                //        : string.Empty;

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(CommonUserViewModel.EmailViewModel), typeof(EmailViewModel)), new EmailViewModel { Email = user.Email } },
                    { (nameof(CommonUserViewModel.UserName), typeof(string)), user.UserName }
                };

                object mappedParentModel = null;

                try
                {
                    // Map the common user properties.
                    var mappedCommonUserModel = _userModelService.MapToCustomModel(user, typeof(CommonUserViewModel), commonUserModelCustomMappings);

                    Type userViewModelType = FlagEnums.HasAnyFlags(roles, Roles.Doctor) ? typeof(DoctorViewModel) : typeof(PatientViewModel);

                    var parentModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                    {
                        { (nameof(CommonUserViewModel), typeof(CommonUserViewModel)), mappedCommonUserModel }
                    };

                    // Map all other potential properties of specific models (patient, doctor, etc.)
                    mappedParentModel = _userModelService.MapToCustomModel(user, userViewModelType, parentModelCustomMappings);
                }
                catch (Exception ex)
                {
                    _logger.LogEvent(LogLevel.Error, nameof(GetViewModelByUserRoles), exception: ex);

                    return (null, null);
                }

                return ((IUserViewModel)mappedParentModel, GetRoleTitle(roles));
            }

            return (null, null);
        }

        /// <summary>
        /// Gets a role title.
        /// </summary>
        /// <param name="roles">Role of the user.</param>
        /// <returns>A friendly name of the role.</returns>
        protected string GetRoleTitle(Roles roles) =>
            FlagEnums.HasAnyFlags(roles, Roles.Doctor)
                ? ResHelper.GetString("ProfileManager.GetRoleTitle.Doctor")
                : ResHelper.GetString("ProfileManager.GetRoleTitle.Patient");

        /// <summary>
        /// Handles exceptions raised in <see cref="ProfileManager.PostProfileAsync(IUserViewModel, RequestContext)"/>
        /// </summary>
        /// <param name="profileResult">The profile manager result.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="resultState">The result state.</param>
        protected void HandlePostProfileException(ref IdentityManagerResult<PostProfileResultState, (IUserViewModel, string)> profileResult, Exception ex, PostProfileResultState resultState)
        {
            var pr = profileResult as IdentityManagerResult<PostProfileResultState>;
            HandleException(nameof(PostProfileAsync), ex, ref pr);
            profileResult.ResultState = resultState;
        }
    }
}