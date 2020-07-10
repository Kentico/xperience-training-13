using System;
using System.Collections.Generic;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using Abstractions;
using XperienceAdapter.Dtos;

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
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPages(
            Action<DocumentQuery<TPage>>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TPage>>? buildCacheAction = default,
            bool includeAttachments = default,
            string? culture = default);

        /// <summary>
        /// Gets pages of multiple types and optionally caches them.
        /// </summary>
        /// <param name="types">Xperience page types.</param>
        /// <param name="filter">Optional ad-hoc filter.</param>
        /// <param name="additionalMapper">Optional ad-hoc mapper.</param>
        /// <param name="buildCacheAction">Cache settings factory.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <param name="culture">Explicitly-stated culture.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPagesOfMultitpleTypes(
            IEnumerable<string> types,
            Action<MultiDocumentQuery>? filter = default,
            Func<TPage, TPageDto, TPageDto>? additionalMapper = default,
            Action<IPageCacheBuilder<TreeNode>>? buildCacheAction = default,
            bool includeAttachments = default,
            string? culture = default);

        /// <summary>
        /// Gets a specific page.
        /// </summary>
        /// <param name="nodeGuid">Page GUID.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPage(Guid nodeGuid, bool includeAttachments = false);

        /// <summary>
        /// Gets a specific page.
        /// </summary>
        /// <param name="pageAlias">Page NodeAlias.</param>
        /// <param name="includeAttachments">Indicates if attachment information shall be included.</param>
        /// <returns>Page DTOs.</returns>
        IEnumerable<TPageDto> GetPage(string pageAlias, bool includeAttachments = false);

        /// <summary>
        /// Maps page onto a DTO.
        /// </summary>
        /// <param name="page">Xperience page.</param>
        /// <param name="dto">Page DTO.</param>
        /// <returns>Page DTO.</returns>
        TPageDto MapDtoProperties(TPage page, TPageDto dto);
    }
}
