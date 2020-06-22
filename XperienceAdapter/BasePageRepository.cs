using System;
using System.Collections.Generic;
using System.Text;

using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace XperienceAdapter
{
    public abstract class BasePageRepository<TPageDto, TPage>
        where TPageDto : BasePageDto, new()
        where TPage : TreeNode, new()
    {
        protected readonly IRepositoryDependencies _repositoryDependencies;

        protected virtual Func<TPageDto> DefaultDtoFactory => () => new TPageDto();

        protected virtual Func<TPage, TPageDto> BaseMapper => (page) =>
        {
            var dto = DefaultDtoFactory();
            dto.Guid = page.DocumentGUID;
            dto.Id = page.DocumentID;
            dto.Name = page.DocumentName;
            dto.NodeAliasPath = page.NodeAliasPath;

            return dto;
        };

        public virtual Func<TPage, TPageDto, TPageDto> Mapper => (document, dto) => dto;

        protected virtual Action<DocumentQuery<TPage>> BasicFilter => query =>
        {
            query
                .OnSite(_repositoryDependencies.SiteService.CurrentSite.SiteName)
                .Culture(_repositoryDependencies.SiteContextService.PreviewCulture)
                .Columns(DefaultDtoFactory().SourceColumns);

            if (_repositoryDependencies.SiteContextService.IsPreviewEnabled)
            {
                query = query
                    .AddColumn("NodeSiteID") // TODO: Test if it is still necessary.
                    .LatestVersion()
                    .Published(false);
            }
            else
            {
                query = query
                    .Published()
                    .PublishedVersion();
            }
        };

        public BasePageRepository(IRepositoryDependencies repositoryDependencies)
        {
            _repositoryDependencies = repositoryDependencies ?? throw new ArgumentNullException(nameof(repositoryDependencies));
        }

        public virtual IEnumerable<TPageDto> GetAll() => GetPages();

        public virtual IEnumerable<TPageDto> GetPages(
            Action<DocumentQuery<TPage>> filter = null!,
            Func<TPage, TPageDto, TPageDto> additionalMapper = null!,
            Action<IPageCacheBuilder<TPage>> buildCacheAction = null!)
        {
            var result = _repositoryDependencies.PageRetriever.Retrieve(
                query =>
                {
                    BasicFilter(query);

                    filter?.Invoke(query);
                },
                buildCacheAction
                );

                if (additionalMapper != null)
                {
                    foreach (var page in result)
                    {
                        TPageDto dto = ApplyMappers(page);

                        yield return additionalMapper(page, dto);
                    }
                }
                else
                {
                    foreach (var page in result)
                    {
                        yield return ApplyMappers(page);
                    }
                }
        }

        public IEnumerable<TPageDto> GetPages(Guid nodeGuid) =>
            GetPages(query => query
                .WhereEquals("NodeGUID", nodeGuid)
                .TopN(1));

        public IEnumerable<TPageDto> GetPages(string pageAlias) =>
            GetPages(query => query
                .WhereEquals("NodeAlias", pageAlias)
                .TopN(1));

        protected TPageDto ApplyMappers(TPage document)
        {
            var dto = BaseMapper(document);
            dto = Mapper(document, dto);

            return dto;
        }
    }
}
