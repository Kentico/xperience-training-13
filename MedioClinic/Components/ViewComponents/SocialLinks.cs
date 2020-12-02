#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Repositories;
using Business.Models;

namespace MedioClinic.ViewComponents
{
    public class SocialLinks : ViewComponent
    {
        private const string PagePath = "/Reused-content/Social-links";

        private readonly IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink> _socialLinkRepository;

        public SocialLinks(IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink> socialLinkRepository)
        {
            _socialLinkRepository = socialLinkRepository ?? throw new ArgumentNullException(nameof(socialLinkRepository));
        }

        public IViewComponentResult Invoke()
        {
            var model = _socialLinkRepository.GetPagesInCurrentCulture(
                filter => filter
                    .Path(PagePath, CMS.DocumentEngine.PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(SocialLinks)}|{nameof(Invoke)}")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.SocialLink.CLASS_NAME)
                        .PagePath(PagePath, CMS.DocumentEngine.PathTypeEnum.Children)
                        .PageOrder()));

            return View(model);
        }
    }
}
