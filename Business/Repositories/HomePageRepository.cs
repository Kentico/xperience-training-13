using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.DocumentEngine.Types.MedioClinic;

using Business.Dtos;
using XperienceAdapter;

namespace Business.Repositories
{
    public class HomePageRepository :
        BasePageRepository<HomePageDto, HomePage>,
        IPageRepository<HomePageDto, HomePage>
    {
        public override Func<HomePage, HomePageDto, HomePageDto> Mapper { get; } = (document, dto) =>
        {
            dto.LinkButtonText = document.LinkButtonText;

            return dto;
        };

        public HomePageRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
