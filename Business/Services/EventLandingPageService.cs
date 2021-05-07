using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using CMS.DocumentEngine;

using XperienceAdapter.Repositories;
using Business.Models;

namespace Business.Services
{
    public class EventLandingPageService : IEventLandingPageService
    {
        private readonly IPageRepository<EventLandingPage, CMS.DocumentEngine.Types.MedioClinic.EventLandingPage> _landingPageRepository;

        public EventLandingPageService(IPageRepository<EventLandingPage, CMS.DocumentEngine.Types.MedioClinic.EventLandingPage> landingPageRepository)
        {
            _landingPageRepository = landingPageRepository ?? throw new ArgumentNullException(nameof(landingPageRepository));
        }

        public async Task<DateTime?> GetEventDateAsync(TreeNode page, CancellationToken cancellationToken)
        {
            if (page is CMS.DocumentEngine.Types.MedioClinic.EventLandingPage != true)
            {
                throw new ArgumentException($"The page is not of type {nameof(EventLandingPage)}.", nameof(page));
            }

            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var landingPagePath = page.NodeAliasPath;

            if (!string.IsNullOrEmpty(landingPagePath))
            {
                var landingPage = (await _landingPageRepository.GetPagesInCurrentCultureAsync(
                    cancellationToken,
                    filter => filter
                        .Path(landingPagePath, PathTypeEnum.Single)
                        .TopN(1),
                    buildCacheAction: cache => cache
                        .Key($"{nameof(EventLandingPageService)}|Page|{landingPagePath}")
                        .Dependencies((_, builder) => builder
                            .PageType(CMS.DocumentEngine.Types.MedioClinic.EventLandingPage.CLASS_NAME)
                            .PagePath(landingPagePath, PathTypeEnum.Single))))
                        .FirstOrDefault();

                return landingPage?.EventDate;
            }

            return null;
        }
    }
}
