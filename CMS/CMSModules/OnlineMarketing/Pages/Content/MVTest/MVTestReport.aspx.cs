using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Web.UI;
using CMS.Reporting.Web.UI;
using CMS.UIControls;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;


[Security(Resource = "CMS.MVTest", UIElements = "MVTestListing;Detail;Reports")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_MVTestReport : CMSMVTestPage
{
    #region "Variables"

    protected string mSave = null;
    protected string mPrint = null;
    protected string mDeleteData = null;
    private bool mIsSaved;
    private bool mReportLoaded;
    private string mTestName = String.Empty;
    private IDisplayReport mUcDisplayReport;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        var cui = MembershipContext.AuthenticatedUser;

        reportHeader.ActionPerformed += HeaderActions_ActionPerformed;

        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.MVTesting);
        }

        // Set disabled module info
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Register actions
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => Save());

        mUcDisplayReport = (IDisplayReport)LoadUserControl("~/CMSModules/Reporting/Controls/DisplayReport.ascx");
        pnlContent.Controls.Add((Control)mUcDisplayReport);

        UIHelper.AllowUpdateProgress = false;

        // MVTest Info
        int mvTestID = QueryHelper.GetInteger("objectID", 0);
        MVTestInfo mvInfo = MVTestInfoProvider.GetMVTestInfo(mvTestID);
        if (mvInfo == null)
        {
            return;
        }

        // Load combination by current template ID and culture
        int nodeID = QueryHelper.GetInteger("NodeID", 0);

        if (nodeID > 0)
        {
            // Create condition for current template combinations            
            TreeProvider tree = new TreeProvider(cui);
            TreeNode node = tree.SelectSingleNode(nodeID, LocalizationContext.PreferredCultureCode);

            if (node != null)
            {
                usCombination.DocumentID = node.DocumentID;
            }
        }
        else
        {
            rbCombinations.Visible = false;
        }

        usCombination.ReloadData(true);

        mTestName = mvInfo.MVTestName;

        ucGraphType.ProcessChartSelectors(false);

        // Enables/disables radio buttons (based on UI elements)
        var ui = MembershipContext.AuthenticatedUser;

        if (!RequestHelper.IsPostBack())
        {
            if (!ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                bool checkedButton = false;

                // Check first enabled button 
                foreach (Control ctrl in pnlRadioButtons.Controls)
                {
                    CMSRadioButton rb = ctrl as CMSRadioButton;
                    if (rb != null)
                    {
                        if (rb.Enabled)
                        {
                            rb.Checked = true;
                            checkedButton = true;
                            break;
                        }
                    }
                }

                // No report avaible -> redirect to access denied
                if (!checkedButton)
                {
                    RedirectToAccessDenied(GetString("mvtest.noreportavaible"));
                }
            }
            else
            {
                // Admin check first radio button
                rbCount.Checked = true;
            }
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
    /// Display report
    /// </summary>
    /// <param name="reload">If true, display report control is reloaded</param>
    private void DisplayReport(bool reload)
    {
        if (mReportLoaded)
        {
            return;
        }

        if (RequestHelper.IsPostBack() && !IsValidInterval())
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }

        // Set repors name
        const string CONVERSION_COUNT = "mvtestconversioncount.yearreport;mvtestconversioncount.monthreport;mvtestconversioncount.weekreport;mvtestconversioncount.dayreport;mvtestconversioncount.hourreport";
        const string CONVERSION_RATE = "mvtestconversionrate.yearreport;mvtestconversionrate.monthreport;mvtestconversionrate.weekreport;mvtestconversionrate.dayreport;mvtestconversionrate.hourreport";
        const string CONVERSION_VALUE = "mvtestconversionvalue.yearreport;mvtestconversionvalue.monthreport;mvtestconversionvalue.weekreport;mvtestconversionvalue.dayreport;mvtestconversionvalue.hourreport";
        const string CONVERSION_COMBINATIONS = "mvtestconversionsbycombinations.yearreport;mvtestconversionsbycombinations.monthreport;mvtestconversionsbycombinations.weekreport;mvtestconversionsbycombinations.dayreport;mvtestconversionsbycombinations.hourreport";

        pnlCombination.Visible = false;
        // Set proper report name
        if (rbCount.Checked)
        {
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_COUNT);
        }

        if (rbRate.Checked)
        {
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_RATE);
        }

        if (rbValue.Checked)
        {
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_VALUE);
        }

        if (rbCombinations.Checked)
        {
            mUcDisplayReport.ReportName = ucGraphType.GetReportName(CONVERSION_COMBINATIONS);
            pnlCombination.Visible = true;
        }

        // Conversion
        ucConversions.PostbackOnDropDownChange = true;
        ucConversions.MVTestName = mTestName;
        ucConversions.ReloadData(true);

        // Conversion
        String conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
        if (conversion == ucConversions.AllRecordValue)
        {
            conversion = String.Empty;
        }

        // Combination
        String combination = ValidationHelper.GetString(usCombination.Value, String.Empty);
        if (combination == usCombination.AllRecordValue)
        {
            combination = String.Empty;
        }

        // General report data
        mUcDisplayReport.LoadFormParameters = false;
        mUcDisplayReport.DisplayFilter = false;
        mUcDisplayReport.GraphImageWidth = 100;
        mUcDisplayReport.IgnoreWasInit = true;
        mUcDisplayReport.TableFirstColumnWidth = Unit.Percentage(30);
        mUcDisplayReport.UseExternalReload = true;
        mUcDisplayReport.UseProgressIndicator = true;
        mUcDisplayReport.SelectedInterval = HitsIntervalEnumFunctions.HitsConversionToString(ucGraphType.SelectedInterval);

        // Resolve report macros 
        DataTable dtp = new DataTable();
        dtp.Columns.Add("FromDate", typeof(DateTime));
        dtp.Columns.Add("ToDate", typeof(DateTime));
        dtp.Columns.Add("CodeName", typeof(string));
        dtp.Columns.Add("MVTestName", typeof(string));
        dtp.Columns.Add("ConversionName", typeof(string));
        dtp.Columns.Add("CombinationName", typeof(string));

        object[] parameters = new object[6];
        parameters[0] = ucGraphType.From;
        parameters[1] = ucGraphType.To;
        parameters[2] = "pageviews";
        parameters[3] = mTestName;
        parameters[4] = conversion;
        parameters[5] = combination;

        dtp.Rows.Add(parameters);
        dtp.AcceptChanges();
        mUcDisplayReport.ReportParameters = dtp.Rows[0];

        if (reload)
        {
            mUcDisplayReport.ReloadData(true);
        }

        mReportLoaded = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayReport(true);

        reportHeader.ReportName = mUcDisplayReport.ReportName;
        reportHeader.ReportParameters = mUcDisplayReport.ReportParameters;
        reportHeader.SelectedInterval = ucGraphType.SelectedInterval;
        
        base.OnPreRender(e);
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


    /// <summary>
    /// Saves the graph report.
    /// </summary>
    private void Save()
    {
        // Check 'SaveReports' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.reporting", "SaveReports"))
        {
            RedirectToAccessDenied("cms.reporting", "SaveReports");
        }

        DisplayReport(false);

        // Saves the report        
        mIsSaved = true;

        if (mUcDisplayReport.SaveReport() > 0)
        {
            ShowConfirmation(String.Format(GetString("Analytics_Report.ReportSavedTo"), mUcDisplayReport.ReportDisplayName + " - " + DateTime.Now.ToString()));
        }

        mIsSaved = false;
    }


    /// <summary>
    /// Returns true if selected interval is valid.
    /// </summary>
    private bool IsValidInterval()
    {
        var from = ucGraphType.From;
        var to = ucGraphType.To;

        if (from == DateTimeHelper.ZERO_TIME || to == DateTimeHelper.ZERO_TIME)
        {
            return false;
        }

        return from <= to;
    }

    #endregion
}
