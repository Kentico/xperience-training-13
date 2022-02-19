using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;

using Common.Configuration;
using MedioClinic.Models;
using Microsoft.Extensions.Localization;
using XperienceAdapter.Localization;

namespace MedioClinic.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;

        protected readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        protected readonly IStringLocalizer<SharedResource> _stringLocalizer;

        protected string ErrorTitle => Localize("General.Error");

        public BaseController(ILogger<BaseController> logger,
                              IOptionsMonitor<XperienceOptions> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
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

        /// <summary>
        /// Localizes a string resource.
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        /// <returns></returns>
        protected string Localize(string resourceKey) => _stringLocalizer[resourceKey];

        /// <summary>
        /// Localizes a string resource using a pattern.
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        /// <param name="args">The values to format the string with.</param>
        /// <returns></returns>
        protected string Localize(string resourceKey, params object[] args) => _stringLocalizer[resourceKey, args];

        /// <summary>
        /// Concatenates an error message with a "contact administrator" suffix.
        /// </summary>
        /// <param name="messageKey">A resource key of the error message.</param>
        /// <returns>Concatenated error messages.</returns>
        protected string ConcatenateContactAdmin(string messageKey) =>
            Localize(messageKey)
                + " "
                + Localize("General.ContactAdministrator");

        /// <summary>
        /// Returns an error message about an invalid user input.
        /// </summary>
        /// <typeparam name="TUploadViewModel">Upload view model.</typeparam>
        /// <param name="uploadModel">Upload view model.</param>
        /// <returns>A message view.</returns>
        protected ActionResult InvalidInput<TUploadViewModel>(
            PageViewModel<TUploadViewModel> uploadModel)
            where TUploadViewModel : class, new()
        {
            var metadata = new Models.PageMetadata
            {
                Title = Localize("General.InvalidInput.Title")
            };

            var viewModel = GetPageViewModel(
                metadata,
                uploadModel.Data,
                Localize("General.InvalidInput.Message"),
                true,
                false,
                MessageType.Error);

            return View(viewModel);
        }
    }
}
