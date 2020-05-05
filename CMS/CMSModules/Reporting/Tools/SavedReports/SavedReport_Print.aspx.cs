using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Tools_SavedReports_SavedReport_Print : CMSReportingModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var user = MembershipContext.AuthenticatedUser;

        int reportId = QueryHelper.GetInteger("reportid", 0);
        SavedReportInfo sri = SavedReportInfoProvider.GetSavedReportInfo(reportId);
        if (sri != null)
        {
            ltlHtml.Text = HTMLHelper.ResolveUrls(sri.SavedReportHTML, ResolveUrl("~/"));
        }
    }
}