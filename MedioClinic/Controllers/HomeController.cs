using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CMS.DocumentEngine;

using MedioClinic.Models;
using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IPageRepository<Business.Models.HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> _homePageRepository;

        private readonly IPageRepository<Business.Models.CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> _companyServiceRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IPageRepository<Business.Models.HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> homePageRepository,
            IPageRepository<Business.Models.CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> companyServiceRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var viewModel = (home, companyServices);

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
