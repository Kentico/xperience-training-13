using System;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_HTMLEnvelope : CMSUserControl
{
    #region "Variables"

    private FormFieldInfo ffi = null;

    #endregion


    #region "Properties"

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
            pnlHtmlEnvelope.Enabled = value;
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

        txtContentAfter.ResolverName = ResolverName;
        txtContentBefore.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblContentAfter.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtContentAfter.NestedControl.Controls);
        lblContentBefore.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtContentBefore.NestedControl.Controls);
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo.
    /// </summary>
    public void Reload()
    {
        if (ffi != null)
        {
            bool isMacro;
            txtContentAfter.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ContentAfter, out isMacro), isMacro);
            txtContentBefore.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ContentBefore, out isMacro), isMacro);
        }
        // If FormFieldInfo is not specified then clear form
        else
        {
            txtContentBefore.SetValue(null);
            txtContentAfter.SetValue(null);
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
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.ContentBefore, ValidationHelper.GetString(txtContentBefore.Value, String.Empty), txtContentBefore.IsMacro);
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.ContentAfter, ValidationHelper.GetString(txtContentAfter.Value, String.Empty), txtContentAfter.IsMacro);
            
            return true;
        }

        return false;
    }

    #endregion
}