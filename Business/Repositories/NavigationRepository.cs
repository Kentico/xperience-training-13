using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using Business.Dtos;
using XperienceAdapter;

namespace Business.Repositories
{
    public class NavigationRepository : BasePageRepository<NavigationItemDto, TreeNode>, INavigationRepository
    {
        protected readonly IPageUrlRetriever _pageUrlRetriever;

        public NavigationRepository(IRepositoryDependencies repositoryDependencies, IPageUrlRetriever pageUrlRetriever) : base(repositoryDependencies)
        {
            _pageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
        }

        public NavigationItemDto GetContentTreeNavigation()
        {
            var rootPath = "/";
            var cacheKeySuffix = $"{nameof(GetContentTreeNavigation)}";

            var root = GetPages(
                query => query.Path(rootPath, PathTypeEnum.Single),
                buildCacheAction: cache => GetCacheBuilder(cache, $"{cacheKeySuffix}|root", rootPath, PathTypeEnum.Single))
                .FirstOrDefault();

            var allItems = GetPages(
                query => GetDefaultQuery(query)
                    .MenuItems(),
                MapItems,
                cache => GetCacheBuilder(cache, $"{cacheKeySuffix}|children", rootPath, PathTypeEnum.Children));

            return DecorateItems(root, allItems);
        }

        public NavigationItemDto GetSecondaryNavigation(string nodeAliasPath)
        {
            var cacheKeySuffix = $"{nameof(GetSecondaryNavigation)}|{nodeAliasPath}";

            var root = GetPages(query => query
                .Path(nodeAliasPath, PathTypeEnum.Single),
                buildCacheAction: cache => GetCacheBuilder(cache, $"{cacheKeySuffix}|root", nodeAliasPath, PathTypeEnum.Single))
                .FirstOrDefault();

            var allItems = GetPages(
                query => GetDefaultQuery(query)
                    .Path(nodeAliasPath, PathTypeEnum.Children),
                MapItems,
                cache => GetCacheBuilder(cache, $"{cacheKeySuffix}|children", nodeAliasPath, PathTypeEnum.Children));

            return DecorateItems(root, allItems);
        }

        /// <summary>
        /// Decorates items with references and URLs.
        /// </summary>
        /// <param name="root">Root navigation item.</param>
        /// <param name="navigationItems">A flat sequence of all other items.</param>
        /// <returns></returns>
        public NavigationItemDto DecorateItems(NavigationItemDto root, IEnumerable<NavigationItemDto> navigationItems)
        {
            var connectableItems = GetConnectableItems(navigationItems.Concat(new[] { root })).ToList();

            foreach (var item in connectableItems)
            {
                DecorateWithUrl(item);
            }

            return BuildHierarchyLevel(root, connectableItems);
        }

        /// <summary>
        /// Filters out items with orphaned <see cref="NavigationItemDto.ParentId"/> values.
        /// </summary>
        /// <param name="navigationItems">Navigation items.</param>
        /// <returns></returns>
        public IEnumerable<NavigationItemDto> GetConnectableItems(IEnumerable<NavigationItemDto> navigationItems) =>
            from navigationItem in navigationItems
            where navigationItems.Select(item => item.Id).Contains(navigationItem.ParentId.GetValueOrDefault())
            select navigationItem;

        /// <summary>
        /// Decorates items with references to parents and children.
        /// </summary>
        /// <param name="parent">Current parent item.</param>
        /// <param name="allItems">A flat sequence of all items.</param>
        /// <returns></returns>
        public NavigationItemDto BuildHierarchyLevel(NavigationItemDto parent, IEnumerable<NavigationItemDto> allItems)
        {
            var children = allItems
                .Where(item => item.ParentId.HasValue && item.ParentId == parent.Id);

            parent.ChildItems.AddRange(children);

            foreach (var item in children)
            {
                if (item != parent && !parent.AllParents.Contains(item))
                {
                    item.Parent = parent;
                    item.AllParents.AddRange(parent.AllParents);
                    item.AllParents.Add(parent);
                    BuildHierarchyLevel(item, allItems);
                }
            }

            return parent;
        }

        protected void DecorateWithUrl(NavigationItemDto item) =>
            item.RelativeUrl = _pageUrlRetriever.Retrieve(item.NodeAliasPath).RelativePath;
        //item.RelativeUrl = "test-url";

        protected static NavigationItemDto MapItems(TreeNode page, NavigationItemDto dto)
        {
            dto.ParentId = page.NodeParentID;

            return dto;
        }

        protected static DocumentQuery<TreeNode> GetDefaultQuery(DocumentQuery<TreeNode> query) =>
            query
                .CombineWithDefaultCulture()
                .FilterDuplicates()
                .OrderByAscending("NodeLevel", "NodeOrder");

        protected static IPageCacheBuilder<TreeNode> GetCacheBuilder
            (IPageCacheBuilder<TreeNode> pageCacheBuilder, string cacheKeySuffix, string path, PathTypeEnum pathType) =>
                pageCacheBuilder
                    .Key($"{nameof(NavigationRepository)}|{cacheKeySuffix}")
                    .Dependencies((_, builder) => builder
                        .PagePath(path, pathType)
                        .ObjectType("cms.documenttype")
                        .PageOrder());
    }
}
