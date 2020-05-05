using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

[Obsolete("Use custom implementation instead.")]
public partial class CMSModules_Membership_FormControls_Passwords_EncryptedPassword : FormEngineUserControl
{
    #region "Constants"

    private const string HIDDEN_PASSWORD = "********";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Encrypted password.
    /// </summary>
    private string CryptedPassword
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CryptedPassword"], String.Empty);
        }
        set
        {
            ViewState["CryptedPassword"] = value;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns encrypted password.
    /// </summary>
    public override object Value
    {
        get
        {
            if (String.IsNullOrEmpty(txtPassword.Text))
            {
                return String.Empty;
            }

            if (txtPassword.Text != HIDDEN_PASSWORD)
            {
                CryptedPassword = EncryptionHelper.EncryptData(txtPassword.Text);

                txtPassword.Text = HIDDEN_PASSWORD;
            }

            return CryptedPassword;
        }
        set
        {
            CryptedPassword = ValidationHelper.GetString(value, String.Empty);

            txtPassword.Text = String.IsNullOrEmpty(CryptedPassword) ? String.Empty : HIDDEN_PASSWORD;
        }
    }


    /// <summary>
    /// Indicates whether control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtPassword.Enabled;
        }
        set
        {
            txtPassword.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets max length.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return txtPassword.MaxLength;
        }
        set
        {
            txtPassword.MaxLength = value;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Force show password placeholder
        txtPassword.Attributes["value"] = txtPassword.Text;

        if (!String.IsNullOrEmpty(txtPassword.Text))
        {
            ScriptHelper.RegisterModule(this, "CMS.Membership/EncryptedPassword", new { textboxSelector = '#' + txtPassword.ClientID });
        }
    }

    #endregion
}