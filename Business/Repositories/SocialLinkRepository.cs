using Kentico.Content.Web.Mvc;

using XperienceAdapter;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores social media links.
    /// </summary>
    public class SocialLinkRepository : BasePageRepository<Dtos.SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink>
    {
        public override Dtos.SocialLink MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.SocialLink page, Dtos.SocialLink dto)
        {
            dto.Url = page?.Url;
            dto.IconServerPath = page?.Fields?.Icon?.GetPath();

            return dto;
        }

        public SocialLinkRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
