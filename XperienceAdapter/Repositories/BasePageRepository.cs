using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Dtos;

namespace XperienceAdapter.Repositories
{
    /// <summary>
    /// Provides base functionality to retrieve pages.
    /// </summary>
    /// <typeparam name="TPageDto">Page DTO.</typeparam>
    /// <typeparam name="TPage">Xperience page.</typeparam>
    public abstract class BasePageRepository<TPageDto, TPage> : IPageRepository<TPageDto, TPage>
        where TPageDto : BasePage, new()
        where TPage : TreeNode, new()
    {
        protected readonly IRepositoryDependencies _repositoryDependencies;

        /// <summary>
        /// Default DTO factory method.
        /// </summary>
        protected virtual Func<TPageDto> DefaultDtoFactory => () => new TPageDto();

        public BasePageRepository(IRepositoryDependencies repositoryDependencies)
        {
            _repositoryDependencies = repositoryDependencies ?? throw new ArgumentNullException(nameof(repositoryDependencies));
        }

        public virtual IEnumerable<TPageDto> GetAll() => GetPages();

        public virtual Task<IEnumerable<TPageDto>> GetAllAsync() => Task.FromResult(GetAll());

        public virtual IEnumerable<TPageDto> GetPages(
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default,
            string? culture = default)
        {
            var result = _repositoryDependencies.PageRetriever.Retrieve(query =>
            {
                query = FilterForSingleType(query, culture);
                query.Columns(DefaultDtoFactory().SourceColumns);
                filter?.Invoke(query);
            },
            buildCacheAction);

            return MapResult(result, additionalMapper, includeAttachments);
        }

        public virtual IEnumerable<TPageDto> GetPagesOfMultitpleTypes(
            IEnumerable<string> types,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TreeNode>>? buildCacheAction = default,
            bool includeAttachments = default,
            string? culture = default)
        {
            var result = _repositoryDependencies.PageRetriever.RetrieveMultiple(query =>
            {
                query = FilterForMultipleTypes(query, culture);
                query
                    .Types(types.ToArray())
                    .WithCoupledColumns();
                filter?.Invoke(query);

            },
            buildCacheAction).Select(h => h as TPage);

            return MapResult(result, additionalMapper, includeAttachments);
        }

        public IEnumerable<TPageDto> GetPage(Guid nodeGuid, bool includeAttachments) =>
            GetPages(query => query
                .WhereEquals("NodeGUID", nodeGuid)
                .TopN(1),
                includeAttachments: includeAttachments);

        public IEnumerable<TPageDto> GetPage(string pageAlias, bool includeAttachments) =>
            GetPages(query => query
                .WhereEquals("NodeAlias", pageAlias)
                .TopN(1),
                includeAttachments: includeAttachments);

        /// <summary>
        /// Adds default filters for <see cref="MultiDocumentQuery"/> queries.
        /// </summary>
        /// <param name="query">Basic query.</param>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <returns>Modified query.</returns>
        protected virtual MultiDocumentQuery FilterForMultipleTypes(MultiDocumentQuery query, string? culture)
        {
            culture = EnsureCulture(culture);

            query
                .OnSite(_repositoryDependencies.SiteService.CurrentSite.SiteName)
                .Culture(culture);
            //.AddColumn("NodeSiteID") // TODO: Test if it is still necessary.

            if (_repositoryDependencies.SiteContextService.IsPreviewEnabled)
            {
                query
                    .LatestVersion()
                    .Published(false);
            }
            else
            {
                query
                    .Published()
                    .PublishedVersion();
            }

            return query;
        }

        /// <summary>
        /// Adds default filters for <see cref="DocumentQuery{TDocument}"/> queries.
        /// </summary>
        /// <param name="query">Basic query.</param>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <returns>Modified query.</returns>
        protected virtual DocumentQuery<TPage> FilterForSingleType(DocumentQuery<TPage> query, string? culture)
        {
            culture = EnsureCulture(culture);

            query
                .OnSite(_repositoryDependencies.SiteService.CurrentSite.SiteName)
                .Culture(culture);
            //.AddColumn("NodeSiteID") // TODO: Test if it is still necessary.

            if (_repositoryDependencies.SiteContextService.IsPreviewEnabled)
            {
                query
                    .LatestVersion()
                    .Published(false);
            }
            else
            {
                query
                    .Published()
                    .PublishedVersion();
            }

            return query;
        }

        /// <summary>
        /// Gets culture from site context, if not specified explicitly.
        /// </summary>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <returns>Context-dependent culture.</returns>
        protected string EnsureCulture(string? culture)
        {
            var contextService = _repositoryDependencies.SiteContextService;

            if (string.IsNullOrEmpty(culture))
            {
                culture = contextService.IsPreviewEnabled
                    ? contextService.PreviewCulture
                    : Thread.CurrentThread.CurrentUICulture.Name;
            }

            return culture;
        }

        /// <summary>
        /// Maps query results onto DTOs.
        /// </summary>
        /// <param name="pages">Xperience pages.</param>
        /// <param name="additionalMapper">Ad-hoc mapper supplied as a parameter.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        protected IEnumerable<TPageDto> MapResult(
            IEnumerable<TPage?>? pages = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default)
        {
            if (pages != null && pages.Any())
            {
                if (additionalMapper != null)
                {
                    foreach (var page in pages)
                    {
                        TPageDto dto = ApplyMappers(page!, includeAttachments);

                        yield return additionalMapper(page!, dto);
                    }
                }
                else
                {
                    foreach (var page in pages)
                    {
                        yield return ApplyMappers(page!, includeAttachments);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the basic mapper as well as the type-specific one.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTO.</returns>
        protected TPageDto ApplyMappers(TPage page, bool includeAttachments)
        {
            var dto = MapBasicDtoProperties(page, includeAttachments);
            dto = MapDtoProperties(page, dto);

            return dto;
        }

        /// <summary>
        /// Maps basic Xperience page properties onto DTO ones.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTO.</returns>
        protected virtual TPageDto MapBasicDtoProperties(TPage page, bool includeAttachments)
        {
            var dto = DefaultDtoFactory();
            dto.Guid = page.DocumentGUID;
            dto.NodeId = page.NodeID;
            dto.Name = page.DocumentName;
            dto.NodeAliasPath = page.NodeAliasPath;
            dto.ParentId = page.NodeParentID;
            dto.Culture = page.DocumentCulture;

            if (includeAttachments)
            {
                foreach (var attachment in page.Attachments)
                {
                    dto.Attachments.Add(new PageAttachment
                    {
                        Extension = attachment.AttachmentExtension,
                        FileName = attachment.AttachmentName,
                        Guid = attachment.AttachmentGUID,
                        Id = attachment.ID,
                        MimeType = attachment.AttachmentMimeType,
                        ServerPath = attachment.GetPath(null),
                        Title = attachment.AttachmentTitle
                    });
                }
            }

            return dto;
        }

        /// <summary>
        /// Default DTO mapping method.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="dto">Page DTO.</param>
        /// <returns></returns>
        public virtual TPageDto MapDtoProperties(TPage page, TPageDto dto)
            => dto;
    }
}
