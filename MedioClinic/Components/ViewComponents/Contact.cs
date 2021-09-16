#define no_suffix

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using XperienceAdapter.Repositories;
using Business.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MedioClinic.Components.ViewComponents
{
    public class Contact : ViewComponent
    {
        private const string PagePath = "/Contact-us/Medio-Clinic";

        private readonly IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> _companyRepository;

        public Contact(IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company> companyRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = (await _companyRepository.GetPagesInCurrentCultureAsync(
                CancellationToken.None,
                filter => filter
                    .Path(PagePath)
                    .TopN(1),
                buildCacheAction:
                    cache => cache
                        .Key($"{nameof(Contact)}|{nameof(InvokeAsync)}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.Company.CLASS_NAME)
                            .PagePath(PagePath, CMS.DocumentEngine.PathTypeEnum.Single))))
                .FirstOrDefault();
                
            return View(model);
        }
    }
}
