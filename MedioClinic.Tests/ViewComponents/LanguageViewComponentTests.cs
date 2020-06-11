using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter;
using MedioClinic.ViewComponents;

namespace MedioClinic.Tests.ViewComponents
{
    public class LanguageViewComponentTests
    {
        private const string CultureSwitchId = "switchId";

        [Fact]
        public void Invoke_ReturnsResult()
        {
            var repositoryMock = GetCultureRepository();
            var component = new Language(repositoryMock.Object);

            var result = component.Invoke(CultureSwitchId);

            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
        }

        private Mock<ICultureRepository> GetCultureRepository()
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

            var cultureRepository = new Mock<ICultureRepository>();
            cultureRepository.Setup(repository => repository.GetAll()).Returns(cultures);

            return cultureRepository;
        }
    }
}
