using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using CMS.SiteProvider;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;
using XperienceAdapter.Extensions;
using Business.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using CMS.Helpers.Caching;

namespace Business.Repositories
{
    public class NavigationRepository : INavigationRepository
    {
        private const string RootPath = "/";

        private static readonly string[] NodeOrdering = new string[] { "NodeLevel", "NodeOrder" };

        private readonly IMemoryCache _memoryCache;

        private readonly ICacheDependencyAdapter _cacheDependencyAdapter;

        private readonly IPageUrlRetriever _pageUrlRetriever;

        private readonly IPageRepository<BasicPage, TreeNode> _basicPageRepository;

        private readonly ISiteCultureRepository _cultureRepository;

        public IEnumerable<string> NavigationEnabledPageTypes => DataClassInfoProvider
            .GetClasses()
            .Where(classInfo => classInfo.ClassIsNavigationItem)
            .Select(classInfo => classInfo.ClassName);

        public IEnumerable<string> NavigationEnabledTypeDependencies => NavigationEnabledPageTypes
            .Select(pageType => $"nodes|{SiteContext.CurrentSiteName}|{pageType}|all");

        private NavigationItem RootDto => _basicPageRepository.GetPagesInCurrentCulture(query =>
            query
                .Path(RootPath, PathTypeEnum.Single)
                .TopN(1),
            buildCacheAction: cache => cache
                .Key($"{nameof(NavigationRepository)}|{nameof(RootDto)}"))
                .Select(basicPageDto => new NavigationItem
                {
                    NodeId = basicPageDto.NodeId,
                    Name = basicPageDto.Name
                })
                .FirstOrDefault();

        public NavigationRepository(
            IMemoryCache memoryCache,
            ICacheDependencyAdapter cacheDependencyAdapter,
            IPageUrlRetriever pageUrlRetriever,
            IPageRepository<BasicPage, TreeNode> basicPageRepository,
            ISiteCultureRepository siteCultureRepository)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _cacheDependencyAdapter = cacheDependencyAdapter ?? throw new ArgumentNullException(nameof(cacheDependencyAdapter));
            _pageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
            _basicPageRepository = basicPageRepository ?? throw new ArgumentNullException(nameof(basicPageRepository));
            _cultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
        }

        public async Task<Dictionary<SiteCulture, NavigationItem>> GetWholeNavigationAsync(CancellationToken? cancellationToken = default) =>
            await GetContentTreeNavigationAsync(cancellationToken);

        public async Task<NavigationItem> GetNavigationAsync(SiteCulture? siteCulture = default, CancellationToken? cancellationToken = default) =>
            await GetContentTreeNavigationAsync(siteCulture, cancellationToken);

        public NavigationItem? GetNavigationItemByNodeId(int nodeId, NavigationItem startPointItem)
        {
            if (startPointItem != null)
            {
                if (startPointItem.NodeId == nodeId)
                {
                    return startPointItem;
                }
                else
                {
                    var matches = new List<NavigationItem>();

                    foreach (var child in startPointItem.ChildItems)
                    {
                        var childMatch = GetNavigationItemByNodeId(nodeId, child);
                        matches.Add(childMatch!);
                    }

                    return matches.FirstOrDefault(match => match != null);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets navigation hierarchies for all cultures, based on data provided by the CTB router.
        /// </summary>
        /// <returns>All navigation hierarchies.</returns>
        private async Task<Dictionary<SiteCulture, NavigationItem>> GetContentTreeNavigationAsync(CancellationToken? cancellationToken)
        {
            var cultures = await _cultureRepository.GetAllAsync();
            var cultureSpecificNavigations = new Dictionary<SiteCulture, NavigationItem>();

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    cultureSpecificNavigations.Add(culture, await GetContentTreeNavigationAsync(culture, cancellationToken));
                }
            }

            return cultureSpecificNavigations;
        }

        /// <summary>
        /// Gets a navigation hierarchy based on data provided by the CTB router.
        /// </summary>
        /// <param name="siteCulture">Site culture.</param>
        /// <returns>Navigation hierarchy of a given culture.</returns>
        private async Task<NavigationItem> GetContentTreeNavigationAsync(SiteCulture? siteCulture, CancellationToken? cancellationToken)
        {
            var checkedCulture = GetSiteCulture(siteCulture);
            NavigationItem navigation;

            if (!_memoryCache.TryGetValue(checkedCulture.IsoCode, out navigation))
            {
                var allItems = (await _basicPageRepository.GetPagesByTypeAndCultureAsync(
                    NavigationEnabledPageTypes,
                    checkedCulture,
                    $"{nameof(NavigationRepository)}|{nameof(GetContentTreeNavigationAsync)}|{checkedCulture.IsoCode}",
                    filter => GetDefaultFilter(filter)
                        .MenuItems(),
                    cacheDependencies: NavigationEnabledTypeDependencies.ToArray(),
                    cancellationToken: cancellationToken))
                        .Select(basicPage => MapBaseToNavigationDto(basicPage));

                navigation = DecorateItems(RootDto, allItems, GetContentTreeBasedUrl);
                var changeToken = _cacheDependencyAdapter.GetChangeToken(NavigationEnabledTypeDependencies?.ToArray());

                _memoryCache.Set(checkedCulture.IsoCode, navigation, changeToken);
            }

            return navigation;
        }

        private SiteCulture GetSiteCulture(SiteCulture? siteCulture) =>
            siteCulture
            ?? Thread.CurrentThread.CurrentUICulture.ToSiteCulture()
            ?? throw new Exception($"The {nameof(siteCulture)} parameter is either null or not a valid site culture.");

        /// <summary>
        /// Maps the <see cref="BasicPage"/> onto a new <see cref="NavigationItem"/>.
        /// </summary>
        /// <param name="basicPage">The base page.</param>
        /// <returns>The navigation item.</returns>
        private static NavigationItem MapBaseToNavigationDto(BasicPage basicPage) => new NavigationItem
        {
            NodeId = basicPage.NodeId,
            Guid = basicPage.Guid,
            ParentId = basicPage.ParentId,
            Name = basicPage.Name,
            NodeAliasPath = basicPage.NodeAliasPath,
            Culture = basicPage.Culture
        };

        /// <summary>
        /// Gets default <see cref="DocumentQuery{TDocument}"/> configuration.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The modified query.</returns>
        private static MultiDocumentQuery GetDefaultFilter(MultiDocumentQuery query)
        {
            query
                .FilterDuplicates()
                .OrderByAscending(NodeOrdering);

            return query;
        }

        /// <summary>
        /// Decorates items with references and URLs.
        /// </summary>
        /// <param name="root">Root navigation item.</param>
        /// <param name="defaultCultureItems">A flat sequence of all other items.</param>
        /// <returns></returns>
        public NavigationItem DecorateItems(NavigationItem root, IEnumerable<NavigationItem> navigationItems, Func<NavigationItem, string?> urlDecorator)
        {
            var connectableItems = GetConnectableItems(navigationItems.Concat(new[] { root })).ToList();

            return BuildHierarchyLevel(root, connectableItems, urlDecorator);
        }

        /// <summary>
        /// Filters out items with orphaned <see cref="NavigationItem.ParentId"/> values.
        /// </summary>
        /// <param name="navigationItems">Navigation items.</param>
        /// <returns></returns>
        public IEnumerable<NavigationItem> GetConnectableItems(IEnumerable<NavigationItem> navigationItems) =>
            from navigationItem in navigationItems
            where navigationItems.Select(item => item?.NodeId).Contains(navigationItem.ParentId.GetValueOrDefault())
            select navigationItem;

        /// <summary>
        /// Decorates items with references to parents and children.
        /// </summary>
        /// <param name="parent">Current parent item.</param>
        /// <param name="allItems">A flat sequence of all items.</param>
        /// <returns>Hierarchical navigation item.</returns>
        public NavigationItem BuildHierarchyLevel(NavigationItem parent, IEnumerable<NavigationItem> allItems, Func<NavigationItem, string?> urlDecorator)
        {
            var children = allItems
                .Where(item => item.ParentId.HasValue && item.ParentId == parent.NodeId);

            parent.ChildItems.AddRange(children);

            foreach (var item in children)
            {
                if (item != parent && !parent.AllParents.Contains(item))
                {
                    item.Parent = parent;
                    item.AllParents.AddRange(parent.AllParents);
                    item.AllParents.Add(parent);
                    item.RelativeUrl = urlDecorator(item);
                    BuildHierarchyLevel(item, allItems, urlDecorator);
                }
            }

            return parent;
        }

        /// <summary>
        /// Gets a URL for a content tree-based navigation item.
        /// </summary>
        /// <param name="item">Item to get the URL for.</param>
        /// <returns>URL.</returns>
        private string? GetContentTreeBasedUrl(NavigationItem item) => GetPageUrl(item);

        /// <summary>
        /// Get a relative URL of a page for a navigation item.
        /// </summary>
        /// <param name="item">Navigation item.</param>
        /// <returns>Relative URL.</returns>
        private string? GetPageUrl(NavigationItem item)
        {
            var culture = item?.Culture?.IsoCode;

            try
            {
                var url = _pageUrlRetriever.Retrieve(item?.NodeAliasPath, culture)?.RelativePath?.ToLowerInvariant()!;

                return url;
            }
            catch
            {
                return null;
            }

        }
    }
}
