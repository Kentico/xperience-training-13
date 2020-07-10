using CMS.DocumentEngine;

using XperienceAdapter;
using XperienceAdapter.Dtos;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores pages without page type-specific data (coupled data).
    /// </summary>
    public class BasePageRepository : BasePageRepository<BasePage, TreeNode>
    {
        public BasePageRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
