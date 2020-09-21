#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Business.Repositories;

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
            // Content tree-based routing
            //var navigation = !string.IsNullOrEmpty(nodeAliasPath)
            //    ? _navigationRepository.GetSecondaryNavigation(nodeAliasPath)
            //    : _navigationRepository.GetContentTreeNavigation();

            // Conventional routing
            var navigation = _navigationRepository.GetConventionalRoutingNavigation();

            return View(placement, navigation);
        }
    }
}
