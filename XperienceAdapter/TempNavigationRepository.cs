using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter.Dtos;

namespace XperienceAdapter
{
    /// <summary>
    /// Temporary mock used for manual view testing.
    /// </summary>
    public class TempNavigationRepository : INavigationRepository
    {
        private const string NodeAliasPath = "/TestNavigationSection";

        public IEnumerable<NavigationItem> GetAll() =>
            new List<NavigationItem> { NavigationItem };

        public NavigationItem GetContentTreeNavigation() =>
            NavigationItem;

        public NavigationItem GetSecondaryNavigation(string nodeAliasPath) =>
            NavigationItem;

        public NavigationItem NavigationItem
        {
            get
            {
                return new NavigationItem
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
            }
        }
    }
}
