using Kentico.Content.Web.Mvc;

using XperienceAdapter.Services;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores social media links.
    /// </summary>
    public class SocialLinkRepository : BasePageRepository<Models.SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink>
    {
        public override void MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.SocialLink page, Models.SocialLink dto)
        {
            dto.Url = page?.Url;

            if (page?.Fields?.Icon != null)
            {
                dto.IconUrl = _repositoryServices.PageAttachmentUrlRetriever.Retrieve(page?.Fields?.Icon);
            }
        }

        public SocialLinkRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
