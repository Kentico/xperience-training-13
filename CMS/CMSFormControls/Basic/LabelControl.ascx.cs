using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Basic_LabelControl : ReadOnlyFormEngineUserControl
{
    #region "Variables"

    private object mValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return label.Enabled;
        }
        set
        {
            label.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the transformation code to use to transform the value.
    /// </summary>
    public string Transformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Transformation"), "");
        }
        set
        {
            SetValue("Transformation", value);
        }
    }


    /// <summary>
    /// Gets or sets the output format which can contains macros.
    /// </summary>
    public string OutputFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OutputFormat"), "");
        }
        set
        {
            SetValue("OutputFormat", value);
        }
    }


    /// <summary>
    /// Gets or sets whether the macros are resolved.
    /// </summary>
    public bool ResolveMacros
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveMacros"), true);
        }
        set
        {
            SetValue("ResolveMacros", value);
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            // Convert the value to a proper type
            if (FieldInfo != null)
            {
                mValue = ConvertInputValue(value);
            }
            else
            {
                mValue = ValidationHelper.GetString(value, String.Empty);
            }
        }
    }


    /// <summary>
    /// Gets the label text
    /// </summary>
    /// <param name="value">Field value</param>
    private string GetLabelText(object value)
    {
        if (!String.IsNullOrEmpty(OutputFormat))
        {
            return OptionallyResolveMacros(OutputFormat);
        }

        string txt;

        // Try to find the transformation
        if (!string.IsNullOrEmpty(Transformation))
        {
            object resolvedValue = OptionallyResolveMacros(ValidationHelper.GetString(value, ""));
            if (UniGridTransformations.Global.ExecuteTransformation(label, Transformation, ref resolvedValue))
            {
                return ValidationHelper.GetString(resolvedValue, "");
            }
        }

        if (FieldInfo != null)
        {
            txt = GetStringValue(value);
        }
        else
        {
            txt = ValidationHelper.GetString(value, "");
        }

        return OptionallyResolveMacros(txt);
    }


    private string OptionallyResolveMacros(string text)
    {
        return ResolveMacros ? ContextResolver.ResolveMacros(text) : text;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Apply styles to control
        if (!String.IsNullOrEmpty(CssClass))
        {
            label.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(label.CssClass))
        {
            label.CssClass = "LabelField form-control-text";
        }

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            label.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        label.Text = GetLabelText(mValue);
    }

    #endregion
}