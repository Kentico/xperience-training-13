using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.DocumentEngine;

using Business.Dtos;
using XperienceAdapter;

namespace Business.Repositories
{
    public interface INavigationRepository : IPageRepository<NavigationItemDto, TreeNode>
    {
        NavigationItemDto GetContentTreeNavigation();

        NavigationItemDto GetSecondaryNavigation(string nodeAliasPath);
    }
}
