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

namespace MedioClinic.Controllers
{
    public class DoctorsController : BaseController
    {
        private const string DoctorsPath = "/Doctors";

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> _doctorRepository;

        public DoctorsController(
            ILogger<HomeController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageRetriever pageRetriever,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var title = (await _pageRetriever.RetrieveAsync<TreeNode>(
                filter => filter
                    .Path(DoctorsPath, PathTypeEnum.Single)
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
                    .Path(DoctorsPath, PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(DoctorsController)}|Doctors")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                        .PageOrder()));

            var viewModel = GetPageViewModel(doctorPages, title);

            return View("Doctors/Index", viewModel);
        }

        public async Task<IActionResult> Detail(string? urlSlug, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(urlSlug))
            {
                var doctor = (await _doctorRepository.GetPagesAsync(
                    cancellationToken,
                    filter => filter
                        .Path(DoctorsPath, PathTypeEnum.Children)
                        .WhereEquals(nameof(CMS.DocumentEngine.Types.MedioClinic.Doctor.DoctorFields.UrlSlug), urlSlug)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsController)}|Doctor|{urlSlug}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME))))
                            .FirstOrDefault();

                var viewModel = GetPageViewModel(doctor, doctor?.Name!);

                return View("Doctors/Detail", viewModel);
            }

            return NotFound();
        }
    }
}
