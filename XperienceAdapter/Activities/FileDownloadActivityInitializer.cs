using System;

using CMS.Activities;

namespace XperienceAdapter.Activities
{
    public class FileDownloadActivityInitializer : CustomActivityInitializerBase
    {
        private readonly DownloadType _downloadType;

        private readonly string _path;

        public override string ActivityType => "FileDownload";

        public FileDownloadActivityInitializer(DownloadType downloadType, string path)
        {
            _downloadType = downloadType;
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public override void Initialize(IActivityInfo activity)
        {
            activity.ActivityTitle = $"File download ({_downloadType})";
            activity.ActivityValue = $"Download path: {_path}";
        }
    }

    public enum DownloadType
    {
        Public,
        Secured
    }
}
