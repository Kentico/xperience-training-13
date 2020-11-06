#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;
using Business.Models;
using Business.Repositories;
using System.Text.RegularExpressions;

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
            var defaultCulture = _siteCultureRepository.DefaultSiteCulture;
            var navigation = _navigationRepository.GetConventionalRoutingNavigation();
            var searchPath = Request.Path.Equals("/") && defaultCulture != null ? $"/{defaultCulture.IsoCode?.ToLowerInvariant()}/home/" : Request.Path.Value;
            var variants = GetDatabaseUrlVariants(navigation, searchPath) ?? GetNonDatabaseUrlVariants(searchPath);
            var model = (cultureSwitchId, variants.ToDictionary(kvp1 => kvp1.Key, kvp2 => kvp2.Value));

            return View(model);
        }

        private IEnumerable<KeyValuePair<SiteCulture, string?>>? GetDatabaseUrlVariants(Dictionary<SiteCulture, NavigationItem> navigation, string url, NavigationItem? currentItem = default)
        {
            var item = currentItem ?? navigation.FirstOrDefault(nav => nav.Key.IsoCode?.Equals(Thread.CurrentThread.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase) == true).Value;

            if (item != null)
            {
                var parsed = Url.Content(item.RelativeUrl);

                if (parsed?.Equals(url, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return navigation
                        .Select(cultureNavigation => new KeyValuePair<SiteCulture, string?>(cultureNavigation.Key, GetUrlByNodeId(item.NodeId, cultureNavigation.Key, cultureNavigation.Value)))
                        .Where(cultureNavigation => cultureNavigation.Value != null);
                }

                foreach (var childItem in item.ChildItems)
                {
                    return GetDatabaseUrlVariants(navigation, url, childItem);
                }
            }

            return null;
        }

        private string? GetUrlByNodeId(int nodeId, SiteCulture siteCulture, NavigationItem currentItem) =>
            currentItem.NodeId == nodeId && currentItem.Culture == siteCulture
                ? currentItem.RelativeUrl
                : currentItem.ChildItems.FirstOrDefault(child => GetUrlByNodeId(nodeId, siteCulture, child) != null)?.RelativeUrl;

        private IEnumerable<KeyValuePair<SiteCulture, string?>>? GetNonDatabaseUrlVariants(string url)
        {
            var cultures = _siteCultureRepository.GetAll();
            var segments = url.Split('/');

            if (cultures.Any(culture => culture.IsoCode?.Equals(segments?[1], StringComparison.InvariantCultureIgnoreCase) == true))
            {
                var trailingPath = string.Join('/', segments.Skip(2));

                return cultures.Select(culture => new KeyValuePair<SiteCulture, string?>(culture, $"/{culture.IsoCode?.ToLower()}/{trailingPath}"));
            }

            return null;
        }
    }
}
