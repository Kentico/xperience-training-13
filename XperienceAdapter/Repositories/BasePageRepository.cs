using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine;
using CMS.Helpers;
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
        where TPageDto : BasicPage, new()
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

        public virtual IEnumerable<TPageDto> GetAll() => GetPagesInCurrentCulture(
            buildCacheAction: cache => cache
                .Key($"{nameof(BasePageRepository<TPageDto, TPage>)}|{typeof(TPage).Name}")
                .Expiration(TimeSpan.FromSeconds(30)));

        public virtual async Task<IEnumerable<TPageDto>> GetAllAsync(CancellationToken? cancellationToken = default) =>
            await GetPagesInCurrentCultureAsync(
                cancellationToken,
                buildCacheAction: cache => cache
                    .Key($"{nameof(BasePageRepository<TPageDto, TPage>)}|{typeof(TPage).Name}")
                    .Expiration(TimeSpan.FromSeconds(30)));

        public virtual IEnumerable<TPageDto> GetPagesInCurrentCulture(
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default)
        {
            var result = _repositoryServices.PageRetriever.Retrieve(query =>
            {
                query.Columns(DefaultDtoFactory().SourceColumns);
                filter?.Invoke(query);
            },
            buildCacheAction);

            return MapPages(result, additionalMapper, includeAttachments);
        }

        public virtual async Task<IEnumerable<TPageDto>> GetPagesInCurrentCultureAsync(
            CancellationToken? cancellationToken = default,
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default)
        {
            var result = await _repositoryServices.PageRetriever.RetrieveAsync(query =>
            {
                query.Columns(DefaultDtoFactory().SourceColumns);
                filter?.Invoke(query);
            },
            buildCacheAction,
            cancellationToken);

            return MapPages(result, additionalMapper, includeAttachments);
        }

        public virtual IEnumerable<TPageDto> GetPagesByTypeAndCulture(
            IEnumerable<string> types,
            SiteCulture culture,
            string cacheKey,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default,
            params string[] cacheDependencies)
        {
            var cacheSettings = GetCacheSettings(cacheKey, cacheDependencies);

            var result = _repositoryServices.ProgressiveCache.Load(cacheSettings =>
                GetPagesOfMultipleTypes(types, culture, filter),
                cacheSettings);

            return MapPages(result, additionalMapper, includeAttachments);
        }

        public virtual async Task<IEnumerable<TPageDto>> GetPagesByTypeAndCultureAsync(
            IEnumerable<string> types,
            SiteCulture culture,
            string cacheKey,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default,
            CancellationToken? cancellationToken = default,
            params string[] cacheDependencies)
        {
            var cacheSettings = GetCacheSettings(cacheKey, cacheDependencies);

            var result = await _repositoryServices.ProgressiveCache.LoadAsync(async cacheSettings =>
                await GetPagesOfMultipleTypesAsync(types, culture, filter),
                cacheSettings);

            return MapPages(result, additionalMapper, includeAttachments);
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
            bool includeAttachments = default)
        {
            if (pages != null && pages.Any())
            {
                if (additionalMapper != null)
                {
                    foreach (var page in pages)
                    {
                        var dto = ApplyMappers(page!, includeAttachments);

                        if (dto != null)
                        {
                            yield return additionalMapper(page!, dto);
                        }
                    }
                }
                else
                {
                    foreach (var page in pages)
                    {
                        var dto = ApplyMappers(page!, includeAttachments);

                        if (dto != null)
                        {
                            yield return dto;
                        }
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
            MapDtoProperties(page, dto);

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
            dto.Culture = _repositoryServices.SiteCultureRepository.GetByExactIsoCode(page.DocumentCulture);

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
        public virtual void MapDtoProperties(TPage page, TPageDto dto) { }

        protected IEnumerable<TPage> GetPagesOfMultipleTypes(
            IEnumerable<string> types,
            SiteCulture culture,
            Action<MultiDocumentQuery>? filter = default)
        {
            MultiDocumentQuery query = GetQueryForMultipleTypes(types, culture, filter);

            return query
                .GetEnumerableTypedResult()
                .Select(page => page as TPage)
                .Where(page => page != null)!;
        }

        protected async Task<IEnumerable<TPage>> GetPagesOfMultipleTypesAsync(
            IEnumerable<string> types,
            SiteCulture culture,
            Action<MultiDocumentQuery>? filter = default)
        {
            MultiDocumentQuery query = GetQueryForMultipleTypes(types, culture, filter);

            return (await query
                .GetEnumerableTypedResultAsync())
                .Select(page => page as TPage)
                .Where(page => page != null)!;
        }

        protected MultiDocumentQuery GetQueryForMultipleTypes(
            IEnumerable<string> types, 
            SiteCulture? culture, 
            Action<MultiDocumentQuery>? filter)
        {
            var query = new MultiDocumentQuery();

            query = FilterFor(query, culture)
                .Types(types.ToArray())
                .WithCoupledColumns();

            filter?.Invoke(query);

            return query;
        }

        protected virtual TQuery FilterFor<TQuery, TObject>(IDocumentQuery<TQuery, TObject> query, SiteCulture? siteCulture = default)
            where TQuery : IDocumentQuery<TQuery, TObject>
            where TObject : TreeNode, new()
        {
            var typedQuery = query.GetTypedQuery();

            typedQuery
                .OnSite(_repositoryServices.SiteService.CurrentSite.SiteName);

            if (siteCulture != null)
            {
                typedQuery.Culture(siteCulture.IsoCode);
            }

            if (_repositoryServices.SiteContextService.IsPreviewEnabled)
            {
                typedQuery
                    .LatestVersion()
                    .Published(false);
            }
            else
            {
                typedQuery
                    .Published()
                    .PublishedVersion();
            }

            return typedQuery;
        }

        protected static CacheSettings GetCacheSettings(string cacheKey, params string[] cacheDependencies)
        {
            var settings = new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);
            settings.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);

            return settings;
        }
    }
}
