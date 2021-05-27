using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using Kentico.Content.Web.Mvc;

using Core.Configuration;
using Identity;
using Identity.Attributes;
using Identity.Models;
using Identity.Models.Profile;
using MedioClinic.Controllers;
using MedioClinic.Models;

using PageMetadata = MedioClinic.Models.PageMetadata;
using XperienceAdapter.Localization;
using Microsoft.Extensions.Localization;

namespace MedioClinic.Areas.Identity.Controllers
{
    // In production, use [RequireHttps].
    public class ProfileController : BaseIdentityController
    {
        private readonly IProfileManager _profileManager;

        public ProfileController(
            ILogger<ProfileController> logger, 
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IPageUrlRetriever pageUrlRetriever,
            IProfileManager profileManager)
            : base(logger, optionsMonitor, stringLocalizer, pageUrlRetriever)
        {
            _profileManager = profileManager ?? throw new ArgumentNullException(nameof(profileManager));
        }

        // GET: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient)]
        public async Task<IActionResult> Index() =>
            await GetProfileAsync();

        // POST: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(PageViewModel<IUserViewModel> uploadModel)
        {
            var message = ConcatenateContactAdmin("General.Error");

            if (ModelState.IsValid)
            {
                var profileResult = await _profileManager.PostProfileAsync(uploadModel.Data);

                switch (profileResult.ResultState)
                {
                    case PostProfileResultState.UserNotFound:
                        message = ConcatenateContactAdmin("Identity.UserNotFound");
                        break;
                    case PostProfileResultState.UserNotMapped:
                    case PostProfileResultState.UserNotUpdated:
                        message = ConcatenateContactAdmin("Identity.Profile.UserNotUpdated");
                        break;
                    case PostProfileResultState.UserUpdated:
                        message = Localize("Identity.Profile.UserUpdated");
                        break;
                }

                var metadata = new PageMetadata
                {
                    Title = profileResult.Data.PageTitle ?? ErrorTitle
                };

                var model = GetPageViewModel(metadata, profileResult.Data.UserViewModel, message);

                return View(model);
            }

            return await GetProfileAsync();
        }

        /// <summary>
        /// Displays the user profile.
        /// </summary>
        /// <returns>Either a user profile page, or a not-found page.</returns>
        private async Task<ActionResult> GetProfileAsync()
        {
            var userName = HttpContext.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                var profileResult = await _profileManager.GetProfileAsync(userName);

                var metadata = new PageMetadata
                {
                    Title = profileResult.Data.PageTitle
                };

                if (profileResult.Success)
                {
                    var model = GetPageViewModel(metadata, profileResult.Data.UserViewModel);

                    return View(model);
                }
            }

            return UserNotFound();
        }

        /// <summary>
        /// Displays a not-found page.
        /// </summary>
        /// <returns>A not-found page.</returns>
        private ActionResult UserNotFound()
        {
            var metadata = new PageMetadata
            {
                Title = ErrorTitle
            };

            var message = Localize("Identity.UserNotFound");

            return View("UserMessage", GetPageViewModel(metadata, message: message, messageType: MessageType.Error));
        }
    }
}
