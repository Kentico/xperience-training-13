using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using Core;
using XperienceAdapter.Models;

namespace XperienceAdapter.Repositories
{
    /// <summary>
    /// Stores pages of various types.
    /// </summary>
    /// <typeparam name="TPageDto">Page DTO.</typeparam>
    /// <typeparam name="TPage">Xperience page.</typeparam>
    public interface IPageRepository<TPageDto, TPage> : IRepository<TPageDto>
        where TPageDto : BasePage, new()
        where TPage : TreeNode, new()
    {
        /// <summary>
        /// Gets pages and optionally caches them.
        /// </summary>
        /// <param name="filter">Optional ad-hoc filter.</param>
        /// <param name="additionalMapper">Optional ad-hoc mapper.</param>
        /// <param name="buildCacheAction">Cache settings factory.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPagesInCurrentCulture(
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default);

        /// <summary>
        /// Gets pages and optionally caches them.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="filter">Optional ad-hoc filter.</param>
        /// <param name="additionalMapper">Optional ad-hoc mapper.</param>
        /// <param name="buildCacheAction">Cache settings factory.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        Task<IEnumerable<TPageDto>> GetPagesInCurrentCultureAsync(
            CancellationToken? cancellationToken = default,
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default);

        /// <summary>
        /// Gets pages of multiple types and optionally caches them.
        /// </summary>
        /// <param name="types">Xperience page types.</param>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <param name="filter">Optional ad-hoc filter.</param>
        /// <param name="additionalMapper">Optional ad-hoc mapper.</param>
        /// <param name="buildCacheAction">Cache settings factory.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPagesByTypeAndCulture(
            IEnumerable<string> types,
            SiteCulture culture,
            string cacheKey,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default,
            params string[] cacheDependencies);

        /// <summary>
        /// Gets pages of multiple types and optionally caches them.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="types">Xperience page types.</param>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <param name="filter">Optional ad-hoc filter.</param>
        /// <param name="additionalMapper">Optional ad-hoc mapper.</param>
        /// <param name="buildCacheAction">Cache settings factory.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        Task<IEnumerable<TPageDto>> GetPagesByTypeAndCultureAsync(
            IEnumerable<string> types,
            SiteCulture culture,
            string cacheKey,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            bool includeAttachments = default,
            CancellationToken? cancellationToken = default,
            params string[] cacheDependencies);

        /// <summary>
        /// Maps page onto a DTO.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="dto">Page DTO.</param>
        void MapDtoProperties(TPage page, TPageDto dto);
    }
}
