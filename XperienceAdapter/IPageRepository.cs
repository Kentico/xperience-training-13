using System;
using System.Collections.Generic;
using System.Text;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

using Abstractions;

namespace XperienceAdapter
{
    public interface IPageRepository<TPageDto, TPage> : IRepository<TPageDto>
        where TPageDto : BasePageDto, new()
        where TPage : TreeNode, new()
    {
        Func<TPage, TPageDto, TPageDto> Mapper { get; }

        IEnumerable<TPageDto> GetPages(
            Action<DocumentQuery<TPage>> filter = null!,
            Func<TPage, TPageDto, TPageDto> additionalMapper = null!,
            Action<IPageCacheBuilder<TPage>> buildCacheAction = null!);

        IEnumerable<TPageDto> GetPages(Guid nodeGuid);

        IEnumerable<TPageDto> GetPages(string pageAlias);
    }
}
