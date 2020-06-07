using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Membership;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAdminControls_UI_ScreenLock_ScreenLockDialog : CMSUserControl, ICallbackEventHandler
{
    #region "Private variables"

    private TimeSpan timeLeft = TimeSpan.Zero;

    private int minutesToLock = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSScreenLockInterval");
    private int secondsToWarning = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSScreenLockWarningInterval");

    private bool userIsLoggingOut = false;
    private bool userAsksForState = false;
    private bool userCanceling = false;
    private bool userValidates = false;
    private bool userWaitingForPasscode = false;
    private bool passcValidates = false;
    private string validatePassword = String.Empty;
    private string validatePasscode = String.Empty;

    #endregion


    #region "Private constants"

    /// <summary>
    /// Argument separator.
    /// </summary>
    private const string ARGUMENTS_SEPARATOR = "|";


    /// <summary>
    /// Start index for password received in message.
    /// </summary>
    private const int START_INDEX_FOR_PASSWORD = 9;


    /// <summary>
    /// Start index for passcode received in message.
    /// </summary>
    private const int START_INDEX_FOR_PASSCODE = 14;

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var enabled = SecurityHelper.IsScreenLockEnabled(SiteContext.CurrentSiteName);

        Visible = enabled;

        if (!enabled)
        {
            return;
        }

        // Set title text and image
        screenLockTitle.TitleText = GetString("settingscategory.cmsscreenlock");
        lblInstructions.Text = GetString("screenlock.instruction");
        
        var user = GetUserForAuthentication();
        txtUserName.Text = user.UserName;

        // Set account locked information
        if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName))
        {
            lblScreenLockWarningLogonAttempts.Text = GetString("invalidlogonattempts.unlockaccount.accountlocked");
        }
        else
        {
            lblScreenLockWarningLogonAttempts.Text = GetString("screenlock.loginfailure");
        }

        lblScreenLockWarningLogonAttempts.Text += " " + GetString("screenlock.loggedout");

        // Hide password field for Active directory users when Windows authentication is used
        if (user.UserIsDomain && AuthenticationMode.IsWindowsAuthentication())
        {
            lblPassword.Visible = false;
            txtScreenLockDialogPassword.Visible = false;
            btnScreenLockSignOut.Visible = false;
        }
        else
        {
            lblInstructions.Text += " " + GetString("screenlock.instruction.part2");
        }

        ScriptHelper.RegisterJQueryDialog(Page);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!RequestHelper.IsCallback())
        {
            // Creating the scripts which need to be called
            string clientScript = "function serverRequest(args, context){{ " +
                Page.ClientScript.GetCallbackEventReference(this, "args", "getResultShort", "") +
                "; }}" +
                "var screenLockEnabled = " + SecurityHelper.IsScreenLockEnabled(SiteContext.CurrentSiteName).ToString().ToLowerCSafe() + ";" +
                "$cmsj('#screenLockWarningDialog').appendTo($cmsj('#cms-header-messages'));";

            // Register the client scripts
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ScreenLock_" + ClientID, ScriptHelper.GetScript(clientScript));
            ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/UI/ScreenLock/ScreenLock.js");
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        var user = GetUserForAuthentication();

        if (!user.Enabled)
        {
            return "accountLocked";
        }

        if (userValidates)
        {
            if (AuthenticationMode.IsWindowsAuthentication())
            {
                return UnlockScreen();
            }

            // User wants to revalidate his session
            if (UserInfoProvider.IsUserPasswordDifferent(user, validatePassword))
            {
                // Password is invalid
                AuthenticationHelper.CheckInvalidPasswordAttempts(user, SiteContext.CurrentSiteName);

                if (!user.Enabled)
                {
                    return "accountLocked";
                }
                return "valbad";
            }

            if (userWaitingForPasscode)
            {
                return GeneratePasscode(user);
            }

            // Password is correct
            return UnlockScreen();
        }

        if (passcValidates)
        {
            var membershipProvider = new CMSMembershipProvider();
            if (membershipProvider.MFValidatePasscode(user, validatePasscode, false))
            {
                return UnlockScreen();
            }

            if (!user.Enabled)
            {
                return "accountLocked";
            }
            return "wrongPassc|" + GetString("mfauthentication.passcode.wrong");
        }

        if (CMSPage.IsScreenLocked)
        {
            if (userAsksForState)
            {
                // Screen is locked
                return "isLocked|True";
            }

            if (!userIsLoggingOut)
            {
                return "";
            }

            // User wants to logout
            string signOutUrl = UIHelper.GetSignOutUrl(SiteContext.CurrentSite);

            if (IsCMSDesk)
            {
                // LiveID sign out URL is set if this LiveID session
                AuthenticationHelper.SignOut(ref signOutUrl);
            }
            else
            {
                AuthenticationHelper.SignOut();
            }

            return "logout|" + signOutUrl;
        }

        // Check if ScreenLock is active
        if (!SecurityHelper.IsScreenLockEnabled(SiteContext.CurrentSiteName))
        {
            return "disabled";
        }

        // User is canceling countdown and wants to stay active
        if (userCanceling)
        {
            SecurityHelper.LogScreenLockAction();
            return "cancelOk|" + SecurityHelper.GetSecondsToShowScreenLockAction(SiteContext.CurrentSiteName);
        }

        if ((int)timeLeft.TotalSeconds <= 0)
        {
            // User was inactive too long - lock screen
            CMSPage.IsScreenLocked = true;
            return "lockScreen";
        }

        if ((int)timeLeft.TotalSeconds <= secondsToWarning)
        {
            // Lock screen timeout is close - display warning
            return "showWarning|" + ((int)timeLeft.TotalSeconds).ToString();
        }

        // User is active - hide warning and lock screen (if opened)
        return "hideWarning|" + ((int)timeLeft.TotalSeconds - secondsToWarning).ToString();
    }


    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument.Contains("logout"))
        {
            userIsLoggingOut = true;
        }
        else if (eventArgument.Contains("validate"))
        {
            userValidates = true;
            validatePassword = eventArgument.Substring(START_INDEX_FOR_PASSWORD);

            if (MFAuthenticationHelper.IsMultiFactorRequiredForUser(GetUserForAuthentication().UserName))
            {
                userWaitingForPasscode = true;
            }
        }
        else if (eventArgument.Contains("validPasscode"))
        {
            userValidates = false;
            userWaitingForPasscode = false;
            passcValidates = true;
            validatePasscode = eventArgument.Substring(START_INDEX_FOR_PASSCODE);
        }
        else if (eventArgument.Contains("isLocked"))
        {
            userAsksForState = true;
        }
        else if (eventArgument.Contains("cancel"))
        {
            userCanceling = true;
        }
        else if (eventArgument.Contains("action"))
        {
            userAsksForState = true;

            SecurityHelper.LogScreenLockAction();
        }

        // Find out when screen will be locked
        timeLeft = CMSPage.LastRequest + TimeSpan.FromMinutes(minutesToLock) - DateTime.Now;
    }


    /// <summary>
    /// Generate passcode and fire it through MultifactorAuthenticate event.
    /// </summary>
    /// <param name="user">User info.</param>
    public string GeneratePasscode(UserInfo user)
    {
        MFAuthenticationHelper.IssuePasscode(user.UserName);
        if (MembershipContext.MFAuthenticationTokenNotInitialized && MFAuthenticationHelper.DisplaySetupCode)
        {
            var sb = new StringBuilder("missingToken|");
            sb.Append(GetString("mfauthentication.isRequired"), " ", GetString("mfauthentication.token.get"),
                ARGUMENTS_SEPARATOR, GetString("mfauthentication.label.token"), ARGUMENTS_SEPARATOR,
                MFAuthenticationHelper.GetSetupCodeForUser(user.UserName));

            return sb.ToString();
        }
        return "waitingForPasscode";
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Does all the actions needed to unlock the screen.
    /// </summary>
    private static string UnlockScreen()
    {
        CMSPage.IsScreenLocked = false;
        
        // Reset invalid logon attempts for current user
        ResetCurrentUserInvalidLogonAttempts();

        SecurityHelper.LogScreenLockAction();
        return "valok|" + SecurityHelper.GetSecondsToShowScreenLockAction(SiteContext.CurrentSiteName);
    }


    /// <summary>
    /// Resets invalid logon attempts for current user.
    /// </summary>
    private static void ResetCurrentUserInvalidLogonAttempts()
    {
        var currentUser = GetUserForAuthentication();

        if (currentUser != null)
        {
            // Disable logging
            using (CMSActionContext context = new CMSActionContext())
            {
                context.LogSynchronization = false;
                context.LogExport = false;
                context.LogEvents = false;

                // Reset invalid logon attempts
                currentUser.UserInvalidLogOnAttempts = 0;

                // Update current user
                UserInfo.Provider.Set(currentUser);
            }
        }
    }



    /// <summary>
    /// Gets authenticated user for authentication check.
    /// If the user is impersonated, returns impersonating user.
    /// </summary>
    private static UserInfo GetUserForAuthentication()
    {
        // If current user is not impersonated, return pure UserInfo (on which can be called Set) instead of casted CurrentUserInfo.
        return MembershipContext.GetImpersonatingUser() ?? UserInfo.Provider.Get(MembershipContext.AuthenticatedUser.UserID);
    }

    #endregion
}
