using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using XperienceAdapter.Models;
using XperienceAdapter.Services;

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
        protected readonly IRepositoryServices _repositoryServices;

        /// <summary>
        /// Default DTO factory method.
        /// </summary>
        protected virtual Func<TPageDto> DefaultDtoFactory => () => new TPageDto();

        public BasePageRepository(IRepositoryServices repositoryDependencies)
        {
            _repositoryServices = repositoryDependencies ?? throw new ArgumentNullException(nameof(repositoryDependencies));
        }

        // TODO: How to access CLASS_NAME to set .PageType()?
        public virtual IEnumerable<TPageDto> GetAll() => GetPages(
            buildCacheAction: cache => cache
                .Key($"{nameof(BasePageRepository<TPageDto, TPage>)}|{typeof(TPage).Name}"));

        public virtual Task<IEnumerable<TPageDto>> GetAllAsync() => Task.FromResult(GetAll());

        public virtual IEnumerable<TPageDto> GetPages(
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default,
            SiteCulture? culture = default)
        {
            var result = _repositoryServices.PageRetriever.Retrieve(query =>
            {
                query = FilterForSingleType(query, culture?.IsoCode);
                query.Columns(DefaultDtoFactory().SourceColumns);
                filter?.Invoke(query);
            },
            buildCacheAction);

            return MapPages(result, additionalMapper, includeAttachments, culture);
        }

        public virtual async Task<IEnumerable<TPageDto>> GetPagesAsync(
            CancellationToken cancellationToken,
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default,
            SiteCulture? culture = default)
        {
            var result = await _repositoryServices.PageRetriever.RetrieveAsync(query =>
            {
                query = FilterForSingleType(query, culture?.IsoCode);
                query.Columns(DefaultDtoFactory().SourceColumns);
                filter?.Invoke(query);
            },
            buildCacheAction,
            cancellationToken);

            return MapPages(result, additionalMapper, includeAttachments, culture);
        }

        public virtual IEnumerable<TPageDto> GetPagesOfMultitpleTypes(
            IEnumerable<string> types,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TreeNode>>? buildCacheAction = default,
            bool includeAttachments = default,
            SiteCulture? culture = default)
        {
            var result = _repositoryServices.PageRetriever.RetrieveMultiple(query =>
            {
                query = FilterForMultipleTypes(query, culture?.IsoCode);
                query
                    .Types(types.ToArray())
                    .WithCoupledColumns();
                filter?.Invoke(query);

            },
            buildCacheAction).Select(h => h as TPage);

            return MapPages(result, additionalMapper, includeAttachments, culture);
        }

        public virtual async Task<IEnumerable<TPageDto>> GetPagesOfMultitpleTypesAsync(
            CancellationToken cancellationToken,
            IEnumerable<string> types,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TreeNode>>? buildCacheAction = default,
            bool includeAttachments = default,
            SiteCulture? culture = default)
        {
            var result = (await _repositoryServices.PageRetriever.RetrieveMultipleAsync(query =>
            {
                query = FilterForMultipleTypes(query, culture?.IsoCode);
                query
                    .Types(types.ToArray())
                    .WithCoupledColumns();
                filter?.Invoke(query);

            },
            buildCacheAction,
            cancellationToken)).Select(h => h as TPage);

            return MapPages(result, additionalMapper, includeAttachments, culture);
        }

        public TPageDto GetPage(Guid nodeGuid, bool includeAttachments) =>
            GetPages(
                filter => GetDefaultPageFilter(filter)
                    .WhereEquals("NodeGUID", nodeGuid),
                buildCacheAction: cache => GetDefaultPageCacheBuilder(cache, nodeGuid.ToString(), includeAttachments),
                includeAttachments: includeAttachments)
                    .FirstOrDefault();

        public async Task<TPageDto> GetPageAsync(Guid nodeGuid, bool includeAttachments, CancellationToken cancellationToken) =>
            (await GetPagesAsync(
                cancellationToken,
                filter => GetDefaultPageFilter(filter)
                    .WhereEquals("NodeGUID", nodeGuid),
                buildCacheAction: cache => GetDefaultPageCacheBuilder(cache, nodeGuid.ToString(), includeAttachments),
                includeAttachments: includeAttachments))
                    .FirstOrDefault();

        public TPageDto GetPage(string pageAlias, bool includeAttachments) =>
            GetPages(
                filter => GetDefaultPageFilter(filter)
                    .WhereEquals("NodeAlias", pageAlias),
                buildCacheAction: cache => GetDefaultPageCacheBuilder(cache, pageAlias, includeAttachments),
                includeAttachments: includeAttachments)
                    .FirstOrDefault();

        public async Task<TPageDto> GetPageAsync(string pageAlias, bool includeAttachments, CancellationToken cancellationToken) =>
            (await GetPagesAsync(
                cancellationToken,
                filter => GetDefaultPageFilter(filter)
                    .WhereEquals("NodeAlias", pageAlias),
                buildCacheAction: cache => GetDefaultPageCacheBuilder(cache, pageAlias, includeAttachments),
                includeAttachments: includeAttachments))
                    .FirstOrDefault();

        protected virtual DocumentQuery<TPage> GetDefaultPageFilter(DocumentQuery<TPage> filter) =>
            filter.TopN(1);

        protected virtual IPageCacheBuilder<TPage> GetDefaultPageCacheBuilder(IPageCacheBuilder<TPage> builder, string identifier, bool includeAttachments)
        {
            var attachmentSegment = includeAttachments ? "WithAttachments" : "WithoutAttachments";

            return GetCacheBuilder(builder, $"{nameof(GetPage)}|{identifier}|{attachmentSegment}", identifier);
        }

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
                .OnSite(_repositoryServices.SiteService.CurrentSite.SiteName)
                .Culture(culture);

            if (_repositoryServices.SiteContextService.IsPreviewEnabled)
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

        protected static IPageCacheBuilder<TPage> GetCacheBuilder(
            IPageCacheBuilder<TPage> pageCacheBuilder,
            string cacheKeySuffix,
            string identifier) =>
                pageCacheBuilder
                    .Key($"{nameof(BasePageRepository<TPageDto, TPage>)}|{cacheKeySuffix}")
                    .Dependencies((_, builder) => builder
                        .Custom(identifier));

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
                .OnSite(_repositoryServices.SiteService.CurrentSite.SiteName)
                .Culture(culture);

            if (_repositoryServices.SiteContextService.IsPreviewEnabled)
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
            var contextService = _repositoryServices.SiteContextService;

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
        protected IEnumerable<TPageDto> MapPages(
            IEnumerable<TPage?>? pages = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default,
            SiteCulture? culture = default)
        {
            if (pages != null && pages.Any())
            {
                if (additionalMapper != null)
                {
                    foreach (var page in pages)
                    {
                        TPageDto dto = ApplyMappers(page!, includeAttachments, culture);

                        yield return additionalMapper(page!, dto);
                    }
                }
                else
                {
                    foreach (var page in pages)
                    {
                        yield return ApplyMappers(page!, includeAttachments, culture);
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
        protected TPageDto ApplyMappers(TPage page, bool includeAttachments, SiteCulture? culture = default)
        {
            var dto = MapBasicDtoProperties(page, includeAttachments, culture);
            dto = MapDtoProperties(page, dto);

            return dto;
        }

        /// <summary>
        /// Maps basic Xperience page properties onto DTO ones.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTO.</returns>
        protected virtual TPageDto MapBasicDtoProperties(TPage page, bool includeAttachments, SiteCulture? culture)
        {
            var dto = DefaultDtoFactory();
            dto.Guid = page.DocumentGUID;
            dto.NodeId = page.NodeID;
            dto.Name = page.DocumentName;
            dto.NodeAliasPath = page.NodeAliasPath;
            dto.ParentId = page.NodeParentID;

            if (culture is null == false && page.CultureVersions.Any())
            {
                var cultureVersion = page.CultureVersions.FirstOrDefault(version => version.DocumentCulture.Equals(culture.IsoCode, StringComparison.InvariantCulture));

                if (cultureVersion != null)
                {
                    dto.Culture = _repositoryServices.SiteCultureRepository.GetByExactIsoCode(cultureVersion.DocumentCulture);
                }
            }

            if (includeAttachments)
            {
                foreach (var attachment in page.Attachments)
                {
                    dto.Attachments.Add(new PageAttachment
                    {
                        AttachmentUrl = _repositoryServices.PageAttachmentUrlRetriever.Retrieve(attachment),
                        Extension = attachment.AttachmentExtension,
                        FileName = attachment.AttachmentName,
                        Guid = attachment.AttachmentGUID,
                        Id = attachment.ID,
                        MimeType = attachment.AttachmentMimeType,
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
        public virtual TPageDto MapDtoProperties(TPage page, TPageDto dto) => dto;
    }
}
