using Common;

namespace XperienceAdapter.Services
{
    /// <summary>
    /// Captures information related the current site.
    /// </summary>
    public interface ISiteContextService : IService
    {
        /// <summary>
        /// Indicates if preview mode is enabled.
        /// </summary>
        bool IsPreviewEnabled { get; }
    }
}
