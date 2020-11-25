using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using Kentico.Web.Mvc;

using XperienceAdapter.Models;

namespace Business.Extensions
{
    /// <summary>
    /// Works with URLs.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Gets a relative URL of a page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The relative URL.</returns>
        public static string? RelativeUrl(this TreeNode page) =>
            page.GetPageUrlPath(Thread.CurrentThread.CurrentUICulture.Name).FullPath;

        /// <summary>
        /// Custom extension method for retrieving image urls based on their path
        /// </summary>
        /// <param name="helper">Html helper</param>
        /// <param name="path">Path to file</param>
        /// <param name="size">Size constraints</param>
        /// <returns></returns>
        //public static string KenticoImageUrl(this IUrlHelper helper, string path, IImageSizeConstraint? size = default) =>
        //    helper.Kentico().ImageUrl(path, size?.GetSizeConstraint() ?? SizeConstraint.Empty);

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

        /// <summary>
        /// Makes a string contain only characters allowed in URLs.
        /// </summary>
        /// <param name="input">String to transform.</param>
        /// <returns>String transformed to be URL-compliant.</returns>
        public static string ToUrlCompliantString(this string input)
        {
            var allowedCharacters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~:/?#[]@!$&'()*+,;=";
            var stringBuilder = new StringBuilder();

            foreach (var character in input)
            {
                var charToAdd = allowedCharacters.Contains(character) ? character : '_';
                stringBuilder.Append(charToAdd);
            }

            return stringBuilder.ToString();
        }

        public static string UrlInCurrentUiCulture(this IUrlHelper helper, string routeName) =>
            helper.RouteUrl($"{routeName}_{Thread.CurrentThread.CurrentUICulture.Name}");
    }
}
