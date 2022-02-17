using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Common.Configuration;

using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

namespace MedioClinic.Components.Widgets
{
    public class SlideshowViewComponent : ViewComponent
    {
        private readonly IMediaFileRepository _mediaFileRepository;

        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        public SlideshowViewComponent(IMediaFileRepository mediaFileRepository, IOptionsMonitor<XperienceOptions> optionsMonitor)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<SlideshowProperties> componentViewModel)
        {
            var properties = componentViewModel?.Properties;
            var imageGuids = properties?.ImageGuids;
            List<MediaLibraryFile> mediaFiles = new List<MediaLibraryFile>();

            if (imageGuids?.Any() == true && !string.IsNullOrEmpty(properties?.MediaLibraryName))
            {
                mediaFiles.AddRange(await _mediaFileRepository.GetMediaFilesAsync(properties.MediaLibraryName, imageGuids));
                componentViewModel.CacheDependencies.CacheKeys = mediaFiles.Select(file => $"mediafile|{file.Guid}").ToList();
            }

            var model = new SlideshowViewModel
            {
                Images = ReorderFilesByGuids(mediaFiles, imageGuids),
                Width = properties.Width,
                Height = properties.Height,
                EnforceDimensions = properties.EnforceDimensions,
                TransitionDelay = properties.TransitionDelay,
                TransitionSpeed = properties.TransitionSpeed,
                DisplayArrowSigns = properties.DisplayArrowSigns
            };

            model.MediaLibraryViewModel.LibraryName = properties.MediaLibraryName;
            model.MediaLibraryViewModel.AllowedFileExtensions = _optionsMonitor.CurrentValue.MediaLibraryOptions.AllowedImageExtensions.ToHashSet();

            return View("~/Components/Widgets/_Slideshow.cshtml", model);
        }

        private static IEnumerable<MediaLibraryFile> ReorderFilesByGuids(IEnumerable<MediaLibraryFile> mediaLibraryFiles, IEnumerable<Guid> guids)
        {
            if (guids is null)
            {
                yield break;
            }

            foreach (var guid in guids)
            {
                var match = mediaLibraryFiles.FirstOrDefault(dto => dto.Guid.Equals(guid));

                if (match != null)
                {
                    yield return match;
                }
            }
        }
    }
}
