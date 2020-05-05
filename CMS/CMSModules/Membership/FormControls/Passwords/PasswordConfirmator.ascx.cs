using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Membership_FormControls_Passwords_PasswordConfirmator : FormEngineUserControl
{
    #region "Properties"

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
            txtConfirmPassword.Enabled = value;
            passStrength.Enabled = value;
        }
    }


    /// <summary>
    /// Returns inserted password.
    /// </summary>
    public override object Value
    {
        get
        {
            // Check if text is set
            if (string.IsNullOrEmpty(passStrength.Text))
            {
                return string.Empty;
            }

            return passStrength.Text;
        }
        set
        {
            passStrength.Text = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Client ID of primary input control.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return passStrength.ValueElementID;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        passStrength.ShowStrengthIndicator = ValidationHelper.GetBoolean(GetValue("showstrength"), true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check if passwords match
        if (passStrength.Text != txtConfirmPassword.Text)
        {
            ValidationError = GetString("PassConfirmator.PasswordDoNotMatch");
        }

        // Check regular expression
        string regularExpression = FieldInfo.RegularExpression;
        if ((!String.IsNullOrEmpty(regularExpression)) && (new Validator().IsRegularExp(passStrength.Text, regularExpression, "error").Result.EqualsCSafe("error", true)))
        {
            ValidationError = GetString("PassConfirmator.InvalidPassword");
        }

        // Check min length
        int minLength = ValidationHelper.GetInteger(FieldInfo.MinValue, -1);
        if ((minLength > 0) && (passStrength.Text.Length < minLength))
        {
            ValidationError = string.Format(GetString("PassConfirmator.PasswordLength"), minLength);
        }

        // Check password policy
        if (!passStrength.IsValid())
        {
            ValidationError = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
        }

        if (!String.IsNullOrEmpty(ValidationError))
        {
            Value = string.Empty;
            return false;
        }

        return true;
    }

    #endregion
}