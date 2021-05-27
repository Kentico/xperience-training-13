using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using Core.Configuration;
using XperienceAdapter.Repositories;
using Business.Models;
using MedioClinic.Models;
using Microsoft.Extensions.Localization;
using XperienceAdapter.Localization;

namespace MedioClinic.Controllers
{
    public class ContactController : BaseController
    {
        private const string ContactPath = "/Contact-us";

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageMetadataRetriever _metadataRetriever;

        private readonly IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> _mapLocationRepository;

        private readonly IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> _namePerexTextRepository;

        private readonly IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> _companyRepository;

        private readonly IMediaFileRepository _mediaFileRepository;

        public ContactController(
                ILogger<ContactController> logger,
                IOptionsMonitor<XperienceOptions> optionsMonitor,
                IStringLocalizer<SharedResource> stringLocalizer,
                IPageRetriever pageRetriever,
                IPageMetadataRetriever metadataRetriever,
                IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> mapLocationRepository,
                IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> namePerexTextRepository,
                IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository,
                IMediaFileRepository mediaFileRepository)
                : base(logger, optionsMonitor, stringLocalizer)
        {
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _metadataRetriever = metadataRetriever ?? throw new ArgumentNullException(nameof(metadataRetriever));
            _mapLocationRepository = mapLocationRepository ?? throw new ArgumentNullException(nameof(mapLocationRepository));
            _namePerexTextRepository = namePerexTextRepository ?? throw new ArgumentNullException(nameof(namePerexTextRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var page = (await _pageRetriever.RetrieveAsync<CMS.DocumentEngine.Types.MedioClinic.NamePerexText>(
                filter => filter
                    .Path(ContactPath, PathTypeEnum.Single)
                    .TopN(1),
                cache => cache
                    .Key($"{nameof(ContactController)}|ContactPage")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME))))
                    .FirstOrDefault();

            var metadata = _metadataRetriever.Retrieve(page);
            var (company, officeLocations, contactPage) = await GetPageData(ContactPath, cancellationToken);
            var medicalServicePictures = await _mediaFileRepository.GetMediaFilesAsync("CommonImages", "MedicalServices");

            if (company != null && officeLocations?.Any() == true && contactPage != null && medicalServicePictures?.Any() == true)
            {
                var data = new ContactViewModel
                {
                    Company = company,
                    ContactPage = contactPage,
                    MedicalServices = medicalServicePictures,
                    OfficeLocations = officeLocations
                };

                var viewModel = GetPageViewModel(metadata, data);

                return View("Contact/Index", viewModel);
            }

            return NotFound();
        }

        private async Task<(Company, IEnumerable<MapLocation>, NamePerexText)> GetPageData(string contactPath, CancellationToken cancellationToken)
        {
            var contactPage = (await _namePerexTextRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path(contactPath, PathTypeEnum.Single)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactCtbController)}|ContactPage")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME)
                        .PagePath(contactPath, PathTypeEnum.Single))))
                    .FirstOrDefault();

            var company = (await _companyRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path(contactPath, PathTypeEnum.Children)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactCtbController)}|Company")
                    .Dependencies((pages, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.Company.CLASS_NAME)
                        .Pages(pages))))
                    .FirstOrDefault();

            var officeLocations = (await _mapLocationRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path(contactPath, PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactCtbController)}|OfficeLocations")
                    .Dependencies((pages, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.MapLocation.CLASS_NAME)
                        .Pages(pages))));

            return (company, officeLocations, contactPage);
        }
    }
}
