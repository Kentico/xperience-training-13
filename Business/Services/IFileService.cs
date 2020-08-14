using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Core;

namespace Business.Services
{
    public interface IFileService : IService
    {
        Task<Stream> ProcessFormFile<T>(IFormFile formFile, ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit);
    }
}