using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Dtos;
using XperienceAdapter.Repositories;
using Business.Dtos;

namespace Business.Repositories
{
    public class NavigationRepository : INavigationRepository
    {
        protected const string RootPath = "/";

        protected static readonly string[] NodeOrdering = new string[] { "NodeLevel", "NodeOrder" };

        protected readonly IPageUrlRetriever _pageUrlRetriever;

        protected readonly IPageRepository<BasePage, TreeNode> _basePageRepository;

        protected readonly IPageRepository<BasicPageWithUrlSlug, TreeNode> _urlSlugPageRepository;

        protected readonly ISiteCultureRepository _cultureRepository;

        protected readonly string[] _slugEnabledPageTypes = new string[]
        {
            "MedioClinic.BasicPageWithUrlSlug", "MedioClinic.SiteSection", "MedioClinic.NamePerexText", "MedioClinic.User", "MedioClinic.Doctor"
        };

        protected NavigationItem RootDto => _basePageRepository.GetPages(query =>
            query
                .Path(RootPath, PathTypeEnum.Single)
                .TopN(1),
            buildCacheAction: cache => GetCacheBuilder(cache, nameof(RootDto), RootPath, PathTypeEnum.Single))
                .Select(basePageDto => new NavigationItem
                {
                    NodeId = basePageDto.NodeId,
                    Name = basePageDto.Name
                })
                .FirstOrDefault();

        public NavigationRepository(
            IPageUrlRetriever pageUrlRetriever,
            IPageRepository<BasePage, TreeNode> basePageRepository,
            IPageRepository<BasicPageWithUrlSlug, TreeNode> urlSlugPageRepository,
            ISiteCultureRepository siteCultureRepository)
        {
            _pageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
            _basePageRepository = basePageRepository ?? throw new ArgumentNullException(nameof(basePageRepository));
            _urlSlugPageRepository = urlSlugPageRepository ?? throw new ArgumentNullException(nameof(urlSlugPageRepository));
            _cultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
        }

        public Dictionary<string, NavigationItem> GetContentTreeNavigation()
        {
            var cacheKeySuffix = $"{nameof(GetContentTreeNavigation)}";
            GetInputData(out IEnumerable<SiteCulture> cultures, out Dictionary<string, NavigationItem> cultureSpecificNavigations);

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    // We need to fetch across different page types, without the need for coupled data.
                    var allItems = _basePageRepository.GetPages(
                        query => GetDefaultQuery(query)
                            .MenuItems(),
                        buildCacheAction: cache => GetCacheBuilder(cache, cacheKeySuffix, RootPath, PathTypeEnum.Children))
                            .Select(dto => MapBaseToNavigationDto(dto));

                    var decorated = DecorateItems(RootDto, allItems, GetContentTreeBasedUrl);

                    cultureSpecificNavigations.Add(culture.IsoCode!, decorated);
                }
            }

            return cultureSpecificNavigations;
        }

        public Dictionary<string, NavigationItem> GetSecondaryNavigation(string nodeAliasPath)
        {
            var cacheKeySuffix = $"{nameof(GetSecondaryNavigation)}|{nodeAliasPath}";
            GetInputData(out IEnumerable<SiteCulture> cultures, out Dictionary<string, NavigationItem> cultureSpecificNavigations);

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    // We need to fetch across different page types, without the need for coupled data.
                    var allItems = _basePageRepository.GetPages(
                        query => GetDefaultQuery(query)
                            .Path(nodeAliasPath, PathTypeEnum.Children),
                        buildCacheAction: cache => GetCacheBuilder(cache, cacheKeySuffix, nodeAliasPath, PathTypeEnum.Children))
                            .Select(dto => MapBaseToNavigationDto(dto));

                    var decorated = DecorateItems(RootDto, allItems, GetContentTreeBasedUrl);

                    cultureSpecificNavigations.Add(culture.IsoCode!, decorated);
                }
            }

            return cultureSpecificNavigations;
        }

        public Dictionary<string, NavigationItem> GetConventionalRoutingNavigation()
        {
            string cacheKeySuffix = $"{nameof(GetConventionalRoutingNavigation)}";
            GetInputData(out IEnumerable<SiteCulture> cultures, out Dictionary<string, NavigationItem> cultureSpecificNavigations);

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    var allItems = _urlSlugPageRepository.GetPagesOfMultitpleTypes(
                        _slugEnabledPageTypes,
                        query => query
                            .FilterDuplicates()
                            .OrderByAscending(NodeOrdering),
                        buildCacheAction: cache => GetCacheBuilder(cache, $"{cacheKeySuffix}|{culture.IsoCode}", RootPath, PathTypeEnum.Section),
                        culture: culture.IsoCode)
                            .Select(dto => new NavigationItem
                            {
                                NodeId = dto.NodeId,
                                Guid = dto.Guid,
                                ParentId = dto.ParentId,
                                Name = dto.Name,
                                NodeAliasPath = dto.NodeAliasPath,
                                UrlSlug = dto.UrlSlug
                            });

                    var decorated = DecorateItems(RootDto, allItems, GetConventionalRoutingUrl);

                    cultureSpecificNavigations.Add(culture.IsoCode!, decorated);
                }
            }

            return cultureSpecificNavigations;
        }

        /// <summary>
        /// Prepares cultures and a new dictionary for navigation sets.
        /// </summary>
        /// <param name="cultures">All site cultures.</param>
        /// <param name="cultureSpecificNavigations">Empty dictionary with navigation sets for each culture.</param>
        protected void GetInputData(out IEnumerable<SiteCulture> cultures, out Dictionary<string, NavigationItem> cultureSpecificNavigations)
        {
            cultures = _cultureRepository.GetAll();
            cultureSpecificNavigations = new Dictionary<string, NavigationItem>();
        }

        /// <summary>
        /// Maps the <see cref="BasePage"/> onto a new <see cref="NavigationItem"/>.
        /// </summary>
        /// <param name="dto">The input DTO.</param>
        /// <returns>The <see cref="NavigationItem"/>.</returns>
        protected static NavigationItem MapBaseToNavigationDto(BasePage dto) => new NavigationItem
        {
            NodeId = dto.NodeId,
            Guid = dto.Guid,
            Name = dto.Name,
            NodeAliasPath = dto.NodeAliasPath,
            ParentId = dto.ParentId
        };

        /// <summary>
        /// Gets default <see cref="DocumentQuery{TDocument}"/> configuration.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The modified query.</returns>
        protected static DocumentQuery<TreeNode> GetDefaultQuery(DocumentQuery<TreeNode> query) =>
            query
                .FilterDuplicates()
                .OrderByAscending(NodeOrdering);

        protected static IPageCacheBuilder<TreeNode> GetCacheBuilder(
            IPageCacheBuilder<TreeNode> pageCacheBuilder, 
            string cacheKeySuffix, 
            string path, 
            PathTypeEnum pathType) =>
                pageCacheBuilder
                    .Key($"{nameof(NavigationRepository)}|{cacheKeySuffix}")
                    .Dependencies((_, builder) => builder
                        .PagePath(path, pathType)
                        .ObjectType("cms.documenttype")
                        .PageOrder());

        /// <summary>
        /// Decorates items with references and URLs.
        /// </summary>
        /// <param name="root">Root navigation item.</param>
        /// <param name="defaultCultureItems">A flat sequence of all other items.</param>
        /// <returns></returns>
        public NavigationItem DecorateItems(NavigationItem root, IEnumerable<NavigationItem> navigationItems, Func<NavigationItem, string> urlDecorator)
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
            where navigationItems.Select(item => item.NodeId).Contains(navigationItem.ParentId.GetValueOrDefault())
            select navigationItem;

        /// <summary>
        /// Decorates items with references to parents and children.
        /// </summary>
        /// <param name="parent">Current parent item.</param>
        /// <param name="allItems">A flat sequence of all items.</param>
        /// <returns>Hierarchical navigation item.</returns>
        public NavigationItem BuildHierarchyLevel(NavigationItem parent, IEnumerable<NavigationItem> allItems, Func<NavigationItem, string> urlDecorator)
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
        /// Gets URL for a content tree-based navigation item.
        /// </summary>
        /// <param name="item">Item to get the URL for.</param>
        /// <returns>URL.</returns>
        protected string GetContentTreeBasedUrl(NavigationItem item) => GetPageUrl(item);

        /// <summary>
        /// Gets URL for conventional routing.
        /// </summary>
        /// <param name="item">Item to get the URL for.</param>
        /// <returns>URL.</returns>
        protected string GetConventionalRoutingUrl(NavigationItem item)
        {
            var patternBasedUrl = GetPageUrl(item);

            if (string.IsNullOrEmpty(patternBasedUrl))
            {
                var trailingPath = string.Join('/', item.AllParents.Concat(new[] { item }).Select(item => item.UrlSlug));

                return $"~/{Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant()}{trailingPath}/";
            }

            return patternBasedUrl;
        }

        protected string GetPageUrl(NavigationItem item)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;

            return _pageUrlRetriever.Retrieve(item.NodeAliasPath, currentCulture).RelativePath;
        }
    }
}
