using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Kentico.Web.Mvc;

namespace Business.Extensions
{
    /// <summary>
    /// Works with URLs.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Builds an absolute URL out of context information.
        /// </summary>
        /// <param name="helper">HTML helper.</param>
        /// <param name="request">HTTP request.</param>
        /// <param name="action">Controller action.</param>
        /// <param name="controller">Controller.</param>
        /// <param name="routeValues">Route values.</param>
        /// <returns></returns>
        public static string AbsoluteUrl(this IUrlHelper helper, HttpRequest request, string action, string? controller = default, object? routeValues = default)
        {
            if (request is null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }

            var scheme = request?.Scheme;
            var domain = request?.Host;

            var actionContext = new UrlActionContext
            {
                Action = action,
                Controller = controller,
                Values = routeValues
            };

            var relativePath = helper.Action(actionContext);

            return $"{scheme}://{domain}{relativePath}";
        }
    }
}
