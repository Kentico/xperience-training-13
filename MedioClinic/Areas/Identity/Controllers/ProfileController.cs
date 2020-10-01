using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;

using Business.Configuration;
using Identity;
using Identity.Attributes;
using Identity.Models;
using Identity.Models.Profile;
using MedioClinic.Controllers;
using MedioClinic.Models;

namespace MedioClinic.Areas.Identity.Controllers
{
    // In production, use [RequireHttps].
    [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 0, Location = ResponseCacheLocation.None)]
    public class ProfileController : BaseController
    {
        private readonly IProfileManager _profileManager;

        public ProfileController(ILogger<ProfileController> logger, ISiteService siteService, IOptionsMonitor<XperienceOptions> optionsMonitor, IProfileManager profileManager)
            : base(logger, siteService, optionsMonitor)
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
            var message = ConcatenateContactAdmin("Error.Message");

            if (ModelState.IsValid)
            {
                var profileResult = await _profileManager.PostProfileAsync(uploadModel.Data);

                switch (profileResult.ResultState)
                {
                    case PostProfileResultState.UserNotFound:
                        message = ConcatenateContactAdmin("Controllers.Profile.Index.UserNotFound.Message");
                        break;
                    case PostProfileResultState.UserNotMapped:
                    case PostProfileResultState.UserNotUpdated:
                        message = ConcatenateContactAdmin("Controllers.Profile.Index.UserNotUpdated.Message");
                        break;
                    case PostProfileResultState.UserUpdated:
                        message = Localize("Controllers.Profile.Index.UserUpdated.Message");
                        break;
                }

                var model = GetPageViewModel(profileResult.Data.UserViewModel, profileResult.Data.PageTitle ?? ErrorTitle, message);

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

                if (profileResult.Success)
                {
                    var model = GetPageViewModel(profileResult.Data.UserViewModel, profileResult.Data.PageTitle);

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
            var message = Localize("General.UserNotFound");

            return View("UserMessage", GetPageViewModel(title: ErrorTitle, message, messageType: MessageType.Error));
        }
    }
}
