using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using Business.Repositories;
using MedioClinic.ViewComponents;
using Business.Models;
using XperienceAdapter.Models;

namespace MedioClinic.Tests.ViewComponents
{
    public class NavigationViewComponentTests
    {
        private const string NodeAliasPath = "/TestNavigationSection";

        //[Fact]
        public void Invoke_ReturnsResult()
        {
            var repositoryMock = GetNavigationRepository();
            var component = new Navigation(repositoryMock.Object);

            var result = component.Invoke("Top", NodeAliasPath);

            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
        }

        private Mock<INavigationRepository> GetNavigationRepository()
        {
            var childrenOf11 = new List<NavigationItem>
            {
                new NavigationItem
                {
                    RelativeUrl = "/Child1-1-1",
                    Name = "Child 1-1-1"
                },
                new NavigationItem
                {
                    RelativeUrl = "/Child1-1-2",
                    Name = "Child 1-1-2"
                }
            };

            var childrenOf12 = new List<NavigationItem>
            {
                new NavigationItem
                {
                    RelativeUrl = "/Child1-2-1",
                    Name = "Child 1-2-1"
                },
                new NavigationItem
                {
                    RelativeUrl = "/Child1-2-2",
                    Name = "Child 1-2-2"
                }
            };

            var childrenOfRoot = new List<NavigationItem>
            {
                new NavigationItem
                {
                    RelativeUrl = "/Child1-1",
                    Name = "Child 1-1"
                },
                new NavigationItem
                {
                    RelativeUrl = "/Child1-2",
                    Name = "Child 1-2"
                }
            };

            var navigationItem = new NavigationItem
            {
                RelativeUrl = NodeAliasPath,
                Name = "Test navigation section"
            };

            navigationItem.ChildItems.AddRange(childrenOfRoot);
            childrenOfRoot[0].ChildItems.AddRange(childrenOf11);
            childrenOfRoot[1].ChildItems.AddRange(childrenOf12);

            var set = new Dictionary<SiteCulture, NavigationItem>
            {
                {
                    new SiteCulture
                    {
                        IsoCode = "en-US"
                    },
                    navigationItem
                },
                {
                    new SiteCulture
                    {
                        IsoCode = "cs-CZ",
                    },
                    navigationItem
                }
            };

            var navigationRepository = new Mock<INavigationRepository>();
            navigationRepository.Setup(repository => repository.GetSecondaryNavigation(NodeAliasPath)).Returns(set);

            return navigationRepository;
        }
    }
}
