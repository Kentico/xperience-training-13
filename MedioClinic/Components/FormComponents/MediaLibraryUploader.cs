using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using CMS.Core;
using CMS.Membership;

using Kentico.Forms.Web.Mvc;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using XperienceAdapter.Repositories;

namespace MedioClinic.Components.FormComponents
{
    public class MediaLibraryUploader : FormComponent<MediaLibraryUploaderProperties, string>
    {
        private readonly IMediaFileRepository _mediaFileRepository;

        [BindableProperty]
        public string FileGuidAsString
        {
            get => FileGuid?.ToString() ?? string.Empty;

            set
            {
                var parsed = Guid.TryParse(value, out Guid guid);

                if (parsed)
                {
                    FileGuid = guid;
                }
            }
        }

        public Guid? FileGuid { get; set; }

        public MediaLibraryUploader(IMediaFileRepository mediaFileRepository)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }

        public override string GetValue() => FileGuidAsString;

        public override void SetValue(string value) => FileGuidAsString = value;

        public override bool CustomAutopostHandling => true;

        public bool ShowViewFileLink =>
            MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.BIZFORM, "ReadData");

        [ValidateNever]
        public string ImageRelativePath
        {
            get
            {
                var parsed = int.TryParse(Properties?.MediaLibraryId, out int libraryId);

                if (parsed && FileGuid.HasValue)
                {
                    var mediaFile = _mediaFileRepository.GetMediaFile(FileGuid.Value);

                    return mediaFile?.MediaFileUrl?.RelativePath;
                }

                return null;
            }
        }
    }
}
