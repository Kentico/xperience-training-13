using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.Helpers;

using Core.Configuration;
using Identity.Models;
using MedioClinic.Models;

namespace MedioClinic.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;

        protected readonly ISiteService _siteService;

        protected readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        protected string ErrorTitle => Localize("General.Error");

        public BaseController(ILogger<BaseController> logger, ISiteService siteService, IOptionsMonitor<XperienceOptions> optionsMonitor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        protected PageViewModel GetPageViewModel(
            string title,
            string? message = default,
            bool displayMessage = true,
            bool displayAsRaw = default,
            MessageType messageType = MessageType.Info) =>
            PageViewModel.GetPageViewModel(title, _siteService, message, displayMessage, displayAsRaw, messageType);

        protected PageViewModel<TViewModel> GetPageViewModel<TViewModel>(
            TViewModel data,
            string title,
            string? message = default,
            bool displayMessage = true,
            bool displayAsRaw = default,
            MessageType messageType = MessageType.Info)
            //where TViewModel : class, new() =>
            =>
            PageViewModel<TViewModel>.GetPageViewModel(data, title, _siteService, message, displayMessage, displayAsRaw, messageType);

        protected string Localize(string resourceKey) =>
            ResHelper.GetString(resourceKey);

        protected string ConcatenateContactAdmin(string messageKey) =>
            Localize(messageKey)
                + " "
                + Localize("General.ContactAdministrator");

        protected ActionResult InvalidInput<TUploadViewModel>(
            PageViewModel<TUploadViewModel> uploadModel)
            where TUploadViewModel : class, new()
        {
            var viewModel = GetPageViewModel(
                uploadModel.Data,
                Localize("General.InvalidInput.Title"),
                Localize("General.InvalidInput.Message"),
                true,
                false,
                MessageType.Error);

            return View(viewModel);
        }

        protected void AddIdentityErrors<TResultState>(IdentityManagerResult<TResultState> result)
            where TResultState : Enum
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
