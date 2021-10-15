#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Extensions;
using XperienceAdapter.Models;
using XperienceAdapter.Repositories;
using Business.Models;
using Business.Repositories;
using System.Threading.Tasks;

namespace MedioClinic.Components.ViewComponents
{
    public class CultureSwitch : ViewComponent
    {
        private readonly string[] ExcludedPaths =
        {
            "landing-pages",
            "paginas-de-destino"
        };

        private readonly INavigationRepository _navigationRepository;

        private readonly ISiteCultureRepository _siteCultureRepository;

        public CultureSwitch(ISiteCultureRepository siteCultureRepository, INavigationRepository navigationRepository)
        {
            _siteCultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }

        public async Task<IViewComponentResult> InvokeAsync(string cultureSwitchId)
        {
            var variants = await GetUrlCultureVariantsAsync();
            var model = (cultureSwitchId, variants?.ToDictionary(kvp1 => kvp1.Key, kvp2 => kvp2.Value));

            return View(model);
        }

        private async Task<IEnumerable<KeyValuePair<SiteCulture, string>>>? GetUrlCultureVariantsAsync()
        {
            var defaultCulture = _siteCultureRepository.DefaultSiteCulture;
            var completePath = string.IsNullOrEmpty(Request.PathBase) ? Request.Path.Value : $"{Request.PathBase}{Request.Path.Value}";
            var searchPath = Request.Path.Equals("/") && defaultCulture != null ? $"/{defaultCulture.IsoCode?.ToLowerInvariant()}/home/" : completePath;
            var currentCulture = Thread.CurrentThread.CurrentUICulture.ToSiteCulture();

            if (currentCulture != null && !ExcludedPaths.Any(path => searchPath.Contains(path)))
            {
                return await GetDatabaseUrlVariantsAsync(searchPath, currentCulture) ?? GetNonDatabaseUrlVariants(searchPath);
            }

            return null;
        }

        private async Task<IEnumerable<KeyValuePair<SiteCulture, string>>>? GetDatabaseUrlVariantsAsync(string searchPath, SiteCulture currentCulture)
        {
            var navigation = await _navigationRepository.GetWholeNavigationAsync();
            var currentPageNavigationItem = GetNavigationItemByRelativeUrl(searchPath, navigation[currentCulture]);

            if (currentPageNavigationItem != null)
            {
                var databaseVariants = new List<KeyValuePair<SiteCulture, NavigationItem>>();
                databaseVariants.Add(new KeyValuePair<SiteCulture, NavigationItem>(currentCulture, currentPageNavigationItem));

                foreach (var cultureVariant in navigation.Where(cultureVariant => !cultureVariant.Key.Equals(currentCulture)))
                {
                    var otherCultureNavigationItem = _navigationRepository.GetNavigationItemByNodeId(currentPageNavigationItem.NodeId, cultureVariant.Value);

                    if (otherCultureNavigationItem != null)
                    {
                        databaseVariants.Add(new KeyValuePair<SiteCulture, NavigationItem>(cultureVariant.Key, otherCultureNavigationItem));
                    }
                }

                return databaseVariants.Select(variant => new KeyValuePair<SiteCulture, string>(variant.Key, variant.Value.RelativeUrl!));
            }

            return null;
        }

        private NavigationItem? GetNavigationItemByRelativeUrl(string searchPath, NavigationItem startingPointItem)
        {
            if (startingPointItem != null)
            {
                var parsed = Url.Content(startingPointItem.RelativeUrl);

                if (parsed?.Equals(searchPath, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return startingPointItem;
                }
                else if (startingPointItem.ChildItems?.Any() == true)
                {
                    var matches = new List<NavigationItem>();

                    foreach (var child in startingPointItem.ChildItems)
                    {
                        var childMatch = GetNavigationItemByRelativeUrl(searchPath, child);
                        matches.Add(childMatch!);
                    }

                    return matches.FirstOrDefault(match => match != null);
                }
            }

            return null;
        }

        private IEnumerable<KeyValuePair<SiteCulture, string>>? GetNonDatabaseUrlVariants(string searchPath)
        {
            var cultures = _siteCultureRepository.GetAll();
            var segments = searchPath.Split('/');

            if (cultures.Any(culture => culture.IsoCode?.Equals(segments?[1], StringComparison.InvariantCultureIgnoreCase) == true))
            {
                var trailingPath = string.Join('/', segments.Skip(2));

                return cultures.Select(culture => new KeyValuePair<SiteCulture, string>(culture, $"/{culture.IsoCode?.ToLower()}/{trailingPath}"));
            }

            return null;
        }
    }
}
