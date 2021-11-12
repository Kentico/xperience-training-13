#define no_suffix

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XperienceAdapter.Cookies;

namespace MedioClinic.Components.ViewComponents
{
    public class XperienceAnalyticalTools : ViewComponent
    {
        private readonly ICookieManager _cookieManager;

        public XperienceAnalyticalTools(ICookieManager cookieManager)
        {
            _cookieManager = cookieManager ?? throw new ArgumentNullException(nameof(cookieManager));
        }

        public IViewComponentResult Invoke()
        {
            if (_cookieManager.VisitorCookiesEnabled)
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
