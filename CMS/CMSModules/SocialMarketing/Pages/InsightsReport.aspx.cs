using System;
using System.Data;
using System.Web.UI;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Reporting.Web.UI;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_SocialMarketing_Pages_InsightsReport : CMSAdministrationPage
{

    #region "Properties"

    /// <summary>
    /// Gets a semicolon separated list of code names of reports that will be displayed.
    /// </summary>
    /// <remarks>
    /// The report code names are specified in descending order from yearly to hourly report.
    /// </remarks>
    private string ReportCodeNames
    {
        get
        {
            return QueryHelper.GetString("reportCodeNames", String.Empty);
        }
    }


    /// <summary>
    /// Gets the insight period type.
    /// </summary>
    private string PeriodType
    {
        get
        {
            return QueryHelper.GetString("periodType", String.Empty);
        }
    }


    /// <summary>
    /// Gets the external Facebook page or post identifier.
    /// </summary>
    private string ExternalId
    {
        get
        {
            return QueryHelper.GetString("externalId", String.Empty);
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        CheckLicense(FeatureEnum.SocialMarketingInsights);
        CheckPermissions(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport();
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Displays the report using the current graph type.
    /// </summary>
    private void DisplayReport()
    {
        // Create control to display the specified report
        Control control = LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
        IDisplayReport ucDisplayReport = control as IDisplayReport;
        if (ucDisplayReport == null)
        {
            return;
        }
        pnlDisplayReport.Controls.Add(control);

        // Social media insights do not provide hourly reports
        ucGraphType.VisibleGraphTypes = "year;month;week;day";
        ucGraphType.ProcessChartSelectors(false);

        // Choose a report using the current graph type
        string reportName = ucGraphType.GetReportName(ReportCodeNames);
        ucDisplayReport.ReportName = reportName;
        if (!ucDisplayReport.IsReportLoaded())
        {
            ShowError(String.Format(GetString("Analytics_Report.ReportDoesnotExist"), HTMLHelper.HTMLEncode(reportName)));
        }
        else
        {
            ucDisplayReport.LoadFormParameters = false;
            ucDisplayReport.DisplayFilter = false;
            ucDisplayReport.ReportParameters = CreateReportParameters();
            ucDisplayReport.GraphImageWidth = 100;
            ucDisplayReport.IgnoreWasInit = true;
            ucDisplayReport.UseExternalReload = true;
            ucDisplayReport.UseProgressIndicator = true;
            ucDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);

            ucDisplayReport.ReloadData(true);
        }
    }


    /// <summary>
    /// Creates parameters for the report, and returns them.
    /// </summary>
    /// <returns>Parameters for the report.</returns>
    private DataRow CreateReportParameters()
    {
        DataTable table = new DataTable();

        table.Columns.Add("FromDate", typeof(DateTime));
        table.Columns.Add("ToDate", typeof(DateTime));
        table.Columns.Add("ExternalID", typeof(string));
        table.Columns.Add("PeriodType", typeof(string));

        object[] parameters = { ucGraphType.From, ucGraphType.To, ExternalId, PeriodType };
        table.Rows.Add(parameters);
        table.AcceptChanges();

        return table.Rows[0];
    }

    #endregion

}
