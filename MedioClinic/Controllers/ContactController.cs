﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Configuration;
using Business.Models;
using CMS.Base;
using CMS.DocumentEngine;
using MedioClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XperienceAdapter.Repositories;
using Kentico.Content.Web.Mvc;

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
                ISiteService siteService,
                IOptionsMonitor<XperienceOptions> optionsMonitor,
                IPageRetriever pageRetriever,
                IPageMetadataRetriever metadataRetriever,
                IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> mapLocationRepository,
                IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> namePerexTextRepository,
                IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository,
                IMediaFileRepository mediaFileRepository)
                : base(logger, siteService, optionsMonitor)
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

            var company = (await _companyRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path($"{ContactPath}/Medio-Clinic", PathTypeEnum.Single)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactController)}|Company")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.Company.CLASS_NAME))))
                    .FirstOrDefault();

            var officeLocations = (await _mapLocationRepository.GetPagesInCurrentCultureAsync(
                cancellationToken,
                filter => filter
                    .Path($"{ContactPath}/Office-locations", PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactController)}|OfficeLocations")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.MapLocation.CLASS_NAME))));

            var contactPage = (await _namePerexTextRepository.GetPagesInCurrentCultureAsync(
               cancellationToken,
               filter => filter
                   .Path(ContactPath, PathTypeEnum.Single)
                   .TopN(1),
               buildCacheAction: cache => cache
                   .Key($"{nameof(ContactController)}|ContactPage")
                   .Dependencies((_, builder) => builder
                       .PageType(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME)),
               includeAttachments: true))
                   .FirstOrDefault();

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
    }
}
