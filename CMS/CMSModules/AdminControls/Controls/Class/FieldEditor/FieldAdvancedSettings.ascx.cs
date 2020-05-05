using System;

using CMS.FormEngine;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAdvancedSettings : CMSUserControl
{
    #region "Variables"

    private FormFieldInfo ffi = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets value indicating if control has another depending controls.
    /// </summary>
    public bool HasDependingFields
    {
        get
        {
            return chkHasDepending.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if control is depending on another control.
    /// </summary>
    public bool DependsOnAnotherField
    {
        get
        {
            return chkDependsOn.Checked;
        }
    }


    /// <summary>
    /// Gets or sets macroresolver used in appearance field.
    /// </summary>
    public string ResolverName
    {
        get
        {
            return visibleMacro.ResolverName;
        }
        set
        {
            visibleMacro.ResolverName = enabledMacro.ResolverName = value;
        }
    }


    /// <summary>
    /// Gets or sets macro for visibility of the field.
    /// </summary>
    public string VisibleMacro
    {
        get
        {
            return visibleMacro.Text;
        }
        set
        {
            visibleMacro.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets macro for Enabled value of field.
    /// </summary>
    public string EnabledMacro
    {
        get
        {
            return enabledMacro.Text;
        }
        set
        {
            enabledMacro.Text = value;
        }
    }


    /// <summary>
    /// Returns if the field should be displayed in simple mode.
    /// </summary>
    public bool DisplayInSimpleMode
    {
        get
        {
            return chkDisplayInSimpleMode.Checked;
        }
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get
        {
            return ffi;
        }
        set
        {
            ffi = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is placed in wizard.
    /// </summary>
    public bool IsWizard
    {
        get;
        set;
    }


    /// <summary>
    /// Decides whether to show 'Display in simple mode' checkbox
    /// </summary>
    public bool ShowDisplayInSimpleModeCheckBox
    {
        get
        {
            return plcDisplayInSimpleMode.Visible;
        }
        set
        {
            plcDisplayInSimpleMode.Visible = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblVisibleMacro.AssociatedControlClientID = visibleMacro.InputClientID;
        lblEnabledMacro.AssociatedControlClientID = enabledMacro.InputClientID;
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo.
    /// </summary>
    public void Reload()
    {
        if (ffi != null)
        {
            chkHasDepending.Checked = ffi.HasDependingFields;
            chkDependsOn.Checked = ffi.DependsOnAnotherField;
            VisibleMacro = ffi.GetPropertyValue(FormFieldPropertyEnum.VisibleMacro);
            EnabledMacro = ffi.GetPropertyValue(FormFieldPropertyEnum.EnabledMacro);
            chkDisplayInSimpleMode.Checked = ffi.DisplayInSimpleMode;
        }
        // If FormFieldInfo is not specified then clear form
        else
        {
            chkHasDepending.Checked = false;
            chkDependsOn.Checked = false;
            chkDisplayInSimpleMode.Checked = false;
            VisibleMacro = String.Empty;
            EnabledMacro = String.Empty;
        }
    }

    #endregion
}