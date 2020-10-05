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
    public class SocialLinksViewComponentTests
    {
        [Fact]
        public void Invoke_ReturnsResult()
        {
            var repositoryMock = GetSocialLinkRepository();
            var component = new SocialLinks(repositoryMock.Object);

            var result = component.Invoke();

            Assert.NotNull(result);
            Assert.IsType<ViewViewComponentResult>(result);
        }

        private Mock<IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink>> GetSocialLinkRepository()
        {
            var repository = new Mock<IPageRepository<SocialLink, CMS.DocumentEngine.Types.MedioClinic.SocialLink>>();
            repository.Setup(repository => repository.GetAll()).Returns(SocialLinks);

            return repository;
        }

        private IEnumerable<SocialLink> SocialLinks
        {
            get =>
                new List<SocialLink>
                {
                    new SocialLink
                    {
                        Name = "Facebook",
                        Url = "http://example.com"
                    },
                    new SocialLink
                    {
                        Name = "Twitter",
                        Url = "http://example.com"
                    }
                };
        }
    }
}
