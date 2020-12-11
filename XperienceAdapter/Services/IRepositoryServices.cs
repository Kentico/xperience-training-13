using CMS.Base;
using CMS.Helpers;
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

        IPageUrlRetriever PageUrlRetriever { get; }

        IPageAttachmentUrlRetriever PageAttachmentUrlRetriever { get; }

        ISiteCultureRepository SiteCultureRepository { get; }

        IProgressiveCache ProgressiveCache { get; }
    }
}
