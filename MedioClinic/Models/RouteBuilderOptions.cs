using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XperienceAdapter.Models;

namespace MedioClinic.Models
{
    public class RouteBuilderOptions
    {
        public string? RouteName { get; set; }

        public object? RouteDefaults { get; set; }

        public List<CulturePattern> CulturePatterns { get; } = new List<CulturePattern>();

        public RouteBuilderOptions(string? routeName, object? routeDefaults, IEnumerable<(string Culture, string RoutePattern)>? culturePatterns)
        {
            RouteName = routeName ?? throw new ArgumentNullException(nameof(routeName));
            RouteDefaults = routeDefaults ?? throw new ArgumentNullException(nameof(routeDefaults));

            if (culturePatterns?.Any() == true)
            {
                var test01 = culturePatterns.Select(pattern => new CulturePattern
                {
                    Culture = pattern.Culture,
                    RoutePattern = pattern.RoutePattern
                });

                CulturePatterns.AddRange(test01);
            }
            else
            {
                throw new ArgumentException("Culture-specific route patterns are either null or empty.");
            }
        }

        public class CulturePattern
        {
            public string? Culture { get; set; }

            public string? RoutePattern { get; set; }

        }
    }
}
