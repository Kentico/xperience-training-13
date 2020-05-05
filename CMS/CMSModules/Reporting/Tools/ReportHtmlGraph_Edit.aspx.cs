using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_ReportHtmlGraph_Edit : CMSReportingModalPage
{
    #region "Variables"

    protected ReportInfo mReportInfo;
    protected ReportGraphInfo mReportGraphInfo;
    protected int mGraphId;

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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        bool newReport = false;
        ucSelectString.Scope = ReportInfo.OBJECT_TYPE;
        ConnectionStringRow.Visible = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "SetConnectionString");
        txtQueryQuery.FullScreenParentElementID = pnlWebPartForm_Properties.ClientID;

        // Test permission for query
        txtQueryQuery.Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries");

        versionList.OnAfterRollback += versionList_onAfterRollback;

        // Register script for resize and rollback
        RegisterScripts("divFooter", divScrolable.ClientID);

        tabControlElem.TabItems.Add(new UITabItem
        {
            Text = GetString("general.general")
        });

        tabControlElem.TabItems.Add(new UITabItem
        {
            Text = GetString("general.preview")
        });

        tabControlElem.UsePostback = true;

        Title = "ReportGraph Edit";

        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        Save += (s, ea) =>
        {
            if (SetData(true))
            {
                ltlScript.Text += ScriptHelper.GetScript("window.RefreshContent();CloseDialog();");
            }
        };

        int reportId = QueryHelper.GetInteger("reportid", 0);
        bool isPreview = QueryHelper.GetBoolean("preview", false);

        // If preview by URL -> select preview tab
        if (isPreview && !RequestHelper.IsPostBack())
        {
            tabControlElem.SelectedTab = 1;
        }

        if (reportId > 0)
        {
            mReportInfo = ReportInfoProvider.GetReportInfo(reportId);
        }

        if (PersistentEditedObject == null)
        {
            if (mReportInfo != null) //must be valid reportid parameter
            {
                int id = QueryHelper.GetInteger("objectid", 0);

                // Try to load graph name from hidden field (adding new graph & preview)
                if (id == 0)
                {
                    id = ValidationHelper.GetInteger(txtNewGraphHidden.Value, 0);
                }

                if (id > 0)
                {
                    PersistentEditedObject = ReportGraphInfoProvider.GetReportGraphInfo(id);
                    mReportGraphInfo = PersistentEditedObject as ReportGraphInfo;
                }
            }
        }
        else
        {
            mReportGraphInfo = PersistentEditedObject as ReportGraphInfo;
        }

        if (mReportInfo != null)
        {
            ucSelectString.DefaultConnectionString = mReportInfo.ReportConnectionString;
        }

        if (mReportGraphInfo != null)
        {
            PageTitle.TitleText = GetString("Reporting_ReportGraph_EditHTML.TitleText");
            mGraphId = mReportGraphInfo.GraphID;

            if (ObjectVersionManager.DisplayVersionsTab(mReportGraphInfo))
            {
                tabControlElem.TabItems.Add(new UITabItem
                {
                    Text = GetString("objectversioning.tabtitle")
                });

                versionList.Object = mReportGraphInfo;
                versionList.IsLiveSite = false;
            }
        }
        else
        {
            PageTitle.TitleText = GetString("Reporting_ReportGraph_EditHTML.NewItemTitleText");
            newReport = true;
        }

        if (!RequestHelper.IsPostBack())
        {
            // Load default data for new report
            if (newReport)
            {
                ucSelectString.Value = String.Empty;
                txtQueryNoRecordText.Text = GetString("attachmentswebpart.nodatafound");
                txtItemValueFormat.Text = "{%yval%}";
                txtSeriesItemTooltip.Text = "{%ser%}";
                chkExportEnable.Checked = true;
                chkSubscription.Checked = true;
            }
            // Otherwise load saved data
            else
            {
                LoadData();
            }
        }
    }


    /// <summary>
    /// Loads data from graph storage
    /// </summary>
    private void LoadData()
    {
        if (mReportGraphInfo != null)
        {
            txtDefaultName.Text = mReportGraphInfo.GraphDisplayName;
            txtDefaultCodeName.Text = mReportGraphInfo.GraphName;
            txtQueryQuery.Text = mReportGraphInfo.GraphQuery;
            chkIsStoredProcedure.Checked = mReportGraphInfo.GraphQueryIsStoredProcedure;

            ReportCustomData settings = mReportGraphInfo.GraphSettings;
            txtQueryNoRecordText.Text = settings["QueryNoRecordText"];
            txtGraphTitle.Text = mReportGraphInfo.GraphTitle;
            txtLegendTitle.Text = settings["LegendTitle"];
            txtItemValueFormat.Text = settings["ItemValueFormat"];
            chkDisplayLegend.Checked = ValidationHelper.GetBoolean(settings["DisplayLegend"], false);
            txtSeriesItemTooltip.Text = settings["SeriesItemToolTip"];
            txtSeriesItemLink.Text = settings["SeriesItemLink"];
            txtItemNameFormat.Text = settings["SeriesItemNameFormat"];
            chkExportEnable.Checked = ValidationHelper.GetBoolean(settings["ExportEnabled"], false);
            chkSubscription.Checked = ValidationHelper.GetBoolean(settings["SubscriptionEnabled"], false);
            ucSelectString.Value = mReportGraphInfo.GraphConnectionString;
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        var tabIndex = tabControlElem.SelectedTab < 0 ? 0 : tabControlElem.SelectedTab;
        switch (tabIndex)
        {
            // Edit
            case 0:
                pnlPreview.Visible = false;
                pnlVersions.Visible = false;
                FormPanelHolder.Visible = true;
                break;

            // Preview
            case 1:
                if (SetData())
                {
                    ShowPreview();
                }
                else
                {
                    tabControlElem.SelectedTab = 0;
                }
                break;

            // Version
            case 2:
                FormPanelHolder.Visible = false;
                pnlPreview.Visible = false;
                if (mReportGraphInfo != null)
                {
                    pnlVersions.Visible = true;
                }
                break;
        }

        AddNoCacheTag();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Save the changes to DB
    /// </summary>
    /// <param name="saveToDatabase">If true, data are saved into database</param>
    private bool SetData(bool saveToDatabase = false)
    {
        string errorMessage = String.Empty;
        if (saveToDatabase)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
            {
                RedirectToAccessDenied("cms.reporting", "Modify");
            }

            errorMessage = new Validator()
                .NotEmpty(txtDefaultName.Text, rfvDisplayName.ErrorMessage)
                .NotEmpty(txtDefaultCodeName.Text, rfvCodeName.ErrorMessage)
                .NotEmpty(txtQueryQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;

            if ((errorMessage == String.Empty) && (!ValidationHelper.IsIdentifier(txtDefaultCodeName.Text.Trim())))
            {
                errorMessage = GetString("general.erroridentifierformat");
            }

            string fullName = mReportInfo.ReportName + "." + txtDefaultCodeName.Text.Trim();
            ReportGraphInfo codeNameCheck = ReportGraphInfoProvider.GetReportGraphInfo(fullName);
            if ((errorMessage == String.Empty) && (codeNameCheck != null) && (codeNameCheck.GraphID != mGraphId))
            {
                errorMessage = GetString("Reporting_ReportGraph_Edit.ErrorCodeNameExist");
            }
        }

        // Test query in all cases
        if (errorMessage == String.Empty)
        {
            errorMessage = new Validator().NotEmpty(txtQueryQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;
        }

        if (errorMessage == String.Empty)
        {
            // New graph
            if (mReportGraphInfo == null)
            {
                mReportGraphInfo = new ReportGraphInfo();
            }

            mReportGraphInfo.GraphDisplayName = txtDefaultName.Text.Trim();
            mReportGraphInfo.GraphName = txtDefaultCodeName.Text.Trim();

            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries"))
            {
                mReportGraphInfo.GraphQuery = txtQueryQuery.Text;
            }

            mReportGraphInfo.GraphQueryIsStoredProcedure = chkIsStoredProcedure.Checked;
            mReportGraphInfo.GraphReportID = mReportInfo.ReportID;
            mReportGraphInfo.GraphTitle = txtGraphTitle.Text;
            mReportGraphInfo.GraphIsHtml = true;
            mReportGraphInfo.GraphType = String.Empty;
            mReportGraphInfo.GraphConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

            ReportCustomData settings = mReportGraphInfo.GraphSettings;
            settings["QueryNoRecordText"] = txtQueryNoRecordText.Text;
            settings["LegendTitle"] = txtLegendTitle.Text;
            settings["DisplayLegend"] = chkDisplayLegend.Checked.ToString();
            settings["SeriesItemToolTip"] = txtSeriesItemTooltip.Text;
            settings["SeriesItemLink"] = txtSeriesItemLink.Text;
            settings["ItemValueFormat"] = txtItemValueFormat.Text;
            settings["SeriesItemNameFormat"] = txtItemNameFormat.Text;
            settings["ExportEnabled"] = chkExportEnable.Checked.ToString();
            settings["SubscriptionEnabled"] = chkSubscription.Checked.ToString();

            if (saveToDatabase)
            {
                ReportGraphInfoProvider.SetReportGraphInfo(mReportGraphInfo);
            }

            return true;
        }

        ShowError(errorMessage);
        return false;
    }


    /// <summary>
    /// Show graph in preview mode
    /// </summary>
    private void ShowPreview()
    {
        FormPanelHolder.Visible = false;
        pnlVersions.Visible = false;

        if (mReportInfo != null)
        {
            pnlPreview.Visible = true;

            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);
            // Get datarow with required columns
            DataRow defaultValues = fi.GetDataRow(false);

            fi.LoadDefaultValues(defaultValues, true);

            ctrlReportGraph.ReportParameters = defaultValues;
            ctrlReportGraph.Visible = true;

            // Prepare fully qualified graph name = with reportname
            string fullReportGraphName = mReportInfo.ReportName + "." + mReportGraphInfo.GraphName;
            ctrlReportGraph.ReportGraphInfo = mReportGraphInfo;
            ctrlReportGraph.Parameter = fullReportGraphName;

            ctrlReportGraph.ReloadData(true);
        }
    }


    /// <summary>
    /// Get info from PersistentEditedObject and reload data
    /// </summary>
    private void ReloadDataAfrterRollback()
    {
        // Load rollbacked info
        GeneralizedInfo gi = PersistentEditedObject as GeneralizedInfo;
        if (gi != null)
        {
            mReportGraphInfo = gi.MainObject as ReportGraphInfo;
        }
        LoadData();
    }


    protected void versionList_onAfterRollback(object sender, EventArgs e)
    {
        ReloadDataAfrterRollback();
    }

    #endregion
}
