using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedioClinic.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent MedioClinicInputFor<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression,
            string? templateName = default,
            string? htmlFieldName = default,
            object? additionalViewData = default)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("row");
            tagBuilder.AddCssClass("input-field");
            tagBuilder.InnerHtml.AppendHtml(htmlHelper.EditorFor(expression, templateName, htmlFieldName, additionalViewData));
            string result;

            using (var writer = new StringWriter())
            {
                tagBuilder.WriteTo(writer, HtmlEncoder.Default);
                result = writer.ToString();
            }

            return new HtmlString(result);
        }
    }
}
