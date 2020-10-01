using System;

using CMS.Base;
using Kentico.Content.Web.Mvc;
using XperienceAdapter.Repositories;

namespace XperienceAdapter.Services
{
    public class RepositoryServices : IRepositoryServices
    {
        public ISiteService SiteService { get; }

        public ISiteContextService SiteContextService { get; }

        public IPageRetriever PageRetriever { get; }

        public ISiteCultureRepository SiteCultureRepository { get; }

        public RepositoryServices(ISiteService siteService, ISiteContextService siteContextService, IPageRetriever pageRetriever, ISiteCultureRepository siteCultureRepository)
        {
            SiteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            SiteContextService = siteContextService ?? throw new ArgumentNullException(nameof(siteContextService));
            PageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
            SiteCultureRepository = siteCultureRepository ?? throw new ArgumentNullException(nameof(siteCultureRepository));
        }
    }
}
