using XperienceAdapter;
using XperienceAdapter.Repositories;

namespace Business.Repositories
{
    /// <summary>
    /// Stores company information.
    /// </summary>
    public class CompanyRepository : BasePageRepository<Dtos.Company, CMS.DocumentEngine.Types.MedioClinic.Company>
    {
        public override Dtos.Company MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.Company page, Dtos.Company dto)
        {
            dto.City = page.City;
            dto.Country = page.Country;
            dto.EmailAddress = page.Email;
            dto.PhoneNumber = page.PhoneNumber;
            dto.PostalCode = page.ZipCode;
            dto.Street = page.Street;

            return dto;
        }

        public CompanyRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
