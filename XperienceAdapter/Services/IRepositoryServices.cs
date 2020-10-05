using CMS.Base;
using Kentico.Content.Web.Mvc;
using XperienceAdapter.Repositories;

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

        IPageAttachmentUrlRetriever PageAttachmentUrlRetriever { get; }

        ISiteCultureRepository SiteCultureRepository { get; }
    }
}
