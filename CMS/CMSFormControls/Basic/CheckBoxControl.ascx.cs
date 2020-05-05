using System;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSFormControls_Basic_CheckBoxControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return checkbox.Enabled;
        }
        set
        {
            checkbox.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets checkbox text.
    /// </summary>
    public new string Text
    {
        get
        {
            return GetValue("Text", String.Empty);
        }
        set
        {
            SetValue("Text", value);
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return checkbox.Checked;
        }
        set
        {
            var boolValue = ValidationHelper.GetNullableBoolean(value, null);
            if (!boolValue.HasValue)
            {
                boolValue = (FieldInfo != null) && ValidationHelper.GetBoolean(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, ContextResolver), false);
            }
            checkbox.Checked = boolValue.Value;
        }
    }


    /// <summary>
    /// Gets or sets if control causes postback.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return checkbox.AutoPostBack;
        }
        set
        {
            checkbox.AutoPostBack = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            checkbox.AddCssClass(CssClass);
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(checkbox.CssClass))
        {
            checkbox.AddCssClass("CheckBoxField");
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            checkbox.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        if (!String.IsNullOrEmpty(Text))
        {
            checkbox.Text = HTMLHelper.HTMLEncode(ResHelper.GetString(Text));
        }

        CheckFieldEmptiness = false;

        // Use Changed event for checkbox state change handling
        checkbox.CheckedChanged += (o, args) => RaiseOnChanged();
    }

    #endregion
}