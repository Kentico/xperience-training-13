using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EnumsNET;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;

using XperienceAdapter.Logging;
using Core.Configuration;
using Business.Models;
using Business.Services;
using Identity.Extensions;
using Identity.Models;
using Identity.Models.Profile;
using Identity.Services;
using Microsoft.Extensions.Localization;
using XperienceAdapter.Localization;

namespace Identity
{
    public class ProfileManager : BaseIdentityManager, IProfileManager
    {
        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly IFileService _fileService;

        private readonly IUserModelService _userModelService;

        private readonly IAvatarService _avatarService;

        private readonly ISiteService _siteService;

        public ProfileManager(
            ILogger<ProfileManager> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IFileService fileService,
            IUserModelService userModelService,
            IAvatarService avatarService,
            ISiteService siteService,
            IMedioClinicUserManager<MedioClinicUser> userManager
            )
                : base(logger, stringLocalizer, userManager)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _userModelService = userModelService ?? throw new ArgumentNullException(nameof(userModelService));
            _avatarService = avatarService ?? throw new ArgumentNullException(nameof(avatarService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public async Task<IdentityManagerResult<GetProfileResultState, (IUserViewModel UserViewModel, string PageTitle)>>
            GetProfileAsync(string userName)
        {
            var profileResult = new IdentityManagerResult<GetProfileResultState, (IUserViewModel, string)>();
            MedioClinicUser user = default!;

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
                profileResult.Data = (model!, title!);
            }

            return profileResult;
        }

        public async Task<IdentityManagerResult<PostProfileResultState, (IUserViewModel UserViewModel, string PageTitle)>>
            PostProfileAsync(IUserViewModel uploadModel)
        {
            var profileResult = new IdentityManagerResult<PostProfileResultState, (IUserViewModel, string)>();
            var userTitle = Localize("General.User");
            var userDoesntExistTitle = Localize("Adm.User.NotExist");
            profileResult.Data = (uploadModel, userTitle);
            var currentUserId = MembershipContext.AuthenticatedUser.UserID;
            MedioClinicUser user = default!;

            if (currentUserId != uploadModel.CommonUserViewModel.Id)
            {
                profileResult.Data = (uploadModel, userDoesntExistTitle);

                return profileResult;
            }

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
                { (nameof(MedioClinicUser.Email), typeof(string)), uploadModel.CommonUserViewModel.EmailViewModel.Email! },
            };

            try
            {
                // Map the common user properties.
                user = _userModelService.MapToMedioClinicUser(uploadModel.CommonUserViewModel, user, commonUserModelCustomMappings);

                // Map all other potential properties of specific models (patient, doctor, etc.). The decision logic is omitted for brevity.
                user = _userModelService.MapToMedioClinicUser(uploadModel, user);
            }
            catch (Exception ex)
            {
                HandlePostProfileException(ref profileResult, ex, PostProfileResultState.UserNotMapped);

                return profileResult;
            }

            try
            {
                // We need to use the user store directly due to the design of Microsoft.AspNetCore.Identity.UserManager.UpdateAsync().
                await _userManager.UserStore.UpdateAsync(user, CancellationToken.None);

                profileResult = await SaveAvatarAsync(uploadModel, profileResult);
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
                profileResult.Data = (model!, title!);
            }

            return profileResult;
        }

        /// <summary>
        /// Computes the user view model, based on roles.
        /// </summary>
        /// <param name="user">User to compute the view model by.</param>
        /// <returns>The view model and a page title.</returns>
        private (IUserViewModel? UserViewModel, string? PageTitle) GetViewModelByUserRoles(MedioClinicUser user)
        {
            if (user != null)
            {
                var roles = user.Roles.ToMedioClinicRoles();

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(CommonUserViewModel.EmailViewModel), typeof(EmailViewModel)), new EmailViewModel { Email = user.Email } }
                };

                object mappedParentModel = default!;

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
        /// Saves the avatar binary into the database.
        /// </summary>
        /// <param name="uploadModel">The upload model.</param>
        /// <param name="profileResult">The result.</param>
        /// <returns></returns>
        private async Task<IdentityManagerResult<PostProfileResultState, (IUserViewModel UserViewModel, string PageTitle)>>
            SaveAvatarAsync(IUserViewModel uploadModel, IdentityManagerResult<PostProfileResultState, (IUserViewModel UserViewModel, string PageTitle)> profileResult)
        {
            var avatarFile = uploadModel.CommonUserViewModel.AvatarFile;
            var allowedExtensions = _optionsMonitor.CurrentValue.MediaLibraryOptions?.AllowedImageExtensions;
            var fileSizeLimit = _optionsMonitor.CurrentValue.MediaLibraryOptions?.FileSizeLimit;

            if (avatarFile != null && allowedExtensions?.Any() == true && fileSizeLimit.HasValue)
            {
                using (var processedFile = await _fileService.ProcessFormFileAsync(avatarFile, allowedExtensions, fileSizeLimit.Value))
                {
                    if (processedFile.ResultState == FormFileResultState.FileOk)
                    {
                        if (!_avatarService.UpdateAvatar(processedFile.UploadedFile, uploadModel.CommonUserViewModel.Id, _siteService.CurrentSite.SiteName))
                        {
                            var exception = new Exception("Updating of the avatar file failed.");
                            HandlePostProfileException(ref profileResult, exception, PostProfileResultState.UserNotUpdated);
                        }
                    }
                }
            }

            return profileResult;
        }

        /// <summary>
        /// Handles exceptions raised in <see cref="ProfileManager.PostProfileAsync(IUserViewModel, RequestContext)"/>
        /// </summary>
        /// <param name="profileResult">The profile manager result.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="resultState">The result state.</param>
        private void HandlePostProfileException(ref IdentityManagerResult<PostProfileResultState, (IUserViewModel, string)> profileResult, Exception ex, PostProfileResultState resultState)
        {
            var pr = profileResult as IdentityManagerResult<PostProfileResultState>;
            HandleException(nameof(PostProfileAsync), ex, ref pr);
            profileResult.ResultState = resultState;
        }

        /// <summary>
        /// Gets a role title.
        /// </summary>
        /// <param name="roles">Role of the user.</param>
        /// <returns>A friendly name of the role.</returns>
        private string GetRoleTitle(Roles roles) =>
            FlagEnums.HasAnyFlags(roles, Roles.Doctor)
                ? Localize("Identity.Profile.Doctor")
                : Localize("Identity.Profile.Patient");
    }
}