#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Dtos;
using XperienceAdapter.Repositories;
using Business.Dtos;
using Business.Repositories;

namespace MedioClinic.ViewComponents
{
    public class Language : ViewComponent
    {
        private readonly INavigationRepository _navigationRepository;

        private readonly ISiteCultureRepository _siteCultureRepository;

        public Language(ISiteCultureRepository siteCultureRepository, INavigationRepository navigationRepository)
        {
            _siteCultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }

        public IViewComponentResult Invoke(string cultureSwitchId)
        {
            var cultures = _siteCultureRepository.GetAll();
            var navigation = _navigationRepository.GetConventionalRoutingNavigation();
            var pageCultureVariants = new Dictionary<SiteCulture, (string, string)>();

            if (cultures != null && cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    if (Request.Path.Equals("/"))
                    {
                        if (culture.IsDefault)
                        {
                            pageCultureVariants.Add(culture, ("/", culture.ShortName!));
                        }
                        else
                        {
                            pageCultureVariants.Add(culture, ($"/{culture.IsoCode}", culture.ShortName!));
                        }
                    }
                    else
                    {
                        var cultureVariantNavigation = navigation[culture.IsoCode!];
                        var navigationItem = GetNavigationItemByUrl(Request.Path, cultureVariantNavigation);

                        if (navigationItem != null)
                        {
                            pageCultureVariants.Add(culture, (navigationItem.RelativeUrl!, culture.ShortName!));
                        }

                    }
                }
            }

            var model = (cultureSwitchId, pageCultureVariants);

            return View(model);
        }

        private NavigationItem? GetNavigationItemByUrl(string url, NavigationItem currentItem)
        {
            var parsed = Url.Content(currentItem.RelativeUrl);

            if (!string.IsNullOrEmpty(parsed) && parsed.Equals(url, StringComparison.OrdinalIgnoreCase))
            {
                return currentItem;
            }

            foreach (var item in currentItem.ChildItems)
            {
                return GetNavigationItemByUrl(url, item);
            }

            return null;
        }
    }
}
