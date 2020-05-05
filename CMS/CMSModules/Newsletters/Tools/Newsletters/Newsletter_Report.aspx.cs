using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.UIControls;
using System.Web;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.Report")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Report : CMSNewsletterPage
{
    private NewsletterInfo mNewsletter;


    /// <summary>
    /// Gets edited Newsletter object.
    /// </summary>
    public NewsletterInfo Newsletter
    {
        get
        {
            if (mNewsletter == null)
            {
                mNewsletter = EditedObject as NewsletterInfo;
            }

            return mNewsletter;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnTabAbsolute.InnerText = GetString("emailmarketing.ui.newsletter.report.tababsolute");
        btnTabPercentages.InnerText = GetString("emailmarketing.ui.newsletter.report.tabpercentages");
        ScriptHelper.RegisterModule(this, "CMS.Newsletter/Report/NewsletterReport", new
        {
            ResourceStrings = new
            {
                Sent = GetString("newsletterissuestatus.finished"),
                Opens = GetString("newsletter.issue.opens"),
                Clicks = GetString("newsletter.issue.clicks"),
                Unsubscribed = GetString("newsletter.issue.unsubscriptions"),
                OpenRate = GetString("newsletters.issueopenrate"),
                ClickRate = GetString("newsletters.issueclickrate"),
                UnsubscribedRate = GetString("newsletters.issueunsubscriberate"),
                ChartZoomButtonText = GetString("emailmarketing.ui.newsletter.report.showall"),
            },
            Parameters = $"newsletterid={Newsletter.NewsletterID}"
        });
    }
}