using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Repositories;
using Business.Configuration;
using Business.Models;
using MedioClinic.Models;
using Kentico.Content.Web.Mvc.Routing;
using MedioClinic.Controllers;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME, typeof(DoctorsCtbController))]
namespace MedioClinic.Controllers
{
    public class DoctorsCtbController : BaseController
    {

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> _doctorRepository;

        

        public DoctorsCtbController(
            ILogger<HomeController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            PageViewModel? viewModel = default;

            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.Doctor>(out var pageDataContext)
                && pageDataContext.Page != null)
            {

               var doctorsPath = pageDataContext.Page.NodeAliasPath;

                var title = (await _pageRetriever.RetrieveAsync<TreeNode>(
                filter => filter
                    .Path(doctorsPath, PathTypeEnum.Single)
                    .TopN(1),
                cache => cache
                    .Key($"{nameof(DoctorsController)}|DoctorsSection")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME)),
                cancellationToken))
                    .FirstOrDefault()?
                    .DocumentName ?? string.Empty;

                var doctorPages = await _doctorRepository.GetPagesAsync(
                    cancellationToken,
                    filter => filter
                        .Path(doctorsPath, PathTypeEnum.Children),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsController)}|Doctors")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                            .PageOrder()));

                viewModel = GetPageViewModel(doctorPages, title);
            }

            return View("Doctors/Index", viewModel);
        }

        public async Task<IActionResult> Detail(string? urlSlug, CancellationToken cancellationToken)
        {

            PageViewModel? viewModel = default;

            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.Doctor>(out var pageDataContext)
                && pageDataContext.Page != null)
            {

                var doctorsPath = pageDataContext.Page.NodeAliasPath;

                if (!string.IsNullOrEmpty(urlSlug))
                {
                    var doctor = (await _doctorRepository.GetPagesAsync(
                        cancellationToken,
                        filter => filter
                            .Path(doctorsPath, PathTypeEnum.Children)
                            .WhereEquals(nameof(CMS.DocumentEngine.Types.MedioClinic.Doctor.DoctorFields.UrlSlug), urlSlug)
                            .TopN(1),
                        buildCacheAction: cache => cache
                            .Key($"{nameof(DoctorsController)}|Doctor|{urlSlug}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME))))
                                .FirstOrDefault();

                    viewModel = GetPageViewModel(doctor, doctor?.Name!);

                    return View("Doctors/Detail", viewModel);
                }
            }

            return NotFound();
        }
    }
}