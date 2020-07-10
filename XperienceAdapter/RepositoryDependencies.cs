using System;

using CMS.Base;
using Kentico.Content.Web.Mvc;

using Abstractions;

namespace XperienceAdapter
{
    public class RepositoryDependencies : IRepositoryDependencies
    {
        public ISiteService SiteService { get; }

        public ISiteContextService SiteContextService { get; }

        public IPageRetriever PageRetriever { get; }

        public RepositoryDependencies(ISiteService siteService, ISiteContextService siteContextService, IPageRetriever pageRetriever)
        {
            SiteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            SiteContextService = siteContextService ?? throw new ArgumentNullException(nameof(siteContextService));
            PageRetriever = pageRetriever ?? throw new ArgumentNullException(nameof(pageRetriever));
        }
    }
}
