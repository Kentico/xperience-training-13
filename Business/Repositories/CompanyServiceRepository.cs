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
        public override void MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.CompanyService page, Models.CompanyService dto)
        {
            dto.ServiceDescription = page?.ServiceDescription;

            if (page?.Fields?.Icon != null)
            {
                dto.IconUrl = _repositoryServices.PageAttachmentUrlRetriever.Retrieve(page?.Fields?.Icon); 
            }
        }

        public CompanyServiceRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
