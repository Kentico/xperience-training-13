using System.Collections.Generic;

using XperienceAdapter.Models;
using Business.Models;

namespace Business.Repositories
{
    /// <summary>
    /// Stores navigation.
    /// </summary>
    public interface INavigationRepository
    {
        /// <summary>
        /// Gets all navigation items, based on page types with the "Navigation item" feature.
        /// </summary>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<SiteCulture, NavigationItem> GetNavigation();

        /// <summary>
        /// Gets URL based on the custom "UrlSlug" field of the "BasicPageWithUrlSlug" page type.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="pageCulture"></param>
        /// <returns></returns>
        string? GetUrlByNodeId(int nodeId, SiteCulture pageCulture);

        /// <summary>
        /// Traverses the hierarchy to find a navigation item by node ID.
        /// </summary>
        /// <param name="nodeId">Node ID.</param>
        /// <param name="startPointItem">Starting point navigation item.</param>
        /// <returns>Navigation item.</returns>
        NavigationItem? GetNavigationItemByNodeId(int nodeId, NavigationItem startPointItem);
    }
}
