using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using CMS.Helpers;

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

        public static IHtmlContent FileInput(this IHtmlHelper htmlHelper,
                                             string buttonResourceKey,
                                             string formFieldName,
                                             IDictionary<string, object> fileInputHtmlAttributes,
                                             IDictionary<string, object> textInputHtmlAttributes,
                                             string buttonCssClasses)
        {
            if (htmlHelper is null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            var htmlPattern = @"<div class=""file-field input-field"">
    <div class=""{0}"">
        <span>{1}</span>
        {2}
    </div>
    <div class=""file-path-wrapper"">
        {3}
    </div>
</div>";

            var fileInputTagBuilder = new TagBuilder("input");
            fileInputTagBuilder.MergeAttributes(fileInputHtmlAttributes);
            fileInputTagBuilder.MergeAttribute("type", "file");
            fileInputTagBuilder.MergeAttribute("name", formFieldName);
            string inputId = htmlHelper.GenerateIdFromName(formFieldName);
            fileInputTagBuilder.MergeAttribute("id", inputId);
            var renderedFileInput = fileInputTagBuilder.RenderSelfClosingTag();
            var buttonTitle = ResHelper.GetString(buttonResourceKey);
            var textInputTagBuilder = new TagBuilder("input");
            textInputTagBuilder.MergeAttribute("type", "text");
            textInputTagBuilder.AddCssClass("file-path");
            textInputTagBuilder.AddCssClass("validate");
            textInputTagBuilder.MergeAttributes(textInputHtmlAttributes);
            var start = textInputTagBuilder.RenderStartTag();
            var end = textInputTagBuilder.RenderEndTag();
            var renderedTextInput = new HtmlContentBuilder().AppendHtml(start).AppendHtml(end);

            var html = new HtmlContentBuilder()
                .AppendFormat(htmlPattern, buttonCssClasses, buttonTitle, renderedFileInput, renderedTextInput);

            return html;
        }

        public static IHtmlContent Button(this IHtmlHelper htmlHelper, string buttonResourceKey, IDictionary<string, object> htmlAttributes)
        {
            string attributes = default;

            using (var stringWriter = new System.IO.StringWriter())
            {
                foreach (var attribute in htmlAttributes)
                {
                    stringWriter.Write($@"{attribute.Key}=""{attribute.Value}"" ");
                }

                attributes = stringWriter.ToString();
            }

            var startTag = $@"<button type=""button"" {attributes}>";
            var body = ResHelper.GetString(buttonResourceKey);
            var endTag = "</button>";

            var result = htmlHelper.Raw($"{startTag}{body}{endTag}");

            return result;
        }
    }
}
