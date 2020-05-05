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

public partial class CMSModules_Reporting_Tools_ReportValue_Edit : CMSReportingModalPage
{
    #region "Variables"

    protected ReportInfo mReportInfo;
    protected ReportValueInfo mReportValueInfo;
    protected int mValueId;

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

        tabControlElem.TabItems.Add(new UITabItem 
        {
            Text = GetString("general.general")
        });

        tabControlElem.TabItems.Add(new UITabItem
        {
            Text = GetString("general.preview")
        });

        tabControlElem.UsePostback = true;

        RegisterScripts("divFooter", divScrolable.ClientID);

        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        int reportId = QueryHelper.GetInteger("reportid", 0);
        bool preview = QueryHelper.GetBoolean("preview", false);
        if (reportId > 0)
        {
            mReportInfo = ReportInfoProvider.GetReportInfo(reportId);
        }

        if (mReportInfo != null) //must be valid reportid parameter
        {
            ucSelectString.DefaultConnectionString = mReportInfo.ReportConnectionString;

            // If preview by URL -> select preview tab
            bool isPreview = QueryHelper.GetBoolean("preview", false);
            if (isPreview && !RequestHelper.IsPostBack())
            {
                tabControlElem.SelectedTab = 1;
            }

            if (PersistentEditedObject == null)
            {
                int id = QueryHelper.GetInteger("objectid", 0);
                if (id > 0)
                {
                    PersistentEditedObject = ReportValueInfoProvider.GetReportValueInfo(id);
                    mReportValueInfo = PersistentEditedObject as ReportValueInfo;
                }
            }
            else
            {
                mReportValueInfo = PersistentEditedObject as ReportValueInfo;
            }

            if (mReportValueInfo != null)
            {
                PageTitle.TitleText = GetString("Reporting_ReportValue_Edit.TitleText");
                mValueId = mReportValueInfo.ValueID;

                if (ObjectVersionManager.DisplayVersionsTab(mReportValueInfo))
                {
                    tabControlElem.TabItems.Add(new UITabItem
                    {
                        Text = GetString("objectversioning.tabtitle")
                    });

                    versionList.Object = mReportValueInfo;
                    versionList.IsLiveSite = false;
                }
            }
            else //new item
            {
                PageTitle.TitleText = GetString("Reporting_ReportValue_Edit.NewItemTitleText");
                chkSubscription.Checked = true;

                if (!RequestHelper.IsPostBack())
                {
                    ucSelectString.Value = String.Empty;
                }
            }

            if (!RequestHelper.IsPostBack())
            {
                LoadData();
            }
        }
        else
        {
            ShowError(GetString("Reporting_ReportValue_Edit.InvalidReportId"));
        }

        Save += (s, ea) =>
        {
            if (SetData(true))
            {
                ltlScript.Text += ScriptHelper.GetScript("window.RefreshContent();CloseDialog();");
            }
        };

        if (preview && !RequestHelper.IsPostBack())
        {
            tabControlElem.SelectedTab = 1;
            ShowPreview();
        }
    }


    /// <summary>
    /// PreRender event handler.
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
                divPanelHolder.Visible = true;
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

            // Versions
            case 2:
                divPanelHolder.Visible = false;
                pnlPreview.Visible = false;
                if (mReportValueInfo != null)
                {
                    pnlVersions.Visible = true;
                }
                break;
        }    

        base.OnPreRender(e);
    }


    /// <summary>
    /// Load data from db.
    /// </summary>
    protected void LoadData()
    {
        if (mReportValueInfo != null)
        {
            txtDisplayName.Text = mReportValueInfo.ValueDisplayName;
            txtCodeName.Text = mReportValueInfo.ValueName;
            txtQuery.Text = mReportValueInfo.ValueQuery;
            chkIsProcedure.Checked = mReportValueInfo.ValueQueryIsStoredProcedure;
            txtFormatString.Text = mReportValueInfo.ValueFormatString;
            chkSubscription.Checked = ValidationHelper.GetBoolean(mReportValueInfo.ValueSettings["SubscriptionEnabled"], true);
            ucSelectString.Value = mReportValueInfo.ValueConnectionString;
        }
    }


    /// <summary>
    /// Save data.
    /// </summary>
    /// <param name="saveToDatabase">If true, data are saved into database</param>
    private bool SetData(bool saveToDatabase = false)
    {
        string errorMessage = String.Empty;
        if (saveToDatabase)
        {
            // Check 'Modify' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
            {
                RedirectToAccessDenied("cms.reporting", "Modify");
            }

            errorMessage = new Validator()
                .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage)
                .NotEmpty(txtCodeName.Text, rfvCodeName.ErrorMessage)
                .NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;

            if ((errorMessage == "") && (!ValidationHelper.IsIdentifier(txtCodeName.Text.Trim())))
            {
                errorMessage = GetString("general.erroridentifierformat");
            }

            string fullName = mReportInfo.ReportName + "." + txtCodeName.Text.Trim();
            ReportValueInfo codeNameCheck = ReportValueInfoProvider.GetReportValueInfo(fullName);

            if ((errorMessage == "") && (codeNameCheck != null) && (codeNameCheck.ValueID != mValueId))
            {
                errorMessage = GetString("Reporting_ReportValue_Edit.ErrorCodeNameExist");
            }
        }

        //test query in all cases
        if (!saveToDatabase)
        {
            errorMessage = new Validator().NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;
        }

        if ((errorMessage == ""))
        {
            //new Value
            if (mReportValueInfo == null)
            {
                mReportValueInfo = new ReportValueInfo();
            }

            mReportValueInfo.ValueDisplayName = txtDisplayName.Text.Trim();
            mReportValueInfo.ValueName = txtCodeName.Text.Trim();

            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries"))
            {
                mReportValueInfo.ValueQuery = txtQuery.Text.Trim();
            }

            mReportValueInfo.ValueQueryIsStoredProcedure = chkIsProcedure.Checked;
            mReportValueInfo.ValueFormatString = txtFormatString.Text.Trim();
            mReportValueInfo.ValueReportID = mReportInfo.ReportID;
            mReportValueInfo.ValueSettings["SubscriptionEnabled"] = chkSubscription.Checked.ToString();
            mReportValueInfo.ValueConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

            if (saveToDatabase)
            {
                ReportValueInfoProvider.SetReportValueInfo(mReportValueInfo);
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
    /// Show preview.
    /// </summary>
    private void ShowPreview()
    {
        if (mReportInfo != null)
        {
            pnlPreview.Visible = true;
            divPanelHolder.Visible = false;
            pnlVersions.Visible = false;

            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);
            DataRow defaultValues = fi.GetDataRow(false);

            fi.LoadDefaultValues(defaultValues, true);
            
            //reportGraph.ContextParameters 
            ctrlReportValue.ReportParameters = defaultValues;

            ctrlReportValue.Visible = true;
            ctrlReportValue.ValueInfo = mReportValueInfo;

            ctrlReportValue.ReloadData(true);
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
            mReportValueInfo = gi.MainObject as ReportValueInfo;
        }
        LoadData();
    }


    protected void versionList_onAfterRollback(object sender, EventArgs e)
    {
        ReloadDataAfrterRollback();
    }
}
