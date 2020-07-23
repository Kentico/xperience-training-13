using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Abstractions;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    /// <summary>
    /// Stores information about media library files.
    /// </summary>
    public interface IMediaFileRepository : IRepository<MediaLibraryFile>
    {
        /// <summary>
        /// Library ID.
        /// </summary>
        int? MediaLibraryId { get; set; }

        /// <summary>
        /// Library code name.
        /// </summary>
        string MediaLibraryName { get; set; }

        /// <summary>
        /// Adds a new media file.
        /// </summary>
        /// <param name="filePath">Filesystem path.</param>
        /// <param name="libraryFolderPath">Library folder path.</param>
        /// <param name="checkPermisions">Indicates if permissions shall be verified.</param>
        /// <returns></returns>
        Task<Guid> AddMediaFileAsync(string filePath, string? libraryFolderPath = default, bool checkPermisions = default);

        /// <summary>
        /// Gets a media file.
        /// </summary>
        /// <param name="fileGuid">File GUID.</param>
        /// <returns>Media file DTO.</returns>
        Task<MediaLibraryFile?> GetMediaFileDtoAsync(Guid fileGuid);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="fileGuids">File GUIDs.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(params Guid[] fileGuids);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="extensions">File name extensions.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(params string[] extensions);

        /// <summary>
        /// Gets media files.
        /// </summary>
        /// <param name="path">Path in the library.</param>
        /// <returns>File DTOs.</returns>
        Task<IEnumerable<MediaLibraryFile>> GetMediaFileDtosAsync(string path);
    }
}
