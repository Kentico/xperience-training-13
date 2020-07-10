using System.Linq;

using XperienceAdapter;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores the home page.
    /// </summary>
    public class HomePageRepository : BasePageRepository<Dtos.HomePage, CMS.DocumentEngine.Types.MedioClinic.HomePage>
    {
        public override Dtos.HomePage MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.HomePage page, Dtos.HomePage dto)
        {
            dto.LinkButtonText = page.LinkButtonText;

            // TODO: File a potential bug.
            //dto.DoctorsUrl = page.Fields.DoctorsLink?.FirstOrDefault()?.RelativeUrl();

            return dto;
        }

        public HomePageRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
