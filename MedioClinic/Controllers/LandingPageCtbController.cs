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
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

// Uncomment if this controller returns a view instead of TemplateResult.
[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME, typeof(LandingPageCtbController))]
namespace MedioClinic.Controllers
{
    public class LandingPageCtbController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<BasePage, TreeNode> _landingPageRepository;

        public LandingPageCtbController(
            ILogger<LandingPageCtbController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<BasePage, TreeNode> landingPageRepository)
            : base(logger, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
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
                            .Key($"{nameof(LandingPageCtbController)}|Page|{landingPagePath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME)
                                .PagePath(landingPagePath, PathTypeEnum.Single))))
                            .FirstOrDefault();

                    if (landingPage != null)
                    {
                        // Implementation without page templates (begin)
                        var viewModel = GetPageViewModel(pageDataContext.Metadata, landingPage);

                        return View("LandingPage/Index", viewModel);
                        // Implementation without page templates (end)

                        // Page template implementation (begin)
                        //return new TemplateResult();
                        // Page template implementation (end)
                    }

                }
            }

            return NotFound();
        }
    }
}
