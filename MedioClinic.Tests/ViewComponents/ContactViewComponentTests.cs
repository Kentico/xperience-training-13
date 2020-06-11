using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter;
using XperienceAdapter.Dtos;
using MedioClinic.ViewComponents;
using Abstractions;

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

        private Mock<IRepository<Company>> GetCompanyRepository()
        {
            var repository = new Mock<IRepository<Company>>();
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
                Region = "Region",
                Street = "Street"
            };
        }
    }
}
