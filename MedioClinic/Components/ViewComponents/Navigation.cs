#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CMS.DataEngine;
using CMS.DocumentEngine.Routing;
using CMS.SiteProvider;

using XperienceAdapter.Models;
using Business.Repositories;
using Business.Models;

namespace MedioClinic.ViewComponents
{
    public class Navigation : ViewComponent
    {
        protected readonly INavigationRepository _navigationRepository;

        public Navigation(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }

        public IViewComponentResult Invoke(string placement, string? nodeAliasPath = default)
        {
            var siteInfoIdentifier = new SiteInfoIdentifier(SiteContext.CurrentSiteID);
            var routingMode = PageRoutingHelper.GetRoutingMode(siteInfoIdentifier);
            Dictionary<SiteCulture, NavigationItem> navigation;

            if (routingMode == PageRoutingModeEnum.BasedOnContentTree)
            {
                // Content tree-based routing
                navigation = !string.IsNullOrEmpty(nodeAliasPath)
                    ? _navigationRepository.GetSecondaryNavigation(nodeAliasPath)
                    : _navigationRepository.GetContentTreeNavigation();
            }
            else
            {
                // Conventional routing
                navigation = _navigationRepository.GetConventionalRoutingNavigation();
            }

            return View(placement, navigation);
        }
    }
}
