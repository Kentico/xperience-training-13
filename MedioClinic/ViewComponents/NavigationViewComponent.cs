#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter;

namespace MedioClinic.ViewComponents
{
    public class Navigation : ViewComponent
    {
        public static string PartialViewPath => $"Components/{nameof(Navigation)}";

        public INavigationRepository NavigationRepository { get; }

        public Navigation(INavigationRepository navigationRepository)
        {
            NavigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }

        public IViewComponentResult Invoke(string placement, string nodeAliasPath = null!)
        {
            var navigation = !string.IsNullOrEmpty(nodeAliasPath)
                ? NavigationRepository.GetSecondaryNavigation(nodeAliasPath)
                : NavigationRepository.GetContentTreeNavigation();

            return View(placement, navigation);
        }
    }
}
