using System;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Reporting.Web.UI;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Controls_AnalyticsReportViewer : CMSAdminControl
{
    #region "Variables"

    private bool mLoadFormParameters;


    /// <summary>
    /// Display control should be loaded only once.
    /// </summary>
    private bool mReportLoaded;


    /// <summary>
    /// Control for display report (separation) problem
    /// </summary>
    private IDisplayReport mUcDisplayReport;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Report name to show.
    /// </summary>
    public string ReportsCodeName
    {
        get;
        set;
    }


    /// <summary>
    /// Data report name.
    /// </summary>
    public string DataName
    {
        get;
        set;
    }


    /// <summary>
    /// If true, report form parameters are loaded,form filter is displayed,period and graph type selector is hidden.
    /// </summary>
    public bool LoadFormParameters
    {
        get
        {
            return mLoadFormParameters;
        }
        set
        {
            EnsureDisplayReport();
            mLoadFormParameters = value;
            mUcDisplayReport.LoadFormParameters = value;
            mUcDisplayReport.DisplayFilter = value;
            pnlPeriodSelectors.Visible = !value;
        }
    }


    /// <summary>
    /// Css class for display report
    /// </summary>
    public string DisplayReportBodyClass
    {
        get
        {
            EnsureDisplayReport();
            return mUcDisplayReport.BodyCssClass;
        }
        set
        {
            EnsureDisplayReport();
            mUcDisplayReport.BodyCssClass = value;
        }
    }


    /// <summary>
    /// Start of shown period.
    /// </summary>
    public DateTime From
    {
        get
        {
            return ucGraphTypePeriod.From;
        }
    }


    /// <summary>
    /// End of show period.
    /// </summary>
    public DateTime To
    {
        get
        {
            return ucGraphTypePeriod.To;
        }
    }


    /// <summary>
    /// Display name of shown report.
    /// </summary>
    public string ReportDisplayName
    {
        get
        {
            EnsureDisplayReport();
            return mUcDisplayReport.ReportDisplayName;
        }
    }


    /// <summary>
    /// Name of single report (f.e. pageviews.monthreport).
    /// </summary>
    public string ReportName
    {
        get
        {
            EnsureDisplayReport();
            if (!String.IsNullOrEmpty(mUcDisplayReport.ReportName))
            {
                return mUcDisplayReport.ReportName;
            }

            return ucGraphTypePeriod.GetReportName(ReportsCodeName);
        }
        set
        {
            EnsureDisplayReport();
            mUcDisplayReport.ReportName = value;
        }
    }


    /// <summary>
    /// Get selected interval of control (year,month,..)
    /// </summary>
    public HitsIntervalEnum SelectedInterval
    {
        get
        {
            return ucGraphTypePeriod.SelectedInterval;
        }
    }


    /// <summary>
    /// Returns report's parameters
    /// </summary>
    public DataRow ReportParameters
    {
        get
        {
            return mUcDisplayReport.ReportParameters;
        }
    }

    #endregion


    #region "Methods"

    private void EnsureDisplayReport()
    {
        if (mUcDisplayReport == null)
        {
            mUcDisplayReport = (IDisplayReport)LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
            divGraphArea.Controls.Add((Control)mUcDisplayReport);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        EnsureDisplayReport();
        if (mUcDisplayReport != null)
        {
            mUcDisplayReport.RenderCssClasses = true;
        }

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        DisplayReport(false);
    }


    /// <summary>
    /// Extracts report code name from report code names list, based on interval selection. Returns single codename for no interval reports.
    /// </summary>
    /// <param name="reportCodeName">Report code name list (delimited by semicolons)</param>
    public string GetReportCodeName(string reportCodeName)
    {
        return ucGraphTypePeriod.GetReportName(reportCodeName);
    }


    /// <summary>
    /// Display report with given criteria.
    /// </summary>
    public void DisplayReport(bool intervalChanged)
    {
        // If load form parameters are loaded display report calls reload on onInit (time charts not allowed)
        if (LoadFormParameters)
        {
            return;
        }

        if (mReportLoaded)
        {
            return;
        }

        if (mUcDisplayReport == null)
        {
            return;
        }

        ucGraphTypePeriod.ProcessChartSelectors(intervalChanged);

        if (pnlPeriodSelectors.Visible && !IsValidInterval())
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }

        mUcDisplayReport.ReportName = ucGraphTypePeriod.GetReportName(ReportsCodeName);
        mUcDisplayReport.GraphImageWidth = 100;
        mUcDisplayReport.IgnoreWasInit = true;

        if (pnlPeriodSelectors.Visible)
        {
            // Prepare report parameters
            DataTable dtp = new DataTable();

            dtp.Columns.Add("FromDate", typeof(DateTime));
            dtp.Columns.Add("ToDate", typeof(DateTime));
            dtp.Columns.Add("CodeName", typeof(string));
            dtp.Columns.Add("FirstCategory", typeof(string));
            dtp.Columns.Add("SecondCategory", typeof(string));
            dtp.Columns.Add("Direct", typeof(string));
            dtp.Columns.Add("Search", typeof(string));
            dtp.Columns.Add("Referring", typeof(string));

            object[] parameters = new object[8];

            parameters[0] = ucGraphTypePeriod.From;
            parameters[1] = ucGraphTypePeriod.To;
            parameters[2] = DataName;
            parameters[3] = StatisticsNames.VISITORS_FIRST;
            parameters[4] = StatisticsNames.VISITORS_RETURNING;
            parameters[5] = StatisticsNames.REFERRINGSITE + "_direct";
            parameters[6] = StatisticsNames.REFERRINGSITE + "_search";
            parameters[7] = StatisticsNames.REFERRINGSITE + "_referring";

            dtp.Rows.Add(parameters);
            dtp.AcceptChanges();

            if (!mUcDisplayReport.IsReportLoaded())
            {
                ShowError(String.Format(GetString("Analytics_Report.ReportDoesnotExist"), mUcDisplayReport.ReportName));
            }
            else
            {
                mUcDisplayReport.LoadFormParameters = false;
                mUcDisplayReport.DisplayFilter = false;
                mUcDisplayReport.ReportParameters = dtp.Rows[0];
                mUcDisplayReport.UseExternalReload = true;
                mUcDisplayReport.UseProgressIndicator = true;
                mUcDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(SelectedInterval);
            }
        }

        mUcDisplayReport.ReloadData(true);
        mReportLoaded = true;
    }


    /// <summary>
    /// Returns true if selected interval is valid.
    /// </summary>
    private bool IsValidInterval()
    {
        var from = ucGraphTypePeriod.From;
        var to = ucGraphTypePeriod.To;

        if ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME))
        {
            return false;
        }

        return from <= to;
    }


    /// <summary>
    /// Saves current report.
    /// </summary>
    public void SaveReport()
    {
        if (mUcDisplayReport != null)
        {
            DisplayReport(false);

            // Check 'SaveReports' permission
            if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
            {
                RedirectToAccessDenied("cms.reporting", "SaveReports");
            }
            
            // Display info label
            if (mUcDisplayReport.SaveReport() > 0)
            {
                ShowConfirmation(String.Format(GetString("Ecommerce_Report.ReportSavedTo"), ReportDisplayName + " - " + DateTime.Now.ToString()));
            }
            else
            {
                ShowError(GetString("reporting.savingreportfailed"));
            }
        }
    }


    /// <summary>
    /// Generates query string parameters.
    /// </summary>
    public string GetQueryStringParameters()
    {
        if (mUcDisplayReport != null)
        {
            return ReportUIHelper.GetQueryStringParameters(mUcDisplayReport.ReportParameters);
        }

        return String.Empty;
    }

    #endregion
}
