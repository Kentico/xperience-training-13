using CMS.DocumentEngine;

using XperienceAdapter.Services;
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
            //TODO: HOw can this work? We don't have such a field... Naming convention of custom field? :)
            dto.UrlSlug = page.GetStringValue("UrlSlug", default);

            return dto;
        }
    }
}
