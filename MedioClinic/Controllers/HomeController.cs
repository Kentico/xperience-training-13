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
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using Common.Configuration;
using XperienceAdapter.Repositories;
using Business.Models;
using MedioClinic.Controllers;
using MedioClinic.Models;
using XperienceAdapter.Localization;
using Microsoft.Extensions.Localization;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.HomePage.CLASS_NAME, typeof(HomeController))]
namespace MedioClinic.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> _homePageRepository;

        private readonly IPageRepository<CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> _companyServiceRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage> homePageRepository,
            IPageRepository<CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService> companyServiceRepository)
            : base(logger, optionsMonitor, stringLocalizer)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _homePageRepository = homePageRepository ?? throw new ArgumentNullException(nameof(homePageRepository));
            _companyServiceRepository = companyServiceRepository ?? throw new ArgumentNullException(nameof(companyServiceRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.HomePage>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var homePath = pageDataContext.Page.NodeAliasPath;

                var homePage = (await _homePageRepository.GetPagesInCurrentCultureAsync(
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

                var companyServices = await _companyServiceRepository.GetPagesInCurrentCultureAsync(
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
                    var viewModel = GetPageViewModel<(HomePage, IEnumerable<CompanyService>)>(pageDataContext.Metadata, data);

                    return View(viewModel);
                }
            }

            return NotFound();
        }
    }
}
