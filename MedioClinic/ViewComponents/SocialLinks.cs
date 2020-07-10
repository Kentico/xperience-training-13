#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Abstractions;
using Business.Dtos;

namespace MedioClinic.ViewComponents
{
    public class SocialLinks : ViewComponent
    {
        public IRepository<SocialLink> SocialLinkRepository { get; }

        public SocialLinks(IRepository<SocialLink> socialLinkRepository)
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
