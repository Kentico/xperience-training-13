using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.Base;

using Core;
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
        /// <param name="filePath">Filesystem path.</param>
        /// <param name="libraryFolderPath">Library folder path.</param>
        /// <param name="checkPermisions">Indicates if permissions shall be verified.</param>
        /// <returns></returns>
        Task<Guid> AddMediaFileAsync(IUploadedFile uploadedFile, string mediaLibraryName, string? libraryFolderPath = default, bool checkPermisions = default);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="path">Path in the library.</param>
        /// <returns>File DTOs.</returns>
        Task<MediaLibraryFile> GetMediaFileAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a media file.
        /// </summary>
        /// <param name="fileGuid">File GUID.</param>
        /// <returns>Media file DTO.</returns>
        Task<MediaLibraryFile?> GetMediaFileAsync(Guid fileGuid, CancellationToken? cancellationToken = default);

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
        /// Gets media files.
        /// </summary>
        /// <param name="path">Folder path.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, string path, CancellationToken? cancellationToken = default);

        Task<IEnumerable<MediaLibraryFile>> GetMediaFilesAsync(string mediaLibraryName, IEnumerable<Guid> fileGuids, CancellationToken? cancellationToken = default);
    }
}
