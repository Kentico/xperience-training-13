#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Repositories;
using Business.Models;

namespace MedioClinic.ViewComponents
{
    public class Contact : ViewComponent
    {
        private readonly IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> _companyRepository;

        public Contact(IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public IViewComponentResult Invoke()
        {
            var company = _companyRepository.GetPages(
                query => query
                    .WhereEquals("Email", "medioclinic.local")
                    .TopN(1),
                buildCacheAction:
                    cache => cache
                        .Key($"{nameof(Contact)}|{nameof(Invoke)}"))
                .FirstOrDefault();
                
            return View(company);
        }
    }
}
