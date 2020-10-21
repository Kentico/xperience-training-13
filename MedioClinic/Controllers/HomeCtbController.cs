using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CMS.Base;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using Business.Configuration;
using MedioClinic.Controllers;
using XperienceAdapter.Models;

[assembly: RegisterPageRoute(CMS.DocumentEngine.Types.MedioClinic.HomePage.CLASS_NAME, typeof(HomeCtbController))]
namespace MedioClinic.Controllers
{
    public class HomeCtbController : BaseController
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        private readonly IPageRetriever _pageRetriever;

        private readonly IPageAttachmentUrlRetriever _pageAttachmentUrlRetriever;

        private readonly IPageUrlRetriever _pageUrlRetriever;

        public HomeCtbController(
            ILogger<HomeCtbController> logger,
            ISiteService siteService,
            IOptionsMonitor<XperienceOptions> optionsMonitor,
            IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever,
            IPageAttachmentUrlRetriever pageAttachmentUrlRetriever,
            IPageUrlRetriever pageUrlRetriever)
            : base(logger, siteService, optionsMonitor)
        {
            _pageDataContextRetriever = pageDataContextRetriever ?? throw new ArgumentNullException(nameof(pageDataContextRetriever));
            _pageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            _pageAttachmentUrlRetriever = pageAttachmentUrlRetriever ?? throw new ArgumentNullException(nameof(pageAttachmentUrlRetriever));
            _pageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var homepageContext = _pageDataContextRetriever.Retrieve<CMS.DocumentEngine.Types.MedioClinic.HomePage>();
            var homepage = homepageContext.Page;
            var doctorsLink = homepage.Fields.DoctorsLink.FirstOrDefault();

            var homepageDto = new Business.Models.HomePage
            {
                Name = homepage.DocumentName,
                Perex = homepage.Perex,
                Text = homepage.Text,
                DoctorsUrl = doctorsLink == null ? null : _pageUrlRetriever.Retrieve(doctorsLink).RelativePath,
                DoctorsLinkButtonText = homepage.DoctorsLinkButtonText,
                ServicesLinkButtonText = homepage.ServicesLinkButtonText
            };

            foreach (var attachment in homepage.Attachments)
            {
                homepageDto.Attachments.Add(new PageAttachment
                {
                    AttachmentUrl = _pageAttachmentUrlRetriever.Retrieve(attachment),
                    FileName = attachment.AttachmentName
                });
            }

            var companyServiceDtos = (await _pageRetriever.RetrieveAsync<CMS.DocumentEngine.Types.MedioClinic.CompanyService>(
                query => query
                    .Path(homepageContext.Page.NodeAliasPath, CMS.DocumentEngine.PathTypeEnum.Children)
                    .OrderBy("NodeOrder"),
                cancellationToken: cancellationToken))
                .Select(page => new Business.Models.CompanyService
                {
                    IconUrl = _pageAttachmentUrlRetriever.Retrieve(page.Fields.Icon),
                    Name = page.DocumentName,
                    ServiceDescription = page.ServiceDescription
                });

            var data = (homepageDto, companyServiceDtos);
            var viewModel = GetPageViewModel<(Business.Models.HomePage, IEnumerable<Business.Models.CompanyService>)>(data, homepageDto.Name);

            return View("Home/Index", viewModel);
        }
    }
}
