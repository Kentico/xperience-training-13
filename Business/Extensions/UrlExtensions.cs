using System.Threading;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;

namespace Business.Extensions
{
    /// <summary>
    /// Works with URLs.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Gets a relative URL of a page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The relative URL.</returns>
        public static string? RelativeUrl(this TreeNode page) =>
            page.GetPageUrlPath(Thread.CurrentThread.CurrentUICulture.Name).FullPath;
    }
}
