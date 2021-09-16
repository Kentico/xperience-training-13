using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


using XperienceAdapter.Models;
using Business.Models;

namespace Business.Services
{
    public class FileService : IFileService
    {
        // If you require a check on specific characters in the IsValidFileExtensionAndSignature
        // method, supply the characters in the _allowedChars field.
        private static readonly byte[] _allowedChars = { };

        // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
        // and the official specifications for the file types you wish to add.
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
            { ".zip", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                    new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                    new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
                }
            },
        };

        public async Task<ProcessedFile> ProcessFormFileAsync(IFormFile formFile,
            string[] permittedExtensions,
            long sizeLimit = 4194304)
        {
            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                formFile.FileName);

            // Check the file length. This check doesn't catch files that only have 
            // a BOM as their content.
            if (formFile.Length == 0)
            {
                return new ProcessedFile(FormFileResultState.FileEmpty);
            }

            if (formFile.Length > sizeLimit)
            {
                return new ProcessedFile(FormFileResultState.FileTooBig);
            }

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);

                // Check the content length in case the file's only
                // content was a BOM and the content is actually
                // empty after removing the BOM.
                if (memoryStream.Length == 0)
                {
                    return new ProcessedFile(FormFileResultState.FileEmpty);
                }

                var safeName = GetSafeFileName(formFile.FileName);

                if (!IsValidFileExtensionAndSignature(
                    safeName.Name, $".{safeName.Extension}", memoryStream, permittedExtensions))
                {
                    return new ProcessedFile(FormFileResultState.ForbiddenFileType);
                }
                else
                {
                    var uploadedFile = new UploadedFile(formFile);
                    uploadedFile.FileName = $"{safeName.Name}.{safeName.Extension}";

                    return new ProcessedFile(FormFileResultState.FileOk, new UploadedFile(formFile));
                }
            }
        }

        public (string Name, string Extension) GetSafeFileName(string completeFileName)
        {
            if (string.IsNullOrEmpty(completeFileName))
            {
                throw new ArgumentException("File name is null or an empty string.", nameof(completeFileName));
            }

            var separator = '.';
            var segments = completeFileName.Split(separator);
            string name, extension;

            if (segments?.Length > 1)
            {
                var subtractedLength = segments.Length - 1;
                string[] segmentsExceptLast = new string[subtractedLength];
                Array.Copy(segments, segmentsExceptLast, subtractedLength);
                name = segmentsExceptLast.Length == 1 ? segmentsExceptLast[0] : string.Join(separator.ToString(), segmentsExceptLast);
                extension = segments[subtractedLength];
            }
            else
            {
                name = completeFileName;
                extension = null;
            }

            var safeName = RemoveNonLettersOrDigits(name)?.ToLowerInvariant();
            var safeExtension = RemoveNonLettersOrDigits(extension)?.ToLowerInvariant();

            return (safeName, safeExtension);
        }

        private static string RemoveNonLettersOrDigits(string input) =>
             new string((from c in input
                         where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                         select c)
                .ToArray());

        private static bool IsValidFileExtensionAndSignature(string fileName, string fileExtension, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(fileExtension) || !permittedExtensions.Contains(fileExtension))
            {
                return false;
            }

            data.Position = 0;

            using (var reader = new BinaryReader(data))
            {
                if (fileExtension.Equals(".txt") || fileExtension.Equals(".csv") || fileExtension.Equals(".prn"))
                {
                    if (_allowedChars.Length == 0)
                    {
                        // Limits characters to ASCII encoding.
                        for (var i = 0; i < data.Length; i++)
                        {
                            if (reader.ReadByte() > sbyte.MaxValue)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // Limits characters to ASCII encoding and
                        // values of the _allowedChars array.
                        for (var i = 0; i < data.Length; i++)
                        {
                            var b = reader.ReadByte();
                            if (b > sbyte.MaxValue ||
                                !_allowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                // File signature check
                // --------------------
                // With the file signatures provided in the _fileSignature
                // dictionary, the following code tests the input content's
                // file signature.
                var signatures = _fileSignature[fileExtension];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
