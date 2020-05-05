using System;

using CMS.FormEngine;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Widgets_Controls_WidgetPropertiesFieldEditor : CMSUserControl
{
    #region "Variables"

    private int mWidgetId = 0;
    private WebPartInfo webpartInfo;
    private WidgetInfo widgetInfo;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates if system fields (node and document fields) are enabled.
    /// </summary>
    public bool EnableSystemFields
    {
        get
        {
            return fieldEditor.EnableSystemFields;
        }
        set
        {
            fieldEditor.EnableSystemFields = value;
        }
    }


    /// <summary>
    /// Widget ID. Widget contains altered form definition.
    /// </summary>
    public int WidgetID
    {
        get
        {
            if (mWidgetId == 0)
            {
                mWidgetId = UIContext.ObjectID;
            }

            return mWidgetId;
        }
        set
        {
            mWidgetId = value;
        }
    }

    #endregion


    #region "Public events"

    public delegate void OnAlternativeFieldEditorSaveEventHandler();

    public event OnAlternativeFieldEditorSaveEventHandler OnBeforeSave;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        widgetInfo = WidgetInfoProvider.GetWidgetInfo(WidgetID);
        UIContext.EditedObject = widgetInfo;

        if (widgetInfo != null)
        {
            webpartInfo = WebPartInfoProvider.GetWebPartInfo(widgetInfo.WidgetWebPartID);

            if (webpartInfo != null)
            {
                // Set original form definition from webpart
                fieldEditor.OriginalFormDefinition = webpartInfo.WebPartProperties;

                // Merge class and alternative form definitions
                string formDef = FormHelper.MergeFormDefinitions(webpartInfo.WebPartProperties, widgetInfo.WidgetProperties);

                // Use alternative form mode for field editor
                fieldEditor.Mode = FieldEditorModeEnum.Widget;
                fieldEditor.FormDefinition = formDef;

                // Use same control for widgets as for webparts
                fieldEditor.DisplayedControls = FieldEditorControlsEnum.Controls;

                // Handle definition update (move up, move down, delete, OK button)
                fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            }
        }
        else
        {
            fieldEditor.Visible = false;
            ShowError(GetString("general.invalidid"));
        }
    }


    /// <summary>
    /// After form definition update event handler.
    /// </summary>
    protected void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        // Perform OnBeforeSave if defined
        if (OnBeforeSave != null)
        {
            OnBeforeSave();
        }

        // Stop processing if set from outside
        if (StopProcessing)
        {
            return;
        }

        if ((widgetInfo != null) && (webpartInfo != null))
        {
            // Compare original and alternative form definitions - store differences only
            widgetInfo.WidgetProperties = FormHelper.GetFormDefinitionDifference(webpartInfo.WebPartProperties, fieldEditor.FormDefinition, true);
            // Update alternative form info in database
            WidgetInfoProvider.SetWidgetInfo(widgetInfo);
        }
        else
        {
            ShowError(GetString("general.invalidid"));
        }
    }

    #endregion
}