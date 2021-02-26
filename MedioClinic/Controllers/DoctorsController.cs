using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;

using XperienceAdapter.Repositories;
using Core.Configuration;
using Business.Models;
using XperienceAdapter.Models;
using Kentico.Content.Web.Mvc;

namespace MedioClinic.Controllers
{
    public class DoctorsController : BaseController
    {
        private const string DoctorsPath = "/Doctors";

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageMetadataRetriever _metadataRetriever;

        private readonly IPageRepository<BasePage, TreeNode> _basePageRepository;

        private readonly IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> _doctorRepository;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageRetriever pageRetriever,
            IPageMetadataRetriever metadataRetriever,
            IPageRepository<BasePage, TreeNode> basePageRepository,
            IPageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor> doctorRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _metadataRetriever = metadataRetriever ?? throw new ArgumentNullException(nameof(metadataRetriever));
            _basePageRepository = basePageRepository ?? throw new ArgumentNullException(nameof(basePageRepository));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var page = (await _pageRetriever.RetrieveAsync<CMS.DocumentEngine.Types.MedioClinic.Doctor>(
                filter => filter
                    .Path(DoctorsPath, PathTypeEnum.Single)
                    .TopN(1),
                cache => cache
                    .Key($"{nameof(DoctorsController)}|DoctorSection")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME))))
                    .FirstOrDefault();

            var metadata = _metadataRetriever.Retrieve(page);

            var doctorsSection = (await _basePageRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path(DoctorsPath, PathTypeEnum.Single)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(DoctorsController)}|DoctorsSection")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.SiteSection.CLASS_NAME))))
                    .FirstOrDefault();

            var title = doctorsSection?.Name ?? string.Empty;

            var doctorPages = await _doctorRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path(DoctorsPath, PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(DoctorsController)}|Doctors")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME)
                        .PageOrder()));

            if (doctorsSection != null && doctorPages?.Any() == true)
            {
                var data = (doctorsSection, doctorPages);
                var viewModel = GetPageViewModel(metadata, data);

                return View("Doctors/Index", viewModel);
            }

            return NotFound();
        }

        public async Task<IActionResult> Detail(string? urlSlug, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(urlSlug))
            {
                var page = (await _pageRetriever.RetrieveAsync<CMS.DocumentEngine.Types.MedioClinic.Doctor>(
                    filter => filter
                        .Path(DoctorsPath, PathTypeEnum.Children)
                        .WhereEquals(nameof(CMS.DocumentEngine.Types.MedioClinic.Doctor.DoctorFields.UrlSlug), urlSlug)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(DoctorsController)}|Doctor|{urlSlug}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Doctor.CLASS_NAME))))
                        .FirstOrDefault();

                var metadata = _metadataRetriever.Retrieve(page);

                var doctor = (await _doctorRepository.GetPagesInCurrentCultureAsync(
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

                if (doctor != null)
                {
                    var viewModel = GetPageViewModel(metadata, doctor);

                    return View("Doctors/Detail", viewModel);
                }
            }

            return NotFound();
        }
    }
}
