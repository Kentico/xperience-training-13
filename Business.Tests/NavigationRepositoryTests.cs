using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Repositories;
using Business.Models;
using Business.Repositories;
using XperienceAdapter.Models;

namespace Business.Tests
{
    public class NavigationRepositoryTests
    {
        [Fact]
        public void GetConnectableItems_DoesntConnectNonconnectables()
        {
            // Arrange.
            var repository = GetRepository();
            var items = new[] { GetRootItem() }.Concat(GetItems());

            // Act.
            var result = repository.GetConnectableItems(items).ToList();

            // Assert.
            Assert.NotNull(result);
            Assert.True(result.Count() == 4);
            Assert.DoesNotContain(result, item => item.NodeId == 1);
        }

        [Fact]
        public void BuildHierarchyLevel_IgnoresCyclicItems()
        {
            // Arrange.
            var repository = GetRepository();
            var items = new[] { GetRootItem() }.Concat(GetCyclicItems());

            // Act.
            var result = repository.BuildHierarchyLevel(GetRootItem(), items, (dto) => string.Empty);

            // Assert.
            Assert.NotNull(result);
            Assert.True(result.NodeId == 0);
            Assert.Collection(result.ChildItems, item => Assert.Equal(1, item.NodeId));
        }

        [Fact]
        public void BuildHierarchyLevel_BuildsHierarchy()
        {
            // Arrange.
            var repository = GetRepository();
            var items = new[] { GetRootItem() }.Concat(GetItems());

            // Act.
            var result = repository.BuildHierarchyLevel(GetRootItem(), items, (dto) => string.Empty);

            // Assert.
            Assert.NotNull(result);
            Assert.True(result.NodeId == 0);
            Assert.Contains(result.ChildItems, item => item.NodeId == 2);
            Assert.DoesNotContain(result.ChildItems, item => item.NodeId == 1);
            Assert.True(result.ChildItems[0].Parent == result);
            Assert.Contains(result.ChildItems[0].ChildItems, item => item.NodeId == 3);
            Assert.DoesNotContain(result.ChildItems[0].ChildItems, item => item.NodeId == 1);
            Assert.True(result.ChildItems[0].ChildItems[0].Parent == result.ChildItems[0]);
            Assert.True(result.ChildItems[0].ChildItems[0].AllParents.Count() == 2);
        }

        private static NavigationRepository GetRepository()
        {
            var pageUrlRetriever = new Mock<IPageUrlRetriever>().Object;
            var basePageRepository = new Mock<IPageRepository<BasePage, TreeNode>>().Object;
            var urlSlugPageRepository = new Mock<IPageRepository<BasicPageWithUrlSlug, TreeNode>>().Object;
            var cultureRepository = new Mock<ISiteCultureRepository>().Object;
            var repository = new NavigationRepository(pageUrlRetriever, basePageRepository, urlSlugPageRepository, cultureRepository);

            return repository;
        }

        private static NavigationItem GetRootItem() => new NavigationItem
        {
            NodeId = 0,
            Name = "RootItem"
        };

        private static IEnumerable<NavigationItem> GetItems() =>
            new List<NavigationItem>
            {
                new NavigationItem
                {
                    NodeId = 1,
                    ParentId = -1
                },
                new NavigationItem
                {
                    NodeId = 2,
                    ParentId = 0
                },
                new NavigationItem
                {
                    NodeId = 3,
                    ParentId = 2
                },
                new NavigationItem
                {
                    NodeId = 4,
                    ParentId = 0
                }

            };

        private static IEnumerable<NavigationItem> GetCyclicItems() =>
            new List<NavigationItem>
            {
                new NavigationItem
                {
                    NodeId = 1,
                    ParentId = 0
                },
                new NavigationItem
                {
                    NodeId = 2,
                    ParentId = 4
                },
                new NavigationItem
                {
                    NodeId = 3,
                    ParentId = 2
                },
                new NavigationItem
                {
                    NodeId = 4,
                    ParentId = 3
                }
            };
    }
}
