using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using Business.Services;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;

using Core.Configuration;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageUploaderController : ControllerBase
    {
        private readonly IPageBuilderDataContextRetriever _pageBuilderDataContextRetriever;

        private readonly IPageRetriever _pageRetriever;

        private readonly ISiteService _siteService;

        private readonly IFileService _fileService;

        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly IMediaFileRepository _mediaFileRepository;

        public ImageUploaderController(
            IPageBuilderDataContextRetriever pageBuilderDataContextRetriever,
            IPageRetriever pageRetriever,
            ISiteService siteService,
            IFileService fileService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IMediaFileRepository mediaFileRepository)
        {
            _pageBuilderDataContextRetriever = pageBuilderDataContextRetriever ?? throw new ArgumentNullException(nameof(pageBuilderDataContextRetriever));
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int? pageId, string? mediaLibraryName)
        {
            if (pageId is null)
            {
                return UnprocessableEntity($"The page ID wasn't specified.");
            }

            if (string.IsNullOrEmpty(mediaLibraryName))
            {
                return UnprocessableEntity($"The media library name was not specified. Set it up in the widget configuration dialog.");
            }

            var file = Request.Form?.Files?.FirstOrDefault();

            if (file is null)
            {
                return UnprocessableEntity("There was no file to upload.");
            }

            var dataContext = _pageBuilderDataContextRetriever.Retrieve();

            if (!dataContext.EditMode)
            {
                return Unauthorized("The page is not in edit mode.");
            }

            if ((await CheckPagePermissionsAsync(pageId.Value)) == false)
            {
                return StatusCode((int)System.Net.HttpStatusCode.Forbidden);
            }

            var allowedExtensions = _optionsMonitor.CurrentValue.MediaLibraryOptions?.AllowedImageExtensions;
            var fileSizeLimit = _optionsMonitor.CurrentValue.MediaLibraryOptions?.FileSizeLimit;

            if (allowedExtensions?.Any() == true && fileSizeLimit.HasValue)
            {
                using (var processedFile = await _fileService.ProcessFormFileAsync(file, allowedExtensions, fileSizeLimit.Value!))
                {
                    if (processedFile.ResultState == Business.Models.FormFileResultState.FileOk)
                    {
                        Guid imageGuid = default;

                        try
                        {
                            imageGuid = await _mediaFileRepository.AddMediaFileAsync(processedFile.UploadedFile, mediaLibraryName, checkPermisions: true);
                        }
                        catch (PermissionException ex)
                        {
                            return StatusCode((int)System.Net.HttpStatusCode.Forbidden);
                        }

                        if (imageGuid != default)
                        {
                            return Ok(new { guid = imageGuid });
                        }
                    }
                }
            }

            return NotFound();
        }

        /// <summary>
        /// Checks if the user has enough permissions to edit the page.
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private async Task<bool> CheckPagePermissionsAsync(int pageId)
        {
            var cacheKey = $"{nameof(ImageUploaderController)}|{nameof(Upload)}|{pageId}";

            var page = (await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query.WithID(pageId),
                cache => cache
                    .Key(cacheKey)
                    .Dependencies((pages, builder) => builder.Pages(pages))))
                .FirstOrDefault();

            return page?.CheckPermissions(PermissionsEnum.Modify, _siteService.CurrentSite.SiteName, MembershipContext.AuthenticatedUser) ?? false;
        }
    }
}
