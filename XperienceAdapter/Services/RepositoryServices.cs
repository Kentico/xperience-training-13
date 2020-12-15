using System;
using System.Collections.Specialized;
using CMS.Base;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using XperienceAdapter.Repositories;

namespace XperienceAdapter.Services
{
    public class RepositoryServices : IRepositoryServices
    {
        public ISiteService SiteService { get; }

        public ISiteContextService SiteContextService { get; }

        public IPageRetriever PageRetriever { get; }

        public IPageUrlRetriever PageUrlRetriever { get; }

        public IPageAttachmentUrlRetriever PageAttachmentUrlRetriever { get; }

        public ISiteCultureRepository SiteCultureRepository { get; }

        public IProgressiveCache ProgressiveCache { get; }

        public RepositoryServices(
            ISiteService siteService, 
            ISiteContextService siteContextService,
            IPageRetriever pageRetriever,
            IPageUrlRetriever pageUrlRetriever,
            IPageAttachmentUrlRetriever pageAttachmentUrlRetriever, 
            ISiteCultureRepository siteCultureRepository,
            IProgressiveCache progressiveCache)
        {
            SiteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            SiteContextService = siteContextService ?? throw new ArgumentNullException(nameof(siteContextService));
            PageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            PageUrlRetriever = pageUrlRetriever ?? throw new ArgumentNullException(nameof(pageUrlRetriever));
            PageAttachmentUrlRetriever = pageAttachmentUrlRetriever ?? throw new ArgumentNullException(nameof(pageAttachmentUrlRetriever));
            SiteCultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
            ProgressiveCache = progressiveCache ?? throw new ArgumentNullException(nameof(progressiveCache));
        }
    }
}
