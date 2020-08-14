using CMS.Base;
using Kentico.Content.Web.Mvc;

namespace XperienceAdapter.Services
{
    /// <summary>
    /// Common repository dependencies.
    /// </summary>
    public interface IRepositoryServices
    {
        ISiteService SiteService { get; }

        ISiteContextService SiteContextService { get; }

        IPageRetriever PageRetriever { get; }
    }
}
