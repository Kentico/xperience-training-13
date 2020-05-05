using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_ReportTable_Edit : CMSReportingModalPage
{
    #region "Constants"

    private const string DEFAULT_SKIN_ID = "ReportGridAnalytics";

    #endregion


    #region "Variables"

    protected ReportInfo mReportInfo;
    protected ReportTableInfo mReportTableInfo;
    protected int mTableId;

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


    protected void Page_Load(object sender, EventArgs e)
    {
        ucSelectString.Scope = ReportInfo.OBJECT_TYPE;
        ConnectionStringRow.Visible = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "SetConnectionString");
        txtQuery.FullScreenParentElementID = pnlWebPartForm_Properties.ClientID;

        // Test permission for query
        txtQuery.Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries");

        versionList.OnAfterRollback += versionList_onAfterRollback;

        // Register common resize and refresh scripts
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
        bool preview = QueryHelper.GetBoolean("preview", false);
        if (reportId > 0)
        {
            mReportInfo = ReportInfoProvider.GetReportInfo(reportId);
        }

        // If preview by URL -> select preview tab
        bool isPreview = QueryHelper.GetBoolean("preview", false);
        if (isPreview && !RequestHelper.IsPostBack())
        {
            tabControlElem.SelectedTab = 1;
        }

        if (PersistentEditedObject == null)
        {
            if (mReportInfo != null) // Must be valid reportid parameter
            {
                int id = QueryHelper.GetInteger("objectid", 0);

                // Try to load tableName from hidden field (adding new graph & preview)
                if (id == 0)
                {
                    id = ValidationHelper.GetInteger(txtNewTableHidden.Value, 0);
                }

                if (id > 0)
                {
                    PersistentEditedObject = ReportTableInfoProvider.GetReportTableInfo(id);
                    mReportTableInfo = PersistentEditedObject as ReportTableInfo;
                }
            }
        }
        else
        {
            mReportTableInfo = PersistentEditedObject as ReportTableInfo;
        }

        if (mReportInfo != null)
        {
            ucSelectString.DefaultConnectionString = mReportInfo.ReportConnectionString;

            // Control text initializations
            if (mReportTableInfo != null)
            {
                PageTitle.TitleText = GetString("Reporting_ReportTable_Edit.TitleText");
                mTableId = mReportTableInfo.TableID;

                if (ObjectVersionManager.DisplayVersionsTab(mReportTableInfo))
                {
                    tabControlElem.TabItems.Add(new UITabItem
                    {
                        Text = GetString("objectversioning.tabtitle")
                    });

                    versionList.Object = mReportTableInfo;
                    versionList.IsLiveSite = false;
                }
            }
            else // New item
            {
                PageTitle.TitleText = GetString("Reporting_ReportTable_Edit.NewItemTitleText");
                if (!RequestHelper.IsPostBack())
                {
                    ucSelectString.Value = String.Empty;
                    txtPageSize.Text = "15";
                    txtQueryNoRecordText.Text = GetString("attachmentswebpart.nodatafound");
                    chkExportEnable.Checked = true;
                    chkSubscription.Checked = true;
                }
            }

            if (!RequestHelper.IsPostBack())
            {
                ControlsHelper.FillListControlWithEnum<PagerButtons>(drpPageMode, "PagerButtons");
                // Preselect page numbers paging
                drpPageMode.SelectedValue = ((int)PagerButtons.Numeric).ToString();

                LoadData();
            }
        }

        if ((preview) && (!RequestHelper.IsPostBack()))
        {
            tabControlElem.SelectedTab = 1;
            ShowPreview();
        }

        // In case of preview paging without saving table
        if (RequestHelper.IsPostBack() && tabControlElem.SelectedTab == 1)
        {
            // Reload default parameters
            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);

            // Get datarow with required columns
            ctrlReportTable.ReportParameters = fi.GetDataRow(false);
            fi.LoadDefaultValues(ctrlReportTable.ReportParameters, true);

            // Collect data and put them in table info
            SetData();
            ctrlReportTable.TableInfo = mReportTableInfo;
        }
    }


    /// <summary>
    /// Reload data for table in prerender because of paging
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        var tabIndex = tabControlElem.SelectedTab < 0 ? 0 : tabControlElem.SelectedTab;
        switch (tabIndex)
        {
            // Versions
            case 2:
                divPanelHolder.Visible = false;
                pnlPreview.Visible = false;
                if (mReportTableInfo != null)
                {
                    pnlVersions.Visible = true;
                }
                break;

            // Preview
            case 1:
                // Create table preview
                if (SetData())
                {
                    ShowPreview();
                }
                else
                {
                    tabControlElem.SelectedTab = 0;
                }
                break;

            // Edit
            case 0:
                pnlPreview.Visible = false;
                pnlVersions.Visible = false;
                divPanelHolder.Visible = true;
                break;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Loads data from mReportTableInfo
    /// </summary>
    protected void LoadData()
    {
        if (mReportTableInfo != null)
        {
            txtDisplayName.Text = mReportTableInfo.TableDisplayName;
            txtCodeName.Text = mReportTableInfo.TableName;
            txtQuery.Text = mReportTableInfo.TableQuery;
            chkIsProcedure.Checked = mReportTableInfo.TableQueryIsStoredProcedure;
            txtSkinID.Text = ValidationHelper.GetString(mReportTableInfo.TableSettings["skinid"], DEFAULT_SKIN_ID);
            txtPageSize.Text = ValidationHelper.GetInteger(mReportTableInfo.TableSettings["pagesize"], 10).ToString();
            chkEnablePaging.Checked = ValidationHelper.GetBoolean(mReportTableInfo.TableSettings["enablepaging"], false);
            drpPageMode.SelectedValue = ValidationHelper.GetString(mReportTableInfo.TableSettings["pagemode"], "");
            txtQueryNoRecordText.Text = ValidationHelper.GetString(mReportTableInfo.TableSettings["QueryNoRecordText"], String.Empty);
            chkExportEnable.Checked = ValidationHelper.GetBoolean(mReportTableInfo.TableSettings["ExportEnabled"], false);
            chkSubscription.Checked = ValidationHelper.GetBoolean(mReportTableInfo.TableSettings["SubscriptionEnabled"], false);
            ucSelectString.Value = mReportTableInfo.TableConnectionString;
        }
        else
        {
            // Load default data
            txtSkinID.Text = DEFAULT_SKIN_ID;
        }

    }


    /// <summary>
    /// Saves data.
    /// </summary>
    /// <param name="saveToDatabase">If true, data are saved into database</param>
    protected bool SetData(bool saveToDatabase = false)
    {
        string errorMessage = String.Empty;
        // Check 'Modify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
        {
            RedirectToAccessDenied("cms.reporting", "Modify");
        }

        if (saveToDatabase)
        {
            errorMessage = new Validator()
                .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage)
                .NotEmpty(txtCodeName.Text, rfvCodeName.ErrorMessage)
                .NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;

            if ((errorMessage == "") && (!ValidationHelper.IsIdentifier(txtCodeName.Text.Trim())))
            {
                errorMessage = GetString("general.erroridentifierformat");
            }

            string fullName = mReportInfo.ReportName + "." + txtCodeName.Text.Trim();
            ReportTableInfo codeNameCheck = ReportTableInfoProvider.GetReportTableInfo(fullName);

            if ((errorMessage == "") && (codeNameCheck != null) && (codeNameCheck.TableID != mTableId))
            {
                errorMessage = GetString("Reporting_ReportTable_Edit.ErrorCodeNameExist");
            }
        }

        // Test query in all cases
        if (errorMessage == String.Empty)
        {
            errorMessage = new Validator().NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;
        }

        if ((errorMessage == "") && (txtPageSize.Text.Trim() != String.Empty) && (!ValidationHelper.IsInteger(txtPageSize.Text) || !ValidationHelper.IsPositiveNumber(txtPageSize.Text)))
        {
            errorMessage = GetString("Reporting_ReportTable_Edit.errorinvalidpagesize");
        }

        if ((errorMessage == ""))
        {
            // New table
            if (mReportTableInfo == null)
            {
                mReportTableInfo = new ReportTableInfo();
            }

            mReportTableInfo.TableDisplayName = txtDisplayName.Text.Trim();
            mReportTableInfo.TableName = txtCodeName.Text.Trim();

            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries"))
            {
                mReportTableInfo.TableQuery = txtQuery.Text.Trim();
            }

            mReportTableInfo.TableQueryIsStoredProcedure = chkIsProcedure.Checked;
            mReportTableInfo.TableReportID = mReportInfo.ReportID;

            mReportTableInfo.TableSettings["SkinID"] = txtSkinID.Text.Trim();
            mReportTableInfo.TableSettings["enablepaging"] = chkEnablePaging.Checked.ToString();
            mReportTableInfo.TableSettings["pagesize"] = txtPageSize.Text;
            mReportTableInfo.TableSettings["pagemode"] = drpPageMode.SelectedValue;
            mReportTableInfo.TableSettings["QueryNoRecordText"] = txtQueryNoRecordText.Text;
            mReportTableInfo.TableSettings["ExportEnabled"] = chkExportEnable.Checked.ToString();
            mReportTableInfo.TableSettings["SubscriptionEnabled"] = chkSubscription.Checked.ToString();
            mReportTableInfo.TableConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

            if (saveToDatabase)
            {
                ReportTableInfoProvider.SetReportTableInfo(mReportTableInfo);
            }
        }
        else
        {
            ShowError(errorMessage);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Show preview
    /// </summary>
    private void ShowPreview()
    {
        divPanelHolder.Visible = false;
        pnlVersions.Visible = false;

        if (mReportInfo != null)
        {
            pnlPreview.Visible = true;

            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);
            // Get datarow with required columns
            DataRow defaultValues = fi.GetDataRow(false);

            fi.LoadDefaultValues(defaultValues, true);

            // ReportGraph.ContextParameters 
            ctrlReportTable.ReportParameters = defaultValues;

            // Prepare fully qualified graph name = with reportname
            if (mReportTableInfo != null)
            {
                string fullReportGraphName = mReportInfo.ReportName + "." + mReportTableInfo.TableName;
                ctrlReportTable.Parameter = fullReportGraphName;
            }
            ctrlReportTable.TableInfo = mReportTableInfo;


            ctrlReportTable.ReloadData(true);
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
            mReportTableInfo = gi.MainObject as ReportTableInfo;
        }
        LoadData();
    }


    protected void versionList_onAfterRollback(object sender, EventArgs e)
    {
        ReloadDataAfrterRollback();
    }
}
