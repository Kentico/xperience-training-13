using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSFormControls_Inputs_TextboxDoubleValidator : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the value of this form control.
    /// </summary>
    [Browsable(false)]
    public override object Value
    {
        get
        {
            String val = txtValue.Text.Trim();

            if (CultureInfo.CurrentCulture.Name != CultureHelper.EnglishCulture.Name)
            {
                // If value is double, convert it to default english format
                double dVal = ValidationHelper.GetDouble(val, 0.0);
                if (dVal != 0)
                {
                    val = dVal.ToString(CultureHelper.EnglishCulture);
                }
            }

            return val;
        }
        set
        {
            String val = ValidationHelper.GetString(value, String.Empty);

            if (CultureInfo.CurrentCulture.Name != CultureHelper.EnglishCulture.Name)
            {
                // Convert to english value format
                double dVal = ValidationHelper.GetDouble(val, 0.0, CultureHelper.EnglishCulture);
                if (dVal != 0)
                {
                    val = dVal.ToString();
                }
            }

            txtValue.Text = val;
        }
    }


    /// <summary>
    /// Gets or sets whether this form control is enabled.
    /// </summary>
    [Browsable(true)]
    [Description("Determines whether this form control is enabled")]
    [Category("Form Control")]
    [DefaultValue(true)]
    public override bool Enabled
    {
        get
        {
            return txtValue.Enabled;
        }
        set
        {
            txtValue.Enabled = value;
        }
    }


    /// <summary>
    /// Max allowed input of textbox
    /// </summary>
    [Browsable(true)]
    public int MaxLength
    {
        get
        {
            return txtValue.MaxLength;
        }
        set
        {
            txtValue.MaxLength = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates control. Returns true, if control contains double or macro.
    /// </summary>
    public override bool IsValid()
    {
        String val = txtValue.Text.Trim();

        if (String.IsNullOrEmpty(val))
        {
            return true;
        }

        if (MacroProcessor.ContainsMacro(val) || ValidationHelper.IsDouble(val))
        {
            return true;
        }

        ErrorMessage = GetString("ecommerce.settings.doubleormacro");
        return false;
    }

    #endregion
}