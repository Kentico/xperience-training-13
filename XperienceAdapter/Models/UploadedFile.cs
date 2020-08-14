using System;
using System.IO;
using Microsoft.AspNetCore.Http;

using CMS.Base;

namespace XperienceAdapter.Models
{
    public class UploadedFile : IUploadedFile
    {
        private readonly IFormFile mFormFile;

        /// <summary>
        /// Gets a value that indicates whether this implementation supports accessing the input stream
        /// via <see cref="OpenReadStream"/> method. This property always returns <see langword="true"/>.
        /// </summary>
        /// <return>
        /// <see langword="true"/> to indicate that the input stream must be accessed via <see cref="OpenReadStream"/> method.
        /// </return>
        public bool CanOpenReadStream => true;


        /// <summary>
        /// Gets the MIME content type of an uploaded file.
        /// </summary>
        public string ContentType => mFormFile.ContentType;


        /// <summary>
        /// Gets the size of an uploaded file in bytes.
        /// </summary>
        public long Length => mFormFile.Length;


        /// <summary>
        /// Gets the fully qualified name of the file on the client.
        /// </summary>
        public string FileName => mFormFile.FileName;


        /// <summary>
        /// Gets a <see cref="Stream"/> object that points to an uploaded file.
        /// This property is not currently supported.
        /// The input stream must be accessed via <see cref="OpenReadStream"/> method. 
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown always.
        /// </exception>
        public Stream InputStream =>
            throw new NotSupportedException($"Input stream must be accessed via {nameof(OpenReadStream)}() method.");


        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFile"/> class.
        /// </summary>
        /// <param name="formFile">The <see cref="IFormFile"/>.</param>
        public UploadedFile(IFormFile formFile)
        {
            mFormFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
        }


        /// <summary>
        /// Opens the request stream for reading the uploaded file.
        /// </summary>
        public Stream OpenReadStream()
        {
            return mFormFile.OpenReadStream();
        }
    }
}
