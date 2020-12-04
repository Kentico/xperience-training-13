using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.Membership;
using Kentico.Content.Web.Mvc;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private int? _mediaLibraryId;

        private string? _mediaLibraryName;

        private readonly IMediaLibraryInfoProvider _mediaLibraryInfoProvider;

        private readonly IMediaFileInfoProvider _mediaFileInfoProvider;

        private readonly ISiteService _siteService;

        private readonly IMediaFileUrlRetriever _mediaFileUrlRetriever;

        //TODO: I don't like the concept - property of repo used only for one method -> parameter of that method + DB call per getter
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

        //TODO: I don't like the concept - property of repo used only for one method -> parameter of that method + DB call per getter
        public string? MediaLibraryName
        {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
            get => string.IsNullOrEmpty(_mediaLibraryName) && _mediaLibraryId.HasValue
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
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

        public MediaFileRepository(IMediaLibraryInfoProvider mediaLibraryInfoProvider, IMediaFileInfoProvider mediaFileInfoProvider, ISiteService siteService, IMediaFileUrlRetriever mediaFileUrlRetriever)
        {
            _mediaLibraryInfoProvider = mediaLibraryInfoProvider ?? throw new ArgumentNullException(nameof(mediaLibraryInfoProvider));
            _mediaFileInfoProvider = mediaFileInfoProvider ?? throw new ArgumentNullException(nameof(mediaFileInfoProvider));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _mediaFileUrlRetriever = mediaFileUrlRetriever ?? throw new ArgumentNullException(nameof(mediaFileUrlRetriever));
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

        public async Task<MediaLibraryFile?> GetMediaFileAsync(Guid fileGuid, CancellationToken? cancellationToken = default)
        {
            var mediaFileInfo = await _mediaFileInfoProvider.GetAsync(fileGuid, _siteService.CurrentSite.SiteID, cancellationToken);

            return mediaFileInfo != null ? MapDtoProperties(mediaFileInfo) : null;
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(CancellationToken? cancellationToken = default, params Guid[] fileGuids)
        {
            var results = await GetQueryAsync(baseQuery => baseQuery
                .WhereIn("FileGUID", fileGuids), cancellationToken);

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(CancellationToken? cancellationToken = default, params string[] extensions)
        {
            var results = await GetQueryAsync(baseQuery => baseQuery
                .WhereIn("FileExtension", extensions), cancellationToken);

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string path, CancellationToken? cancellationToken = default)
        {
            var results = await GetQueryAsync(baseQuery => baseQuery
                .WhereStartsWith("FilePath", path), cancellationToken);

            return results.Select(item => MapDtoProperties(item));
        }

        public async Task<MediaLibraryFile> GetMediaFileAsync(string path, CancellationToken? cancellationToken = default)
        {
            var results = await GetQueryAsync(baseQuery => baseQuery
                .WhereStartsWith("FilePath", path), cancellationToken);

            return results.Select(item => MapDtoProperties(item)).FirstOrDefault();
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetAllAsync(CancellationToken? cancellationToken = default)
        {
            var results = await GetQueryAsync(cancellationToken: cancellationToken);

            return results.Select(item => MapDtoProperties(item));
        }

        public IEnumerable<MediaLibraryFile> GetAll() => GetAllAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Gets a query with an optional filter.
        /// </summary>
        /// <param name="filter">Optional filter.</param>
        /// <returns></returns>
        protected async Task<IEnumerable<MediaFileInfo>> GetQueryAsync(Func<ObjectQuery<MediaFileInfo>, ObjectQuery<MediaFileInfo>>? filter = default, CancellationToken? cancellationToken = default)
        {
            var baseQuery = _mediaFileInfoProvider.Get()
                .WhereEquals("FileLibraryID", MediaLibraryId);

            return filter != null
                ? await filter(baseQuery).GetEnumerableTypedResultAsync(cancellationToken: cancellationToken)
                : await baseQuery.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);
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
                Name = mediaFileInfo.FileTitle,
                Extension = mediaFileInfo.FileExtension,
                MediaFileUrl = _mediaFileUrlRetriever.Retrieve(mediaFileInfo),
                Width = mediaFileInfo.FileImageWidth,
                Height = mediaFileInfo.FileImageHeight
            };
    }
}
