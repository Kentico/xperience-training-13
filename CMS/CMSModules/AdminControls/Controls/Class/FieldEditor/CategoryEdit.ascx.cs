using System;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets form category info.
    /// </summary>
    public FormCategoryInfo CategoryInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macro resolver name used in macro editor.
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

        FormEngineUserControl categoryCaptionControl = (FormEngineUserControl)txtCategoryCaption.NestedControl;
        
        // Disable autosave on LocalizableTextBox controls
        categoryCaptionControl?.SetValue("AutoSave", false);

        txtCategoryCaption.ResolverName = ResolverName;
        chkCollapsible.ResolverName = ResolverName;
        chkCollapsedByDefault.ResolverName = ResolverName;
        chkVisible.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCategoryCaption.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtCategoryCaption.NestedControl.Controls);
        lblCollapsible.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkCollapsible.NestedControl.Controls);
        lblCollapsedByDefault.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkCollapsedByDefault.NestedControl.Controls);
        lblVisible.AssociatedControlClientID = EditingFormControl.GetInputClientID(chkVisible.NestedControl.Controls);
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        if (CategoryInfo != null)
        {
            bool isMacro;
            txtCategoryCaption.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Caption, out isMacro), isMacro);
            chkCollapsible.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, out isMacro), isMacro);
            chkCollapsedByDefault.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, out isMacro), isMacro);
            chkVisible.SetValue(CategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Visible, out isMacro), isMacro);
        }
        else
        {
            txtCategoryCaption.SetValue(null);
            chkCollapsible.SetValue("false");
            chkCollapsedByDefault.SetValue("false");
            chkVisible.SetValue("true");
        }
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (CategoryInfo != null)
        {
            // Save LocalizableTextBox control
            LocalizableFormEngineUserControl fieldCaptionControl = (LocalizableFormEngineUserControl)txtCategoryCaption.NestedControl;
            fieldCaptionControl?.Save();

            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Caption, ValidationHelper.GetString(txtCategoryCaption.Value, String.Empty).Replace("'", string.Empty), txtCategoryCaption.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Collapsible, Convert.ToString(chkCollapsible.Value), chkCollapsible.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, Convert.ToString(chkCollapsedByDefault.Value), chkCollapsedByDefault.IsMacro);
            CategoryInfo.SetPropertyValue(FormCategoryPropertyEnum.Visible, Convert.ToString(chkVisible.Value), chkVisible.IsMacro);

            return true;
        }

        return false;
    }

    #endregion
}