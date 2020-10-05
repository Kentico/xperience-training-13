using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.ImportExport;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniGrid_Controls_AdvancedExport : AdvancedExport, IPostBackEventHandler
{
    #region "Variables"

    private string mCurrentDelimiter;
    private string alertMessage;
    private bool mControlLoaded;
    private bool? mUserCanEditSql;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the value that indicates whether current user is able to edit SQL code
    /// </summary>
    private bool UserCanEditSql
    {
        get
        {
            if (mUserCanEditSql == null)
            {
                mUserCanEditSql = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", "editsqlcode");
            }
            return mUserCanEditSql.Value;
        }
    }


    /// <summary>
    /// Currently selected format (in dropdown list).
    /// </summary>
    protected DataExportFormatEnum CurrentFormat
    {
        get
        {
            return (DataExportFormatEnum)Enum.Parse(typeof(DataExportFormatEnum), drpExportTo.SelectedItem.Value);
        }
    }


    /// <summary>
    /// Currently selected delimiter (in dropdown list).
    /// </summary>
    protected string CurrentDelimiter
    {
        get
        {
            if (string.IsNullOrEmpty(mCurrentDelimiter))
            {
                ListItem delimiterItem = drpDelimiter.SelectedItem;
                if ((delimiterItem != null) && !string.IsNullOrEmpty(delimiterItem.Value))
                {
                    // Parse delimiter from drop down list
                    Delimiter delimiter = (Delimiter)Enum.Parse(typeof(Delimiter), delimiterItem.Value);
                    switch (delimiter)
                    {
                        case Delimiter.Comma:
                            mCurrentDelimiter = ",";
                            break;

                        case Delimiter.Semicolon:
                            mCurrentDelimiter = ";";
                            break;
                    }
                }
                if (string.IsNullOrEmpty(mCurrentDelimiter))
                {
                    mCurrentDelimiter = CultureHelper.PreferredUICultureInfo.TextInfo.ListSeparator;
                }
            }
            return mCurrentDelimiter;
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            orderByElem.StopProcessing = value;
            advancedExportTitle.StopProcessing = value;
            mdlAdvancedExport.StopProcessing = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        StopProcessing = !UniGrid.ShowActionsMenu;
        SetupControl();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetupControl();

        if (!StopProcessing)
        {
            var fixHeight = false;
            if (Visible && pnlUpdate.Visible && !ShouldCloseDialog())
            {
                if (RequestHelper.IsPostBack() && (CurrentModal != null))
                {
                    // Show popup after postback
                    CurrentModal.Show();
                    fixHeight = true;
                }
            }

            pnlAdvancedExport.EnableViewState = (CurrentDialog == pnlAdvancedExport);

            // Setup the javascript module
            object config = new
            {
                id = ClientID,
                uniqueId = UniqueID,
                unigridId = UniGrid.ClientID,
                hdnParamId = hdnParameter.ClientID,
                btnFullPostbackUniqueId = btnFullPostback.UniqueID,
                chlColumnsId = chlColumns.ClientID,
                hdnDefaultSelectionId = hdnDefaultSelection.ClientID,
                revRecordsId = revRecords.ClientID,
                cvColumnsId = cvColumns.ClientID,
                mdlAdvancedExportId = mdlAdvancedExport.ClientID,
                fixHeight,
                alertMessage
            };

            ScriptHelper.EnsurePostbackMethods(this);
            ScriptHelper.RegisterJQueryDialog(Page);
            ScriptHelper.RegisterModule(this, "CMS/AdvancedExport", config);
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// When dialog's export button is clicked.
    /// </summary>
    protected void btnExport_Click(object sender, EventArgs e)
    {
        TryExport(drpExportTo.SelectedItem.Value);
    }


    /// <summary>
    /// When dialog's preview export button is clicked.
    /// </summary>
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        TryExport(drpExportTo.SelectedItem.Value, true);
    }


    /// <summary>
    /// When postback is invoked to perform direct export.
    /// </summary>
    protected void btnFullPostback_Click(object sender, EventArgs e)
    {
        TryExport(hdnParameter.Value);
    }


    /// <summary>
    /// When export format is changed (in dropdown list).
    /// </summary>
    protected void drpExportTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitializeDelimiter();
        InitializeExportHeader();
    }


    /// <summary>
    /// When raw data checkbox changes its checked state.
    /// </summary>
    protected void chkExportRawData_CheckedChanged(object sender, EventArgs e)
    {
        InitializeColumns(true);
        InitializeOrderBy(true);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handles postbacks invoked upon this control.
    /// </summary>
    /// <param name="eventArgument">Argument that goes with postback</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument))
        {
            try
            {
                // Parse event argument
                switch (eventArgument)
                {
                    case "advancedexport":
                        pnlAdvancedExport.Visible = true;
                        ShowPopup(pnlAdvancedExport, mdlAdvancedExport);
                        InitializeAdvancedExport();
                        break;
                    case CLOSE_DIALOG:
                        HideCurrentPopup();
                        break;
                }

                // Update panel with dialog
                pnlUpdate.Update();
            }
            catch (Exception ex)
            {
                AddAlert(GetString("general.erroroccurred") + " " + ex.Message);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets the control up
    /// </summary>
    private void SetupControl()
    {
        if (mControlLoaded || StopProcessing)
        {
            return;
        }

        // Register full postback buttons for direct and advanced export
        ControlsHelper.RegisterPostbackControl(btnExport);
        ControlsHelper.RegisterPostbackControl(btnPreview);
        ControlsHelper.RegisterPostbackControl(btnFullPostback);

        // Initialize page title
        advancedExportTitle.TitleText = GetString("export.advancedexport");
        advancedExportTitle.ShowFullScreenButton = false;
        advancedExportTitle.SetCloseJavaScript(ControlsHelper.GetPostBackEventReference(this, CLOSE_DIALOG) + "; return false;");

        // Initialize help icon
        advancedExportTitle.IsDialog = true;
        advancedExportTitle.HelpTopicName = HELP_TOPIC_LINK;

        // Initialize column-selecting buttons
        btnSelectAll.OnClientClick = GetModuleMethod("checkAll", true);
        btnDeselectAll.OnClientClick = GetModuleMethod("uncheckAll", true);
        btnDefaultSelection.OnClientClick = GetModuleMethod("defaultSelection", true);

        lblCurrentPageOnly.ToolTip = GetString("export.currentpagetooltip");
        chkCurrentPageOnly.ToolTip = GetString("export.currentpagetooltip");

        // Set up validator
        string validationGroup = "advancedExport_" + ClientID;

        revRecords.ValidationGroup = validationGroup;
        revRecords.ErrorMessage = GetString("export.validinteger");
        revRecords.ValidationExpression = "^\\d{1,9}$";

        cvColumns.ValidationGroup = validationGroup;
        cvColumns.ClientValidationFunction = GetModuleMethod("validateExport", false, false);
        cvColumns.ErrorMessage = GetString("export.selectcolumns");

        btnExport.ValidationGroup = validationGroup;
        btnExport.OnClientClick = ScriptHelper.GetDisableProgressScript();

        btnPreview.ValidationGroup = validationGroup;
        btnPreview.OnClientClick = ScriptHelper.GetDisableProgressScript();

        orderByElem.Enabled = txtWhereCondition.Enabled = txtOrderBy.Enabled = !chkCurrentPageOnly.Checked;

        // Initialize
        if (!UserCanEditSql)
        {
            InitializeOrderBy(false);
        }
        mControlLoaded = true;
    }


    /// <summary>
    /// Tries to export data in the given format.
    /// </summary>
    /// <param name="format">Format name</param>
    /// <param name="isPreview">Indicates if export is only preview (first 100 items)</param>
    private void TryExport(string format, bool isPreview = false)
    {
        try
        {
            ExportData(EnumStringRepresentationExtensions.ToEnum<DataExportFormatEnum>(format), isPreview ? (int?)100 : null);
        }
        catch (Exception ex)
        {
            AddAlert(ex.Message);
        }
    }


    /// <summary>
    /// Exports data based on given format.
    /// </summary>
    /// <param name="format">Format to export data in</param>
    /// <param name="forceMaxItems">Max items count (for preview export)</param>
    private void ExportData(DataExportFormatEnum format, int? forceMaxItems = null)
    {
        UniGridExportHelper.ExportRawData = chkExportRawData.Checked && UserCanEditSql;
        UniGridExportHelper.CSVDelimiter = CurrentDelimiter;
        UniGridExportHelper.GenerateHeader = chkExportHeader.Checked;
        UniGridExportHelper.CurrentPageOnly = chkCurrentPageOnly.Checked;
        UniGridExportHelper.Records = ValidationHelper.GetInteger(txtRecords.Text, -1);

        // Preview export
        if (forceMaxItems != null)
        {
            int limit;

            if (UniGridExportHelper.CurrentPageOnly)
            {
                limit = UniGrid.Pager.DisplayPager ? UniGrid.Pager.CurrentPageSize : 0;
            }
            else
            {
                limit = UniGridExportHelper.Records;
            }
            if ((limit >= forceMaxItems) || (limit <= 0))
            {
                UniGridExportHelper.Records = forceMaxItems.Value;
                UniGridExportHelper.CurrentPageOnly = false;
            }
        }

        // Get order by clause from correct control
        if (!chkCurrentPageOnly.Checked)
        {
            UniGridExportHelper.OrderBy = UserCanEditSql ? TrimExtendedTextAreaValue(txtOrderBy.Text) : ValidationHelper.GetString(orderByElem.Value, null);
            UniGridExportHelper.WhereCondition = UserCanEditSql ? TrimExtendedTextAreaValue(txtWhereCondition.Text) : String.Empty;
        }
        UniGridExportHelper.Columns = GetSelectedColumns();

        if (UIContext.SiteID > 0)
        {
            UniGridExportHelper.SiteName = SiteInfoProvider.GetSiteName(UIContext.SiteID);
        }
        else if (IsCMSDesk)
        {
            UniGridExportHelper.SiteName = SiteContext.CurrentSiteName;
        }

        UniGridExportHelper.ExportData(format, Page.Response);
    }


    /// <summary>
    /// Extracts list of columns from checkbox list.
    /// </summary>
    /// <returns>List of columns selected to be exported</returns>
    private List<string> GetSelectedColumns()
    {
        List<string> exportedColumns = new List<string>();

        // Edge case for user who had permission to export raw data but lost it on postback
        if (chkExportRawData.Checked && !UserCanEditSql)
        {
            // Get default column indexes of UI columns to export
            var cols = UniGridExportHelper.BoundFields.Where(f => !String.IsNullOrEmpty(f.HeaderText));
            var range = Enumerable.Range(0, cols.Count());
            exportedColumns.AddRange(range.Select(t => t.ToString()));

            return exportedColumns;
        }

        for (int i = 0; i < chlColumns.Items.Count; i++)
        {
            ListItem column = chlColumns.Items[i];
            if (!column.Selected)
            {
                continue;
            }

            // Get correct set of selected columns
            string col = chkExportRawData.Checked ? UniGridExportHelper.AvailableColumns[i] : i.ToString();
            if (!string.IsNullOrEmpty(col))
            {
                exportedColumns.Add(col);
            }
        }
        return exportedColumns;
    }


    /// <summary>
    /// Initializes advanced export dialog.
    /// </summary>
    private void InitializeAdvancedExport()
    {
        // Initialize dropdown lists
        drpExportTo.Items.Clear();
        ControlsHelper.FillListControlWithEnum<DataExportFormatEnum>(drpExportTo, "export");

        drpDelimiter.Items.Clear();
        ControlsHelper.FillListControlWithEnum<Delimiter>(drpDelimiter, "export");

        // Initialize rest of dialog
        InitializeDelimiter();
        InitializeExportHeader();
        InitializeColumns(false);

        plcWhere.Visible = UserCanEditSql;
        plcExportRawData.Visible = UserCanEditSql;
        orderByElem.Visible = !UserCanEditSql;
        txtOrderBy.Visible = UserCanEditSql;
        btnDefaultSelection.Visible = UserCanEditSql;
    }


    /// <summary>
    /// Sets visibility of delimiter dropdown list.
    /// </summary>
    private void InitializeDelimiter()
    {
        plcDelimiter.Visible = (CurrentFormat == DataExportFormatEnum.CSV);
    }


    /// <summary>
    /// Sets visibility of export header placeholder.
    /// </summary>
    private void InitializeExportHeader()
    {
        plcExportHeader.Visible = (CurrentFormat != DataExportFormatEnum.XML);
    }


    /// <summary>
    /// Initializes columns in order by selector.
    /// </summary>
    /// <param name="force">Whether to force loading</param>
    private void InitializeOrderBy(bool force)
    {
        if (force)
        {
            orderByElem.Columns.Clear();
        }

        if (orderByElem.Columns.Count != 0)
        {
            return;
        }

        IEnumerable<ListItem> columns;

        if (chkExportRawData.Checked)
        {
            columns = UniGridExportHelper.AvailableColumns.Select(c => new ListItem(c, c));
        }
        else
        {
            columns = from field in UniGridExportHelper.BoundFields
                      let column = (field.DataField == UniGrid.ALL) ? field.SortExpression : field.DataField
                      where !String.IsNullOrEmpty(column) && IsColumnAvailable(column)
                      select new ListItem(field.HeaderText, column);
        }

        orderByElem.Columns.AddRange(columns);

        orderByElem.ReloadData();
    }


    /// <summary>
    /// Initializes columns checkboxlist (and default selection JS array).
    /// </summary>
    /// <param name="force">Whether to force loading</param>
    private void InitializeColumns(bool force)
    {
        string defaultSelection = string.Empty;
        if (force)
        {
            chlColumns.Items.Clear();
        }

        if (chlColumns.Items.Count == 0)
        {
            // Initialize column selector
            if (chkExportRawData.Checked)
            {
                // Using raw db columns
                for (int i = 0; i < UniGridExportHelper.AvailableColumns.Count; i++)
                {
                    string col = UniGridExportHelper.AvailableColumns[i];
                    if (AddColumn(i, col, (UniGridExportHelper.BoundFields.FirstOrDefault(bf => bf.DataField == col) != null)))
                    {
                        defaultSelection += i + ",";
                    }
                }
            }
            else
            {
                // Using UI columns
                for (int i = 0; i < UniGridExportHelper.BoundFields.Count; i++)
                {
                    BoundField field = UniGridExportHelper.BoundFields[i];
                    if (AddColumn(i, field.HeaderText, true))
                    {
                        defaultSelection += i + ",";
                    }
                }
            }
        }

        hdnDefaultSelection.Value = defaultSelection.TrimEnd(',');
    }


    /// <summary>
    /// Adds column to checkbox list.
    /// </summary>
    /// <param name="i">Index of a column (used as value)</param>
    /// <param name="text">Text of a column (used as caption) - value is required</param>
    /// <param name="selected">Whether the column is selected (checked)</param>
    /// <returns>Whether the column should be listed as selected by default</returns>
    private bool AddColumn(int i, string text, bool selected)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        ListItem chkCol = new ListItem(text, i.ToString())
        {
            Selected = selected
        };

        chlColumns.Items.Add(chkCol);
        return selected;
    }


    /// <summary>
    /// Displays alert message.
    /// </summary>
    /// <param name="message">Message to show</param>
    private void AddAlert(string message)
    {
        if (String.IsNullOrEmpty(alertMessage))
        {
            alertMessage = GetString("general.erroroccurred") + " " + message;
        }
    }


    /// <summary>
    /// Returns javascript method from AdvancedExport module.
    /// </summary>
    /// <param name="methodName">Method name</param>
    /// <param name="isReturn">Indicates whether the script is return statement.</param>
    /// <param name="callFunction">Indicates whether the script is function call.</param>
    /// <returns></returns>
    private string GetModuleMethod(string methodName, bool isReturn = false, bool callFunction = true)
    {
        return String.Format("{0} CMS.UG_Export_{1}.{2}{3}", isReturn ? "return" : "", UniGrid.ClientID, methodName, callFunction ? "()" : "");
    }

    #endregion
}
