using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Inputs_InternationalPhone : FormEngineUserControl
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
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (String.IsNullOrWhiteSpace(txt1st.Text) && String.IsNullOrWhiteSpace(txt2nd.Text))
            {
                return "";
            }

            return ("+" + txt1st.Text.Trim() + " " + txt2nd.Text.Trim());
        }
        set
        {
            string number = (string)value;
            // Parse numbers from incoming string.
            ParsePhoneNumberString(number);
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (!String.IsNullOrEmpty(txt1st.Text) || !String.IsNullOrEmpty(txt2nd.Text))
        {
            Validator val = new Validator();
            // International phone number must be in the form '+d{1-4} d{1-20}' where 'd' is digit.
            string result = val.IsRegularExp(txt1st.Text, @"\d{1,4}", "error").IsRegularExp(txt2nd.Text, @"\d[0-9\s]{1,18}\d", "error").Result;

            if (result != "")
            {
                ValidationError = GetString("InternationalPhone.ValidationError");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Parses given phone number and sets textboxes' values.
    /// </summary>
    /// <param name="phoneNumber">Phone number to parse</param>
    private void ParsePhoneNumberString(string phoneNumber)
    {
        if (!String.IsNullOrEmpty(phoneNumber))
        {
            int spaceIndex = phoneNumber.IndexOfCSafe(" ");
            if (spaceIndex <= 0)
            {
                txt2nd.Text = phoneNumber.Trim().TrimStart('+');
            }
            else
            {
                int partOneLength = spaceIndex - 1;
                int partTwoStart = spaceIndex + 1;
                txt1st.Text = phoneNumber.Substring(1, partOneLength);
                txt2nd.Text = phoneNumber.Substring(partTwoStart, phoneNumber.Length - partTwoStart);
            }
        }
    }
}