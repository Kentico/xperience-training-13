#define no_suffix

using System;
using Microsoft.AspNetCore.Mvc;

using Business.Repositories;

namespace MedioClinic.ViewComponents
{
    public class Navigation : ViewComponent
    {
        private readonly INavigationRepository _navigationRepository;

        public Navigation(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }

        public IViewComponentResult Invoke(string placement, string? nodeAliasPath = default)
        {
            var navigation = _navigationRepository.GetNavigation(nodeAliasPath: nodeAliasPath);

            return View(placement, navigation);
        }
    }
}
