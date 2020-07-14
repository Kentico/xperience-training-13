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
            var defaultCulture = _siteCultureRepository.DefaultSiteCulture;
            var navigation = _navigationRepository.GetConventionalRoutingNavigation();
            var pageCultureVariants = new Dictionary<SiteCulture, string>();
            

            if (cultures?.Any() == true)
            {
                var searchPath = Request.Path.Equals("/") ? $"/{defaultCulture.IsoCode}/home/" : Request.Path.Value;

                foreach (var culture in cultures)
                {
                    NavigationItem cultureVariantNavigation = null!;

                    try
                    {
                        cultureVariantNavigation = navigation[culture.IsoCode!];
                    }
                    catch
                    {

                    }

                    if (cultureVariantNavigation != null)
                    {
                        var navigationItem = GetNavigationItemByUrl(searchPath, cultureVariantNavigation);

                        if (navigationItem != null)
                        {
                            pageCultureVariants.Add(culture, navigationItem.RelativeUrl!);
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

            if (parsed?.Equals(url, StringComparison.OrdinalIgnoreCase) == true)
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
