using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_ResetPassword : CMSUserControl
{
    #region "Constants"

    /// <summary>
    /// Identifies key used to store reset password request identifier
    /// </summary>
    private const string RESET_REQUEST_ID = "UserPasswordRequestID";

    #endregion


    #region "Variables"

    private string siteName;
    private string hash;
    private string time;
    private string returnUrl;
    private double interval;
    private int policyReq;
    private int pwdExp;
    private string securedAreasLogonUrl;

    #endregion


    #region "Properties"

    /// <summary>
    /// Text shown if request hash isn't found.
    /// </summary>
    public string InvalidRequestText
    {
        get;
        set;
    }


    /// <summary>
    /// Text shown when request time was exceeded
    /// </summary>
    public string ExceededIntervalText
    {
        get;
        set;
    }


    /// <summary>
    /// Url on which is user redirected after successful password reset.
    /// </summary>
    public string RedirectUrl
    {
        get;
        set;
    }

    /// <summary>
    /// E-mail address from which e-mail is sent.
    /// </summary>
    public string SendEmailFrom
    {
        get;
        set;
    }


    /// <summary>
    /// Text shown when password reset was successful.
    /// </summary>
    public string SuccessText
    {
        get;
        set;
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack() && !IsCancellationRequest())
        {
            // Clear session value
            ClearResetRequestID();
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        hash = QueryHelper.GetString("hash", string.Empty);
        time = QueryHelper.GetString("datetime", string.Empty);
        policyReq = QueryHelper.GetInteger("policyreq", 0);
        pwdExp = QueryHelper.GetInteger("exp", 0);

        // Prepare URL of logon page
        securedAreasLogonUrl = AuthenticationHelper.DEFAULT_LOGON_PAGE;
        securedAreasLogonUrl = UrlResolver.ResolveUrl(securedAreasLogonUrl);

        returnUrl = QueryHelper.GetString("returnurl", securedAreasLogonUrl);

        rfvConfirmPassword.Text = GetString("general.requiresvalue");

        siteName = SiteContext.CurrentSiteName;

        // Get interval from settings
        interval = SettingsKeyInfoProvider.GetDoubleValue("CMSResetPasswordInterval", siteName);

        // Prepare failed message
        string invalidRequestMessage = DataHelper.GetNotEmpty(InvalidRequestText, String.Format(ResHelper.GetString("membership.passwresetfailed"), URLHelper.AddParameterToUrl(securedAreasLogonUrl, "forgottenpassword", "1")));

        // Reset password cancellation
        if (IsCancellationRequest())
        {
            // Get user info
            UserInfo ui = UserInfoProvider.GetUsersDataWithSettings()
                   .WhereEquals("UserPasswordRequestHash", hash)
                   .TopN(1)
                   .FirstOrDefault();

            if (ui != null)
            {
                ui.UserPasswordRequestHash = null;
                UserInfo.Provider.Set(ui);
                ClearResetRequestID();

                ShowInformation(GetString("membership.passwresetcancelled"));
            }
            else
            {
                ShowError(invalidRequestMessage);
            }

            pnlReset.Visible = false;
            return;
        }

        // Reset password request
        if (!RequestHelper.IsPostBack())
        {
            if (policyReq > 0)
            {
                ShowInformation(GetString("passwordpolicy.policynotmet") + "<br />" + passStrength.GetPasswordPolicyHint());
            }

            UserInfo ui;

            // Get user info
            int userId = GetResetRequestID();
            if (userId > 0)
            {
                // Invalidation forces user info to load user settings from DB and not use cached values.
                ui = UserInfo.Provider.Get(userId);
                ui?.Generalized.Invalidate(false);
            }
            else
            {
                ui = UserInfoProvider.GetUsersDataWithSettings()
                    .WhereEquals("UserPasswordRequestHash", hash).TopN(1).FirstOrDefault();
            }

            // There is nobody to reset password for
            if (ui == null)
            {
                return;
            }

            // Validate request
            ResetPasswordResultEnum result = AuthenticationHelper.ValidateResetPassword(ui, hash, time, interval, "Reset password control");

            // Prepare messages
            string resultMessage = string.Empty;

            // Check result
            switch (result)
            {
                case ResetPasswordResultEnum.Success:
                    // Save user to session
                    SetResetRequestID(ui.UserID);

                    // Delete it from user info
                    ui.UserPasswordRequestHash = null;

                    // Invalidate JSON Web Token for user
                    AuthenticationHelper.RegenerateAuthenticationGuid(ui, saveObject: false);

                    UserInfo.Provider.Set(ui);

                    break;

                case ResetPasswordResultEnum.TimeExceeded:
                    resultMessage = DataHelper.GetNotEmpty(ExceededIntervalText, String.Format(ResHelper.GetString("membership.passwreqinterval"), URLHelper.AddParameterToUrl(securedAreasLogonUrl, "forgottenpassword", "1")));
                    break;

                default:
                    resultMessage = invalidRequestMessage;
                    break;
            }

            if (!string.IsNullOrEmpty(resultMessage))
            {
                // Show error message
                ShowError(resultMessage);

                pnlReset.Visible = false;
            }
        }
    }


    /// <summary>
    /// Click event of btnOk.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        if ((passStrength.Text.Length > 0) && rfvConfirmPassword.IsValid)
        {
            if (passStrength.Text == txtConfirmPassword.Text)
            {
                // Check policy
                if (passStrength.IsValid())
                {
                    int userId = GetResetRequestID();

                    // Check if password expired
                    if (pwdExp > 0)
                    {
                        UserInfo ui = UserInfo.Provider.Get(userId);
                        if (!UserInfoProvider.IsUserPasswordDifferent(ui, passStrength.Text))
                        {
                            ShowError(GetString("passreset.newpasswordrequired"));
                            return;
                        }
                    }

                    // Get e-mail address of sender
                    string emailFrom = DataHelper.GetNotEmpty(SendEmailFrom, SettingsKeyInfoProvider.GetValue("CMSSendPasswordEmailsFrom", siteName));

                    // Try to reset password and show result to user
                    bool success;
                    string resultText = AuthenticationHelper.ResetPassword(hash, time, userId, interval, passStrength.Text, "Reset password control", emailFrom, siteName, null, out success, InvalidRequestText, ExceededIntervalText);

                    // If password reset was successful
                    if (success)
                    {
                        ClearResetRequestID();

                        // Redirect to specified URL
                        if (!string.IsNullOrEmpty(RedirectUrl))
                        {
                            URLHelper.Redirect(UrlResolver.ResolveUrl(RedirectUrl));
                        }

                        // Get proper text
                        ShowConfirmation(DataHelper.GetNotEmpty(SuccessText, resultText));
                        pnlReset.Visible = false;
                        returnUrl = URLHelper.IsLocalUrl(returnUrl) ? ResolveUrl(returnUrl) : securedAreasLogonUrl;
                        lblLogonLink.Text = String.Format(GetString("memberhsip.logonlink"), HTMLHelper.EncodeForHtmlAttribute(returnUrl));
                    }
                    else
                    {
                        ShowError(resultText);
                    }
                }
                else
                {
                    ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
                }
            }
            else
            {
                ShowError(GetString("passreset.notmatch"));
            }
        }
        else
        {
            ShowError(GetString("general.requiresvalue"));
        }
    }

    #endregion


    #region "Helper methods"

    private static bool IsCancellationRequest()
    {
        return QueryHelper.GetBoolean("cancel", false);
    }


    /// <summary>
    /// Gets reset password request identifier from session
    /// </summary>
    private int GetResetRequestID()
    {
        return ValidationHelper.GetInteger(SessionHelper.GetValue(RESET_REQUEST_ID), 0);
    }


    /// <summary>
    /// Stores reset password request identifier to session
    /// </summary>
    /// <param name="requestId">Request ID to be stored</param>
    private void SetResetRequestID(int requestId)
    {
        SessionHelper.SetValue(RESET_REQUEST_ID, requestId);
    }


    /// <summary>
    /// Removes reset password request identifier from session
    /// </summary>
    private void ClearResetRequestID()
    {
        SessionHelper.Remove(RESET_REQUEST_ID);
    }

    #endregion
}