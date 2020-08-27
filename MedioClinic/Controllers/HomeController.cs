using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;

using XperienceAdapter.Repositories;
using Business.Configuration;
using Business.Models;
using MedioClinic.Models;

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

        public IActionResult Index()
        {
            var homePath = "/Home";

            var home = _homePageRepository.GetPages(
                query => query
                    .Path(homePath),
                buildCacheAction: cache => cache
                    .Key($"{nameof(HomeController)}|HomePage")
                    .Dependencies((_, builder) => builder
                        .PageType("MedioClinic.HomePage")),
                includeAttachments: true)
                    .FirstOrDefault();

            var companyServices = _companyServiceRepository.GetPages(
                query => query
                    .Path(homePath, PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(HomeController)}|CompanyServices")
                    .Dependencies((_, builder) => builder
                        .PageType("MedioClinic.CompanyService")
                        .PagePath(homePath, PathTypeEnum.Children)
                        .PageOrder()));

            var data = (home, companyServices);
            var viewModel = GetPageViewModel<(HomePage, IEnumerable<CompanyService>)>(data, home.Name!);

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
