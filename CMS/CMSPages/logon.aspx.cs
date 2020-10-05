using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.MembershipProvider;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSPages_logon : CMSPage, ICallbackEventHandler
{
    #region "Variables"

    private LocalizedLabel mFailureLabel;
    private bool? mShowForgottenPassword;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the main text resource string
    /// </summary>
    protected string MainTextResString
    {
        get;
        set;
    }

    /// <summary>
    /// Failure text label.
    /// </summary>
    public LocalizedLabel FailureLabel
    {
        get
        {
            return mFailureLabel ?? (mFailureLabel = (LocalizedLabel)Login1.FindControl("FailureText"));
        }
    }


    /// <summary>
    /// Returns whether is page in forgotten password "mode"
    /// </summary>
    private bool IsForgottenPassword
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ForgottenPassword"], false);
        }
        set
        {
            ViewState["ForgottenPassword"] = value;
        }
    }


    /// <summary>
    /// Gets or sets whether link to forgotten password is shown on logon page.
    /// </summary>
    public bool ShowForgottenPassword
    {
        get
        {
            if (mShowForgottenPassword == null)
            {
                mShowForgottenPassword = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSShowForgottenPassLink"], true);
            }

            return mShowForgottenPassword.Value;
        }
        set
        {
            mShowForgottenPassword = value;
        }
    }


    /// <summary>
    /// Gets return URL for logon page
    /// </summary>
    public string ReturnUrl
    {
        get
        {
            return QueryHelper.GetString("returnurl", string.Empty);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        SetBrowserClass();
        AddNoCacheTag();
        HideMessages();

        MainTextResString = "LogonForm.LogOn";

        // Ensure the refresh script
        const string defaultCondition = "((top.frames['cmsdesktop'] != null) || (top.frames['propheader'] != null))";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TopWindow", ScriptHelper.GetScript(" if " + defaultCondition + " { try {top.window.location.reload();} catch(err){} }"));

        // Enable caps lock check
        if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSUseCapsLockNotification"], true))
        {
            RegisterCAPSLOCKScript();
            TextBox txtPassword = (TextBox)Login1.FindControl("Password");
            if (txtPassword != null)
            {
                txtPassword.Attributes.Add("onkeypress", "CheckCapsLock(event)");
            }
        }

        LocalizedLabel lblItem = (LocalizedLabel)Login1.FindControl("lblUserName");
        if (lblItem != null)
        {
            lblItem.Text = "{$LogonForm.UserName$}";
        }
        lblItem = (LocalizedLabel)Login1.FindControl("lblPassword");
        if (lblItem != null)
        {
            lblItem.Text = "{$LogonForm.Password$}";
        }

        // Display culture link due to value of the key stored in the web.config file
        bool showCultureSelector = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSShowLogonCultureSelector"], true);
        if (showCultureSelector)
        {
            LocalizedLinkButton lnkLanguage = (LocalizedLinkButton)Login1.FindControl("lnkLanguage");
            if (lnkLanguage != null)
            {
                lnkLanguage.Visible = true;

                // Ensure language selection panel functionality
                HtmlGenericControl pnlLanguage = (HtmlGenericControl)Login1.FindControl("pnlLanguage");
                if (pnlLanguage != null)
                {
                    ltlScript.Text = ScriptHelper.GetScript("function ShowLanguage(id){var panel=document.getElementById(id);if(panel!=null){panel.style.display=(panel.style.display == 'block')?'none':'block';}}");
                    lnkLanguage.Attributes.Add("onclick", "ShowLanguage('" + pnlLanguage.ClientID + "'); return false;");
                }
            }
        }

        // Set up forgotten password link
        if (ShowForgottenPassword)
        {
            LocalizedLinkButton lnkPassword = (LocalizedLinkButton)Login1.FindControl("lnkPassword");
            if (lnkPassword != null)
            {
                lnkPassword.Visible = true;
                lnkPassword.Click += lnkPassword_Click;
            }
        }

        PlaceHolder plcRememberMe = (PlaceHolder)Login1.FindControl("plcRememberMe");
        if ((MFAuthenticationHelper.IsMultiFactorAuthEnabled) && (plcRememberMe != null))
        {
            plcRememberMe.Visible = false;
        }


        LocalizedButton btnItem = (LocalizedButton)Login1.FindControl("LoginButton");
        if (btnItem != null)
        {
            btnItem.Text = "{$LogonForm.LogOnButton$}";
            btnItem.Click += btnItem_Click;
        }

        // Load UI cultures for the dropdown list
        if (!RequestHelper.IsPostBack())
        {
            LoadCultures();
        }

        Login1.LoggingIn += Login1_LoggingIn;
        Login1.LoggedIn += Login1_LoggedIn;
        Login1.LoginError += Login1_LoginError;
        Login1.Authenticate += Login1_Authenticate;

        if (!RequestHelper.IsPostBack())
        {
            Login1.UserName = QueryHelper.GetString("username", String.Empty); 
        }

        // Ensure username textbox focus
        CMSTextBox txtUserName = (CMSTextBox)Login1.FindControl("UserName");
        if (txtUserName != null)
        {
            ScriptHelper.RegisterStartupScript(this, GetType(), "SetFocus", ScriptHelper.GetScript("var txt=document.getElementById('" + txtUserName.ClientID + "');if(txt!=null){txt.focus();}"));
            txtUserName.EnableAutoComplete = SecurityHelper.IsAutoCompleteEnabledForLogin(SiteContext.CurrentSiteName);
        }

        if (QueryHelper.GetBoolean("forgottenpassword", false))
        {
            SetForgottenPasswordMode();
        }   

        // Register script to update logon error message
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append(@"
var failedText_", ClientID, "= document.getElementById('", FailureLabel.ClientID, @"');

function UpdateLabel_", ClientID, @"(content, context) {
    var lbl = document.getElementById(context);   
    if(lbl)
    {
        lbl.innerHTML = content;
        lbl.className = """";
    }
}");
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InvalidLogonAttempts_" + ClientID, sbScript.ToString(), true);
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Keep latest application after logon 
        PortalScriptHelper.RegisterApplicationStorageScript(Page);

        base.OnPreRender(e);
    }


    private void btnItem_Click(object sender, EventArgs e)
    {
        // Check if should send password
        if (IsForgottenPassword)
        {
            SetForgottenPasswordMode();

            TextBox txtUserName = (TextBox)Login1.FindControl("UserName");
            var email = "";
            if (txtUserName != null)
            {
                email = txtUserName.Text.Trim();
            }

            if (!string.IsNullOrEmpty(email) && ValidationHelper.IsEmail(email))
            {
                // Reset password
                string siteName = SiteContext.CurrentSiteName;

                // Prepare URL to which may user return after password reset
                string returnUrl = RequestContext.CurrentURL;
                if (!string.IsNullOrEmpty(Login1.UserName))
                {
                    returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", Login1.UserName);
                }

                AuthenticationHelper.ForgottenEmailRequest(email, siteName, "Logon page", SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendPasswordEmailsFrom"), null, AuthenticationHelper.GetResetPasswordUrl(siteName), returnUrl);
                DisplayInformation(String.Format(GetString("LogonForm.EmailSent"), email));
            }
            else
            {
                DisplayError(GetString("LogonForm.EmailNotValid"));
            }
        }
    }


    private void lnkPassword_Click(object sender, EventArgs e)
    {
        if (!IsForgottenPassword)
        {
            SetForgottenPasswordMode();
        }
        else
        {
            string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "forgottenpassword");
            URLHelper.Redirect(url);
        }
    }


    private void Login1_LoginError(object sender, EventArgs e)
    {
        bool showError = true;
        if (FailureLabel != null)
        {
            if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName) && MembershipContext.UserAccountLockedDueToInvalidLogonAttempts)
            {
                DisplayAccountLockedError(GetString("invalidlogonattempts.unlockaccount.accountlocked"));
            }
            else if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName) && MembershipContext.UserAccountLockedDueToPasswordExpiration)
            {
                DisplayAccountLockedError(GetString("passwordexpiration.accountlocked"));
            }
            else if (MembershipContext.UserIsPartiallyAuthenticated && !MembershipContext.UserAuthenticationFailedDueToInvalidPasscode)
            {
                if (MembershipContext.MFAuthenticationTokenNotInitialized && MFAuthenticationHelper.DisplaySetupCode)
                {
                    var lblTokenID = Login1.FindControl("lblTokenID") as LocalizedLabel;
                    var plcTokenInfo = Login1.FindControl("plcTokenInfo");

                    if ((lblTokenID != null) && (plcTokenInfo != null))
                    {
                        DisplayWarning(string.Format("<strong>{0}</strong> {1}", GetString("mfauthentication.isRequired"), GetString("mfauthentication.token.get")));

                        lblTokenID.Text = MFAuthenticationHelper.GetSetupCodeForUser(Login1.UserName);
                        plcTokenInfo.Visible = true;
                    }
                }

                showError = false;
            }
            else if (!MembershipContext.UserIsPartiallyAuthenticated)
            {
                // Show login and password screen
                var plcPasscodeBox = Login1.FindControl("plcPasscodeBox");
                var plcLoginInputs = Login1.FindControl("plcLoginInputs");
                var plcTokenInfo = Login1.FindControl("plcTokenInfo");
                if (plcLoginInputs != null)
                {
                    plcLoginInputs.Visible = true;
                }
                if (plcPasscodeBox != null)
                {
                    plcPasscodeBox.Visible = false;
                }
                if (plcTokenInfo != null)
                {
                    plcTokenInfo.Visible = false;
                }
            }

            if (showError && string.IsNullOrEmpty(FailureLabel.Text))
            {
                DisplayError(GetString("Login_FailureText"));
            }
        }
    }


    private void Login1_LoggedIn(object sender, EventArgs e)
    {
        // ScreenLock - unlock screen
        IsScreenLocked = false;

        // Ensure response cookie
        CookieHelper.EnsureResponseCookie(FormsAuthentication.FormsCookieName);

        // Set cookie expiration
        if (Login1.RememberMeSet)
        {
            CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddYears(1), false);
        }
        else
        {
            // Extend the expiration of the authentication cookie if required
            if (!AuthenticationHelper.UseSessionCookies && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddMinutes(Session.Timeout), false);
            }
        }

        // Current username
        string userName = Login1.UserName;

        // Get info on the authenticated user
        UserInfo ui = UserInfo.Provider.Get(userName);

        // Check whether safe user name is required and if so get safe username
        if (ui == null && AuthenticationMode.IsMixedAuthentication() && UserInfoProvider.UseSafeUserName)
        {
            userName = ValidationHelper.GetSafeUserName(userName, SiteContext.CurrentSiteName);
            
            AuthenticationHelper.AuthenticateUser(userName, Login1.RememberMeSet);
        }

        // Set culture
        CMSDropDownList drpCulture = (CMSDropDownList)Login1.FindControl("drpCulture");
        if (drpCulture != null)
        {
            string selectedCulture = drpCulture.SelectedValue;

            // Not the default culture
            if (selectedCulture != "")
            {
                // Update the user
                if (ui != null)
                {
                    ui.PreferredUICultureCode = selectedCulture;
                    UserInfo.Provider.Set(ui);
                }

                // Update current user
                MembershipContext.AuthenticatedUser.PreferredUICultureCode = selectedCulture;
            }
        }

        URLHelper.LocalRedirect(ReturnUrl);
    }


    private void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        Login1.RememberMeSet = ((CMSCheckBox)Login1.FindControl("chkRememberMe")).Checked;
    }


    /// <summary>
    /// Handling login authenticate event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Authenticate event arguments.</param>
    private void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        if (MFAuthenticationHelper.IsMultiFactorRequiredForUser(Login1.UserName))
        {
            var plcPasscodeBox = Login1.FindControl("plcPasscodeBox");
            var plcLoginInputs = Login1.FindControl("plcLoginInputs");
            var txtPasscode = Login1.FindControl("txtPasscode") as CMSTextBox;

            if (txtPasscode == null)
            {
                return;
            }
            if (plcPasscodeBox == null)
            {
                return;
            }
            if (plcLoginInputs == null)
            {
                return;
            }
            
            // Handle passcode
            string passcode = txtPasscode.Text;
            txtPasscode.Text = string.Empty;

            var provider = new CMSMembershipProvider();

            // Validate username and password
            if (plcLoginInputs.Visible)
            {
                if (provider.MFValidateCredentials(Login1.UserName, Login1.Password))
                {
                    // Show passcode screen
                    plcLoginInputs.Visible = false;
                    plcPasscodeBox.Visible = true;
                }
            }
            // Validate passcode
            else
            {
                if (provider.MFValidatePasscode(Login1.UserName, passcode))
                {
                    e.Authenticated = true;
                }
            }
        }
        else
        {
            try
            {
                e.Authenticated = Membership.Provider.ValidateUser(Login1.UserName, Login1.Password);
            }
            catch (ConfigurationException ex)
            {
                Service.Resolve<IEventLogService>().LogException("LogonPage", "VALIDATEUSER", ex);
                var provider = new CMSMembershipProvider();
                e.Authenticated = provider.ValidateUser(Login1.UserName, Login1.Password);
            }
        }
    }


    /// <summary>
    /// Load UI cultures for the dropdown list.
    /// </summary>
    private void LoadCultures()
    {
        CMSDropDownList drpCulture = (CMSDropDownList)Login1.FindControl("drpCulture");
        if (drpCulture != null)
        {
            DataSet ds = CultureInfoProvider.GetUICultures();
            DataView dvCultures = ds.Tables[0].DefaultView;
            dvCultures.Sort = "CultureName ASC";

            drpCulture.DataValueField = "CultureCode";
            drpCulture.DataTextField = "CultureName";
            drpCulture.DataSource = dvCultures;
            drpCulture.DataBind();

            // Add default value
            drpCulture.Items.Insert(0, new ListItem(GetString("LogonForm.DefaultCulture"), ""));

            LocalizedLabel lblCulture = (LocalizedLabel)Login1.FindControl("lblCulture");
            if (lblCulture != null)
            {
                lblCulture.AssociatedControlID = drpCulture.ID;
                lblCulture.Text = GetString("general.select");
                lblCulture.Display = false;
            }
        }
    }


    /// <summary>
    /// Registers the script to handle the CAPSLOCK check.
    /// </summary>
    private void RegisterCAPSLOCKScript()
    {
        string script =
            "function OnCapslockOn() {\n " +
            "document.getElementById('JavaScript-Errors').innerHTML = " +
            "'<div class=\"alert alert-warning\"><span class=\"alert-icon\"><i class=\"icon-exclamation-triangle\"></i></span><div class=\"alert-label\">" + GetString(GetString("General.Capslock")) + "</div>'" +
            "; \n" +
            "} \n" +
            "function OnCapslockOff() {\n " +
            "var elem = document.getElementById('JavaScript-Errors');\n" +
            "if(elem.innerHTML != ''){elem.innerHTML = '';} \n" +
            "} \n";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "CapsLockHandling", ScriptHelper.GetScript(script) + ScriptHelper.CapslockScript);
    }


    /// <summary>
    /// Sets forgotten password mode.
    /// </summary>
    private void SetForgottenPasswordMode()
    {
        var txtUserName = (CMSTextBox)Login1.FindControl("UserName");
        if (txtUserName != null)
        {
            // Clear input field in case pre-filled user name is not a valid email
            if (!ValidationHelper.IsEmail(txtUserName.Text))
            {
                txtUserName.Text = "";
                Login1.UserName = "";            
            }
        }
 
        var plcPasscodeBox = Login1.FindControl("plcPasscodeBox");
        if (plcPasscodeBox != null)
        {
            plcPasscodeBox.Visible = false;
        }
        
        var plcLoginInputs = Login1.FindControl("plcLoginInputs");
        if (plcLoginInputs != null)
        {
            plcLoginInputs.Visible = true;
        }

        var divPassword = Login1.FindControl("divPassword");
        if (divPassword != null)
        {
            divPassword.Visible = false;
        }

        var plcRememberMe = Login1.FindControl("plcRememberMe");
        if (plcRememberMe != null)
        {
            plcRememberMe.Visible = false;
        }

        var plcTokenInfo = Login1.FindControl("plcTokenInfo");
        if (plcTokenInfo != null)
        {
            plcTokenInfo.Visible = false;
        }

        LocalizedButton btnItem = (LocalizedButton)Login1.FindControl("LoginButton");
        if (btnItem != null)
        {
            btnItem.ResourceString = "LogonForm.SendRequest";
            btnItem.CommandName = string.Empty;
        }

        MainTextResString = "logonform.lnkpasswordretrieval";

        LocalizedLabel lblUserName = (LocalizedLabel)Login1.FindControl("lblUserName");
        if (lblUserName != null)
        {
            lblUserName.ResourceString = "logonform.lblpasswordretrieval";
        }

        RequiredFieldValidator rfvUserName = (RequiredFieldValidator)Login1.FindControl("rfvUserNameRequired");
        if (rfvUserName != null)
        {
            rfvUserName.ToolTip = GetString("LogonForm.NameOrEmailRequired");
            rfvUserName.Text = rfvUserName.ErrorMessage = GetString("logonform.rqvalue");
        }

        var lnkPassword = Login1.FindControl("lnkPassword") as LocalizedLinkButton;
        if (lnkPassword != null)
        {
            lnkPassword.ResourceString = "LogonForm.BackToLogon";
        }

        IsForgottenPassword = true;
    }


    /// <summary>
    /// Displays error.
    /// </summary>
    /// <param name="msg">Message.</param>
    private void DisplayError(string msg)
    {
        var plcError = Login1.FindControl("plcError");

        if (plcError != null)
        {
            FailureLabel.Text = msg;
            plcError.Visible = !string.IsNullOrEmpty(msg);
        }
    }


    /// <summary>
    /// Displays information.
    /// </summary>
    /// <param name="msg">Message.</param>
    private void DisplayInformation(string msg)
    {
        var plcInfo = Login1.FindControl("plcInfo");
        var txtInfo = (LocalizedLabel)Login1.FindControl("txtInfo");

        if (plcInfo != null)
        {
            txtInfo.Text = msg;
            plcInfo.Visible = !string.IsNullOrEmpty(msg);
        }
    }


    /// <summary>
    /// Hides error, warning and information messages.
    /// </summary>
    private void HideMessages()
    {
        var plcWarning = Login1.FindControl("plcWarning");
        var plcError = Login1.FindControl("plcError");
        var plcInfo = Login1.FindControl("plcInfo");

        if (plcWarning != null)
        {
            plcWarning.Visible = false;
        }

        if (plcError != null)
        {
            plcError.Visible = false;
        }

        if (plcInfo != null)
        {
            plcInfo.Visible = false;
        }
    }


    /// <summary>
    /// Displays error.
    /// </summary>
    /// <param name="msg">Message.</param>
    private void DisplayWarning(string msg)
    {
        var plcWarning = Login1.FindControl("plcWarning");
        var txtWarning = (LocalizedLabel)Login1.FindControl("txtWarning");

        if (plcWarning != null)
        {
            plcWarning.Visible = true;
            txtWarning.Text = msg;
        }
    }


    /// <summary>
    /// Displays locked account error message.
    /// </summary>
    /// <param name="specificMessage">Specific part of the message.</param>
    private void DisplayAccountLockedError(string specificMessage)
    {
        string link = "<a href=\"#\" onclick=\"" + Page.ClientScript.GetCallbackEventReference(this, "null", "UpdateLabel_" + ClientID, "'" + FailureLabel.ClientID + "'") + ";\">" + GetString("general.clickhere") + "</a>";
        DisplayError(string.Format(specificMessage + " " + GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link));
    }


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        string result = "";
        UserInfo user = UserInfo.Provider.Get(Login1.UserName);
        if (user != null)
        {
            string siteName = SiteContext.CurrentSiteName;

            // Prepare return URL
            string returnUrl = RequestContext.CurrentURL;
            if (!string.IsNullOrEmpty(Login1.UserName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", Login1.UserName);
            }

            switch (UserAccountLockCode.ToEnum(user.UserAccountLockReason))
            {
                case UserAccountLockEnum.MaximumInvalidLogonAttemptsReached:
                    result = AuthenticationHelper.SendUnlockAccountRequest(user, siteName, "USERLOGON", SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendPasswordEmailsFrom"), null, returnUrl);
                    break;

                case UserAccountLockEnum.PasswordExpired:
                    result = AuthenticationHelper.SendPasswordRequest(user, siteName, "USERLOGON", SettingsKeyInfoProvider.GetValue(siteName + ".CMSSendPasswordEmailsFrom"), "Membership.PasswordExpired", null, AuthenticationHelper.GetResetPasswordUrl(siteName), returnUrl);
                    break;
            }
        }

        return result;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
    }

    #endregion
}
