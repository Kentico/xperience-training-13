using System;
using System.Linq;
using System.Threading;

using XperienceAdapter.Extensions;
using XperienceAdapter.Repositories;
using XperienceAdapter.Services;
using Business.Models;

namespace Business.Repositories
{
    /// <summary>
    /// Stores the home page.
    /// </summary>
    public class HomePageRepository : BasePageRepository<HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage>
    {
        private readonly INavigationRepository _navigationRepository;

        public override HomePage MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.HomePage page, HomePage dto)
        {
            dto.Perex = page.Perex;
            dto.Text = page.Text;
            dto.DoctorsLinkButtonText = page.DoctorsLinkButtonText;
            dto.ServicesLinkButtonText = page.ServicesLinkButtonText;
            var doctorsNodeId = page.Fields.DoctorsLink?.FirstOrDefault()?.NodeID;
            var currentCulture = Thread.CurrentThread.CurrentUICulture.ToSiteCulture();

            if (doctorsNodeId.HasValue && currentCulture != null)
            {
                dto.DoctorsUrl = _navigationRepository.GetConventionalRoutingUrl(doctorsNodeId.Value, currentCulture);
            }

            return dto;
        }

        public HomePageRepository(IRepositoryServices repositoryServices, INavigationRepository navigationRepository) : base(repositoryServices)
        {
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }
    }
}
