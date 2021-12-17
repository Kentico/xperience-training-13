using CMS.Activities;
using CMS.ContactManagement;
using CMS.DataEngine;

using Core.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using XperienceAdapter.Activities;
using XperienceAdapter.Localization;
using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    public class DownloadController : BaseController
    {
        private readonly IMediaFileRepository _mediaFileRepository;

        private readonly IActivityLogService _activityLogService;

        public DownloadController(
            ILogger<DownloadController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IMediaFileRepository mediaFileRepository,
            IActivityLogService activityLogService
            )
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
        }

        public async Task<IActionResult> Public(Guid? fileGuid, CancellationToken cancellationToken)
        {
            if (fileGuid is null)
            {
                return BadRequest();
            }

            //var currentContact = ContactManagementContext.GetCurrentContact(true);

            //var mediaFile = await _mediaFileRepository.GetMediaFileAsync(mediaLibraryId, path, cancellationToken);
            var mediaFile = await _mediaFileRepository.GetMediaFileAsync(fileGuid.Value, cancellationToken);

            if (mediaFile != null)
            {
                var initializer = new FileDownloadActivityInitializer(DownloadType.Public, mediaFile.MediaFileUrl?.RelativePath!);
                _activityLogService.Log(initializer);

                var etag = new EntityTagHeaderValue(mediaFile.LastModified?.DateTime.ToString(), false);

                return File(mediaFile.MediaFileUrl?.RelativePath, mediaFile.MimeType, mediaFile.LastModified, etag);
            }

            return NotFound();
        }
    }
}
