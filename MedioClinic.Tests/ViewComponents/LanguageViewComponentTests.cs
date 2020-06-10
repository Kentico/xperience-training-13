using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using Abstractions;
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
            var cultureRepository = GetCultureRepository();
            var component = new LanguageViewComponent(cultureRepository.Object);

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
            cultureRepository.Setup(service => service.GetAll()).Returns(cultures);

            return cultureRepository;
        }
    }
}
