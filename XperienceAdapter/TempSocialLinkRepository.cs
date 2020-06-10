using Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceAdapter
{
    public class TempSocialLinkRepository : IRepository<SocialLink>
    {
        public IEnumerable<SocialLink> GetAll() =>
            SocialLinks;

        public IEnumerable<SocialLink> SocialLinks
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
