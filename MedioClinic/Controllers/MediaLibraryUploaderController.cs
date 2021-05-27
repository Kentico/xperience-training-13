using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Business.Services;

using CMS.Helpers;

using Core.Configuration;

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
            if (string.IsNullOrEmpty(fileInputElementId))
            {
                return UnprocessableEntity($"The {nameof(fileInputElementId)} parameter was missing.");
            }

            if (!mediaLibraryId.HasValue)
            {
                return UnprocessableEntity($"The {nameof(mediaLibraryId)} parameter was missing.");
            }

            var file = Request.Form?.Files?.FirstOrDefault();

            if (file is null)
            {
                return UnprocessableEntity("There was no file to upload.");
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
                            return UnprocessableEntity("There was an error when saving the uploaded file.");
                        }

                        if (imageGuid != default)
                        {
                            return Ok(new 
                            { 
                                fileInputElementId,
                                fileGuid = imageGuid
                            });
                        }
                    }
                }
            }

            return NotFound();
        }
    }
}
