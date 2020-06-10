using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter;
using MedioClinic.ViewComponents;

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
            var navigationItem = new NavigationItem
            {
                RelativeUrl = NodeAliasPath,
                Title = "Test navigation section",
                ChildItems = new List<NavigationItem>
                {
                    new NavigationItem
                    {
                        RelativeUrl = "/Child1-1",
                        Title = "Child 1-1",
                        ChildItems = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                RelativeUrl = "/Child1-1-1",
                                Title = "Child 1-1-1",
                                ChildItems = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        RelativeUrl = "/Child1-1-1-1",
                                        Title = "Child 1-2-1"
                                    },
                                    new NavigationItem
                                    {
                                        RelativeUrl = "/Child1-1-1-2",
                                        Title = "Child 1-1-1-2"
                                    }
                                }
                            },
                            new NavigationItem
                            {
                                RelativeUrl = "/Child1-1-2",
                                Title = "Child 1-1-2",
                                ChildItems = new List<NavigationItem>
                                {
                                    new NavigationItem
                                    {
                                        RelativeUrl = "/Child1-1-2-1",
                                        Title = "Child 1-1-2-1"
                                    },
                                    new NavigationItem
                                    {
                                        RelativeUrl = "/Child1-1-2-2",
                                        Title = "Child 1-1-2-2"
                                    }
                                }
                            }
                        }
                    },
                    new NavigationItem
                    {
                        RelativeUrl = "/Child1-2",
                        Title = "Child 1-2",
                        ChildItems = new List<NavigationItem>
                        {
                            new NavigationItem
                            {
                                RelativeUrl = "/Child1-2-1",
                                Title = "Child 1-2-1"
                            },
                            new NavigationItem
                            {
                                RelativeUrl = "/Child1-2-2",
                                Title = "Child 1-2-2"
                            }
                        }
                    },
                }
            };

            var navigationRepository = new Mock<INavigationRepository>();
            navigationRepository.Setup(repository => repository.GetSecondaryNavigation(NodeAliasPath)).Returns(navigationItem);

            return navigationRepository;
        }
    }
}
