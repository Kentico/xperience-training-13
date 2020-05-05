using System;
using System.Data;

using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Tools_Report_Print : CMSReportingModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        
        // Get report info
        int reportId = QueryHelper.GetInteger("reportid", 0);
        ReportInfo report = ReportInfoProvider.GetReportInfo(reportId);

        if (report != null)
        {
            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", string.Empty);
            DataRow reportParameters = ReportHelper.GetReportParameters(report, parameters, null, CultureHelper.EnglishCulture);

            // Init report
            if (reportParameters != null)
            {
                DisplayReport.LoadFormParameters = false;
                DisplayReport.ReportParameters = reportParameters;
            }

            DisplayReport.ReportName = report.ReportName;
            DisplayReport.DisplayFilter = false;

            Page.Title = GetString("Report_Print.lblPrintReport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ManagersContainer = pnlManager;
    }
}