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

namespace Business.Repositories
{
    public class NavigationRepository : INavigationRepository
    {
        private const string RootPath = "/";

        private static readonly string[] NodeOrdering = new string[] { "NodeLevel", "NodeOrder" };

        private readonly IPageUrlRetriever _pageUrlRetriever;

        private readonly IPageRepository<BasePage, TreeNode> _basePageRepository;

        /* Conventional routing: Begin */
        //private readonly IPageRepository<BasicPageWithUrlSlug, TreeNode> _urlSlugPageRepository;
        /* Conventional routing: End */

        private readonly ISiteCultureRepository _cultureRepository;

        private readonly string[] _navigationEnabledPageTypes = new string[]
        {
            CMS.DocumentEngine.Types.MedioClinic.HomePage.CLASS_NAME,
            CMS.DocumentEngine.Types.MedioClinic.BasicPageWithUrlSlug.CLASS_NAME,
            CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME,
            CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME,
            CMS.DocumentEngine.Types.MedioClinic.User.CLASS_NAME,
            CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME
        };

        private IEnumerable<string> NavigationEnabledTypeDependencies => _navigationEnabledPageTypes
            .Select(pageType => $"nodes|{SiteContext.CurrentSiteName}|{pageType}|all");

        private NavigationItem RootDto => _basePageRepository.GetPagesInCurrentCulture(query =>
            query
                .Path(RootPath, PathTypeEnum.Single)
                .TopN(1),
            buildCacheAction: cache => cache
                .Key($"{nameof(NavigationRepository)}|{nameof(RootDto)}"))
                .Select(basePageDto => new NavigationItem
                {
                    NodeId = basePageDto.NodeId,
                    Name = basePageDto.Name
                })
                .FirstOrDefault();

        public NavigationRepository(
            IPageUrlRetriever pageUrlRetriever,
            IPageRepository<BasePage, TreeNode> basePageRepository,
            /* Conventional routing: Begin */
            //IPageRepository<BasicPageWithUrlSlug, TreeNode> urlSlugPageRepository,
            /* Conventional routing: End */
            ISiteCultureRepository siteCultureRepository)
        {
            _pageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
            _basePageRepository = basePageRepository ?? throw new ArgumentNullException(nameof(basePageRepository));
            /* Conventional routing: Begin */
            //_urlSlugPageRepository = urlSlugPageRepository ?? throw new ArgumentNullException(nameof(urlSlugPageRepository));
            /* Conventional routing: End */
            _cultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
        }

        /* CTB routing: Begin */
        public Dictionary<SiteCulture, NavigationItem> GetWholeNavigation() => GetContentTreeNavigation();

        public NavigationItem GetNavigation(SiteCulture? siteCulture = default) =>
            GetContentTreeNavigation(siteCulture);
        /* CTB routing: End */

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
        private Dictionary<SiteCulture, NavigationItem> GetContentTreeNavigation()
        {
            var cultures = _cultureRepository.GetAll();
            var cultureSpecificNavigations = new Dictionary<SiteCulture, NavigationItem>();

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    cultureSpecificNavigations.Add(culture, GetContentTreeNavigation(culture));
                }
            }

            return cultureSpecificNavigations;
        }

        /// <summary>
        /// Gets a navigation hierarchy based on data provided by the CTB router.
        /// </summary>
        /// <param name="siteCulture">Site culture.</param>
        /// <returns>Navigation hierarchy of a given culture.</returns>
        private NavigationItem GetContentTreeNavigation(SiteCulture? siteCulture)
        {
            var checkedCulture = GetSiteCulture(siteCulture);

            var allItems = _basePageRepository.GetPagesByTypeAndCulture(
                _navigationEnabledPageTypes,
                checkedCulture,
                $"{nameof(NavigationRepository)}|{nameof(GetContentTreeNavigation)}|{checkedCulture.IsoCode}",
                filter => GetDefaultFilter(filter)
                    .MenuItems(),
                cacheDependencies: NavigationEnabledTypeDependencies.ToArray())
                    .Select(basePage => MapBaseToNavigationDto(basePage));

            return DecorateItems(RootDto, allItems, GetContentTreeBasedUrl);
        }

        private SiteCulture GetSiteCulture(SiteCulture? siteCulture) =>
            siteCulture
            ?? Thread.CurrentThread.CurrentUICulture.ToSiteCulture()
            ?? throw new Exception($"The {nameof(siteCulture)} parameter is either null or not a valid site culture.");

        /// <summary>
        /// Maps the <see cref="BasePage"/> onto a new <see cref="NavigationItem"/>.
        /// </summary>
        /// <param name="basePage">The base page.</param>
        /// <returns>The navigation item.</returns>
        private static NavigationItem MapBaseToNavigationDto(BasePage basePage) => new NavigationItem
        {
            NodeId = basePage.NodeId,
            Guid = basePage.Guid,
            ParentId = basePage.ParentId,
            Name = basePage.Name,
            NodeAliasPath = basePage.NodeAliasPath,
            Culture = basePage.Culture
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

        /* Conventional routing: Begin */
        //public Dictionary<SiteCulture, NavigationItem> GetWholeNavigation() => GetConventionalRoutingNavigation();

        //public NavigationItem GetNavigation(SiteCulture? siteCulture = default, string? nodeAliasPath = default) =>
        //    GetConventionalRoutingNavigation(siteCulture, nodeAliasPath);

        //private Dictionary<SiteCulture, NavigationItem> GetConventionalRoutingNavigation()
        //{
        //    GetInputData(out IEnumerable<SiteCulture> cultures, out Dictionary<SiteCulture, NavigationItem> cultureSpecificNavigations);

        //    if (cultures.Any())
        //    {
        //        foreach (var culture in cultures)
        //        {
        //            NavigationItem decorated = GetConventionalRoutingNavigation(culture, null);

        //            cultureSpecificNavigations.Add(culture, decorated);
        //        }
        //    }

        //    return cultureSpecificNavigations;
        //}

        //private NavigationItem GetConventionalRoutingNavigation(SiteCulture? siteCulture, string? nodeAliasPath)
        //{
        //    var checkedCulture = GetSiteCulture(siteCulture);

        //    var allItems = _urlSlugPageRepository.GetPagesByTypeAndCulture(
        //        _navigationEnabledPageTypes,
        //        checkedCulture,
        //        $"{nameof(NavigationRepository)}|{nameof(GetConventionalRoutingNavigation)}|{checkedCulture.IsoCode}",
        //        filter => GetDefaultFilter(filter, nodeAliasPath),
        //        cacheDependencies: NavigationEnabledTypeDependencies.ToArray())
        //            .Select(basicPage => MapBasicPageWithSlugToNavigation(basicPage));

        //    return DecorateItems(RootDto, allItems, GetConventionalRoutingUrl);
        //}

        //public string? GetUrlByNodeId(int nodeId, SiteCulture pageCulture)
        //{
        //    var navigation = RoutingMode == PageRoutingModeEnum.BasedOnContentTree
        //        ? GetContentTreeNavigation(pageCulture, RootPath)
        //        : GetConventionalRoutingNavigation(pageCulture, RootPath);

        //    return GetNavigationItemByNodeId(nodeId, navigation)?.RelativeUrl;
        //}

        ///// <summary>
        ///// Maps the <see cref="BasicPageWithUrlSlug"/> onto a new <see cref="NavigationItem"/>.
        ///// </summary>
        ///// <param name="basicPage">The basic page with URL slug.</param>
        ///// <returns>The navigation item.</returns>
        //private static NavigationItem MapBasicPageWithSlugToNavigation(BasicPageWithUrlSlug basicPage)
        //{
        //    var navigationItem = MapBaseToNavigationDto(basicPage);
        //    navigationItem.UrlSlug = basicPage.UrlSlug;

        //    return navigationItem;
        //}

        ///// <summary>
        ///// Gets a URL for conventional routing.
        ///// </summary>
        ///// <param name="item">Item to get the URL for.</param>
        ///// <returns>URL.</returns>
        //private string GetConventionalRoutingUrl(NavigationItem item)
        //{
        //    var patternBasedUrl = GetPageUrl(item);

        //    if (string.IsNullOrEmpty(patternBasedUrl))
        //    {
        //        var trailingPath = string.Join('/', item.AllParents.Concat(new[] { item }).Select(item => item.UrlSlug));
        //        var culture = item.Culture ?? _cultureRepository.DefaultSiteCulture;

        //        return $"~/{culture?.IsoCode?.ToLowerInvariant()}{trailingPath}/";
        //    }

        //    return patternBasedUrl;
        //}
        /* Conventional routing: End */
    }
}
