using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Core;
using XperienceAdapter.Models;
using Business.Models;

namespace Business.Services
{
    public interface IFileService : IService
    {
        Task<(FormFileResultState ResultState, UploadedFile? UploadedFile)> ProcessFormFile(IFormFile formFile, string[] permittedExtensions, long sizeLimit);
    }
}