using System;
using System.Collections.Generic;
using System.Text;

using CMS.Base;
using Kentico.Content.Web.Mvc;

using Abstractions;

namespace XperienceAdapter
{
    public interface IRepositoryDependencies
    {
        ISiteService SiteService { get; }

        ISiteContextService SiteContextService { get; }

        IPageRetriever PageRetriever { get; }
    }
}
