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
        private readonly IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink> _socialLinkRepository;

        public SocialLinks(IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink> socialLinkRepository)
        {
            _socialLinkRepository = socialLinkRepository ?? throw new ArgumentNullException(nameof(socialLinkRepository));
        }

        public IViewComponentResult Invoke()
        {
            var path = "/Reused-content/Social-links";

            var model = _socialLinkRepository.GetPages(
                query => query
                    .Path(path, CMS.DocumentEngine.PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(SocialLinks)}|all")
                    .Dependencies((_, builder) => builder
                        .PageType("MedioClinic.SocialLink")
                        .PagePath(path, CMS.DocumentEngine.PathTypeEnum.Children)
                        .PageOrder()));

            return View(model);
        }
    }
}
