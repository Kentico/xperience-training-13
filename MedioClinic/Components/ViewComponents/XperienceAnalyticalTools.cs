#define no_suffix

using CMS.Helpers;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Components.ViewComponents
{
    public class XperienceAnalyticalTools : ViewComponent
    {
        private readonly ICurrentCookieLevelProvider _currentCookieLevelProvider;

        public bool VisitorCookiesEnabled => _currentCookieLevelProvider.GetCurrentCookieLevel() == CookieLevel.Visitor;

        public XperienceAnalyticalTools(ICurrentCookieLevelProvider currentCookieLevelProvider)
        {
            _currentCookieLevelProvider = currentCookieLevelProvider ?? throw new ArgumentNullException(nameof(currentCookieLevelProvider));
        }

        public IViewComponentResult Invoke()
        {
            if (VisitorCookiesEnabled)
            {
                return View("LoggingScripts");
            }
            else
            {
                return Content(string.Empty);
            }
        }
    }
}
