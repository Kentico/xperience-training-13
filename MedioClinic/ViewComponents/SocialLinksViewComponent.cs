using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MedioClinic.ViewComponents
{
    public class SocialLinksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // TODO: Retrieve link data.
            return View();
        }
    }
}
