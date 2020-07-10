#define no_suffix

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Abstractions;
using Business.Dtos;

namespace MedioClinic.ViewComponents
{
    public class Contact : ViewComponent
    {
        public IRepository<Company> CompanyRepository { get; }

        public Contact(IRepository<Company> companyRepository)
        {
            CompanyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public IViewComponentResult Invoke()
        {
            var company = CompanyRepository.GetAll().FirstOrDefault();

            return View(company);
        }
    }
}
