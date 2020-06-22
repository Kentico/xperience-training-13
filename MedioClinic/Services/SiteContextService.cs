﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using Abstractions;

namespace XperienceAdapter
{
    public class SiteContextService : ISiteContextService
    {
        public bool IsPreviewEnabled => HttpContextAccessor.HttpContext.Kentico().Preview().Enabled;

        public string PreviewCulture => HttpContextAccessor.HttpContext.Kentico().Preview().CultureName;

        protected IHttpContextAccessor HttpContextAccessor { get; }

        public SiteContextService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
    }
}
