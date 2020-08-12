using CMS.DocumentEngine;

using XperienceAdapter;
using XperienceAdapter.Repositories;
using Business.Models;

namespace Business.Repositories
{
    public class UrlSlugPageRepository : BasePageRepository<BasicPageWithUrlSlug, TreeNode>
    {
        public UrlSlugPageRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }

        public override BasicPageWithUrlSlug MapDtoProperties(TreeNode page, BasicPageWithUrlSlug dto)
        {
            dto.UrlSlug = page.GetStringValue("UrlSlug", default);

            return dto;
        }
    }
}
