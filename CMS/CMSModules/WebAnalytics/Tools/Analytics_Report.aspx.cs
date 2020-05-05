using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.WebAnalytics.Web.UI;


public partial class CMSModules_WebAnalytics_Tools_Analytics_Report : CMSWebAnalyticsPage
{
    private bool mIsSaved;
    private string mStatCodeName;
    

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check analytics UI
        CheckWebAnalyticsUI();

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        CurrentMaster.PanelContent.CssClass = "";
        ScriptHelper.RegisterDialogScript(Page);

        // Set disabled module info
        ucDisabledModule.ParentPanel = pnlDisabled;

        UIHelper.AllowUpdateProgress = false;

        mStatCodeName = QueryHelper.GetString("statCodeName", String.Empty);
        var reportCodeName = QueryHelper.GetString("reportCodeName", String.Empty);
        var dataCodeName = QueryHelper.GetText("dataCodeName", String.Empty);

        ucReportViewer.DataName = dataCodeName;
        ucReportViewer.ReportsCodeName = reportCodeName;

        var displayTitle = QueryHelper.GetBoolean("DisplayTitle", true);
        if (displayTitle)
        {
            PageTitle.TitleText = GetString("analytics_codename." + HTMLHelper.HTMLEncode(mStatCodeName));
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                Save();
                break;
        }
    }


    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!mIsSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ucReportViewer.DisplayReport(false);
        reportHeader.ReportName = ucReportViewer.ReportName;
        reportHeader.ReportParameters = ucReportViewer.ReportParameters;
        reportHeader.SelectedInterval = ucReportViewer.SelectedInterval;
        base.OnPreRender(e);
    }

    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        // Check web analytics save permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "SaveReports"))
        {
            RedirectToAccessDenied("CMS.WebAnalytics", "SaveReports");
        }

        mIsSaved = true;

        // Saves the report 
        ucReportViewer.SaveReport();

        mIsSaved = false;
    }
}
