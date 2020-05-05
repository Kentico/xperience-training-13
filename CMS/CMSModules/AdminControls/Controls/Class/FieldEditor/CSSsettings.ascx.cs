using System;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_CSSsettings : CMSUserControl
{
    #region "Variables"

    private FormFieldInfo ffi;

    #endregion


    #region "Parameters"

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
    /// Sets value indicating if control is enabled.
    /// </summary>
    public bool Enabled
    {
        set
        {
            pnlCss.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets macroresolver used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        txtFieldCssClass.ResolverName = ResolverName;
        txtCaptionCellCssClass.ResolverName = ResolverName;
        txtCaptionCssClass.ResolverName = ResolverName;
        txtCaptionStyle.ResolverName = ResolverName;
        txtControlCellCssClass.ResolverName = ResolverName;
        txtControlCssClass.ResolverName = ResolverName;
        txtInputStyle.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblFieldCssClass.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtFieldCssClass.NestedControl.Controls);
        lblCaptionCellCssClass.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtCaptionCellCssClass.NestedControl.Controls);
        lblCaptionCssClass.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtCaptionCssClass.NestedControl.Controls);
        lblCaptionStyle.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtCaptionStyle.NestedControl.Controls);
        lblControlCellCssClass.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtControlCellCssClass.NestedControl.Controls);
        lblControlCssClass.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtControlCssClass.NestedControl.Controls);
        lblInputStyle.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtInputStyle.NestedControl.Controls);
    }


    /// <summary>
    /// Load field.
    /// </summary>
    public void Reload()
    {
        if (ffi != null)
        {
            bool isMacro;
            txtCaptionStyle.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.CaptionStyle, out isMacro), isMacro);
            txtCaptionCssClass.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.CaptionCssClass, out isMacro), isMacro);
            txtCaptionCellCssClass.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.CaptionCellCssClass, out isMacro), isMacro);
            txtInputStyle.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.InputControlStyle, out isMacro), isMacro);
            txtControlCssClass.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.ControlCssClass, out isMacro), isMacro);
            txtControlCellCssClass.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.ControlCellCssClass, out isMacro), isMacro);
            txtFieldCssClass.SetValue(ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCssClass, out isMacro), isMacro);
            pnlCss.Enabled = ffi.Visible;
        }
        else
        {
            txtCaptionStyle.SetValue(null, false);
            txtCaptionCssClass.SetValue(null, false);
            txtCaptionCellCssClass.SetValue(null, false);
            txtInputStyle.SetValue(null, false);
            txtControlCssClass.SetValue(null, false);
            txtControlCellCssClass.SetValue(null, false);
            txtFieldCssClass.SetValue(null, false);
            pnlCss.Enabled = true;
        }
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (ffi != null)
        {
            ffi.SetPropertyValue(FormFieldPropertyEnum.CaptionStyle, ValidationHelper.GetString(txtCaptionStyle.Value, String.Empty), txtCaptionStyle.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.CaptionCssClass, ValidationHelper.GetString(txtCaptionCssClass.Value, String.Empty), txtCaptionCssClass.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.CaptionCellCssClass, ValidationHelper.GetString(txtCaptionCellCssClass.Value, String.Empty), txtCaptionCellCssClass.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.InputControlStyle, ValidationHelper.GetString(txtInputStyle.Value, String.Empty), txtInputStyle.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.ControlCssClass, ValidationHelper.GetString(txtControlCssClass.Value, String.Empty), txtControlCssClass.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.ControlCellCssClass, ValidationHelper.GetString(txtControlCellCssClass.Value, String.Empty), txtControlCellCssClass.IsMacro);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCssClass, ValidationHelper.GetString(txtFieldCssClass.Value, String.Empty), txtFieldCssClass.IsMacro);
            return true;
        }
        return false;
    }

    #endregion
}