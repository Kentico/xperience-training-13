using System.Linq;

using XperienceAdapter.Services;
using XperienceAdapter.Repositories;
using CMS.DocumentEngine;
using System;
using System.Threading;
using XperienceAdapter.Extensions;

namespace Business.Repositories
{
    /// <summary>
    /// Stores the home page.
    /// </summary>
    public class HomePageRepository : BasePageRepository<Models.HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage>
    {
        private readonly INavigationRepository _navigationRepository;

        public override Models.HomePage MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.HomePage page, Models.HomePage dto)
        {
            dto.Perex = page.Perex;
            dto.Text = page.Text;
            dto.DoctorsLinkButtonText = page.DoctorsLinkButtonText;
            dto.ServicesLinkButtonText = page.ServicesLinkButtonText;

            // TODO: File a potential bug.
            var doctorsNodeId = (int?)9; //page.Fields.DoctorsLink?.FirstOrDefault()?.NodeID;
            var currentCulture = Thread.CurrentThread.CurrentUICulture.ToSiteCulture();

            if (doctorsNodeId.HasValue && currentCulture != null)
            {
                dto.DoctorsUrl = _navigationRepository.GetConventionalRoutingUrl(doctorsNodeId.Value, currentCulture);
            }

            return dto;
        }

        public HomePageRepository(IRepositoryServices repositoryDependencies, INavigationRepository navigationRepository) : base(repositoryDependencies)
        {
            _navigationRepository = navigationRepository ?? throw new ArgumentNullException(nameof(navigationRepository));
        }
    }
}
