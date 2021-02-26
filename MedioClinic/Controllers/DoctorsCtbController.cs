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
using Core.Configuration;
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

        public DoctorsCtbController(
            ILogger<DoctorsCtbController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.SiteSection>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var doctorsPath = pageDataContext.Page.NodeAliasPath;
                var title = pageDataContext.Page.DocumentName ?? string.Empty;

                var doctorPages = await _doctorRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        .Path(doctorsPath, PathTypeEnum.Children),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsCtbController)}|Doctors")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                            .PagePath(doctorsPath, PathTypeEnum.Children)
                            .PageOrder()));

                if (doctorPages?.Any() == true)
                {
                    var viewModel = GetPageViewModel(pageDataContext.Metadata, doctorPages);

                    return View("Doctors/Index", viewModel);
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Detail(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.Doctor>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var doctorPath = pageDataContext.Page.NodeAliasPath;

                if (!string.IsNullOrEmpty(doctorPath))
                {
                    var doctor = (await _doctorRepository.GetPagesInCurrentCultureAsync(
                        cancellationToken,
                        filter => filter
                            .Path(doctorPath, PathTypeEnum.Single)
                            .TopN(1),
                        buildCacheAction: cache => cache
                            .Key($"{nameof(DoctorsCtbController)}|Doctor|{doctorPath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                                .PagePath(doctorPath, PathTypeEnum.Single))))
                                .FirstOrDefault();

                    if (doctor != null)
                    {
                        var viewModel = GetPageViewModel(pageDataContext.Metadata, doctor);

                        return View("Doctors/Detail", viewModel);
                    }
                }
            }

            return NotFound();
        }
    }
}