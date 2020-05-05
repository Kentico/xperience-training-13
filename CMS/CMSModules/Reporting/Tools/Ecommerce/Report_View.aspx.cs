using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Tools_Ecommerce_Report_View : CMSEcommerceReportsPage
{
    private bool isSaved = false;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ManagersContainer = plcContainerManager;

        // For purposes of IsEcommerceReport its sufficient to get report name of any period (year,month,week)
        string reportCodeName = QueryHelper.GetString("reportCodeName", String.Empty);
        string name = ucReportViewer.GetReportCodeName(reportCodeName);

        // No interval report (only one report in url - pass it now
        if (!reportCodeName.Contains(";"))
        {
            ucReportViewer.ReportName = name;
        }

        IsEcommerceReport = ReportInfoProvider.IsEcommerceReport(name);

        // To display filter form control (basic form) need to pass report name before control's init
        // To set fields before basic form control state fills them with actual data
        bool displayFilter = QueryHelper.GetBoolean("displayFilter", false);
        if (displayFilter)
        {
            ucReportViewer.LoadFormParameters = true;
            ucReportViewer.DisplayReportBodyClass = "DisplayReportBody";
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        string reportCodeName = QueryHelper.GetString("reportcodename", String.Empty);
        string dataCodeName = QueryHelper.GetString("datacodename", String.Empty);
        string statCodeName = QueryHelper.GetString("statcodename", String.Empty);

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        // Check ui personalization
        String uiName = "ecreports." + statCodeName;
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, uiName);

        ucReportViewer.DataName = dataCodeName;
        ucReportViewer.ReportsCodeName = reportCodeName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ucReportViewer.DisplayReport(false);

        reportHeader.ReportName = ucReportViewer.ReportName;
        reportHeader.ReportParameters = ucReportViewer.ReportParameters;
        reportHeader.SelectedInterval = ucReportViewer.SelectedInterval;
        
        RegisterModalDialogScript();
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                isSaved = true;

                // Save report - info label displays AnalyticsReportViewer control
                ucReportViewer.SaveReport();
                isSaved = false;
                break;
        }
    }


    /// <summary>
    /// VerifyRenderingInServerForm.
    /// </summary>
    /// <param name="control">Control</param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!isSaved)
        {
            base.VerifyRenderingInServerForm(control);
        }
    }


    /// <summary>
    /// Handles lnkSave click event.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        isSaved = true;

        // Save report - info label displays AnalyticsReportViewer control
        ucReportViewer.SaveReport();

        isSaved = false;
    }
}