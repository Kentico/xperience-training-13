using System.Collections.Generic;

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
        Dictionary<string, NavigationItem> GetContentTreeNavigation();

        /// <summary>
        /// Gets all navigation items, modeled in a dedicated location in the content tree.
        /// </summary>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<string, NavigationItem> GetSecondaryNavigation(string nodeAliasPath);


        /// <summary>
        /// Gets all navigation items for the conventional ASP.NET routing.
        /// </summary>
        /// <remarks>Get data from pages based on the "BasicPageWithUrlSlug" page type</remarks>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Dictionary<string, NavigationItem> GetConventionalRoutingNavigation();
    }
}
