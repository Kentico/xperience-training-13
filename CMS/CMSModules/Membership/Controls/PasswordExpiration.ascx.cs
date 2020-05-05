using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_PasswordExpiration : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Text to be displayed before change password link, when user account password has expired.
    /// </summary>
    public string ExpirationTextBefore
    {
        get;
        set;
    }


    /// <summary>
    /// Text to be displayed after change password link, when user account password has expired.
    /// </summary>
    public string ExpirationTextAfter
    {
        get;
        set;
    }


    /// <summary>
    /// Text to be displayed before change password link, when user account password will expire soon.
    /// </summary>
    public string ExpirationWarningTextBefore
    {
        get;
        set;
    }


    /// <summary>
    /// Text to be displayed after change password link, when user account password will expire soon.
    /// </summary>
    public string ExpirationWarningTextAfter
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if change password link should be displayed.
    /// </summary>
    public bool ShowChangePasswordLink
    {
        get;
        set;
    }


    /// <summary>
    /// Text to be displayed as change password link.
    /// </summary>
    public string ChangePasswordLinkText
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        // Get setting values
        string siteName = SiteContext.CurrentSiteName;
        var currentUser = MembershipContext.AuthenticatedUser;
        int expDays;

        if (AuthenticationHelper.IsAuthenticated() && AuthenticationHelper.IsPasswordExpirationEnabled(siteName, out expDays))
        {
            int dayDiff = (DateTime.Now - currentUser.UserPasswordLastChanged).Days - expDays;
            int warningDays = SettingsKeyInfoProvider.GetIntValue(siteName + ".CMSPasswordExpirationWarningPeriod");

            string changePwdLink = string.Format(@"<a href=""#"" onclick=""modalDialog('{0}','ChangePassword','720','350',null,null,true);"">{1}</a>", ResolveUrl("~/CMSModules/Membership/" + (IsLiveSite ? "CMSPages" : "Pages") + "/ChangePassword.aspx"), DataHelper.GetNotEmpty(ChangePasswordLinkText, GetString("passwordexpiration.changepassword")));

            // Check if account password expired
            if (dayDiff >= 0)
            {
                lblExpText.Text = string.Format("{0}{1}{2}", ExpirationTextBefore, (ShowChangePasswordLink ? changePwdLink : ""), ExpirationTextAfter);
            }
            // Check for password expiration warning
            else if ((warningDays > 0) && (Math.Abs(dayDiff) <= warningDays))
            {
                lblExpText.Text = string.Format("{0}{1}{2}", ExpirationWarningTextBefore, (ShowChangePasswordLink ? changePwdLink : ""), ExpirationWarningTextAfter);
            }
            else
            {
                Visible = false;
            }
        }
        else
        {
            Visible = false;
        }

        // Ensure dialog script
        ScriptHelper.RegisterDialogScript(Page);
    }
}
