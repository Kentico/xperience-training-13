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
        Dictionary<SiteCulture, NavigationItem> GetContentTreeNavigation();

        /// <summary>
        /// Gets all navigation items, modeled in a dedicated location in the content tree.
        /// </summary>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<SiteCulture, NavigationItem> GetSecondaryNavigation(string nodeAliasPath);


        /// <summary>
        /// Gets all navigation items for the conventional ASP.NET routing.
        /// </summary>
        /// <remarks>Get data from pages based on the "BasicPageWithUrlSlug" page type</remarks>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<SiteCulture, NavigationItem> GetConventionalRoutingNavigation();

        /// <summary>
        /// Gets URL based on the custom "UrlSlug" field of the "BasicPageWithUrlSlug" page type.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="pageCulture"></param>
        /// <returns></returns>
        string? GetConventionalRoutingUrl(int nodeId, SiteCulture pageCulture);
    }
}
