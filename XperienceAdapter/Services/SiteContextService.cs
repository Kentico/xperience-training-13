using System;
using Microsoft.AspNetCore.Http;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace XperienceAdapter.Services
{
    public class SiteContextService : ISiteContextService
    {
        public bool IsPreviewEnabled => HttpContextAccessor.HttpContext.Kentico().Preview().Enabled;

        // TODO: You should not need this
        public string PreviewCulture => HttpContextAccessor.HttpContext.Kentico().Preview().CultureName;

        protected IHttpContextAccessor HttpContextAccessor { get; }

        public SiteContextService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
    }
}
