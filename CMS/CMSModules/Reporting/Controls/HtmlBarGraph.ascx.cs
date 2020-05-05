using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CMS.EmailEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Controls_HtmlBarGraph : AbstractReportControl
{
    #region "Variables"

    private ReportGraphInfo mReportGraphInfo;
    private string mParameter = String.Empty;
    private ReportCustomData reportSettings = null;

    private ReportInfo report = null;

    /// <summary>
    /// Indicates whether exception was thrown during data loading
    /// </summary>
    private bool errorOccurred = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the graph datasource.
    /// </summary>
    public DataSet DataSource
    {
        get;
        set;
    }


    /// <summary>
    /// Report HTML graph connection string.
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String graphConn = (ReportGraphInfo == null) ? String.Empty : ReportGraphInfo.GraphConnectionString;
            if (String.IsNullOrEmpty(graphConn))
            {
                return (report == null) ? String.Empty : report.ReportConnectionString;
            }

            return graphConn;
        }
    }


    /// <summary>
    /// Returns graph info from DB or from memory.
    /// </summary>
    public ReportGraphInfo ReportGraphInfo
    {
        get
        {
            // If graph info is not set already
            if (mReportGraphInfo == null)
            {
                mReportGraphInfo = ReportGraphInfoProvider.GetReportGraphInfo(Parameter);
            }
            return mReportGraphInfo;
        }
        set
        {
            mReportGraphInfo = value;
        }
    }


    /// <summary>
    /// Graph name - prevent using viewstate  (problems with displayreportcontrol and postback).
    /// </summary>
    public override string Parameter
    {
        get
        {
            return mParameter;
        }
        set
        {
            mParameter = value;
        }
    }


    /// <summary>
    /// Gets the graph settings for current report, if report is not defined returns an empty object.
    /// </summary>
    private ReportCustomData ReportSettings
    {
        get
        {
            // If graph info is defined, return graph settings
            if (ReportGraphInfo != null)
            {
                return ReportGraphInfo.GraphSettings;
            }

            // Create empty object
            if (reportSettings == null)
            {
                reportSettings = new ReportCustomData();
            }
            return reportSettings;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnPreRender(EventArgs e)
    {
        if (report != null)
        {
            if (ReportGraphInfo != null)
            {
                EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["SubscriptionEnabled"], true) && report.ReportEnableSubscription);
                EnableExport = (EnableExport && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["ExportEnabled"], false));
                // Register context menu for export and subscription - if allowed
                RegisterSubscriptionScript(ReportGraphInfo.GraphReportID, "graphid", ReportGraphInfo.GraphID, menuCont);
            }

            // Export data
            if (!errorOccurred)
            {
                ProcessExport(ValidationHelper.GetCodeName(report.ReportDisplayName));
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    /// <param name="forceLoad">Indicates whether data should be loaded forcibly</param>
    public override void ReloadData(bool forceLoad)
    {
        base.ReloadData(forceLoad);

        if ((GraphImageWidth != 0) && (ComputedWidth == 0))
        {
            // Graph width is computed no need to create graph
            return;
        }

        GetReportGraph(ReportGraphInfo);

        EnsureGraph();
    }


    /// <summary>
    /// Returns true if graph belongs to report.
    /// </summary>
    /// <param name="report">Report to validate</param>
    public override bool IsValid(ReportInfo report)
    {
        ReportGraphInfo rgi = ReportGraphInfo;
        // Test validity
        if ((report != null) && (rgi != null) && (report.ReportID == rgi.GraphReportID))
        {
            return true;
        }

        return false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns report graph.
    /// </summary>
    private void GetReportGraph(ReportGraphInfo reportGraph)
    {
        // Check whether report graph is defined
        if (reportGraph == null)
        {
            return;
        }

        report = ReportInfoProvider.GetReportInfo(reportGraph.GraphReportID);
        if (report == null)
        {
            return;
        }

        // Check graph security settings
        if (!(CheckReportAccess(report) && CheckEmailModeSubscription(report, ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["SubscriptionEnabled"], true))))
        {
            Visible = false;
            return;
        }

        // Prepare query attributes
        QueryText = reportGraph.GraphQuery;
        QueryIsStoredProcedure = reportGraph.GraphQueryIsStoredProcedure;

        // Init parameters
        InitParameters(report.ReportParameters);

        // Init macro resolver
        InitResolver();

        // Indicaates whether exception was throw during data loading
        errorOccurred = false;

        // Create graph data
        try
        {
            // Load data
            DataSource = LoadData();
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Get report graph", "E", ex);
            lblError.Text = ex.Message;
            lblError.Visible = true;
            errorOccurred = true;
        }

        if (DataHelper.DataSourceIsEmpty(DataSource) && EmailMode && SendOnlyNonEmptyDataSource)
        {
            Visible = false;
            return;
        }
    }


    /// <summary>
    /// Ensures graph HTML code.
    /// </summary>
    private void EnsureGraph()
    {
        ItemType = ReportItemType.HtmlGraph;

        // Get html graph content
        string content = Generate();

        // If graph is not defined display info message
        if (String.IsNullOrEmpty(content))
        {
            // Check whether no data text is defiend
            if (!String.IsNullOrEmpty(ReportSettings["QueryNoRecordText"]))
            {
                // Display no data text
                lblInfo.Visible = true;
                lblInfo.Text = HTMLHelper.HTMLEncode(ResolveMacros(ReportSettings["QueryNoRecordText"]));
                EnableExport = false;
            }
        }
        // Display graph
        else
        {
            // Set generated HTML to the literal control
            ltlGraph.Text = content;
        }

        if (EmailMode)
        {
            menuCont.Visible = false;
            ltlEmail.Text = content;
            ltlEmail.Visible = true;
        }
    }


    /// <summary>
    /// Generates graph html code.
    /// </summary>
    private string Generate()
    {
        // Check whether dataset contains at least one row
        if (DataHelper.DataSourceIsEmpty(DataSource))
        {
            if (EmailMode && SendOnlyNonEmptyDataSource)
            {
                Visible = false;
            }

            return String.Empty;
        }

        // Do not generate graph for email and plain text
        if (EmailMode)
        {
            EmailFormatEnum format = EmailHelper.GetEmailFormat(ReportSubscriptionSiteID);
            if (format == EmailFormatEnum.PlainText)
            {
                return GetString("subscription.htmlnotsupportedforplain");
            }
        }

        #region "Max & sum computing"

        // Find max value and sum from current dataset
        double max = 0.0;
        double sum = 0.0;
        // Loop thru all data rows
        foreach (DataRow dr in DataSource.Tables[0].Rows)
        {
            // Loop thru all columns
            foreach (DataColumn dc in DataSource.Tables[0].Columns)
            {
                // Skip first column with data name
                if (dc.Ordinal > 0)
                {
                    // Get column value
                    double value = ValidationHelper.GetDouble(dr[dc.ColumnName], 0.0);
                    sum += value;
                    // Set max value from current value if is higher than current max value
                    if (max < value)
                    {
                        max = value;
                    }
                }
            }
        }

        #endregion

        // Initialize string builder
        StringBuilder sb = new StringBuilder(1024);
        sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"0\" class=\"ReportBarGraphTable\">");

        bool displayLegend = ValidationHelper.GetBoolean(ReportSettings["DisplayLegend"], false);
        bool headerDisplayed = !String.IsNullOrEmpty(ReportGraphInfo.GraphTitle) || displayLegend;

        #region "Legend/Title row"

        if (headerDisplayed)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine("<td class=\"ReportBarGraphLegend\" colspan=\"2\">");

            if (EmailMode)
            {
                sb.AppendLine("<table width=\"100%\"><tr><td style=\"width:100%\">");
                sb.AppendLine("<div class=\"ReportBarGraphTitle\">" + HTMLHelper.HTMLEncode(ResolveMacros(ReportGraphInfo.GraphTitle)) + "</div></td>");

                string legendText = HTMLHelper.HTMLEncode(ReportSettings["LegendTitle"]);
                if (!String.IsNullOrEmpty(legendText))
                {
                    legendText += ":";
                    sb.AppendLine("<td>" + legendText + "</td>");
                }

                // Loop thru all columns in reverse order due to similarity with graph representation
                for (int i = DataSource.Tables[0].Columns.Count - 1; i > 0; i--)
                {
                    DataColumn dc = DataSource.Tables[0].Columns[i];
                    string backColorClass = GetColorClass(i);

                    sb.AppendLine("<td nowrap=\"nowrap\"><div class=\"ReportBarGraphLegendItemEnvelope\"> ");
                    sb.AppendLine("<table><tr><td><div class=\"" + backColorClass + "\">&nbsp;&nbsp;&nbsp;</div></td><td nowrap=\"nowrap\"> ");
                    sb.AppendLine(HTMLHelper.HTMLEncode(dc.ColumnName));
                    sb.AppendLine("</td><td>&nbsp;&nbsp;</td></tr></table></div></td>");
                }

                sb.AppendLine("</tr></table>");
            }
            else
            {
                // Graph title
                sb.AppendLine("<div class=\"ReportBarGraphTitle FloatLeft\">" + HTMLHelper.HTMLEncode(ResolveMacros(ReportGraphInfo.GraphTitle)) + "</div>");

                // Check whether legend should be displayed
                if (displayLegend)
                {
                    // Loop thru all columns in reverse order due to similarity with graph representation
                    for (int i = DataSource.Tables[0].Columns.Count - 1; i > 0; i--)
                    {
                        DataColumn dc = DataSource.Tables[0].Columns[i];
                        string backColorClass = GetColorClass(i);

                        sb.AppendLine("<div class=\"FloatRight ReportBarGraphLegendItemEnvelope\"> ");
                        sb.AppendLine("<div class=\"FloatLeft ReportBarGraphLegendItem " + backColorClass + " \">&nbsp;</div> ");
                        sb.AppendLine(HTMLHelper.HTMLEncode(dc.ColumnName));
                        sb.AppendLine("</div>");
                    }

                    string legendText = HTMLHelper.HTMLEncode(ReportSettings["LegendTitle"]);
                    if (!String.IsNullOrEmpty(legendText))
                    {
                        legendText += ":";
                    }

                    sb.AppendLine("<div class=\"FloatRight ReportBarGraphLegendTitle\">" + legendText + "</div>");
                }
            }

            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
        }

        #endregion

        string rowClass = "ReportBarGraphNameCellFirst";
        string itemValueFormat = ReportSettings["ItemValueFormat"];

        // Loop thru all data rows
        for (int i = DataSource.Tables[0].Rows.Count - 1; i >= 0; i--)
        {
            // Get current datarow
            DataRow dr = DataSource.Tables[0].Rows[i];
            sb.AppendLine("<tr>");

            // Loop thru all columns in current dataset
            foreach (DataColumn dc in DataSource.Tables[0].Columns)
            {
                // Generate data name cell
                if (dc.Ordinal == 0)
                {
                    sb.AppendLine("<td class=\"ReportBarGraphNameCell " + rowClass + "\">");

                    // Get column name
                    string name = dr[dc.ColumnName].ToString();
                    // Get 
                    string format = ReportSettings["SeriesItemNameFormat"];

                    #region "Item name formating"

                    // Check whether format is defined
                    if (!String.IsNullOrEmpty(format))
                    {
                        // Convert to specific type
                        if (ValidationHelper.IsDateTime(name))
                        {
                            DateTime dt = ValidationHelper.GetDateTime(name, DateTimeHelper.ZERO_TIME);
                            name = dt.ToString(format);
                        }
                        else
                        {
                            name = String.Format(name, format);
                        }
                    }

                    #endregion

                    // Name value
                    sb.AppendLine(HTMLHelper.HTMLEncode(name));
                    sb.AppendLine("</td>");
                }
                else
                {
                    // Generate open cell tag
                    if (dc.Ordinal == 1)
                    {
                        sb.AppendLine("<td class=\"ReportBarGraphDataCell " + rowClass + "\">");
                    }
                    // Generate cell data divider
                    else
                    {
                        sb.AppendLine("<div style=\"clear:both\"></div>");
                    }

                    // Default width type
                    string widthType = "%";
                    // Default width value
                    double widthValue = 0.0;
                    // Current value
                    double currentValue = ValidationHelper.GetDouble(dr[dc.ColumnName], 0.0);

                    // If current value is defined compute reltive width
                    if (currentValue > 0.0)
                    {
                        // Relative width
                        widthValue = (currentValue / max) * 80;

                        // If value is defined but relative width is lower than 1, generate simple line
                        if (widthValue < 1)
                        {
                            widthValue = 1;
                            widthType = "px";
                        }

                        // Get background color for current column
                        string backColorClass = GetColorClass(dc.Ordinal);
                        // Item link
                        string itemLink = ReportSettings["SeriesItemLink"];
                        // Item tooltip
                        string itemToolTip = ReportSettings["SeriesItemToolTip"];

                        if (!EmailMode)
                        {
                            if (!String.IsNullOrEmpty(itemLink))
                            {
                                sb.AppendLine("<a href=\"" + ResolveUrl(ResolveCustomMacros(itemLink, dr, dc.ColumnName, sum)) + "\">");
                            }

                            if (!String.IsNullOrEmpty(itemToolTip))
                            {
                                itemToolTip = " title=\"" + ResolveCustomMacros(itemToolTip.Replace("\"", String.Empty), dr, dc.ColumnName, sum) + "\"";
                            }

                            // <DIV> - bar
                            sb.AppendLine(@"<div " + itemToolTip + @" class=""FloatLeft ReportBarGraphDataItem " + backColorClass + @""" style="" width:" + (ValidationHelper.GetInteger(Math.Truncate(widthValue), 0)).ToString() + widthType + @""">&nbsp;</div>");

                            // Check whether item value should be displayed
                            if (!String.IsNullOrEmpty(itemValueFormat))
                            {
                                string itemValue = ResolveCustomMacros(itemValueFormat, dr, dc.ColumnName, sum);
                                // <DIV> - item value
                                sb.AppendLine(@"<div " + itemToolTip + @" class=""FloatLeft ReportBarGraphDataItemValue"">" + itemValue + "</div>");
                            }

                            if (!String.IsNullOrEmpty(itemLink))
                            {
                                sb.AppendLine("</a>");
                            }
                        }
                        else
                        {
                            sb.AppendLine(@"<table width=""100%""><tr><td class=""" + backColorClass + @""" style=""width:" + (ValidationHelper.GetInteger(Math.Truncate(widthValue), 0)).ToString() + widthType + @"""></td>");

                            // Check whether item value should be displayed
                            if (!String.IsNullOrEmpty(itemValueFormat))
                            {
                                string itemValue = ResolveCustomMacros(itemValueFormat, dr, dc.ColumnName, sum);
                                sb.AppendLine("<td width=\"20px\" class=\"ReportBarGraphDataItemValue\" >" + itemValue + "</td>");
                            }

                            sb.AppendLine("</tr></table>");
                        }
                    }
                    else
                    {
                        sb.AppendLine(@"<div class=""FloatLeft ReportBarGraphDataItem"" style=""width:1px"">&nbsp;</div>");
                    }
                }
            }

            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            // Clear first row class
            rowClass = String.Empty;
        }

        sb.AppendLine("</table>");

        return sb.ToString();
    }


    /// <summary>
    /// Resolve custom macros for specified input value.
    /// </summary>
    /// <param name="value">Value to resolve</param>
    /// <param name="dr">Current data row</param>
    /// <param name="columnName">Column name</param>
    /// <param name="sum">Summary of all items</param>
    private string ResolveCustomMacros(string value, DataRow dr, string columnName, double sum)
    {
        // Get current item value
        double itemvalue = ValidationHelper.GetDouble(dr[columnName], 0.0);

        // Set custom macros
        ContextResolver.SetNamedSourceData(new Dictionary<string, object>
        {
            { "xval", Convert.ToString(dr[0]) },
            { "yval", Convert.ToString(itemvalue) },
            { "ser", columnName },
            { "pval", Convert.ToString(itemvalue / sum * 100) }
        }, isPrioritized: false);

        // Resolve macros
        return ResolveMacros(value);
    }


    /// <summary>
    /// Returns html color value for specific position.
    /// </summary>
    /// <param name="position">Column position</param>
    private string GetColorClass(int position)
    {
        String css = EmailMode ? String.Empty : "ReportBarGraphItem ";
        return css + "ReportBarGraphItem" + position;
    }

    #endregion
}