using Core;

namespace XperienceAdapter
{
    /// <summary>
    /// Captures information related the current site.
    /// </summary>
    public interface ISiteContextService : IService
    {
        /// <summary>
        /// Indicates what preview culture should be used in the preview mode.
        /// </summary>
        string PreviewCulture { get; }

        /// <summary>
        /// Indicates if preview mode is enabled.
        /// </summary>
        bool IsPreviewEnabled { get; }
    }
}
