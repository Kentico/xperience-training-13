using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;

using XperienceAdapter.Dtos;

namespace XperienceAdapter.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private int? _mediaLibraryId;

        private string? _mediaLibraryName;

        private readonly IMediaLibraryInfoProvider _mediaLibraryInfoProvider;

        private readonly IMediaFileInfoProvider _mediaFileInfoProvider;

        private readonly ISiteService _siteService;

        public int? MediaLibraryId
        {
            get => _mediaLibraryId == null && !string.IsNullOrEmpty(_mediaLibraryName)
                ? _mediaLibraryInfoProvider
                    .Get(_mediaLibraryName, _siteService.CurrentSite.SiteID)?
                    .LibraryID
                : _mediaLibraryId;

            set
            {
                if (value != null)
                {
                    _mediaLibraryId = value.Value;
                }
            }
        }

        public string? MediaLibraryName
        {
            get => string.IsNullOrEmpty(_mediaLibraryName) && _mediaLibraryId.HasValue
                ? _mediaLibraryInfoProvider
                    .Get(_mediaLibraryId.Value)
                    .LibraryName
                : _mediaLibraryName;

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _mediaLibraryName = value;
                }
            }
        }

        public MediaFileRepository(IMediaLibraryInfoProvider mediaLibraryInfoProvider, IMediaFileInfoProvider mediaFileInfoProvider, ISiteService siteService)
        {
            _mediaLibraryInfoProvider = mediaLibraryInfoProvider ?? throw new ArgumentNullException(nameof(mediaLibraryInfoProvider));
            _mediaFileInfoProvider = mediaFileInfoProvider ?? throw new ArgumentNullException(nameof(mediaFileInfoProvider));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public async Task<Guid> AddMediaFileAsync(string filePath, string? libraryFolderPath = default, bool checkPermissions = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path was not specified.", nameof(filePath));
            }

            return await AddMediaFileAsyncImplementation();

            async Task<Guid> AddMediaFileAsyncImplementation()
            {
                var siteId = _siteService.CurrentSite.SiteID;
                var siteName = _siteService.CurrentSite.SiteName;
                MediaLibraryInfo mediaLibraryInfo;

                try
                {
                    mediaLibraryInfo = await _mediaLibraryInfoProvider.GetAsync(MediaLibraryName, siteId)
                            ?? await _mediaLibraryInfoProvider.GetAsync(MediaLibraryId!.Value);
                }
                catch (Exception)
                {
                    throw new Exception($"The {MediaLibraryName} library was not found on the {siteName} site.");
                }

                if (checkPermissions && !mediaLibraryInfo.CheckPermissions(PermissionsEnum.Create, siteName, MembershipContext.AuthenticatedUser))
                {
                    throw new PermissionException(
                        $"The user {MembershipContext.AuthenticatedUser.FullName} lacks permissions to the {MediaLibraryName} library.");
                }

                MediaFileInfo mediaFile = !string.IsNullOrEmpty(libraryFolderPath)
                    ? new MediaFileInfo(filePath, mediaLibraryInfo.LibraryID, libraryFolderPath)
                    : new MediaFileInfo(filePath, mediaLibraryInfo.LibraryID);

                var fileInfo = FileInfo.New(filePath);
                mediaFile.FileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                mediaFile.FileExtension = fileInfo.Extension;
                mediaFile.FileMimeType = MimeTypeHelper.GetMimetype(fileInfo.Extension);
                mediaFile.FileSiteID = siteId;
                mediaFile.FileLibraryID = mediaLibraryInfo.LibraryID;
                mediaFile.FileSize = fileInfo.Length;
                _mediaFileInfoProvider.Set(mediaFile);

                return mediaFile.FileGUID;
            }
        }

        public async Task<MediaLibraryFile?> GetMediaFileDtoAsync(Guid fileGuid)
        {
            var mediaFileInfo = await _mediaFileInfoProvider.GetAsync(fileGuid, _siteService.CurrentSite.SiteID);

            return mediaFileInfo != null ? MapDtoProperties(mediaFileInfo) : null;
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(params Guid[] fileGuids)
        {
            var results = await GetQueryAsync((baseQuery) => baseQuery
                .WhereIn("FileGUID", fileGuids));

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(params string[] extensions)
        {
            var results = await GetQueryAsync((baseQuery) => baseQuery
                .WhereIn("FileExtension", extensions));

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(string path)
        {
            var results = await GetQueryAsync((baseQuery) => baseQuery
                .WhereStartsWith("FilePath", path));

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetAllAsync()
        {
            var results = await GetQueryAsync();

            return results.Select(item => MapDtoProperties(item));
        }

        public IEnumerable<MediaLibraryFile> GetAll() => GetAllAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Gets a query with an optional filter.
        /// </summary>
        /// <param name="filter">Optional filter.</param>
        /// <returns></returns>
        protected async Task<IEnumerable<MediaFileInfo>> GetQueryAsync(Func<ObjectQuery<MediaFileInfo>, ObjectQuery<MediaFileInfo>>? filter = default)
        {
            var baseQuery = _mediaFileInfoProvider.Get()
                .WhereEquals("FileLibraryID", MediaLibraryId);

            return filter != null
                ? await filter(baseQuery).GetEnumerableTypedResultAsync()
                : await baseQuery.GetEnumerableTypedResultAsync();
        }

        /// <summary>
        /// Maps DTO properties.
        /// </summary>
        /// <param name="mediaFileInfo">Xperience media file.</param>
        /// <returns>Media file DTO.</returns>
        protected MediaLibraryFile MapDtoProperties(MediaFileInfo mediaFileInfo) =>
            new MediaLibraryFile()
            {
                Guid = mediaFileInfo.FileGUID,
                Title = mediaFileInfo.FileTitle,
                Extension = mediaFileInfo.FileExtension,
                DirectUrl = MediaLibraryHelper.GetDirectUrl(mediaFileInfo),
                PermanentUrl = MediaLibraryHelper.GetPermanentUrl(mediaFileInfo),
                Width = mediaFileInfo.FileImageWidth,
                Height = mediaFileInfo.FileImageHeight
            };
    }
}
