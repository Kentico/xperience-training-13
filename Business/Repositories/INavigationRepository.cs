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
        /// Gets navigation hierarchies of all site cultures.
        /// </summary>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<SiteCulture, NavigationItem> GetWholeNavigation();

        /// <summary>
        /// Gets a navigation hierarchy for a specified or actual site culture, further constrained by the starting node alias path.
        /// </summary>
        /// <param name="siteCulture">Site culture.</param>
        /// <param name="nodeAliasPath">Starting node alias path.</param>
        /// <returns></returns>
        NavigationItem GetNavigation(SiteCulture? siteCulture = default, string? nodeAliasPath = default);

        /* Conventional routing: Begin */
        ///// <summary>
        ///// Gets a navigation item's URL based on the "UrlSlug" field of the "BasicPageWithUrlSlug" page type.
        ///// </summary>
        ///// <param name="nodeId">Node ID.</param>
        ///// <param name="pageCulture">Page culture.</param>
        ///// <returns></returns>
        //string? GetUrlByNodeId(int nodeId, SiteCulture pageCulture);
        /* Conventional routing: End */

        /// <summary>
        /// Traverses the hierarchy to find a navigation item by node ID.
        /// </summary>
        /// <param name="nodeId">Node ID.</param>
        /// <param name="startPointItem">Starting point navigation item.</param>
        /// <returns>Navigation item.</returns>
        NavigationItem? GetNavigationItemByNodeId(int nodeId, NavigationItem startPointItem);
    }
}
