using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;

using XperienceAdapter.Repositories;
using Business.Configuration;
using Business.Models;

namespace MedioClinic.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IPageRepository<HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> _homePageRepository;

        private readonly IPageRepository<CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> _companyServiceRepository;

        public HomeController(
            ILogger<HomeController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageRepository<HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> homePageRepository,
            IPageRepository<CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> companyServiceRepository) 
            : base(logger, siteService, optionsMonitor)
        {
            _homePageRepository = homePageRepository ?? throw new ArgumentNullException(nameof(homePageRepository));
            _companyServiceRepository = companyServiceRepository ?? throw new ArgumentNullException(nameof(companyServiceRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var homePath = "/Home";

            var homePage = (await _homePageRepository.GetPagesAsync(
                cancellationToken,
                filter => filter
                    .Path(homePath, PathTypeEnum.Single)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(HomeController)}|HomePage")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.HomePage.CLASS_NAME)),
                includeAttachments: true))
                    .FirstOrDefault();

            var companyServices = await _companyServiceRepository.GetPagesAsync(
                cancellationToken,
                filter => filter
                    .Path(homePath, PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(HomeController)}|CompanyServices")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.CompanyService.CLASS_NAME)
                        .PagePath(homePath, PathTypeEnum.Children)
                        .PageOrder()));

            if (homePage != null && companyServices?.Any() == true)
            {
                var data = (homePage, companyServices);
                var viewModel = GetPageViewModel<(HomePage, IEnumerable<CompanyService>)>(data, homePage.Name!);

                return View("Home/Index", viewModel); 
            }

            return NotFound();
        }
    }
}
