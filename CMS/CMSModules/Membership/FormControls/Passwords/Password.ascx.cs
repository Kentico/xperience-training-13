using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Membership_FormControls_Passwords_Password : FormEngineUserControl
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
            txtPassword.Enabled = value;
        }
    }


    /// <summary>
    /// Returns encrypted password.
    /// </summary>
    public override object Value
    {
        get
        {
           return txtPassword.Text;
        }
        set
        {
        }
    }

    #endregion


    /// <summary>
    /// Pre render event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            txtPassword.CssClass = CssClass;
            CssClass = null;
        }

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            txtPassword.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check regular expression
        string regularExpression = FieldInfo.RegularExpression;
        if ((!String.IsNullOrEmpty(regularExpression)) && (new Validator().IsRegularExp(txtPassword.Text, regularExpression, "error").Result == "error"))
        {
            ValidationError = GetString("PassConfirmator.InvalidPassword");
        }

        // Check min length
        int minLength = ValidationHelper.GetInteger(FieldInfo.MinValue, -1);
        if ((minLength > 0) && (txtPassword.Text.Length < minLength))
        {
            ValidationError = String.Format(GetString("PassConfirmator.PasswordLength"), minLength);
        }

        // Check max length
        int maxLength = ValidationHelper.GetInteger(FieldInfo.MaxValue, -1);
        if ((maxLength > 0) && (txtPassword.Text.Length > maxLength))
        {
            ValidationError = String.Format(GetString("basicform.errortexttoolong"));
        }

        string siteName = SiteContext.CurrentSiteName;

        // Check password policy
        if (!SecurityHelper.CheckPasswordPolicy(txtPassword.Text, siteName))
        {
            ValidationError = AuthenticationHelper.GetPolicyViolationMessage(siteName);
        }

        if (!String.IsNullOrEmpty(ValidationError))
        {
            Value = string.Empty;
            return false;
        }

        return true;
    }
}