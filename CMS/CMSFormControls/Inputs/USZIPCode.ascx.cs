using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Inputs_USZIPCode : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtZIPCode.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtZIPCode.Text;
        }
        set
        {
            txtZIPCode.Text = (string)value;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (!DataHelper.IsEmpty(txtZIPCode.Text.Trim()))
        {
            // US ZIP Code must have 5 digits.
            Validator val = new Validator();
            string result = val.IsRegularExp(txtZIPCode.Text, @"\d{5}", "error").Result;

            if (result != "")
            {
                ValidationError = GetString("USZIPcode.ValidationError");
                return false;
            }
        }
        return true;
    }
}