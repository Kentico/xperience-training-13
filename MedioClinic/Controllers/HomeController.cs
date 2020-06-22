using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.MedioClinic;

using XperienceAdapter;
using Business.Dtos;
using MedioClinic.Models;

namespace MedioClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IPageRepository<HomePageDto, HomePage> _homePageRepository;

        private readonly IPageRepository<CompanyServiceDto, CompanyService> _companyServiceRepository;

        public HomeController(
            ILogger<HomeController> logger, 
            IPageRepository<HomePageDto, HomePage> homePageRepository,
            IPageRepository<CompanyServiceDto, CompanyService> companyServiceRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _homePageRepository = homePageRepository ?? throw new ArgumentNullException(nameof(homePageRepository));
            _companyServiceRepository = companyServiceRepository ?? throw new ArgumentNullException(nameof(companyServiceRepository));

        }

        public IActionResult Index()
        {
            var companyServices = _companyServiceRepository.GetPages(query => 
                query.Path("/Home/", PathTypeEnum.Children));

            _logger.LogInformation("info 1", "params object 1", "params object 2");
            _logger.LogInformation(99, "info 2");
            _logger.LogInformation(new Exception(), "info 3");
            _logger.LogInformation(999, new Exception(), "info 4");
            _logger.LogDebug("debug 1");
            _logger.LogError("error 1");
            _logger.LogTrace("trace 1");
            _logger.LogCritical("critical 1");
            _logger.Log<int>(LogLevel.Information, 9999, 99991, new Exception("state 1"), (state, exception) => $"{state}; {exception.Message}");

            return View();
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
