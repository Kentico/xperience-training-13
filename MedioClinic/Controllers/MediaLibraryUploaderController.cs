using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Business.Services;

using CMS.Helpers;

using Common.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaLibraryUploaderController : ControllerBase
    {
        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        private readonly IFileService _fileService;

        private readonly IMediaFileRepository _mediaFileRepository;

        public MediaLibraryUploaderController(
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IFileService fileService,
            IMediaFileRepository mediaFileRepository)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string? fileInputElementId, int? mediaLibraryId)
        {
            var result = new MediaLibraryUploaderResult
            {
                fileInputElementId = fileInputElementId
            };

            if (string.IsNullOrEmpty(fileInputElementId))
            {
                result.error = $"The {nameof(fileInputElementId)} parameter was missing.";

                return JsonWithStatusCode(result, StatusCodes.Status422UnprocessableEntity);
            }

            if (!mediaLibraryId.HasValue)
            {
                result.error = $"The {nameof(mediaLibraryId)} parameter was missing.";

                return JsonWithStatusCode(result, StatusCodes.Status422UnprocessableEntity);
            }

            var file = Request.Form?.Files?.FirstOrDefault();

            if (file is null)
            {
                result.error = "There was no file to upload.";

                return JsonWithStatusCode(result, StatusCodes.Status422UnprocessableEntity);
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
                            imageGuid = await _mediaFileRepository.AddMediaFileAsync(processedFile.UploadedFile, mediaLibraryId.Value, checkPermisions: false);
                        }
                        catch (Exception ex)
                        {
                            result.error = "There was an error when saving the uploaded file.";

                            return JsonWithStatusCode(result, StatusCodes.Status422UnprocessableEntity);
                        }

                        if (imageGuid != default)
                        {
                            result.fileGuid = imageGuid;

                            return JsonWithStatusCode(result, StatusCodes.Status200OK);
                        }
                    }
                }
            }

            return NotFound();
        }

        private IActionResult JsonWithStatusCode(object value, int statusCode) =>
            new ObjectResult(value)
            {
                StatusCode = statusCode
            };
    }

    internal class MediaLibraryUploaderResult
    {
        public string fileInputElementId { get; set; }

        public Guid fileGuid { get; set; }

        public string? error { get; set; }
    }
}
