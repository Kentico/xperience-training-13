using Kentico.Content.Web.Mvc;

using XperienceAdapter;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores social media links.
    /// </summary>
    public class SocialLinkRepository : BasePageRepository<Models.SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink>
    {
        public override Models.SocialLink MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.SocialLink page, Models.SocialLink dto)
        {
            dto.Url = page?.Url;
            dto.IconServerPath = page?.Fields?.Icon?.GetPath();

            return dto;
        }

        public SocialLinkRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
