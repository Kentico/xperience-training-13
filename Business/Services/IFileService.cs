using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Core;
using XperienceAdapter.Models;
using Business.Models;

namespace Business.Services
{
    public interface IFileService : IService
    {
        /// <summary>
        /// Validates a form file and converts it into <see cref="UploadedFile"/>.
        /// </summary>
        /// <param name="formFile">Input file.</param>
        /// <param name="permittedExtensions">Permitted file name extensions.</param>
        /// <param name="sizeLimit">File size limit.</param>
        /// <returns>Uploaded file.</returns>
        Task<ProcessedFile> ProcessFormFileAsync(IFormFile formFile, string[] permittedExtensions, long sizeLimit);

        /// <summary>
        /// Sanitizes a file name and extension.
        /// </summary>
        /// <param name="completeFileName">Input file name.</param>
        /// <returns>Name and extension.</returns>
        (string Name, string Extension) GetSafeFileName(string completeFileName);
    }
}