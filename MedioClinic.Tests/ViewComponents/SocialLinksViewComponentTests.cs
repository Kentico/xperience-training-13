using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;

using XperienceAdapter;
using MedioClinic.ViewComponents;
using Abstractions;

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

        private Mock<IRepository<SocialLink>> GetSocialLinkRepository()
        {
            var repository = new Mock<IRepository<SocialLink>>();
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
                        IconUrl = "/images/social/fb.png",
                        Name = "Facebook",
                        Url = "http://example.com"
                    },
                    new SocialLink
                    {
                        IconUrl = "/images/social/tw.png",
                        Name = "Twitter",
                        Url = "http://example.com"
                    }
                };
        }
    }
}
