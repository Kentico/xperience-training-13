using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.SiteProvider;


public partial class CMSModules_Reporting_Controls_DisplayReport : AbstractReportControl, IDisplayReport
{
    #region "Variables"

    private int mSavedReportId;
    private bool mSaveMode;
    private bool wasInit;
    private bool wasPreRender;
    private bool isSave;
    private bool reportLoaded;
    private bool mDisplayFilterResult = true; // internal display filter based on DisplayFilter and visible elements
    private string mReportHTML = String.Empty;
    private ArrayList mReportControls;
    private ReportInfo mReportInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value that indicates whether progress indicator should be used.
    /// </summary>
    public bool UseProgressIndicator
    {
        get;
        set;
    }


    /// <summary>
    /// Report name.
    /// </summary>
    public ReportInfo ReportInfo
    {
        get
        {
            if ((mReportInfo == null) && (!String.IsNullOrEmpty(ReportName)))
            {
                mReportInfo = ReportInfoProvider.GetReportInfo(ReportName);
            }
            return mReportInfo;
        }
        set
        {
            mReportInfo = value;
        }
    }


    /// <summary>
    /// Content panel.
    /// </summary>
    public Panel ReportPanel
    {
        get
        {
            return pnlContent;
        }
    }


    /// <summary>
    /// Graph possible width of control.
    /// </summary>
    public int AreaMaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["AreaMaxWidth"], 0);
        }
        set
        {
            ViewState["AreaMaxWidth"] = value;
        }
    }


    /// <summary>
    /// If true, reload is not called from this control.
    /// </summary>
    public bool UseExternalReload
    {
        get;
        set;
    }


    /// <summary>
    /// BasicForm object.
    /// </summary>
    public BasicForm ParametersForm
    {
        get
        {
            return formParameters;
        }
    }


    /// <summary>
    /// Returns report display name
    /// </summary>
    public string ReportDisplayName
    {
        get
        {
            if (ReportInfo != null)
            {
                return ReportInfo.ReportDisplayName;
            }

            return null;
        }
    }


    /// <summary>
    /// Report name.
    /// </summary>
    public string ReportName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if check inner sub controls.
    /// </summary>
    public bool CheckInnerControls
    {
        get;
        set;
    } = true;


    /// <summary>
    /// ReportHTML.
    /// </summary>
    public string ReportHTML
    {
        get
        {
            return mReportHTML;
        }
        set
        {
            mReportHTML = value;
        }
    }


    /// <summary>
    /// Display filter.
    /// </summary>
    public bool DisplayFilter
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Body CSS class.
    /// </summary>
    public string BodyCssClass
    {
        get
        {
            return pnlContent.CssClass;
        }
        set
        {
            pnlContent.CssClass = value;
        }
    }


    /// <summary>
    /// Form CSS class.
    /// </summary>
    public string FormCssClass
    {
        get
        {
            return formParameters.CssClass;
        }
        set
        {
            formParameters.CssClass = value;
        }
    }


    /// <summary>
    /// Assigned if parameters will be loaded automatically.
    /// </summary>
    public bool LoadFormParameters
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Load always default values.
    /// </summary>
    public bool ForceLoadDefaultValues
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ForceLoadDefaultValues"], false);
        }
        set
        {
            ViewState["ForceLoadDefaultValues"] = value;
        }
    }


    /// <summary>
    /// Reloads parameters even if were already initialized.
    /// </summary>
    public bool IgnoreWasInit
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IgnoreWasInit"], false);
        }
        set
        {
            ViewState["IgnoreWasInit"] = value;
        }
    }


    /// <summary>
    /// Child report controls.
    /// </summary>
    public ArrayList ReportControls
    {
        get
        {
            if (mReportControls == null)
            {
                mReportControls = new ArrayList();

                // Collect the controls
                foreach (Control c in pnlContent.Controls)
                {
                    if (c is AbstractReportControl)
                    {
                        mReportControls.Add(c);
                    }
                }
            }

            return mReportControls;
        }
        private set
        {
            mReportControls = value;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// OnInit.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Load parameters from data form
        LoadParameters();

        // Get handler to OnAfter save
        formParameters.OnAfterSave += formParameters_OnAfterSave;
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        bool preloaderRequired = ValidationHelper.GetBoolean(RequestStockHelper.GetItem("CMSGraphAutoWidth"), false);
        if (UseProgressIndicator && (!preloaderRequired || RequestHelper.IsPostBack()))
        {
            ScriptHelper.RegisterLoader(Page);
        }

        wasPreRender = true;

        if (RequestHelper.IsPostBack() && (!UseExternalReload || DisplayFilter))
        {
            try
            {
                formParameters.SaveData(null, false);
            }
            catch (Exception)
            {
            }
        }

        if (!reportLoaded)
        {
            ReloadData(true);
        }

        // Disable CSS class for filter if filter isn't visible
        if ((!mDisplayFilterResult) ||
            (ReportInfo == null) || (ReportInfo.ReportParameters == String.Empty) ||
            ReportInfo.ReportParameters.Equals("<form></form>", StringComparison.OrdinalIgnoreCase))
        {
            formParameters.CssClass = String.Empty;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// OnAfterSave handler.
    /// </summary>
    private void formParameters_OnAfterSave(object sender, EventArgs e)
    {
        // Only want to react to SaveData()
        if (wasPreRender)
        {
            bool tmpForceLoad = ForceLoadDefaultValues;
            ForceLoadDefaultValues = false;
            ReloadData(false);
            ForceLoadDefaultValues = tmpForceLoad;
        }
        else
        {
            // Set report parameters for parent control - used for print   
            if (LoadFormParameters)
            {
                ReportParameters = formParameters.DataRow;
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData(bool forceLoad)
    {
        pnlContent.Controls.Clear();
        ReportControls = null;

        // Report info must exists
        if (ReportInfo == null)
        {
            return;
        }

        // Check permissions for report
        if (!CheckReportAccess(ReportInfo))
        {
            Visible = false;
            return;
        }

        // If ReloadData is called before Init, load parameters
        if (!wasInit || IgnoreWasInit)
        {
            LoadParameters();
        }

        // Set parameters only if it is allowed
        if (LoadFormParameters)
        {
            ReportParameters = formParameters.DataRow;
        }

        // Clear resolver because it can contains old parameters values
        ClearResolver();

        // Build report HTML
        var html = ReportInfo.ReportLayout;

        html = ResolveMacros(html);
        html = HTMLHelper.ResolveUrls(html, null);

        // For emails - resolve metafile images as inline attachments
        if (EmailMode)
        {
            html = ReportSubscriptionSender.ResolveMetaFiles(ReportName, html);
        }

        // Add the content
        pnlContent.Controls.Clear();
        pnlContent.Controls.Add(new LiteralControl(html));

        ControlsHelper.ResolveDynamicControls(pnlContent);

        // Null GraphImageWidh if no graph present to enable load alone tables,values,etc 
        bool containsGraph = html.Contains("%%control:ReportGraph?");
        if (!containsGraph)
        {
            GraphImageWidth = 0;
        }

        mReportHTML = html;

        // Indicates if any item in the report is visible
        bool itemVisible = false;

        // Init the child controls
        foreach (AbstractReportControl ctrl in ReportControls)
        {
            if ((CheckInnerControls) && (!ctrl.IsValid(ReportInfo)))
            {
                ctrl.Visible = false;
                continue;
            }

            ctrl.Colors = Colors;
            ctrl.RenderCssClasses = RenderCssClasses;
            ctrl.ReportParameters = ReportParameters;
            ctrl.ContextParameters = ContextParameters;
            ctrl.AllParameters = AllParameters;
            ctrl.SavedReportGuid = Guid.Empty;
            ctrl.GraphImageWidth = GraphImageWidth;
            ctrl.DynamicMacros = DynamicMacros;
            ctrl.TableFirstColumnWidth = TableFirstColumnWidth;
            ctrl.SelectedInterval = SelectedInterval;
            ctrl.RenderCssClasses = RenderCssClasses;
            ctrl.SendOnlyNonEmptyDataSource = SendOnlyNonEmptyDataSource;
            ctrl.EmailMode = EmailMode;
            ctrl.ReportSubscriptionSiteID = ReportSubscriptionSiteID;
            ctrl.SubscriptionInfo = SubscriptionInfo;
            ctrl.EnableSubscription = EnableSubscription;

            // Do no generate export context menu for saved graph
            ctrl.EnableExport = !isSave && EnableExport;

            if (AreaMaxWidth != 0)
            {
                ctrl.ComputedWidth = AreaMaxWidth;
            }

            // In save mode must be defined new Guid for graph image and saved report id
            if (mSaveMode)
            {
                ctrl.SavedReportGuid = Guid.NewGuid();
                ctrl.SavedReportID = mSavedReportId;
            }

            if (EmailMode || !formParameters.Visible || formParameters.FieldControls != null && formParameters.ValidateData())
            {
                ctrl.ReloadData(forceLoad);
            }
            else if (formParameters.FieldControls == null)
            {
                void DelayReportReload(object sender, EventArgs eventArgs)
                {
                    formParameters.PreRender -= DelayReportReload;
                    ctrl.Page = Page;

                    if (formParameters.ValidateData())
                    {
                        ctrl.ReloadData(forceLoad);
                    }
                }

                formParameters.PreRender += DelayReportReload;
            }

            if (ctrl.ComputedWidth != 0)
            {
                AreaMaxWidth = ctrl.ComputedWidth;
            }

            itemVisible = itemVisible || ctrl.Visible;
        }

        if (!itemVisible && EmailMode)
        {
            Visible = false;
        }

        reportLoaded = true;

        // Display/hide the filtering form
        formParameters.Visible = mDisplayFilterResult;
    }


    /// <summary>
    /// Returns true if report is loaded.
    /// </summary>
    public bool IsReportLoaded()
    {
        return (ReportInfo != null);
    }


    /// <summary>
    /// Saves the report - Returns the saved report ID or 0 if some error was occurred or don't have permissions to this report.
    /// </summary>    
    public int SaveReport()
    {
        // Report info must exists
        if (ReportInfo != null)
        {
            // Check permissions for report
            if (!CheckReportAccess(ReportInfo))
            {
                Visible = false;
                return 0;
            }

            // Check 'SaveReports' permission
            if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
            {
                Visible = false;
                return 0;
            }

            // Validate report parameters
            if (!formParameters.ValidateData())
            {
                return 0;
            }

            try
            {
                // Save saved report info object
                SavedReportInfo sri = new SavedReportInfo();
                sri.SavedReportDate = DateTime.Now;
                sri.SavedReportGUID = Guid.NewGuid();
                sri.SavedReportHTML = String.Empty;
                sri.SavedReportReportID = ReportInfo.ReportID;
                sri.SavedReportCreatedByUserID = MembershipContext.AuthenticatedUser.UserID;
                sri.SavedReportParameters = ReportInfo.ReportParameters;

                string name = ReportInfo.ReportDisplayName;
                string timeStamp = " - " + DateTime.Now.ToString();
                if (name.Length + timeStamp.Length > 200)
                {
                    name = name.Substring(0, name.Length - (name.Length + timeStamp.Length - 200));
                }
                sri.SavedReportTitle = name + timeStamp;
                SavedReportInfoProvider.SetSavedReportInfo(sri);

                // Render control to get HTML code
                mSavedReportId = sri.SavedReportID;
                mSaveMode = true;

                formParameters.SaveData(null, false);

                isSave = true;
                ReloadData(true);
                isSave = false;

                // Render panel with controls and save it to string
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                Html32TextWriter writer2 = new Html32TextWriter(sw);
                pnlContent.RenderControl(writer2);

                // Save HTML to the saved report
                sri.SavedReportHTML = ResolveMacros(sb.ToString());
                sri.SavedReportHTML = HTMLHelper.UnResolveUrls(sri.SavedReportHTML, ResolveUrl("~/"));
                SavedReportInfoProvider.SetSavedReportInfo(sri);

                mSaveMode = false;

                // When no 'read' for reporting - reload data with mSaveMode set to false. This will prevent GetReportGraph.aspx to stop rendering graph.
                if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "read"))
                {
                    ReloadData(true);
                }

                return sri.SavedReportID;
            }
            catch
            {
            }
        }
        return 0;
    }


    /// <summary>
    /// Renders control to String representation
    /// </summary>
    /// <param name="siteID">This SiteID is used in report query instead of default CMSContext one</param>
    public string RenderToString(int siteID)
    {
        // Change siteID from context to subscription id
        AllParameters.Add("CMSContextCurrentSiteID", siteID);

        // Reload data
        isSave = true;
        ReloadData(true);
        isSave = false;

        // If control is not visible - subscription (report's or all items) is not allowed or no data found
        if (!Visible)
        {
            return String.Empty;
        }

        // Render to string
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        Html32TextWriter writer2 = new Html32TextWriter(sw);
        pnlContent.RenderControl(writer2);

        return sb.ToString();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load parameters from data form.
    /// </summary>
    private void LoadParameters()
    {
        if (ReportInfo != null)
        {
            // Set the form
            FormInfo fi = new FormInfo(ReportInfo.ReportParameters);
            // Get datarow with required columns
            DataRow dr = fi.GetDataRow(false);
            if (!RequestHelper.IsPostBack() || ForceLoadDefaultValues || !DisplayFilter)
            {
                fi.LoadDefaultValues(dr, true);
            }
            else if ((!ForceLoadDefaultValues) && (formParameters.DataRow != null))
            {
                dr = formParameters.DataRow;
            }

            // Show filter - based on DisplayFilter and number of visible elements
            mDisplayFilterResult = DisplayFilter && (fi.GetFormElements(true, false).Any());

            formParameters.DataRow = dr;
            formParameters.SubmitButton.Visible = mDisplayFilterResult;
            formParameters.SubmitButton.RegisterHeaderAction = false;
            formParameters.FormInformation = fi;
            formParameters.SubmitButton.ResourceString = "report_view.btnupdate";
            formParameters.SiteName = SiteContext.CurrentSiteName;
            formParameters.Mode = FormModeEnum.Insert;
            formParameters.Visible = mDisplayFilterResult;

            wasInit = true;
        }
    }

    #endregion
}