using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Configuration;
using Business.Models;
using CMS.Base;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using MedioClinic.Controllers;
using MedioClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XperienceAdapter.Models;
using XperienceAdapter.Repositories;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME, typeof(ContactCtbController))]
namespace MedioClinic.Controllers
{
    public class ContactCtbController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> _mapLocationRepository;

        private readonly IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> _namePerexTextRepository;

        private readonly IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> _companyRepository;

        private readonly IMediaFileRepository _mediaFileRepository;

        public ContactCtbController(
                ILogger<ContactController> logger,
                ISiteService siteService,
                IOptionsMonitor<XperienceOptions> optionsMonitor,
                IPageDataContextRetriever pageDataContextRetriever,
                IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> mapLocationRepository,
                IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> namePerexTextRepository,
                IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository,
                IMediaFileRepository mediaFileRepository)
                : base(logger, siteService, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _mapLocationRepository = mapLocationRepository ?? throw new ArgumentNullException(nameof(mapLocationRepository));
            _namePerexTextRepository = namePerexTextRepository ?? throw new ArgumentNullException(nameof(namePerexTextRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (_pageDataContextRetriever.TryRetrieve<CMS.DocumentEngine.Types.MedioClinic.NamePerexText>(out var pageDataContext)
                && pageDataContext.Page != null)
            {
                var contactPath = pageDataContext.Page.NodeAliasPath;

                var company = (await _companyRepository.GetPagesAsync(
                    cancellationToken,
                    filter => filter
                        .Path(contactPath, PathTypeEnum.Children)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(ContactController)}|Company")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Company.CLASS_NAME)),
                    includeAttachments: true))
                        .FirstOrDefault();

                var officeLocations = (await _mapLocationRepository.GetPagesAsync(
                    cancellationToken,
                    filter => filter
                        .Path(contactPath, PathTypeEnum.Children),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(ContactController)}|OfficeLocations")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.MapLocation.CLASS_NAME)),
                    includeAttachments: true));

                var contactPage = (await _namePerexTextRepository.GetPagesAsync(
                   cancellationToken,
                   filter => filter
                       .Path(contactPath, PathTypeEnum.Single)
                       .TopN(1),
                   buildCacheAction: cache => cache
                       .Key($"{nameof(ContactController)}|ContactPage")
                       .Dependencies((_, builder) => builder
                           .PageType(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME)),
                   includeAttachments: true))
                       .FirstOrDefault();

                _mediaFileRepository.MediaLibraryName = "CommonImages";
                var medicalServicePictures = await _mediaFileRepository.GetMediaFilesAsync("MedicalServices");

                if (company != null && officeLocations?.Any() == true && contactPage != null && medicalServicePictures?.Any() == true)
                {
                    var data = new ContactViewModel
                    {
                        Company = company,
                        ContactPage = contactPage,
                        MedicalServices = medicalServicePictures,
                        OfficeLocations = officeLocations
                    };

                    var viewModel = GetPageViewModel(data, title: contactPage.Name!);

                    return View("Contact/Index", viewModel); 
                }
            }

            return NotFound();
        }
    }
}