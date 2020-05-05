using System;
using System.Collections.Generic;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_ValidationSettings : CMSUserControl
{
    #region "Variables"

    private FieldEditorModeEnum mMode = FieldEditorModeEnum.General;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets value indicating if current item is primary.
    /// </summary>
    public bool IsPrimary
    {
        get;
        set;
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macroresolver used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value indicating if Specll-check is checked.
    /// </summary>
    public bool SpellCheck
    {
        get
        {
            return chkSpellCheck.Checked;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        FormEngineUserControl errorMessageControl = (FormEngineUserControl)txtErrorMessage.NestedControl;
        if (errorMessageControl != null)
        {
            // Disable autosave on LocalizableTextBox controls
            errorMessageControl.SetValue("AutoSave", false);
        }

        txtErrorMessage.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblErrorMessage.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtErrorMessage.NestedControl.Controls);
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (FieldInfo != null)
        {
            FieldInfo.SpellCheck = SpellCheck;
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.ValidationErrorMessage, ValidationHelper.GetString(txtErrorMessage.Value, String.Empty), txtErrorMessage.IsMacro);
            FieldInfo.FieldMacroRules = ruleDesigner.Value;

            return true;
        }

        return false;
    }


    /// <summary>
    /// Show validation options according to selected attribute type.
    /// </summary>
    public void Reload()
    {
        if (FieldInfo != null)
        {
            chkSpellCheck.Checked = FieldInfo.SpellCheck;
            if (pnlSectionValidation.Visible)
            {
                bool isMacro;
                txtErrorMessage.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ValidationErrorMessage, out isMacro), isMacro);
                ruleDesigner.Value = FieldInfo.FieldMacroRules;
                ruleDesigner.DefaultErrorMessage = FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ValidationErrorMessage);
            }
        }
        else
        {
            chkSpellCheck.Checked = true;
            txtErrorMessage.SetValue(null);

            ruleDesigner.Value = new List<FieldMacroRule>();
            ruleDesigner.DefaultErrorMessage = string.Empty;
        }
    }


    /// <summary>
    /// Displays controls according to current field settings.
    /// </summary>
    public void DisplayControls()
    {
        // Set default values
        Visible = !IsPrimary;
        plcSpellCheck.Visible = false;

        if ((Mode == FieldEditorModeEnum.ClassFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeClassFormDefinition))
        {
            plcSpellCheck.Visible = true;
        }
    }

    #endregion
}