using System.Collections.Generic;

using XperienceAdapter.Models;
using Business.Models;
using System.Threading.Tasks;
using System.Threading;

namespace Business.Repositories
{
    /// <summary>
    /// Stores navigation.
    /// </summary>
    public interface INavigationRepository
    {
        /// <summary>
        /// Code names of page types with the 'Navigation item' feature enabled.
        /// </summary>
        IEnumerable<string> NavigationEnabledPageTypes { get; }

        /// <summary>
        /// Cache dependency keys computed out of <see cref="NavigationEnabledPageTypes"/>.
        /// </summary>
        IEnumerable<string> NavigationEnabledTypeDependencies { get; }

        /// <summary>
        /// Gets navigation hierarchies of all site cultures.
        /// </summary>
        /// <returns>Dictionary with navigation hierarchies per each site culture.</returns>
        Task<Dictionary<SiteCulture, NavigationItem>> GetWholeNavigationAsync(CancellationToken? cancellationToken = default);

        /// <summary>
        /// Gets a navigation hierarchy for a specified or actual site culture, further constrained by the starting node alias path.
        /// </summary>
        /// <param name="siteCulture">Site culture.</param>
        /// <returns>Navigation item in a given culture.</returns>
        Task<NavigationItem> GetNavigationAsync(SiteCulture? siteCulture = default, CancellationToken? cancellationToken = default);

        /// <summary>
        /// Traverses the hierarchy to find a navigation item by node ID.
        /// </summary>
        /// <param name="nodeId">Node ID.</param>
        /// <param name="startPointItem">Starting point navigation item.</param>
        /// <returns>Navigation item.</returns>
        NavigationItem? GetNavigationItemByNodeId(int nodeId, NavigationItem startPointItem);
    }
}
