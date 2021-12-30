using CMS.Activities;

using Core.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XperienceAdapter.Activities;
using XperienceAdapter.Localization;
using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    public class DownloadController : BaseController
    {
        private readonly IWebHostEnvironment _environment;

        private readonly IMediaFileRepository _mediaFileRepository;

        private readonly IActivityLogService _activityLogService;

        public DownloadController(
            ILogger<DownloadController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IWebHostEnvironment environment,
            IMediaFileRepository mediaFileRepository,
            IActivityLogService activityLogService
            )
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
        }

        public async Task<IActionResult> Public(Guid? fileGuid, CancellationToken cancellationToken)
        {
            if (fileGuid is null)
            {
                return BadRequest();
            }

            var mediaFile = await _mediaFileRepository.GetMediaFileAsync(fileGuid.Value, cancellationToken);

            if (mediaFile != null)
            {
                var initializer = new FileDownloadActivityInitializer(DownloadType.Public, mediaFile.MediaFileUrl?.RelativePath!);
                _activityLogService.Log(initializer);
                var etagValue = mediaFile.LastModified?.DateTime.ToString() ?? mediaFile.GetHashCode().ToString();
                var hashedEtagValue = GetHash(etagValue!);
                var etag = new EntityTagHeaderValue($"\"{hashedEtagValue}\"", false);
                var path = mediaFile.MediaFileUrl?.DirectPath.Trim('~').Replace('/', '\\');
                var completePath = $"{_environment.ContentRootPath}{path}";

                // ControllerBase.File() takes care of the object disposal, hence no 'using' statement.
                var stream = System.IO.File.OpenRead(completePath);

                return File(stream, mediaFile.MimeType, mediaFile.LastModified, etag);
            }

            return NotFound();
        }

        private static string GetHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null!;
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashedBytes = sha256.ComputeHash(bytes);

                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
            }
        }
    }
}
