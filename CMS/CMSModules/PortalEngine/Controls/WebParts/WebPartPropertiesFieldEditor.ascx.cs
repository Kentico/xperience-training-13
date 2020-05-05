using System;
using System.Data;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartPropertiesFieldEditor : CMSUserControl
{
    #region "Variables"

    private WebPartInfo webPartInfo;
    private WebPartInfo parentWebPartInfo;
    private int mWebPartID;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Web part ID.
    /// </summary>
    public int WebPartID
    {
        get
        {
            if (mWebPartID == 0)
            {
                mWebPartID = UIContext.ObjectID;
            }

            return mWebPartID;
        }
        set
        {
            mWebPartID = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get web part info
        webPartInfo = WebPartInfoProvider.GetWebPartInfo(WebPartID);
        UIContext.EditedObject = webPartInfo;

        // Check if info object exists
        if (webPartInfo != null)
        {
            // Set field editor       
            if (webPartInfo.WebPartParentID > 0)
            {
                parentWebPartInfo = WebPartInfoProvider.GetWebPartInfo(webPartInfo.WebPartParentID);

                if (parentWebPartInfo != null)
                {
                    fieldEditor.Mode = FieldEditorModeEnum.InheritedWebPartProperties;
                    fieldEditor.AllowExtraFields = true;
                    fieldEditor.OriginalFormDefinition = parentWebPartInfo.WebPartProperties;
                    fieldEditor.FormDefinition = FormHelper.MergeFormDefinitions(parentWebPartInfo.WebPartProperties, webPartInfo.WebPartProperties);
                    fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
                }
            }
            else
            {
                fieldEditor.Mode = FieldEditorModeEnum.WebPartProperties;
                fieldEditor.WebPartId = WebPartID;

                // Check newly created field for widgets update
                fieldEditor.OnFieldCreated += UpdateWidgetsDefinition;
                fieldEditor.AfterItemDeleted += fieldEditor_AfterItemDeleted;
            }
        }
    }


    void fieldEditor_AfterItemDeleted(object sender, FieldEditorEventArgs e)
    {
        if (e == null)
        {
            return;
        }

        // Remove deleted field or category from inherited web parts
        InfoDataSet<WebPartInfo> webParts = WebPartInfoProvider.GetWebParts()
                .WhereEquals("WebPartParentID", WebPartID).TypedResult;

        if (!DataHelper.DataSourceIsEmpty(webParts))
        {
            foreach (WebPartInfo info in webParts)
            {
                switch (e.ItemType)
                {
                    case FieldEditorSelectedItemEnum.Field:
                        info.WebPartProperties = FormHelper.RemoveFieldFromAlternativeDefinition(info.WebPartProperties, e.ItemName, e.ItemOrder);
                        break;

                    case FieldEditorSelectedItemEnum.Category:
                        info.WebPartProperties = FormHelper.RemoveCategoryFromAlternativeDefinition(info.WebPartProperties, e.ItemName, e.ItemOrder);
                        break;
                }

                // Update web part
                info.Update();
            }
        }

        // Remove deleted field or category from widgets based on this web part
        InfoDataSet<WidgetInfo> widgets = WidgetInfoProvider.GetWidgets()
                .WhereEquals("WidgetWebPartID", WebPartID).TypedResult;

        if (!DataHelper.DataSourceIsEmpty(widgets))
        {
            foreach (WidgetInfo info in widgets)
            {
                switch (e.ItemType)
                {
                    case FieldEditorSelectedItemEnum.Field:
                        info.WidgetProperties = FormHelper.RemoveFieldFromAlternativeDefinition(info.WidgetProperties, e.ItemName, e.ItemOrder);
                        break;

                    case FieldEditorSelectedItemEnum.Category:
                        info.WidgetProperties = FormHelper.RemoveCategoryFromAlternativeDefinition(info.WidgetProperties, e.ItemName, e.ItemOrder);
                        break;
                }

                // Update widget
                info.Update();
            }
        }
    }


    /// <summary>
    /// Handles OnAfterDefinitionUpdate action and updates form definition of inherited web part.
    /// </summary>
    protected void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if ((webPartInfo != null) && (parentWebPartInfo != null))
        {
            // Compare original and alternative form definitions - store differences only
            webPartInfo.WebPartProperties = FormHelper.GetFormDefinitionDifference(parentWebPartInfo.WebPartProperties, fieldEditor.FormDefinition, true);

            // If there is no difference save empty form
            if (string.IsNullOrEmpty(webPartInfo.WebPartProperties))
            {
                webPartInfo.WebPartProperties = "<form></form>";
            }

            // Update web part info in database
            WebPartInfoProvider.SetWebPartInfo(webPartInfo);
        }
        else
        {
            ShowError(GetString("general.invalidid"));
        }
    }


    /// <summary>
    /// Handles OnFieldCreated action and updates form definition of all widgets based on current webpart.
    /// Newly created field is set to be disabled in widget definition for security reasons.
    /// </summary>
    /// <param name="newField">Newly created field</param>
    protected void UpdateWidgetsDefinition(object sender, FormFieldInfo newField)
    {
        if ((webPartInfo != null) && (newField != null))
        {
            // Get widgets based on this webpart
            DataSet ds = WidgetInfoProvider.GetWidgets()
                .WhereEquals("WidgetWebPartID", WebPartID)
                .Columns("WidgetID");

            // Continue only if there are some widgets
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int widgetId = ValidationHelper.GetInteger(dr["WidgetID"], 0);
                    WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo(widgetId);
                    if (widget != null)
                    {
                        // Prepare disabled field definition
                        string disabledField = String.Format("<form><field column=\"{0}\" visible=\"false\" /></form>", newField.Name);

                        // Incorporate disabled field into original definition of widget
                        widget.WidgetProperties = FormHelper.CombineFormDefinitions(widget.WidgetProperties, disabledField);

                        // Update widget
                        WidgetInfoProvider.SetWidgetInfo(widget);
                    }
                }
            }
        }
    }

    #endregion
}