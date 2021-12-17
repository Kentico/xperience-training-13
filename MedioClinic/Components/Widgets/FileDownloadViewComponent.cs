using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XperienceAdapter.Repositories;

namespace MedioClinic.Components.Widgets
{
    public class FileDownloadViewComponent : ViewComponent
    {
        private readonly IMediaFileRepository _mediaFileRepository;

        public FileDownloadViewComponent(IMediaFileRepository mediaFileRepository)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }

        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<FileDownloadProperties> componentViewModel)
        {
            var properties = componentViewModel?.Properties;
            var mediaLibraryId = properties?.MediLibraryId;
            var fileGuid = properties?.FileGuid;
            var secured = properties?.SecuredDownload ?? false;
            string? mediaLibraryName = default;

            if (int.TryParse(mediaLibraryId, out var mediaLibraryIdAsInt))
            {
                mediaLibraryName = await _mediaFileRepository.GetLibraryNameAsync(mediaLibraryIdAsInt);
            }

            var model = (mediaLibraryName, fileGuid, secured);

            return View("~/Components/Widgets/_FileDownload.cshtml", model);
        }
    }
}
