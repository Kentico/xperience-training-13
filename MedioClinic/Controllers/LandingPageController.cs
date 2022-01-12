using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Business.Models;

using CMS.DocumentEngine;

using Common.Configuration;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using XperienceAdapter.Localization;
using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

// Uncomment if this controller returns a view instead of TemplateResult.
//[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME, typeof(LandingPageController))]
namespace MedioClinic.Controllers
{
    public class LandingPageController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<BasicPage, TreeNode> _landingPageRepository;

        public LandingPageController(
            ILogger<LandingPageController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<BasicPage, TreeNode> landingPageRepository)
            : base(logger, optionsMonitor, stringLocalizer)
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
                            .Key($"{nameof(LandingPageController)}|Page|{landingPagePath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME)
                                .PagePath(landingPagePath, PathTypeEnum.Single))))
                            .FirstOrDefault();

                    if (landingPage != null)
                    {
                        // Implementation without page templates (begin)
                        //var viewModel = GetPageViewModel(pageDataContext.Metadata, landingPage);

                        //return View(viewModel);
                        // Implementation without page templates (end)

                        // Page template implementation (begin)
                        return new TemplateResult();
                        // Page template implementation (end)
                    }

                }
            }

            return NotFound();
        }
    }
}
