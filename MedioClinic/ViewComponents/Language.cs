#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter;

namespace MedioClinic.ViewComponents
{
    public class Language : ViewComponent
    {
        public ICultureRepository CultureRepository { get; }

        public Language(ICultureRepository cultureRepository)
        {
            CultureRepository = cultureRepository ?? throw new ArgumentNullException(nameof(cultureRepository));
        }

        public IViewComponentResult Invoke(string cultureSwitchId)
        {
            var cultures = CultureRepository.GetAll();
            var model = (cultureSwitchId, cultures);

            return View(model);
        }
    }
}
