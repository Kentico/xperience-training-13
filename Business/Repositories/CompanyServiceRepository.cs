using Kentico.Content.Web.Mvc;

using XperienceAdapter.Services;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores company services.
    /// </summary>
    public class CompanyServiceRepository : BasePageRepository<Models.CompanyService, CMS.DocumentEngine.Types.MedioClinic.CompanyService>
    {
        public override Models.CompanyService MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.CompanyService page, Models.CompanyService dto)
        {
            dto.ServiceDescription = page?.ServiceDescription;
            dto.IconServerPath = page?.Fields?.Icon?.GetPath();

            return dto;
        }

        public CompanyServiceRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
