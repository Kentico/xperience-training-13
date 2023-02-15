using CMS.Base;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;

using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MedioClinic.Components.FormComponents
{
    public class NewsletterSelection : DropDownComponent
    {
        private readonly ISiteService _siteService;

        public NewsletterSelection(ISiteService siteService)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

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
                .OnSite(_siteService.CurrentSite.SiteID)
                .Columns(columns)
                .TypedResult
                .Select(newsletter => new HtmlOptionItem { Value = newsletter.NewsletterGUID.ToString(), Text = newsletter.NewsletterDisplayName });

            return newsletters;
        }
    }
}
