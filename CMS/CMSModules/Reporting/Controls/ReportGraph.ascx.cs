using System;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Reporting;
using CMS.Reporting.Web.UI;


public partial class CMSModules_Reporting_Controls_ReportGraph : AbstractReportControl
{
    #region "Variables"

    private string mReportGraphName = String.Empty;
    private string mParameter = String.Empty;
    private bool mRegisterWidthScript;
    private string mWidth = String.Empty;
    private ReportInfo mReport;


    /// <summary>
    /// Indicates whether exception was thrown during data loading
    /// </summary>
    private bool mErrorOccurred;


    /// <summary>
    /// Store info object loaded from DB - prevents from double load.
    /// </summary>
    private ReportGraphInfo mGraphInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns graph info from DB or from memory.
    /// </summary>
    private ReportGraphInfo ReportGraphInfo
    {
        get
        {
            // This is used for preview or direct set of info without using DB
            if (GraphInfo != null)
            {
                return GraphInfo;
            }

            // If graph info is not set already
            return mGraphInfo ?? (mGraphInfo = ReportGraphInfoProvider.GetReportGraphInfo(Parameter));
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
    /// Report graph connection string
    /// </summary>
    public override string ConnectionString
    {
        get
        {
            String graphConn = (ReportGraphInfo == null) ? String.Empty : ReportGraphInfo.GraphConnectionString;
            if (String.IsNullOrEmpty(graphConn))
            {
                return (mReport == null) ? String.Empty : mReport.ReportConnectionString;
            }

            return graphConn;
        }
    }


    /// <summary>
    /// If true, graph is render for email purpose.
    /// </summary>
    public override bool EmailMode
    {
        get
        {
            return base.EmailMode;
        }
        set
        {
            graphDiv.Visible = !value;
            pnlImage.Visible = value;
            base.EmailMode = value;
        }
    }


    /// <summary>
    /// Graph name.
    /// </summary>
    private string ReportGraphName
    {
        get
        {
            return mReportGraphName;
        }
        set
        {
            mReportGraphName = value;
        }
    }


    /// <summary>
    /// If set graphinfo will not be loaded from database.
    /// </summary>
    public ReportGraphInfo GraphInfo
    {
        get;
        set;
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


    /// <summary>
    /// Gets or sets width of graph. If graph contains '%', width is relative to max possible width.
    /// </summary>
    public String Width
    {
        get
        {
            return mWidth;
        }
        set
        {
            mWidth = value;
            if (mWidth.Contains("%"))
            {
                string pureWidth = mWidth.Replace('%', ' ');
                int val = ValidationHelper.GetInteger(pureWidth.Trim(), 0);
                if (val != 0)
                {
                    GraphImageWidth = val;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets height of graph.
    /// </summary>
    public int Height
    {
        get;
        set;
    }

    #endregion


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        ItemType = ReportItemType.Graph;

        // If hidden field contains information about width - add to request for multiple charts on one page
        int width = ValidationHelper.GetInteger(Request.Params[hdnValues.UniqueID], 0);
        if (RequestHelper.IsPostBack() && (width != 0))
        {
            // Fix the position to page with slider
            ComputedWidth = width - 17;
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// OnPreRender handler - register progress script.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (mReport != null)
        {
            if (ReportGraphInfo != null)
            {
                EnableSubscription = (EnableSubscription && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["SubscriptionEnabled"], true) && mReport.ReportEnableSubscription);
                EnableExport = (EnableExport && ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["ExportEnabled"], false));
                // Register context menu for export - if allowed
                RegisterSubscriptionScript(ReportGraphInfo.GraphReportID, "graphid", ReportGraphInfo.GraphID, menuCont);
            }

            if (!mErrorOccurred)
            {
                ProcessExport(ValidationHelper.GetCodeName(mReport.ReportDisplayName));
            }
        }

        if (mRegisterWidthScript)
        {
            string script = ControlsHelper.GetPostBackEventReference(btnRefresh, "refresh");
            // Count width of div and postback
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ReportGraphReloader",
                                               ScriptHelper.GetScript(@"var graphDivObject = document.getElementById('" + graphDiv.ClientID + @"'); document.getElementById('" + hdnValues.ClientID + "').value = graphDivObject.offsetWidth;" + script));
        }

        base.OnPreRender(e);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        try
        {
            base.Render(writer);
        }
        catch (Exception e)
        {
            Service.Resolve<IEventLogService>().LogException("Get report graph", "E", e);
            lblError.Visible = true;
            lblError.Text = GetString("Reporting.Graph.InvalidDataGraph");
            lblError.RenderControl(writer);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        // If percent Width is Set and reloadonPrerender == false  - it means first run .. ad postback scripts and exit
        if ((GraphImageWidth != 0) && (ComputedWidth == 0))
        {
            string argument = Request.Params[Page.postEventArgumentID];
            string target = Request.Params[Page.postEventSourceID];

            // Check for empty (invisible) div to prevent neverending loop
            // If refresh postback and still no computedwidth - display graph with default width
            if (!((target == btnRefresh.UniqueID) && (argument == "refresh")))
            {
                mRegisterWidthScript = true;
                ucChart.Visible = false;
                RequestStockHelper.Add("CMSGraphAutoWidth", true);
                return;
            }
        }

        RequestStockHelper.Add("CMSGraphAutoWidth", false);
        mRegisterWidthScript = false;

        // ReportGraphName is set from AbstractReportControl parameter
        ReportGraphName = Parameter;

        // Preview
        if (GraphInfo != null)
        {
            GetReportGraph(GraphInfo);
        }
        else
        {
            ReportGraphInfo rgi = ReportGraphInfo;
            // If saved report guid is empty ==> create "live" graph, else load saved graph
            if (SavedReportGuid == Guid.Empty)
            {
                // Get graph info object
                if (rgi != null)
                {
                    GetReportGraph(rgi);
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Report graph '" + ReportGraphName + "' not found.";
                    Service.Resolve<IEventLogService>().LogException("Report graph", "E", new Exception("Report graph '" + ReportGraphName + "' not found."));
                }
            }
            else
            {
                if (rgi != null)
                {
                    int correctWidth = 0;
                    if (ComputedWidth != 0)
                    {
                        correctWidth = GetGraphWidth();
                    }

                    rgi.GraphTitle = ResHelper.LocalizeString(rgi.GraphTitle);
                    rgi.GraphXAxisTitle = ResHelper.LocalizeString(rgi.GraphXAxisTitle);
                    rgi.GraphYAxisTitle = ResHelper.LocalizeString(rgi.GraphYAxisTitle);

                    QueryIsStoredProcedure = rgi.GraphQueryIsStoredProcedure;
                    QueryText = ResolveMacros(rgi.GraphQuery);

                    // Load data, generate image
                    ReportGraph rg = new ReportGraph() { Colors = Colors };

                    // Save image
                    SavedGraphInfo sgi = new SavedGraphInfo();

                    string noRecordText = ResolveMacros(ValidationHelper.GetString(rgi.GraphSettings["QueryNoRecordText"], String.Empty));

                    try
                    {
                        rg.CorrectWidth = correctWidth;
                        DataSet ds = LoadData();
                        if (!DataHelper.DataSourceIsEmpty(ds) || !String.IsNullOrEmpty(noRecordText))
                        {
                            sgi.SavedGraphBinary = rg.CreateChart(rgi, ds, ContextResolver, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Service.Resolve<IEventLogService>().LogException("Report graph", "E", ex);

                        rg = new ReportGraph { Colors = Colors };
                        byte[] invalidGraph = rg.CreateChart(rgi, null, ContextResolver, true);
                        sgi.SavedGraphBinary = invalidGraph;
                    }

                    if (sgi.SavedGraphBinary != null)
                    {
                        sgi.SavedGraphGUID = SavedReportGuid;
                        sgi.SavedGraphMimeType = "image/png";
                        sgi.SavedGraphSavedReportID = SavedReportID;

                        SavedGraphInfoProvider.SetSavedGraphInfo(sgi);

                        // Create graph image
                        imgGraph.Visible = true;
                        ucChart.Visible = false;
                        imgGraph.ImageUrl = ResolveUrl("~/CMSModules/Reporting/CMSPages/GetReportGraph.aspx") + "?graphguid=" + SavedReportGuid.ToString();
                    }
                    else
                    {
                        Visible = false;
                    }
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns report graph.
    /// </summary>
    private void GetReportGraph(ReportGraphInfo reportGraph)
    {
        Visible = true;
        ucChart.Visible = true;

        int correctWidth = 0;
        if (ComputedWidth != 0)
        {
            correctWidth = GetGraphWidth();
        }

        if (Width != String.Empty)
        {
            int graphWidth = ValidationHelper.GetInteger(Width, 0);
            if (graphWidth != 0)
            {
                reportGraph.GraphWidth = graphWidth;
            }
        }

        if (Height != 0)
        {
            reportGraph.GraphHeight = Height;
        }

        ReportGraph graph = new ReportGraph() { Colors = Colors };
        graph.ChartControl = ucChart;

        mReport = ReportInfoProvider.GetReportInfo(reportGraph.GraphReportID);
        if (mReport == null)
        {
            return;
        }

        // Check graph security settings
        if (!(CheckReportAccess(mReport) && CheckEmailModeSubscription(mReport, ValidationHelper.GetBoolean(ReportGraphInfo.GraphSettings["SubscriptionEnabled"], true))))
        {
            Visible = false;
            return;
        }

        // Prepare query attributes
        QueryText = reportGraph.GraphQuery;
        QueryIsStoredProcedure = reportGraph.GraphQueryIsStoredProcedure;

        // Init parameters
        InitParameters(mReport.ReportParameters);

        // Init macro resolver
        InitResolver();

        mErrorOccurred = false;
        DataSet dsGraphData = null;

        // Ensure report item name for caching
        if (String.IsNullOrEmpty(ReportItemName))
        {
            ReportItemName = String.Format("{0};{1}", mReport.ReportName, reportGraph.GraphName);
        }

        // Create graph image                    
        try
        {
            // LoadData
            dsGraphData = LoadData();
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Get report graph", "E", ex);
            graph.CorrectWidth = correctWidth;
            graph.CreateInvalidDataGraph(reportGraph, "Reporting.Graph.InvalidDataGraph", false);
            mErrorOccurred = true;
        }

        // Test if dataset is empty
        if (DataHelper.DataSourceIsEmpty(dsGraphData))
        {
            if (EmailMode && SendOnlyNonEmptyDataSource)
            {
                // Empty dataset, and flag not send empty dataset is set
                Visible = false;
                return;
            }

            string noRecordText = ResolveMacros(ValidationHelper.GetString(reportGraph.GraphSettings["QueryNoRecordText"], String.Empty));
            if (noRecordText != String.Empty)
            {
                ltlEmail.Text = noRecordText;
                lblInfo.Text = noRecordText;
                ucChart.Visible = false;
                menuCont.MenuID = String.Empty;
                EnableExport = false;
                lblInfo.Visible = true;
            }
            else
            {
                Visible = false;
            }
        }
        else
        {
            // Create chart
            graph.CorrectWidth = correctWidth;
            if (EmailMode)
            {
                byte[] data = graph.CreateChart(reportGraph, dsGraphData, ContextResolver, true);
                ltlEmail.Text = "##InlineImage##" + reportGraph.GraphName + "##InlineImage##";
                ReportSubscriptionSender.AddToRequest(mReport.ReportName, "g" + reportGraph.GraphName, data);
            }
            else
            {
                graph.CreateChart(reportGraph, dsGraphData, ContextResolver);
            }
        }
    }


    /// <summary>
    /// Computes the width of graph - depends on percent sets.
    /// </summary>
    private int GetGraphWidth()
    {
        return (int)((ComputedWidth) * ((float)GraphImageWidth / 100));
    }

    #endregion
}
