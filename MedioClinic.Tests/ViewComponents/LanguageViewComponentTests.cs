using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter.Models;
using XperienceAdapter.Repositories;
using MedioClinic.ViewComponents;
using Business.Repositories;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Business.Models;
using Kentico.Content.Web.Mvc;
using CMS.DocumentEngine;
using Microsoft.AspNetCore.Mvc;

namespace MedioClinic.Tests.ViewComponents
{
    public class LanguageViewComponentTests
    {
        private const string CultureSwitchId = "switchId";

        private const string HomeUrl = "/en-us/home/";

        [Fact]
        public void Invoke_ReturnsResult()
        {
            var cultureRepositoryMock = GetCultureRepository();
            var navigationRepositoryMock = GetNavigationRepository();
            var component = new CultureSwitch(cultureRepositoryMock.Object, navigationRepositoryMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/";
            component.ViewComponentContext.ViewContext.HttpContext = httpContext;
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(helper => helper.Content(It.Is<string>(input => input == HomeUrl))).Returns(HomeUrl);
            urlHelperMock.Setup(helper => helper.Content(It.Is<string>(input => input == "/"))).Returns("/");
            component.Url = urlHelperMock.Object;

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
                    Name = "English",
                    IsDefault = true,
                    IsoCode = "en-US",
                    ShortName = "EN"
                },
                new SiteCulture
                {
                    Name = "Czech",
                    IsoCode = "cz-CS",
                    ShortName = "CZ"
                }
            };

            var repository = new Mock<ISiteCultureRepository>();
            repository.Setup(repository => repository.GetAll()).Returns(cultures);
            repository.Setup(repository => repository.DefaultSiteCulture).Returns(cultures.FirstOrDefault(culture => culture.IsDefault));

            return repository;
        }

        private Mock<INavigationRepository> GetNavigationRepository()
        {
            var navigationVariant = GetNavigation();

            var navigation = new Dictionary<SiteCulture, NavigationItem>
            {
                {
                    new SiteCulture
                    {
                        IsoCode = "en-US"
                    },
                    navigationVariant
                },
                {
                    new SiteCulture
                    {
                        IsoCode = "cs-CZ",
                    },
                    navigationVariant
                }
            };

            var repository = new Mock<INavigationRepository>();
            repository.Setup(repository => repository.GetConventionalRoutingNavigation()).Returns(navigation);

            return repository;
        }

        private static NavigationItem GetNavigation()
        {
            var child = new NavigationItem
            {
                NodeId = 1,
                ParentId = 0,
                UrlSlug = "home",
                RelativeUrl = HomeUrl
            };

            var root = new NavigationItem
            {
                NodeId = 0,
                RelativeUrl = "/"
            };

            root.ChildItems.Add(child);

            return root;
        }

    }
}
