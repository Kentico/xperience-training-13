using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_Password : FormEngineUserControl
{
    #region "Variables"

    /// <summary>
    /// Default hidden password value.
    /// </summary>
    private const string HIDDEN_PASSWORD = "********";


    /// <summary>
    /// Current password.
    /// </summary>
    private string mPassword = String.Empty;


    /// <summary>
    /// Indicates whether textbox should be filled.
    /// </summary>
    private bool fillPassword;


    /// <summary>
    /// Indicates whether load phase finished.
    /// </summary>
    private bool loadFinished;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public override object Value
    {
        get
        {
            return Password;
        }
        set
        {
            Password = Convert.ToString(value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled.
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
    /// Gets or sets the current password.
    /// </summary>
    public string Password
    {
        get
        {
            // Get password from textbox if is set
            if (loadFinished && RequestHelper.IsPostBack() && (CMSString.Compare(txtPassword.Text, HIDDEN_PASSWORD, StringComparison.CurrentCulture) != 0))
            {
                Password = txtPassword.Text;
            }
            else
            {
                mPassword = GetPassword();
            }

            return mPassword;
        }
        set
        {
            mPassword = value;
            SetPassword(mPassword);
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with password.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtPassword.ClientID;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad - Set loading flag.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;
        loadFinished = true;
    }


    /// <summary>
    /// Sets password in plaintext form.
    /// </summary>
    /// <param name="password">Password value</param>
    protected void SetPassword(string password)
    {
        fillPassword = !String.IsNullOrEmpty(password);

        mPassword = password;
    }


    /// <summary>
    /// Returns password in plaintext form.
    /// </summary>
    protected string GetPassword()
    {
        return ValidationHelper.GetString(mPassword, String.Empty);
    }


    /// <summary>
    /// OnPreRender override - set password text.
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected override void OnPreRender(EventArgs e)
    {
        if (fillPassword)
        {
            txtPassword.Attributes.Add("value", HIDDEN_PASSWORD);
        }
        else
        {
            txtPassword.Attributes.Remove("value");
        }

        base.OnPreRender(e);
    }

    #endregion
}