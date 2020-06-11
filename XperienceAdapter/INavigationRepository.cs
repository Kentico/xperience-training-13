using System;
using System.Collections.Generic;
using System.Text;

using Abstractions;
using XperienceAdapter.Dtos;

namespace XperienceAdapter
{
    public interface INavigationRepository : IRepository<NavigationItem>
    {
        NavigationItem GetContentTreeNavigation();

        NavigationItem GetSecondaryNavigation(string nodeAliasPath);
    }
}
