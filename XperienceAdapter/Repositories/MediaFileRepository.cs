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

using Common.Configuration;

using Kentico.Content.Web.Mvc;

using Microsoft.Extensions.Options;

using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly IMediaLibraryInfoProvider _mediaLibraryInfoProvider;

        private readonly IMediaFileInfoProvider _mediaFileInfoProvider;

        private readonly ISiteService _siteService;

        private readonly IMediaFileUrlRetriever _mediaFileUrlRetriever;

        private readonly IOptionsMonitor<XperienceOptions> _optionsMonitor;

        public MediaFileRepository(
            IMediaLibraryInfoProvider mediaLibraryInfoProvider,
            IMediaFileInfoProvider mediaFileInfoProvider,
            ISiteService siteService,
            IMediaFileUrlRetriever mediaFileUrlRetriever,
            IOptionsMonitor<XperienceOptions> optionsMonitor)
        {
            _mediaLibraryInfoProvider = mediaLibraryInfoProvider ?? throw new ArgumentNullException(nameof(mediaLibraryInfoProvider));
            _mediaFileInfoProvider = mediaFileInfoProvider ?? throw new ArgumentNullException(nameof(mediaFileInfoProvider));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _mediaFileUrlRetriever = mediaFileUrlRetriever ?? throw new ArgumentNullException(nameof(mediaFileUrlRetriever));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public async Task<Guid> AddMediaFileAsync(IUploadedFile uploadedFile,
                                                  int mediaLibraryId,
                                                  string? libraryFolderPath = default,
                                                  bool checkPermissions = default,
                                                  CancellationToken? cancellationToken = default)
        {
            if (uploadedFile is null)
            {
                throw new ArgumentNullException(nameof(uploadedFile));
            }

            var library = await GetMediaLibrary(cancellationToken, mediaLibraryId);

            return await AddMediaFileInternalAsync(uploadedFile, libraryFolderPath, library, checkPermissions);
        }

        public async Task<Guid> AddMediaFileAsync(IUploadedFile uploadedFile,
                                              string mediaLibraryName,
                                              string? libraryFolderPath = default,
                                              bool checkPermissions = default,
                                              CancellationToken? cancellationToken = default)
        {
            if (uploadedFile is null)
            {
                throw new ArgumentNullException(nameof(uploadedFile));
            }

            if (string.IsNullOrEmpty(mediaLibraryName))
            {
                throw new ArgumentException($"'{nameof(mediaLibraryName)}' cannot be null or empty.", nameof(mediaLibraryName));
            }

            var library = await GetMediaLibrary(cancellationToken, libraryName: mediaLibraryName);

            return await AddMediaFileInternalAsync(uploadedFile, libraryFolderPath, library, checkPermissions);
        }

        private async Task<Guid> AddMediaFileInternalAsync(IUploadedFile uploadedFile, string? libraryFolderPath, MediaLibraryInfo mediaLibraryInfo, bool checkPermissions)
        {
            if (checkPermissions && !mediaLibraryInfo.CheckPermissions(PermissionsEnum.Create, _siteService.CurrentSite.SiteName, MembershipContext.AuthenticatedUser))
            {
                throw new PermissionException(
                    $"The user {MembershipContext.AuthenticatedUser.FullName} lacks permissions to the {mediaLibraryInfo.LibraryDisplayName} library.");
            }

            MediaFileInfo mediaFile = default;

            try
            {
                mediaFile = !string.IsNullOrEmpty(libraryFolderPath)
                    ? new MediaFileInfo(uploadedFile, mediaLibraryInfo.LibraryID, libraryFolderPath)
                    : new MediaFileInfo(uploadedFile, mediaLibraryInfo.LibraryID);
            }
            catch (Exception)
            {
                throw new Exception($"The {uploadedFile.FileName} file could not be created in the {mediaLibraryInfo.LibraryName} library.");
            }

            _mediaFileInfoProvider.Set(mediaFile);

            return mediaFile.FileGUID;
        }

        private async Task<MediaLibraryInfo> GetMediaLibrary(CancellationToken? cancellationToken, int? libraryId = default, string? libraryName = default)
        {
            if (!libraryId.HasValue && string.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentException("Neither library ID nor library name was specified.");
            }

            return libraryId.HasValue
                ? await _mediaLibraryInfoProvider.GetAsync(libraryId.Value, cancellationToken)
                : await _mediaLibraryInfoProvider.GetAsync(libraryName, _siteService.CurrentSite.SiteID, cancellationToken);
        }

        public MediaLibraryFile GetMediaFile(Guid fileGuid)
        {
            var mediaFileInfo = MediaFileInfoProvider.GetMediaFileInfo(fileGuid, _siteService.CurrentSite.SiteName);

            return mediaFileInfo != null ? MapDtoProperties(mediaFileInfo) : null;
        }

        public async Task<MediaLibraryFile?> GetMediaFileAsync(Guid fileGuid, CancellationToken? cancellationToken = default)
        {
            var mediaFileInfo = await _mediaFileInfoProvider.GetAsync(fileGuid, _siteService.CurrentSite.SiteID, cancellationToken);

            return mediaFileInfo != null ? MapDtoProperties(mediaFileInfo) : null;
        }

        public async Task<MediaLibraryFile> GetMediaFileAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default)
        {
            var libraryId = await GetLibraryIdAsync(mediaLibraryName, cancellationToken);

            return await GetMediaFileAsync(libraryId, path, cancellationToken);
        }

        public async Task<MediaLibraryFile> GetMediaFileAsync(int mediaLibraryId, string path, CancellationToken? cancellationToken = default) =>
            (await GetResultAsync(baseQuery =>
                baseQuery
                    .WhereEquals("FileLibraryID", mediaLibraryId)
                    .WhereStartsWith("FilePath", path),
                cancellationToken))
                .FirstOrDefault();



        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(CancellationToken? cancellationToken = default, params Guid[] fileGuids) =>
            await GetResultAsync(baseQuery =>
                    baseQuery
                    .WhereIn("FileGUID", fileGuids),
                cancellationToken);

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, CancellationToken? cancellationToken = default, params string[] extensions)
        {
            var libraryId = GetLibraryIdAsync(mediaLibraryName, cancellationToken);

            return await GetResultAsync(baseQuery =>
                baseQuery
                    .WhereIn("FileExtension", extensions),
                cancellationToken);
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default)
        {
            var libraryId = GetLibraryIdAsync(mediaLibraryName, cancellationToken);

            return await GetResultAsync(baseQuery =>
                baseQuery
                    .WhereStartsWith("FilePath", path),
                cancellationToken);
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, IEnumerable<Guid> fileGuids, CancellationToken? cancellationToken = default)
        {
            var libraryId = GetLibraryIdAsync(mediaLibraryName, cancellationToken);

            return await GetResultAsync(baseQuery =>
                baseQuery
                    .WhereIn("FileGUID", fileGuids.ToArray()),
                cancellationToken);
        }

        public async Task<IEnumerable<MediaLibraryFile>> GetAllAsync(CancellationToken? cancellationToken = default) =>
            await GetResultAsync(null, cancellationToken: cancellationToken);

        public IEnumerable<MediaLibraryFile> GetAll() => GetResult(null);


        public async Task<int> GetLibraryIdAsync(string mediaLibraryName, CancellationToken? cancellationToken = default)
        {
            if (string.IsNullOrEmpty(mediaLibraryName))
            {
                throw new ArgumentException($"The {nameof(mediaLibraryName)} parameter must a non-empty string.");
            }

            return (await _mediaLibraryInfoProvider
                .GetAsync(mediaLibraryName, _siteService.CurrentSite.SiteID))
                .LibraryID;
        }

        public async Task<string> GetLibraryNameAsync(int mediaLibraryId, CancellationToken? cancellationToken = default) =>
            (await _mediaLibraryInfoProvider.GetAsync(mediaLibraryId, cancellationToken)).LibraryName;

        /// <summary>
        /// Gets a query with an optional filter.
        /// </summary>
        /// <param name="filter">Optional filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A sequence of media file DTOs.</returns>
        private async Task<IEnumerable<MediaLibraryFile>> GetResultAsync(
            Func<ObjectQuery<MediaFileInfo>, ObjectQuery<MediaFileInfo>>? filter,
            CancellationToken? cancellationToken)
        {
            var query = GetQuery(filter);

            return (await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken))
                .Select(item => MapDtoProperties(item));
        }

        /// <summary>
        /// Gets a query with an optional filter.
        /// </summary>
        /// <param name="filter">Optional filter.</param>
        /// <returns>A sequence of media file DTOs.</returns>
        private IEnumerable<MediaLibraryFile> GetResult(Func<ObjectQuery<MediaFileInfo>, ObjectQuery<MediaFileInfo>>? filter)
        {
            var query = GetQuery(filter);

            return query.GetEnumerableTypedResult()
                .Select(item => MapDtoProperties(item));
        }

        /// <summary>
        /// Gets an <see cref="ObjectQuery"/>.
        /// </summary>
        /// <param name="filter">Filtering criteria.</param>
        /// <returns>The query.</returns>
        private ObjectQuery<MediaFileInfo> GetQuery(Func<ObjectQuery<MediaFileInfo>, ObjectQuery<MediaFileInfo>>? filter)
        {
            var query = _mediaFileInfoProvider.Get();

            if (filter != null)
            {
                query = filter(query);
            }

            return query;
        }

        /// <summary>
        /// Maps DTO properties.
        /// </summary>
        /// <param name="mediaFileInfo">Xperience media file.</param>
        /// <returns>Media file DTO.</returns>
        private MediaLibraryFile MapDtoProperties(MediaFileInfo mediaFileInfo) =>
            new MediaLibraryFile()
            {
                Guid = mediaFileInfo.FileGUID,
                Name = !string.IsNullOrEmpty(mediaFileInfo.FileTitle) ? mediaFileInfo.FileTitle : mediaFileInfo.FileName,
                Extension = mediaFileInfo.FileExtension,
                MediaFileUrl = _mediaFileUrlRetriever.Retrieve(mediaFileInfo),
                Width = mediaFileInfo.FileImageWidth,
                Height = mediaFileInfo.FileImageHeight,
                MimeType = mediaFileInfo.FileMimeType,
                // No Offset is set since we set a UTC value.
                // See https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset?view=netcore-3.1
                LastModified = mediaFileInfo.FileModifiedWhen.ToUniversalTime()
            };
    }
}
