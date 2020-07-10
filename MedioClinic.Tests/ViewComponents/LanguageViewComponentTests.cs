using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter.Dtos;
using XperienceAdapter.Repositories;
using MedioClinic.ViewComponents;
using Business.Repositories;

namespace MedioClinic.Tests.ViewComponents
{
    public class LanguageViewComponentTests
    {
        private const string CultureSwitchId = "switchId";

        [Fact]
        public void Invoke_ReturnsResult()
        {
            var cultureRepositoryMock = GetCultureRepository();
            var navigationRepositoryMock = new Mock<INavigationRepository>();
            var component = new Language(cultureRepositoryMock.Object, navigationRepositoryMock.Object);

            // TODO: Mock HttpRequest.
            var result = component.Invoke(CultureSwitchId);

            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
        }

        private Mock<ISiteCultureRepository> GetCultureRepository()
        {
            var cultures = new List<SiteCulture>
            {
                new SiteCulture
                {
                    FriendlyName = "English",
                    IsDefault = true,
                    IsoCode = "en-US",
                    ShortName = "EN"
                },
                new SiteCulture
                {
                    FriendlyName = "Czech",
                    IsoCode = "cz-CS",
                    ShortName = "CZ"
                }
            };

            var repository = new Mock<ISiteCultureRepository>();
            repository.Setup(repository => repository.GetAll()).Returns(cultures);

            return repository;
        }
    }
}
