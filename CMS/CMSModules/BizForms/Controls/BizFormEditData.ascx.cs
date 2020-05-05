using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;

using Action = CMS.UIControls.UniGridConfig.Action;


public partial class CMSModules_BizForms_Controls_BizFormEditData : CMSAdminEditControl
{
    #region "Variables"

    private int formId;
    private BizFormInfo bfi;
    private DataClassInfo dci;
    private FormInfo mFormInfo;
    private string primaryColumn;
    private bool isDialog;

    private const string DATE_TRANSFORMATION = "#date";
    private const string EDIT_PAGE = "BizForm_Edit_EditRecordMVC.aspx";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the form info.
    /// </summary>
    private FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                if (dci != null)
                {
                    mFormInfo = FormHelper.GetFormInfo(dci.ClassName, false);
                }
            }
            return mFormInfo;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether New Record button should be visible.
    /// </summary>
    public bool ShowNewRecordButton { get; set; } = true;


    /// <summary>
    /// Gets or sets the value that indicates whether Select Fields button should be visible.
    /// </summary>
    public bool ShowSelectFieldsButton { get; set; } = true;

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Get edited object
        if (UIContext.EditedObject != null)
        {
            bfi = (BizFormInfo)UIContext.EditedObject;
            formId = bfi.FormID;
        }

        isDialog = QueryHelper.GetBoolean("dialogmode", false);
        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'ReadData' permission
        CheckPermissions("ReadData");

        // Register scripts
        ScriptHelper.RegisterDialogScript(Page);
        if (ShowSelectFieldsButton)
        {
            var url = ResolveUrl("~/CMSModules/BizForms/Tools/BizForm_Edit_Data_SelectFields.aspx") + QueryHelper.BuildQueryWithHash("formid", formId.ToString());
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "SelectFields", ScriptHelper.GetScript("function SelectFields() { modalDialog('" + url + "'  ,'BizFormFields', 500, 500); }"));
        }
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "Edit", ScriptHelper.GetScript(
            "function EditRecord(formId, recordId) { " +
            "  document.location.replace('" + ResolveUrl($"~/CMSModules/BizForms/Tools/{EDIT_PAGE}") + "?formID=' + formId + '&formRecordID=' + recordId); } "));

        // Initialize unigrid
        gridData.OnExternalDataBound += gridData_OnExternalDataBound;
        gridData.OnLoadColumns += gridData_OnLoadColumns;
        gridData.OnAction += gridData_OnAction;

        if (bfi != null)
        {
            dci = DataClassInfoProvider.GetDataClassInfo(bfi.FormClassID);
            if (dci != null)
            {
                string className = dci.ClassName;

                // Set alternative form and data container
                gridData.ObjectType = BizFormItemProvider.GetObjectType(className);
                gridData.FilterFormName = className + ".filter";
                gridData.FilterFormData = bfi;

                // Get primary column name
                gridData.OrderBy = primaryColumn = GetPrimaryColumn(FormInfo, bfi.FormName);
            }
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        if (ShowNewRecordButton)
        {
            AddHeaderAction(new HeaderAction
            {
                Text = GetString("bizform_edit_data.newrecord"),
                RedirectUrl = ResolveUrl($"~/CMSModules/BizForms/Tools/{EDIT_PAGE}?formid={formId}")
            });
        }

        if (ShowSelectFieldsButton)
        {
            AddHeaderAction(new HeaderAction
            {
                Text = GetString("bizform_edit_data.selectdisplayedfields"),
                OnClientClick = "javascript:SelectFields();",
                ButtonStyle = ButtonStyle.Default,
            });
        }
    }


    protected void gridData_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerInvariant())
        {
            case "delete":
                CheckPermissions("DeleteData");

                // Get record ID
                int formRecordID = ValidationHelper.GetInteger(actionArgument, 0);

                // Get BizFormInfo object
                if (bfi != null)
                {
                    // Get class object
                    if (dci != null)
                    {
                        // Get record object
                        var item = BizFormItemProvider.GetItem(formRecordID, dci.ClassName);

                        // Delete all files of the record
                        BizFormInfoProvider.DeleteBizFormRecordFiles(dci.ClassFormDefinition, item, SiteContext.CurrentSiteName);

                        // Delete the form record
                        item.Delete();
                    }
                }

                break;
        }
    }


    protected object gridData_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton button = sender as CMSGridActionButton;
        GridViewRow grv = parameter as GridViewRow;

        if (grv != null)
        {
            DataRowView drv = (DataRowView)grv.DataItem;

            switch (sourceName.ToLowerInvariant())
            {
                case "edit":
                    if (button != null)
                    {
                        if (!isDialog)
                        {
                            button.OnClientClick = "EditRecord(" + formId + ", " + drv[primaryColumn] + "); return false;";
                        }
                        else
                        {
                            button.Visible = false;
                        }
                    }
                    break;
            }
        }

        return parameter;
    }


    protected void gridData_OnLoadColumns()
    {
        if ((bfi != null) && (FormInfo != null))
        {
            // Update the actions command argument
            foreach (var action in gridData.GridActions.Actions)
            {
                ((Action)action).CommandArgument = primaryColumn;
            }

            // Get existing columns names
            var columnList = GetExistingColumns();

            string columns = bfi.FormReportFields;
            if (!string.IsNullOrEmpty(columns))
            {
                var selectedColumns = GetSelectedColumns(columns);

                columnList = columnList.Intersect(selectedColumns, StringComparer.InvariantCultureIgnoreCase).ToList();

                // Set columns which should be retrieved in query and ensure primary column
                gridData.Columns = (!columnList.Contains(primaryColumn, StringComparer.InvariantCultureIgnoreCase) ? "[" + primaryColumn + "],[" : "[") + columnList.Join("],[") + "]";
            }

            // Get macro resolver for current form
            MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(FormHelper.FORM_PREFIX + dci.ClassName);

            // Loop trough all columns
            int lastIndex = columnList.Count - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                string column = columnList[i];

                // Get field caption
                FormFieldInfo ffi = FormInfo.GetFormField(column);

                string fieldCaption;
                if (ffi == null)
                {
                    fieldCaption = column;
                }
                else
                {
                    string caption = ffi.GetDisplayName(resolver);
                    fieldCaption = (String.IsNullOrEmpty(caption)) ? column : ResHelper.LocalizeString(caption);
                }

                Column columnDefinition = new Column
                {
                    Caption = fieldCaption,
                    Source = column,
                    ExternalSourceName = ((ffi != null) && ffi.DataType.Equals(FieldDataType.Date, StringComparison.OrdinalIgnoreCase)) ? DATE_TRANSFORMATION : null,
                    AllowSorting = true,
                    Wrap = false
                };

                if (i == lastIndex)
                {
                    // Stretch last column
                    columnDefinition.Width = "100%";
                }

                gridData.GridColumns.Columns.Add(columnDefinition);
            }
        }
    }


    /// <summary>
    /// Overridden SetValue.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerInvariant())
        {
            case "shownewrecordbutton":
                ShowNewRecordButton = ValidationHelper.GetBoolean(value, false);
                break;
            case "showselectfieldsbutton":
                ShowSelectFieldsButton = ValidationHelper.GetBoolean(value, false);
                break;
        }

        return true;
    }


    /// <summary>
    /// Checks the specified permission.
    /// </summary>
    private void CheckPermissions(string permissionName)
    {
        // Check 'Modify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", permissionName))
        {
            CMSPage.RedirectToAccessDenied("cms.form", permissionName);
        }
    }


    /// <summary>
    /// Get form columns.
    /// </summary>
    private List<string> GetExistingColumns()
    {
        return FormInfo?.GetColumnNames(false, i => i.System);
    }


    /// <summary>
    /// Get list of columns that have been selected to be visible.
    /// </summary>
    /// <param name="columns">Selected columns</param>
    private List<string> GetSelectedColumns(string columns)
    {
        return columns.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }


    /// <summary>
    /// Returns name of the primary key column.
    /// </summary>
    /// <param name="fi">Form info</param>
    /// <param name="bizFormName">Bizform code name</param>
    private static string GetPrimaryColumn(FormInfo fi, string bizFormName)
    {
        string result = null;

        if ((fi != null) && (!string.IsNullOrEmpty(bizFormName)))
        {
            // Seek primary key column in all fields
            var query = from field in fi.GetFields(true, true)
                        where (field.PrimaryKey)
                        select field.Name;

            // Try to get field with the name 'bizformnameID'
            result = query.FirstOrDefault();
        }

        return result;
    }

    #endregion
}
