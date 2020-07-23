using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter.Repositories;
using Business.Models;
using MedioClinic.ViewComponents;

namespace MedioClinic.Tests.ViewComponents
{
    public class ContactViewComponentTests
    {
        [Fact]
        public void Invoke_ReturnsResult()
        {
            var repositoryMock = GetCompanyRepository();
            var component = new Contact(repositoryMock.Object);

            var result = component.Invoke();

            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
        }

        private Mock<IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company>> GetCompanyRepository()
        {
            var repository = new Mock<IPageRepository<Company, CMS.DocumentEngine.Types.MedioClinic.Company>>();
            repository.Setup(repository => repository.GetAll()).Returns(new List<Company> { Company });

            return repository;
        }

        private Company Company
        {
            get => new Company
            {
                City = "City",
                Country = "Country",
                EmailAddress = "email@address.com",
                Name = "Company name",
                PhoneNumber = "1234567890",
                PostalCode = "12345",
                Street = "Street"
            };
        }
    }
}
