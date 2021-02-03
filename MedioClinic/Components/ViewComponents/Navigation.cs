#define no_suffix

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Extensions;
using XperienceAdapter.Repositories;
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

        public IViewComponentResult Invoke(string placement)
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.ToSiteCulture();
            var navigation = _navigationRepository.GetNavigation(currentCulture);

            return View(placement, navigation);
        }
    }
}
