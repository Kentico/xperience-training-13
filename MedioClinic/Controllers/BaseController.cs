using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;

using Core.Configuration;
using Identity.Models;
using MedioClinic.Models;

namespace MedioClinic.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;

        protected readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        protected string ErrorTitle => Localize("General.Error");

        public BaseController(ILogger<BaseController> logger, IOptionsMonitor<XperienceOptions> optionsMonitor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        protected PageViewModel GetPageViewModel(
            IPageMetadata pageMetadata,
            string? message = default,
            bool displayMessage = true,
            bool displayAsRaw = default,
            MessageType messageType = MessageType.Info)
            =>
            PageViewModel.GetPageViewModel(
                pageMetadata,
                message, 
                displayMessage, 
                displayAsRaw, 
                messageType);

        protected PageViewModel<TViewModel> GetPageViewModel<TViewModel>(
            IPageMetadata pageMetadata,
            TViewModel data,
            string? message = default,
            bool displayMessage = true, 
            bool displayAsRaw = default, 
            MessageType messageType = MessageType.Info)
            =>
            PageViewModel<TViewModel>.GetPageViewModel(
                data, 
                pageMetadata,
                message, 
                displayMessage, 
                displayAsRaw, 
                messageType);

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
                null,
                uploadModel.Data,
                Localize("General.InvalidInput.Message"),
                true,
                false,
                MessageType.Error);

            return View(viewModel);
        }

        // TODO: Create a middle-tier BaseIdentityController.
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
