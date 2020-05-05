using System;
using System.Data;

using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Reporting_Tools_Ecommerce_Report_Print : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (QueryHelper.ValidateHash("hash", "UILang"))
        {
            // Get report name
            string reportName = QueryHelper.GetString("reportName", null);

            // Check permissions
            bool isEcommerceReport = ReportInfoProvider.IsEcommerceReport(reportName);
            CMSEcommerceReportsPage.CheckPermissions(isEcommerceReport);

            ReportInfo report = ReportInfoProvider.GetReportInfo(reportName);
            if (report != null)
            {
                // Get report parameters
                string parameters = QueryHelper.GetString("parameters", String.Empty);
                DataRow reportParameters = ReportHelper.GetReportParameters(report, parameters, null, CultureHelper.EnglishCulture);

                // Init report
                if (reportParameters != null)
                {
                    DisplayReport1.LoadFormParameters = false;
                    DisplayReport1.ReportParameters = reportParameters;
                }

                DisplayReport1.ReportName = reportName;
                DisplayReport1.DisplayFilter = false;

                Page.Title = GetString("report_print.lblprintreport") + " " + HTMLHelper.HTMLEncode(report.ReportDisplayName);
            }
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ManagersContainer = pnlManager;
    }
}