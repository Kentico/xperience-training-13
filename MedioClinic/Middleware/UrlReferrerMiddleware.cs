﻿using CMS.ContactManagement;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XperienceAdapter.Cookies;

namespace MedioClinic.Middleware
{
    public class UrlReferrerMiddleware
    {
        private const string ReferrerHeaderName = "Referer";

        private const string ReferrerDbColumnName = "UrlReferrer";

        private readonly RequestDelegate _next;

        private readonly ICookieManager _cookieManager;

        private readonly IContactInfoProvider _contactInfoProvider;

        public UrlReferrerMiddleware(RequestDelegate next,
                                     ICookieManager cookieManager,
                                     IContactInfoProvider contactInfoProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cookieManager = cookieManager ?? throw new ArgumentNullException(nameof(cookieManager));
            _contactInfoProvider = contactInfoProvider ?? throw new ArgumentNullException(nameof(contactInfoProvider));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            await invokeImplementation();

            async Task invokeImplementation()
            {
                var referrer = httpContext.Request.Headers[ReferrerHeaderName].FirstOrDefault();
                var firstReferrer = httpContext.Request.Cookies[CookieManager.FirstReferrerCookieName];
                var isReferrerSet = !string.IsNullOrEmpty(referrer);
                var isFirstReferrerSet = !string.IsNullOrEmpty(firstReferrer);

                if (!httpContext.Response.HasStarted)
                {
                    if (isReferrerSet && !isFirstReferrerSet)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            IsEssential = true,
                            Expires = DateTime.Today.AddDays(1),
                            SameSite = SameSiteMode.Strict,
                            Secure = false
                        };

                        httpContext.Response.OnStarting(() =>
                        {
                            httpContext.Response.Cookies.Append(CookieManager.FirstReferrerCookieName, referrer, cookieOptions);

                            return Task.CompletedTask;
                        });
                    }

                    if (_cookieManager.VisitorCookiesEnabled && (isReferrerSet || isFirstReferrerSet))
                    {
                        var valueToSave = isFirstReferrerSet ? firstReferrer : referrer;
                        var contact = ContactManagementContext.GetCurrentContact(false);
                        var urlReferrerDbValue = contact?.GetStringValue(ReferrerDbColumnName, string.Empty);

                        if (contact != null && string.IsNullOrEmpty(urlReferrerDbValue))
                        {
                            contact.SetValue(ReferrerDbColumnName, valueToSave);
                            _contactInfoProvider.Set(contact);

                            httpContext.Response.OnStarting(() =>
                            {
                                // TODO: Why the heck it is not deleted or is created again?
                                httpContext.Response.Cookies.Delete(CookieManager.FirstReferrerCookieName);

                                return Task.CompletedTask;
                            });
                        }
                    }
                }

                await _next(httpContext);
            }
        }
    }
}
