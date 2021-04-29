using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Business.Models;

using CMS.DocumentEngine;

using Core.Configuration;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using MedioClinic.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using XperienceAdapter.Repositories;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME, typeof(LandingPageCtbController))]
namespace MedioClinic.Controllers
{
    public class LandingPageCtbController : BaseController
    {
        private readonly IPageDataContextInitializer _pageDataContextInitializer;

        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<BasicPageWithUrlSlug, TreeNode> _landingPageRepository;

        public LandingPageCtbController(
            ILogger<LandingPageCtbController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextInitializer pageDataContextInitializer,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<BasicPageWithUrlSlug, TreeNode> landingPageRepository)
            : base(logger, optionsMonitor)
        {
            _pageDataContextInitializer = pageDataContextInitializer ?? throw new ArgumentNullException(nameof(pageDataContextInitializer));
            _landingPageRepository = landingPageRepository ?? throw new ArgumentNullException(nameof(landingPageRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.LandingPage>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var landingPagePath = pageDataContext.Page.NodeAliasPath;

                if (!string.IsNullOrEmpty(landingPagePath))
                {
                    var landingPage = (await _landingPageRepository.GetPagesInCurrentCultureAsync(
                        cancellationToken,
                        filter => filter
                            .Path(landingPagePath, PathTypeEnum.Single)
                            .TopN(1),
                        buildCacheAction: cache => cache
                            .Key($"{nameof(LandingPageController)}|Page|{landingPagePath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME)
                                .PagePath(landingPagePath, PathTypeEnum.Single))))
                            .FirstOrDefault();

                    _pageDataContextInitializer.Initialize(pageDataContext.Page);

                    if (landingPage != null)
                    {
                        var viewModel = GetPageViewModel(pageDataContext.Metadata, landingPage);

                        return View("LandingPage/Index", viewModel);
                    }

                }
            }

            return NotFound();
        }
    }
}
