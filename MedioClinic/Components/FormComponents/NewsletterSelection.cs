using CMS.Helpers;
using CMS.Newsletters;

using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MedioClinic.Components.FormComponents
{
    public class NewsletterSelection : DropDownComponent
    {
        public override void LoadProperties(FormComponentProperties properties)
        {
            base.LoadProperties(properties);

            Properties.OptionLabel = ResHelper.GetString("OnlineMarketing.SelectNewsletter");
        }

        protected override IEnumerable<HtmlOptionItem> GetHtmlOptions()
        {
            var columns = new List<string>() { NewsletterInfo.TYPEINFO.GUIDColumn, NewsletterInfo.TYPEINFO.DisplayNameColumn };

            var newsletters = NewsletterInfo
                .Provider
                .Get()
                .Columns(columns)
                .TypedResult
                .Select(newsletter => new HtmlOptionItem { Value = newsletter.NewsletterGUID.ToString(), Text = newsletter.NewsletterDisplayName });

            return newsletters;
        }
    }
}
