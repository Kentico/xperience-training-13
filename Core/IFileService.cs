using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core
{
    public interface IFileService : IService
    {
        Task<Stream> ProcessFormFile<T>(IFormFile formFile, ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit);
    }
}