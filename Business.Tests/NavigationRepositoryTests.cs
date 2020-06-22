using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;

using CMS.Base;
using Kentico.Content.Web.Mvc;

using Abstractions;
using XperienceAdapter;
using Business.Dtos;
using Business.Repositories;

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
            Assert.DoesNotContain(result, item => item.Id == 1);
        }

        [Fact]
        public void BuildHierarchyLevel_IgnoresCyclicItems()
        {
            // Arrange.
            var repository = GetRepository();
            var items = new[] { GetRootItem() }.Concat(GetCyclicItems());

            // Act.
            var result = repository.BuildHierarchyLevel(GetRootItem(), items);

            // Assert.
            Assert.NotNull(result);
            Assert.True(result.Id == 0);
            Assert.Collection(result.ChildItems, item => Assert.Equal(1, item.Id));
        }

        [Fact]
        public void BuildHierarchyLevel_BuildsHierarchy()
        {
            // Arrange.
            var repository = GetRepository();
            var items = new[] { GetRootItem() }.Concat(GetItems());

            // Act.
            var result = repository.BuildHierarchyLevel(GetRootItem(), items);

            // Assert.
            Assert.NotNull(result);
            Assert.True(result.Id == 0);
            Assert.Contains(result.ChildItems, item => item.Id == 2);
            Assert.DoesNotContain(result.ChildItems, item => item.Id == 1);
            Assert.True(result.ChildItems[0].Parent == result);
            Assert.Contains(result.ChildItems[0].ChildItems, item => item.Id == 3);
            Assert.DoesNotContain(result.ChildItems[0].ChildItems, item => item.Id == 1);
            Assert.True(result.ChildItems[0].ChildItems[0].Parent == result.ChildItems[0]);
            Assert.True(result.ChildItems[0].ChildItems[0].AllParents.Count() == 2);
        }

        private static NavigationRepository GetRepository()
        {
            var siteService = new Mock<ISiteService>().Object;
            var siteContextService = new Mock<ISiteContextService>().Object;
            var pageRetriever = new Mock<IPageRetriever>().Object;
            var pageUrlRetriever = new Mock<IPageUrlRetriever>().Object;
            var repository = new NavigationRepository(new RepositoryDependencies(siteService, siteContextService, pageRetriever), pageUrlRetriever);

            return repository;
        }

        private static NavigationItemDto GetRootItem() => new NavigationItemDto
        {
            Id = 0,
            Name = "RootItem"
        };

        private static IEnumerable<NavigationItemDto> GetItems() =>
            new List<NavigationItemDto>
            {
                new NavigationItemDto
                {
                    Id = 1,
                    ParentId = -1
                },
                new NavigationItemDto
                {
                    Id = 2,
                    ParentId = 0
                },
                new NavigationItemDto
                {
                    Id = 3,
                    ParentId = 2
                },
                new NavigationItemDto
                {
                    Id = 4,
                    ParentId = 0
                }

            };

        private static IEnumerable<NavigationItemDto> GetCyclicItems() =>
            new List<NavigationItemDto>
            {
                new NavigationItemDto
                {
                    Id = 1,
                    ParentId = 0
                },
                new NavigationItemDto
                {
                    Id = 2,
                    ParentId = 4
                },
                new NavigationItemDto
                {
                    Id = 3,
                    ParentId = 2
                },
                new NavigationItemDto
                {
                    Id = 4,
                    ParentId = 3
                }
            };
    }
}
