using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_UIControls_EditParameters : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Filter for controls
    /// </summary>
    public String DisplayedControls
    {
        get
        {
            return GetStringContextValue("DisplayedControls");
        }
        set
        {
            SetValue("DisplayedControls", value);
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
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
            editElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Mode for field editor
    /// </summary>
    public String FieldEditorMode
    {
        get
        {
            return GetStringContextValue("FieldEditorMode");
        }
        set
        {
            SetValue("FieldEditorMode", value);
        }
    }


    /// <summary>
    /// Column name for parameters in info object
    /// </summary>
    public String ParametersColumnName
    {
        get
        {
            return GetStringContextValue("ParametersColumnName");
        }
        set
        {
            SetValue("ParametersColumnName", value);
        }
    }


    /// <summary>
    /// Indicates if quick links can be displayed under the attribute list for selected fields.
    /// </summary>
    public bool ShowQuickLinks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowQuickLinks"), true);
        }
        set
        {
            SetValue("ShowQuickLinks", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            //Check view permission
            if (!CheckViewPermissions(UIContext.EditedObject as BaseInfo))
            {
                editElem.StopProcessing = true;
                editElem.Visible = false;
                return;
            }

            if (!CheckEditPermissions())
            {
                editElem.Enabled = false;
                editElem.ShowError(GetString("ui.notauthorizemodified"));
            }


            editElem.OnAfterDefinitionUpdate += new EventHandler(editElem_OnAfterDefinitionUpdate);
            if (DisplayedControls != String.Empty)
            {
                editElem.DisplayedControls = EnumStringRepresentationExtensions.ToEnum<FieldEditorControlsEnum>(DisplayedControls);
            }

            if (FieldEditorMode != String.Empty)
            {
                editElem.Mode = EnumStringRepresentationExtensions.ToEnum<FieldEditorModeEnum>(FieldEditorMode);
            }
            editElem.ShowQuickLinks = ShowQuickLinks;

            BaseInfo bi = UIContext.EditedObject as BaseInfo;

            // Set the form defintion to the FieldEditor
            if ((bi != null) && (ParametersColumnName != String.Empty))
            {
                // Set properties for webpart
                switch (editElem.Mode)
                {
                    case FieldEditorModeEnum.WebPartProperties:
                    case FieldEditorModeEnum.SystemWebPartProperties:
                        editElem.WebPartId = bi.Generalized.ObjectID;
                        break;

                }

                editElem.FormDefinition = ValidationHelper.GetString(bi.GetValue(ParametersColumnName), String.Empty);
            }

            ScriptHelper.HideVerticalTabs(Page);
        }
    }


    protected void editElem_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if (!editElem.Enabled)
        {
            editElem.ShowError(GetString("ui.notauthorizemodified"));
            return;
        }

        // Set the form defintion to the FieldEditor
        if ((UIContext.EditedObject is BaseInfo) && (ParametersColumnName != String.Empty))
        {
            BaseInfo bi = (BaseInfo)UIContext.EditedObject;
            bi.SetValue(ParametersColumnName, editElem.FormDefinition);
            bi.Update();
        }
    }

    #endregion
}
