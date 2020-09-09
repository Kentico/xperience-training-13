using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MedioClinic.Middleware
{
    public class CultureMiddleware
    {
        private const string CultureParameterName = "culture";

        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // TODO: Nested function.
            var cultureParameterValue = httpContext
                .Request
                .RouteValues?
                .FirstOrDefault(value => value.Key.Equals(CultureParameterName, StringComparison.OrdinalIgnoreCase))
                .Value as string;

            if (!string.IsNullOrEmpty(cultureParameterValue))
            {
                var cultureInfo = new CultureInfo(cultureParameterValue);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }

            await _next(httpContext);
        }
    }
}
