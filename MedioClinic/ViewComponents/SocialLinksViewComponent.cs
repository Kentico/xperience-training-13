using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Abstractions;
using XperienceAdapter;

namespace MedioClinic.ViewComponents
{
    public class SocialLinksViewComponent : ViewComponent
    {
        public IRepository<SocialLink> SocialLinkRepository { get; }

        public SocialLinksViewComponent(IRepository<SocialLink> socialLinkRepository)
        {
            SocialLinkRepository = socialLinkRepository ?? throw new ArgumentNullException(nameof(socialLinkRepository));
        }

        public IViewComponentResult Invoke()
        {
            var model = SocialLinkRepository.GetAll();

            return View(model);
        }
    }
}
