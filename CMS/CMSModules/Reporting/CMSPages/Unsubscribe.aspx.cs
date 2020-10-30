using System;

using CMS.Core;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Reporting_CMSPages_Unsubscribe : CMSPage
{
    #region "Variables"

    private ReportSubscriptionInfo mReportSubscriptionInfo;
    private ReportInfo mReportInfo;
    private String mEmail = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        mEmail = QueryHelper.GetString(Server.UrlDecode("email"), String.Empty);
        Guid subscriptionGUID = QueryHelper.GetGuid("guid", Guid.Empty);
        mReportSubscriptionInfo = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(subscriptionGUID);
        if (mReportSubscriptionInfo != null)
        {
            mReportInfo = ReportInfoProvider.GetReportInfo(mReportSubscriptionInfo.ReportSubscriptionReportID);
            if (mReportInfo != null)
            {
                var reportDisplayName = Service.Resolve<ILocalizationService>().LocalizeString(mReportInfo.ReportDisplayName);
                // Set info label based by subscription's report
                lblInfo.Text = String.Format(GetString("reportsubscription.unsubscription.info"), HTMLHelper.HTMLEncode(mEmail), HTMLHelper.HTMLEncode(reportDisplayName));
            }
        }
        else
        {
            ShowError(GetString("reportsubscription.notfound"));
            pnlInfo.Visible = false;
            btnUnsubscribe.Enabled = false;
        }

        btnUnsubscribe.Text = GetString("reportsubscription.unsubscribe");
        Title = GetString("reportsubscription.unsubscribe.title");

        PageTitle.TitleText = GetString("reportsubscription.unsubscribe.title");
    }


    protected void btnUnsubscribe_click(object sender, EventArgs ea)
    {
        // Validate email
        if (String.IsNullOrEmpty(mEmail) || !ValidationHelper.IsEmail(mEmail))
        {
            ShowError(GetString("om.contact.enteremail"));
            return;
        }

        // Delete subscription if email is valid
        if (mReportSubscriptionInfo != null)
        {
            if (mReportSubscriptionInfo.ReportSubscriptionEmail.Trim() != mEmail)
            {
                ShowError(GetString("reportsubscription.emailnotmatch"));
                return;
            }

            ReportSubscriptionInfoProvider.DeleteReportSubscriptionInfo(mReportSubscriptionInfo.ReportSubscriptionID);
            ShowInformation(GetString("reportsubscription.unsubscription.success"));
            btnUnsubscribe.Visible = false;
            pnlInfo.Visible = false;

            // Send info about successful unsubscription to set email
            String siteName = SiteContext.CurrentSiteName;
            EmailTemplateInfo eti = EmailTemplateInfo.Provider.Get("Reporting_Unsubscription_template", SiteContext.CurrentSiteID);
            if (eti != null)
            {
                // Create email
                EmailMessage em = new EmailMessage();
                em.EmailFormat = EmailFormatEnum.Default;
                em.From = EmailHelper.Settings.NotificationsSenderAddress(siteName);
                em.Recipients = mReportSubscriptionInfo.ReportSubscriptionEmail;

                MacroResolver resolver = ReportSubscriptionSender.CreateSubscriptionMacroResolver(mReportInfo, mReportSubscriptionInfo, SiteContext.CurrentSite, em.Recipients);
                EmailSender.SendEmailWithTemplateText(siteName, em, eti, resolver, false);
            }
        }
    }

    #endregion
}
