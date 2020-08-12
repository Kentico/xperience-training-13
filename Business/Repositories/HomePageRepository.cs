using System.Linq;

using XperienceAdapter;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores the home page.
    /// </summary>
    public class HomePageRepository : BasePageRepository<Models.HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage>
    {
        public override Models.HomePage MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.HomePage page, Models.HomePage dto)
        {
            dto.LinkButtonText = page.LinkButtonText;

            // TODO: File a potential bug.
            //dto.DoctorsUrl = page.Fields.DoctorsLink?.FirstOrDefault()?.RelativeUrl();

            return dto;
        }

        public HomePageRepository(IRepositoryServices repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
