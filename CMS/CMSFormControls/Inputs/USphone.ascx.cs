using System;
using System.Text.RegularExpressions;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Inputs_USphone : FormEngineUserControl
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

            txt1st.Enabled = value;
            txt2nd.Enabled = value;
            txt3rd.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (IsEmpty())
            {
                return String.Empty;
            }

            return String.Format("({0}) {1}-{2}", txt1st.Text, txt2nd.Text, txt3rd.Text);
        }
        set
        {
            string number = (string)value;
            Clear();

            if (!String.IsNullOrEmpty(number))
            {
                Match match = ValidationHelper.UsPhoneNumberRegExp.Match(number);
                if (match.Success)
                {
                    txt1st.Text = match.Groups[1].ToString();
                    txt2nd.Text = match.Groups[2].ToString();
                    txt3rd.Text = match.Groups[3].ToString();
                }
            }
        }
    }


    /// <summary>
    /// First "three digits" input client ID.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return txt1st.ClientID;
        }
    }


    /// <summary>
    /// Clears current value.
    /// </summary>
    public void Clear()
    {
        txt1st.Text = String.Empty;
        txt2nd.Text = String.Empty;
        txt3rd.Text = String.Empty;
    }


    /// <summary>
    /// Returns true if the number is empty.
    /// </summary>
    public bool IsEmpty()
    {
        return (DataHelper.IsEmpty(txt1st.Text) && DataHelper.IsEmpty(txt2nd.Text) && DataHelper.IsEmpty(txt3rd.Text));
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (IsEmpty())
        {
            return true;
        }

        // US phone number must be in form: (ddd) ddd-dddd, where 'd' is digit
        Validator val = new Validator();
        string result = val.IsRegularExp(txt1st.Text, @"\d{3}", "error").IsRegularExp(txt2nd.Text, @"\d{3}", "error").IsRegularExp(txt3rd.Text, @"\d{4}", "error").Result;

        if (result != "")
        {
            ValidationError = GetString("USPhone.ValidationError");
            return false;
        }
        return true;
    }
}