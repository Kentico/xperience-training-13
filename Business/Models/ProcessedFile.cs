using System;
using System.Collections.Generic;
using System.Text;

using XperienceAdapter.Models;

namespace Business.Models
{
    public class ProcessedFile : IDisposable
    {
        private bool _disposed;

        public UploadedFile? UploadedFile { get; }

        public FormFileResultState ResultState { get; }

        public ProcessedFile(FormFileResultState formFileResultState, UploadedFile? uploadedFile = default)
        {
            UploadedFile = uploadedFile ?? throw new ArgumentNullException(nameof(uploadedFile));
            ResultState = formFileResultState;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                UploadedFile?.Dispose(); 
            }

            _disposed = true;
        }
    }
}
