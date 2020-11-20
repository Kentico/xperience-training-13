using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Configuration;
using Business.Models;
using CMS.Base;
using CMS.DocumentEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XperienceAdapter.Repositories;

namespace MedioClinic.Controllers
{
    public class ContactController : BaseController
    {

        private readonly IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> _mapLocationRepository;

        private readonly IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> _namePerexTextRepository;

        private readonly IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> _companyRepository;



    public ContactController(
            ILogger<ContactController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageRepository<MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation> mapLocationRepository,
            IPageRepository<NamePerexText, CMS.DocumentEngine.Types.MedioClinic.NamePerexText> namePerexTextRepository,
            IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository)
            : base(logger, siteService, optionsMonitor)
        {
            _mapLocationRepository = mapLocationRepository ?? throw new ArgumentNullException(nameof(mapLocationRepository));
            _namePerexTextRepository = namePerexTextRepository ?? throw new ArgumentNullException(nameof(namePerexTextRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            var company = (await _companyRepository.GetPagesAsync(
                cancellationToken,
                filter => filter
                    .Path("/Contact-us/Medio-Clinic", PathTypeEnum.Single)
                    .TopN(1),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactController)}|Company")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.Company.CLASS_NAME)),
                includeAttachments: true))
                    .FirstOrDefault();

            var mapLocations = (await _mapLocationRepository.GetPagesAsync(
                cancellationToken,
                filter => filter
                    .Path("/Contact-us/Office-locations", PathTypeEnum.Children),
                buildCacheAction: cache => cache
                    .Key($"{nameof(ContactController)}|MapLocation")
                    .Dependencies((_, builder) => builder
                        .PageType(CMS.DocumentEngine.Types.MedioClinic.MapLocation.CLASS_NAME)),
                includeAttachments: true));
                    

            var namePerexText = (await _namePerexTextRepository.GetPagesAsync(
               cancellationToken,
               filter => filter
                   .Path("/Contact-us", PathTypeEnum.Single)
                   .TopN(1),
               buildCacheAction: cache => cache
                   .Key($"{nameof(ContactController)}|NamePerexText")
                   .Dependencies((_, builder) => builder
                       .PageType(CMS.DocumentEngine.Types.MedioClinic.NamePerexText.CLASS_NAME)),
               includeAttachments: true))
                   .FirstOrDefault();
            

            var data = (company, mapLocations, namePerexText);
            var viewModel = GetPageViewModel(data, namePerexText.Name!); 

            return View("Contact/Index" ,viewModel);
        }
    }
}
 