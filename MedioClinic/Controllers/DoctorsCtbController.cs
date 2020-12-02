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
using XperienceAdapter.Models;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME, typeof(DoctorsCtbController), ActionName = nameof(DoctorsCtbController.Index), Path = "/Doctors")]
[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME, typeof(DoctorsCtbController), ActionName = nameof(DoctorsCtbController.Detail))]
namespace MedioClinic.Controllers
{
    public class DoctorsCtbController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> _doctorRepository;

        private readonly IPageRepository<BasePage, TreeNode> _basePageRepository;

        public DoctorsCtbController(
            ILogger<DoctorsCtbController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<BasePage, TreeNode> basePageRepository,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _basePageRepository = basePageRepository ?? throw new ArgumentNullException(nameof(basePageRepository));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.SiteSection>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var doctorsPath = pageDataContext.Page.NodeAliasPath;

                var doctorsSection = (await _basePageRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        .Path(doctorsPath, PathTypeEnum.Single)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsCtbController)}|DoctorsSection")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME))))
                                .FirstOrDefault();

                var title = doctorsSection?.Name ?? string.Empty;

                var doctorPages = await _doctorRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        .Path(doctorsPath, PathTypeEnum.Children),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsCtbController)}|Doctors")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                            .PageOrder()));

                if (doctorsSection != null && doctorPages?.Any() == true)
                {
                    var data = (doctorsSection, doctorPages);
                    var viewModel = GetPageViewModel(data, title);

                    return View("Doctors/Index", viewModel);
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Detail(CancellationToken cancellationToken)
        {
            PageViewModel? viewModel = default;

            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.Doctor>(out var pageDataContext)
                && pageDataContext.Page != null)
            {

                var doctorsPath = pageDataContext.Page.NodeAliasPath;

                if (!string.IsNullOrEmpty(doctorsPath))
                {
                    var doctor = (await _doctorRepository.GetPagesInCurrentCultureAsync(
                        cancellationToken,
                        filter => filter
                            .Path(doctorsPath, PathTypeEnum.Single),
                        buildCacheAction: cache => cache
                            .Key($"{nameof(DoctorsCtbController)}|Doctor|{doctorsPath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME))))
                                .FirstOrDefault();

                    if (doctor != null)
                    {
                        viewModel = GetPageViewModel(doctor, doctor?.Name!);

                        return View("Doctors/Detail", viewModel); 
                    }
                }
            }

            return NotFound();
        }
    }
}