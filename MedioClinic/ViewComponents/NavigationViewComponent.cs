#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MedioClinic.ViewComponents
{
    public class Navigation : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string placement)
        {
            // TODO: Retrieve menu items.
            // TODO: Separate the language switch into a component of its own.
            return View(placement);
        }
    }
}
