using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using Business.Repositories;
using MedioClinic.ViewComponents;
using Business.Dtos;

namespace MedioClinic.Tests.ViewComponents
{
    public class NavigationViewComponentTests
    {
        private const string NodeAliasPath = "/TestNavigationSection";

        [Fact]
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
            var childrenOf11 = new List<NavigationItemDto>
            {
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-1-1",
                    Name = "Child 1-1-1"
                },
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-1-2",
                    Name = "Child 1-1-2"
                }
            };

            var childrenOf12 = new List<NavigationItemDto>
            {
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-2-1",
                    Name = "Child 1-2-1"
                },
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-2-2",
                    Name = "Child 1-2-2"
                }
            };

            var childrenOfRoot = new List<NavigationItemDto>
            {
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-1",
                    Name = "Child 1-1"
                },
                new NavigationItemDto
                {
                    RelativeUrl = "/Child1-2",
                    Name = "Child 1-2"
                }
            };

            var navigationItem = new NavigationItemDto
            {
                RelativeUrl = NodeAliasPath,
                Name = "Test navigation section"
            };

            navigationItem.ChildItems.AddRange(childrenOfRoot);
            childrenOfRoot[0].ChildItems.AddRange(childrenOf11);
            childrenOfRoot[1].ChildItems.AddRange(childrenOf12);

            var navigationRepository = new Mock<INavigationRepository>();
            navigationRepository.Setup(repository => repository.GetSecondaryNavigation(NodeAliasPath)).Returns(navigationItem);

            return navigationRepository;
        }
    }
}
