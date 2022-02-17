using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.Base;

using Common;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    /// <summary>
    /// Stores information about media library files.
    /// </summary>
    public interface IMediaFileRepository : IRepository<MediaLibraryFile>
    {
        /// <summary>
        /// Adds a new media file.
        /// </summary>
        /// <param name="uploadedFile">Uploaded file.</param>
        /// <param name="mediaLibraryId">Media library ID.</param>
        /// <param name="libraryFolderPath">Folder path.</param>
        /// <param name="checkPermisions">Indicates if permissions shall be verified.</param>
        /// <returns>File GUID.</returns>
        Task<Guid> AddMediaFileAsync(IUploadedFile uploadedFile,
                                     int mediaLibraryId,
                                     string? libraryFolderPath = default,
                                     bool checkPermisions = default,
                                     CancellationToken? cancellationToken = default);

        /// <summary>
        /// Adds a new media file.
        /// </summary>
        /// <param name="uploadedFile">Uploaded file.</param>
        /// <param name="mediaLibraryName">Media library code name.</param>
        /// <param name="libraryFolderPath">Folder path.</param>
        /// <param name="checkPermisions">Indicates if permissions shall be verified.</param>
        /// <returns>File GUID.</returns>
        Task<Guid> AddMediaFileAsync(IUploadedFile uploadedFile,
                                     string mediaLibraryName,
                                     string? libraryFolderPath = default,
                                     bool checkPermisions = default,
                                     CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets media file.
        /// </summary>
        /// <param name="mediaLibraryName">Media library code name.</param>
        /// <param name="path">Path in the library.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>File DTOs.</returns>
        Task<MediaLibraryFile> GetMediaFileAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets media file.
        /// </summary>
        /// <param name="mediaLibraryId">Media library ID.</param>
        /// <param name="path">Path in the library.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>File DTOs.</returns>
        Task<MediaLibraryFile> GetMediaFileAsync(int mediaLibraryId, string path, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a media file.
        /// </summary>
        /// <param name="fileGuid">File GUID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Media file DTO.</returns>
        Task<MediaLibraryFile?> GetMediaFileAsync(Guid fileGuid, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a media file.
        /// </summary>
        /// <param name="fileGuid">File GUID.</param>
        /// <returns>Media file DTO.</returns>
        MediaLibraryFile GetMediaFile(Guid fileGuid);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="fileGuids">File GUIDs.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(CancellationToken? cancellationToken = default, params Guid[] fileGuids);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="extensions">File name extensions.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, CancellationToken? cancellationToken = default, params string[] extensions);

        /// <summary>
        /// Gets media files by folder path.
        /// </summary>
        /// <param name="mediaLibraryName">Media library code name.</param>
        /// <param name="path">Folder path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets media files by their GUIDs.
        /// </summary>
        /// <param name="mediaLibraryName">Media library code name.</param>
        /// <param name="fileGuids">File GUIDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, IEnumerable<Guid> fileGuids, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a media library ID by its code name.
        /// </summary>
        /// <param name="mediaLibraryName">Code name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Library ID.</returns>
        Task<int> GetLibraryIdAsync(string mediaLibraryName, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a media library code name.
        /// </summary>
        /// <param name="mediaLibraryId">Library ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<string> GetLibraryNameAsync(int mediaLibraryId, CancellationToken? cancellationToken = default);
    }
}
