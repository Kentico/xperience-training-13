using CMS.Base;
using Kentico.Content.Web.Mvc;

using Abstractions;

namespace XperienceAdapter
{
    /// <summary>
    /// Common repository dependencies.
    /// </summary>
    public interface IRepositoryDependencies
    {
        ISiteService SiteService { get; }

        ISiteContextService SiteContextService { get; }

        IPageRetriever PageRetriever { get; }
    }
}
