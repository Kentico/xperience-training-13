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
using Kentico.Content.Web.Mvc.Routing;

using Common.Configuration;
using XperienceAdapter.Models;
using XperienceAdapter.Repositories;
using Business.Models;
using MedioClinic.Controllers;
using MedioClinic.Models;
using XperienceAdapter.Localization;
using Microsoft.Extensions.Localization;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME, typeof(DoctorsController), ActionName = nameof(DoctorsController.Index), Path = "/Doctors")]
[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME, typeof(DoctorsController), ActionName = nameof(DoctorsController.Detail))]
namespace MedioClinic.Controllers
{
    public class DoctorsController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> _doctorRepository;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IStringLocalizer<SharedResource> stringLocalizer,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, optionsMonitor, stringLocalizer)
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

                var doctorPages = await _doctorRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        // TODO: Bug?
                        .FilterDuplicates()
                        .Path(doctorsPath, PathTypeEnum.Children),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsController)}|Doctors")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                            .PagePath(doctorsPath, PathTypeEnum.Children)
                            .PageOrder()));

                if (doctorPages?.Any() == true)
                {
                    var data = (pageDataContext.Page.DocumentName, doctorPages);
                    var viewModel = GetPageViewModel(pageDataContext.Metadata, data);

                    return View(viewModel);
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
                            .Key($"{nameof(DoctorsController)}|Doctor|{doctorPath}")
                            .Dependencies((_, builder) => builder
                                .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                                .PagePath(doctorPath, PathTypeEnum.Single))))
                                .FirstOrDefault();

                    if (doctor != null)
                    {
                        var viewModel = GetPageViewModel(pageDataContext.Metadata, doctor);

                        return View(viewModel);
                    }
                }
            }

            return NotFound();
        }
    }
}