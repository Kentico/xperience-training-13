using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;

using Action = CMS.UIControls.UniGridConfig.Action;

public partial class CMSModules_CustomTables_Controls_CustomTableDataList : CMSUserControl
{
    #region "Private & protected variables"

    protected string editToolTip = String.Empty;
    protected string deleteToolTip = String.Empty;
    protected string viewToolTip = String.Empty;
    protected string upToolTip = String.Empty;
    protected string downToolTip = String.Empty;

    // Default pages
    private string mEditItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_EditItem.aspx";
    private string mViewItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_ViewItem.aspx";
    private string mEditItemPageAdditionalParams;
    private string mViewItemPageAdditionalParams;

    protected DataSet ds = null;
    private FormInfo mFormInfo;
    private ObjectTypeInfo ti;

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
    /// Gets or sets URL of the page where item is edited.
    /// </summary>
    public string EditItemPage
    {
        get
        {
            return mEditItemPage;
        }
        set
        {
            mEditItemPage = value;
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for Edit page.
    /// </summary>
    public string EditItemPageAdditionalParams
    {
        get
        {
            return mEditItemPageAdditionalParams;
        }
        set
        {
            mEditItemPageAdditionalParams = value;
        }
    }


    /// <summary>
    /// Gets or sets URL of the page where whole item is displayed.
    /// </summary>
    public string ViewItemPage
    {
        get
        {
            return mViewItemPage;
        }
        set
        {
            mViewItemPage = value;
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for View page.
    /// </summary>
    public string ViewItemPageAdditionalParams
    {
        get
        {
            return mViewItemPageAdditionalParams;
        }
        set
        {
            mViewItemPageAdditionalParams = value;
        }
    }


    /// <summary>
    /// Gets or sets the class info of custom table which data are displayed.
    /// </summary>
    public DataClassInfo CustomTableClassInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the form info.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                if (CustomTableClassInfo != null)
                {
                    mFormInfo = FormHelper.GetFormInfo(CustomTableClassInfo.ClassName, true);
                }
            }
            return mFormInfo;
        }
    }


    /// <summary>
    /// Determines whether custom table has ItemOrder field.
    /// </summary>
    public bool HasItemOrderField
    {
        get
        {
            if (FormInfo != null)
            {
                return (FormInfo.FieldExists("ItemOrder") && FormInfo.FieldExists("ItemID"));
            }
            else
            {
                // If form info is not available assume ItemOrder is not present to prevent further exceptions
                return false;
            }
        }
    }


    /// <summary>
    /// Gets custom table data unigrid.
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridData;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return gridData.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            gridData.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Register Javascripts
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteEditView", ScriptHelper.GetScript(
            "var deleteConfirmation = ''; " +
            "function DeleteConfirm() { return confirm(deleteConfirmation); } " +
            "function EditItem(customtableid, itemId) { " +
            "  document.location.replace('" + ResolveUrl(EditItemPage) + "?" +
            (String.IsNullOrEmpty(mEditItemPageAdditionalParams) ? String.Empty : mEditItemPageAdditionalParams + "&") + "objectid=' + customtableid + '&itemId=' + itemId); } " +
            "function ViewItem(customtableid, itemId) { " +
            "  modalDialog('" + ResolveUrl(ViewItemPage) + "?" +
            (String.IsNullOrEmpty(mViewItemPageAdditionalParams) ? String.Empty : mViewItemPageAdditionalParams + "&") + "customtableid=' + customtableid + '&itemId=' + itemId,'ViewItem',600,600); } "));

        // Buttons' tooltips
        editToolTip = GetString("general.edit");
        deleteToolTip = GetString("general.delete");
        viewToolTip = GetString("general.view");
        upToolTip = GetString("general.up");
        downToolTip = GetString("general.down");

        // Delete confirmation
        ltlScript.Text = ScriptHelper.GetScript("deleteConfirmation = '" + GetString("customtable.data.DeleteConfirmation") + "';");

        gridData.ObjectType = CustomTableItemProvider.GetObjectType(CustomTableClassInfo.ClassName);
        gridData.OnLoadColumns += gridData_OnLoadColumns;
        gridData.OnExternalDataBound += gridData_OnExternalDataBound;
        gridData.OnAction += gridData_OnAction;

        ti = CustomTableItemProvider.GetTypeInfo(CustomTableClassInfo.ClassName);

        if (HasItemOrderField)
        {
            gridData.OrderBy = "ItemOrder ASC";
        }
        else
        {
            gridData.OrderBy = ti.IDColumn;
        }
    }

    #endregion


    #region "Grid events"

    protected void gridData_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            // Delete item action
            case "delete":
                if (CheckPermissions(PermissionsEnum.Delete))
                {
                    if (CustomTableClassInfo != null)
                    {
                        CustomTableItem item = CustomTableItemProvider.GetItem(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                        if (item != null)
                        {
                            item.Delete();
                        }
                    }

                    URLHelper.RefreshCurrentPage();
                }
                break;

            // Move item up action
            case "moveup":
                if (CheckPermissions(PermissionsEnum.Modify))
                {
                    if (CustomTableClassInfo != null)
                    {
                        var item = CustomTableItemProvider.GetItem(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                        item.Generalized.MoveObjectUp();
                    }

                    URLHelper.RefreshCurrentPage();
                }
                break;

            // Move item down action
            case "movedown":
                if (CheckPermissions(PermissionsEnum.Modify))
                {
                    if (CustomTableClassInfo != null)
                    {
                        var item = CustomTableItemProvider.GetItem(ValidationHelper.GetInteger(actionArgument, 0), CustomTableClassInfo.ClassName);
                        item.Generalized.MoveObjectDown();
                    }

                    URLHelper.RefreshCurrentPage();
                }
                break;
        }
    }


    protected object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string source = sourceName.ToLowerCSafe();
        // Get button and grid view row
        CMSGridActionButton button = sender as CMSGridActionButton;
        GridViewRow grv = parameter as GridViewRow;

        if (grv != null)
        {
            DataRowView drv = grv.DataItem as DataRowView;

            // Hide Move Up/Down buttons when there is no Order field
            switch (source)
            {
                case "edit":
                    if ((button != null) && (drv != null))
                    {
                        // Add edit script
                        button.OnClientClick = "EditItem(" + CustomTableClassInfo.ClassID + ", " + drv[ti.IDColumn] + "); return false;";
                    }
                    break;

                case "view":
                    if ((button != null) && (drv != null))
                    {
                        // Add view script
                        button.OnClientClick = "ViewItem(" + CustomTableClassInfo.ClassID + ", " + drv[ti.IDColumn] + "); return false;";
                    }
                    break;

                case "moveup":
                case "movedown":
                    if (!HasItemOrderField && (button != null))
                    {
                        // Hide button
                        button.Visible = false;
                    }
                    break;
            }
        }
        else
        {
            switch (source)
            {
                case "itemcreatedby":
                case "itemmodifiedby":
                    int userId = ValidationHelper.GetInteger(parameter, 0);
                    return HTMLHelper.HTMLEncode(GetUserName(userId));

                default:
                    return HTMLHelper.HTMLEncode(parameter.ToString());
            }
        }

        return parameter;
    }


    private string GetUserName(int userId)
    {
        if (userId != 0)
        {
            // Get user information
            UserInfo ui = UserInfo.Provider.Get(userId);
            if (ui != null)
            {
                return ui.GetFormattedUserName(false);
            }
        }

        return String.Empty;
    }


    protected void gridData_OnLoadColumns()
    {
        if ((CustomTableClassInfo != null) && (FormInfo != null))
        {
            // Update the actions command argument
            foreach (Action action in gridData.GridActions.Actions)
            {
                action.CommandArgument = ti.IDColumn;
            }

            string columnNames = null;
            List<string> columnList;

            string columns = CustomTableClassInfo.ClassShowColumns;
            if (String.IsNullOrEmpty(columns))
            {
                columnList = new List<string>();
                columnList.AddRange(GetExistingColumns(false).Take(5));

                // Show info message
                ShowInformation(GetString("customtable.columnscount.default"));
            }
            else
            {
                // Get existing columns names
                List<string> existingColumns = GetExistingColumns(true);

                // Get selected columns
                List<string> selectedColumns = GetSelectedColumns(columns);

                columnList = new List<string>();
                StringBuilder sb = new StringBuilder();

                // Remove non-existing columns
                foreach (string col in selectedColumns)
                {
                    int index = existingColumns.BinarySearch(col, StringComparer.InvariantCultureIgnoreCase);
                    if (index >= 0)
                    {
                        string colName = existingColumns[index];
                        columnList.Add(colName);
                        sb.Append(",[", colName, "]");
                    }
                }

                // Ensure item order column
                selectedColumns.Sort();
                if ((selectedColumns.BinarySearch("ItemOrder", StringComparer.InvariantCultureIgnoreCase) < 0) && HasItemOrderField)
                {
                    sb.Append(",[ItemOrder]");
                }

                // Ensure itemid column
                if (selectedColumns.BinarySearch(ti.IDColumn, StringComparer.InvariantCultureIgnoreCase) < 0)
                {
                    sb.Insert(0, ",[" + ti.IDColumn + "]");
                }

                columnNames = sb.ToString().TrimStart(',');
            }

            // Get macro resolver
            MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(FormHelper.FORM_PREFIX + CustomTableClassInfo.ClassName);

            // Loop trough all columns
            for (int i = 0; i < columnList.Count; i++)
            {
                string column = columnList[i];

                // Get field caption
                FormFieldInfo ffi = FormInfo.GetFormField(column);
                string fieldCaption = String.Empty;
                if (ffi == null)
                {
                    fieldCaption = column;
                }
                else
                {
                    string caption = ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, resolver);
                    fieldCaption = (caption == String.Empty) ? column : ResHelper.LocalizeString(caption);
                }

                Column columnDefinition = new Column
                {
                    Caption = fieldCaption,
                    Source = column,
                    ExternalSourceName = column,
                    AllowSorting = true,
                    Wrap = false
                };

                if (i == columnList.Count - 1)
                {
                    // Stretch last column
                    columnDefinition.Width = "100%";
                }

                gridData.GridColumns.Columns.Add(columnDefinition);
            }

            // Set column names
            gridData.Columns = columnNames;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks the specified permission.
    /// </summary>
    private bool CheckPermissions(PermissionsEnum permissionName)
    {
        // Check permission
        if (!CustomTableClassInfo.CheckPermissions(permissionName, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            ShowError(String.Format(GetString("customtable.permissiondenied." + permissionName), CustomTableClassInfo.ClassName));
            return false;
        }
        return true;
    }


    /// <summary>
    /// Gets existing columns from form info
    /// </summary>
    /// <param name="sort">Indicates if the columns should be sorted</param>
    private List<string> GetExistingColumns(bool sort)
    {
        if (FormInfo == null)
        {
            return null;
        }

        var existingColumns = FormInfo.GetColumnNames(false);
        if (sort)
        {
            existingColumns.Sort(StringComparer.InvariantCultureIgnoreCase);
        }

        return existingColumns;
    }


    /// <summary>
    /// Gets list of selected columns
    /// </summary>
    private static List<string> GetSelectedColumns(string columns)
    {
        return columns.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    #endregion
}
