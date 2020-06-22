using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.DocumentEngine.Types.MedioClinic;
using Kentico.Content.Web.Mvc;

using Business.Dtos;
using XperienceAdapter;

namespace Business.Repositories
{
    public class CompanyServiceRepository :
        BasePageRepository<CompanyServiceDto, CompanyService>,
        IPageRepository<CompanyServiceDto, CompanyService>
    {
        public override Func<CompanyService, CompanyServiceDto, CompanyServiceDto> Mapper { get; } = (document, dto) =>
        {
            dto.ServiceDescription = document.ServiceDescription;
            dto.IconPath = document.Fields.Icon.GetPath();

            return dto;
        };

        public CompanyServiceRepository(IRepositoryDependencies repositoryDependencies) : base(repositoryDependencies)
        {
        }
    }
}
