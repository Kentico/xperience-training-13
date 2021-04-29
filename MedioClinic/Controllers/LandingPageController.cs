using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Business.Models;

using CMS.DocumentEngine;

using Core.Configuration;

using Kentico.Content.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    public class LandingPageController : BaseController
    {
        private const string LandingPagePath = "/Landing-pages";

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageMetadataRetriever _metadataRetriever;

        private readonly IPageDataContextInitializer _pageDataContextInitializer;

        private readonly IPageRepository<BasicPageWithUrlSlug, TreeNode> _landingPageRepository;

        public LandingPageController(
            ILogger<LandingPageController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageRetriever pageRetriever,
            IPageMetadataRetriever metadataRetriever,
            IPageDataContextInitializer pageDataContextInitializer,
            IPageRepository<BasicPageWithUrlSlug, TreeNode> landingPageRepository)
            : base (logger, optionsMonitor)
        {
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _metadataRetriever = metadataRetriever ?? throw new ArgumentNullException(nameof(metadataRetriever));
            _pageDataContextInitializer = pageDataContextInitializer ?? throw new ArgumentNullException(nameof(pageDataContextInitializer));
            _landingPageRepository = landingPageRepository ?? throw new ArgumentNullException(nameof(landingPageRepository));
        }

        public async Task<IActionResult> Index(string? urlSlug, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(urlSlug))
            {
                var page = (await _pageRetriever.RetrieveAsync<CMS.DocumentEngine.Types.MedioClinic.LandingPage>(
                    filter => filter
                        .Path(LandingPagePath, PathTypeEnum.Children)
                        .WhereEquals(nameof(CMS.DocumentEngine.Types.MedioClinic.BasicPageWithUrlSlug.UrlSlug), urlSlug)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(LandingPageController)}|Page|{urlSlug}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME))))
                        .FirstOrDefault();

                var landingPage = (await _landingPageRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        .Path(LandingPagePath, PathTypeEnum.Children)
                        .WhereEquals(nameof(CMS.DocumentEngine.Types.MedioClinic.BasicPageWithUrlSlug.UrlSlug), urlSlug)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(LandingPageController)}|Page|{urlSlug}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.LandingPage.CLASS_NAME))))
                        .FirstOrDefault();

                var metadata = _metadataRetriever.Retrieve(page);

                _pageDataContextInitializer.Initialize(page);

                if (landingPage != null)
                {
                    var viewModel = GetPageViewModel(metadata, landingPage);

                    return View("LandingPage/Index", viewModel);
                }
            }

            return NotFound();
        }
    }
}
