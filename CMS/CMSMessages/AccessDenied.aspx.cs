using System;

using CMS.Base.Web.UI;
using CMS.Membership;
using CMS.Membership.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSMessages_AccessDenied : AccessDeniedPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    { 
        string title = GetString("CMSDesk.AccessDenied");
        string message = GetString("CMSMessages.AccessDenied");

        GetTexts(ref message, ref title);

        lblMessage.Text = message;
        titleElem.TitleText = title;

        // Display SignOut button
        if (AuthenticationHelper.IsAuthenticated())
        {
            if (!AuthenticationMode.IsWindowsAuthentication())
            {
                btnSignOut.Visible = true;
            }
        }
        else
        {
            btnLogin.Visible = true;
        }
    }

    #endregion


    #region "Methods"

    protected override void PerformSignOut()
    {
        base.PerformSignOut();

        ltlScript.Text = ScriptHelper.GetScript($"window.top.location.href= '{UIHelper.GetSignOutUrl(SiteContext.CurrentSite)}';");
    }

    #endregion


    #region "Button handling"

    protected void btnSignOut_Click(object sender, EventArgs e)
    {
        PerformSignOut();
    }


    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // Get the logon page URL
        ltlScript.Text = ScriptHelper.GetScript("window.top.location.href = '" + ResolveUrl(AuthenticationHelper.DEFAULT_LOGON_PAGE) + "';");
    }

    #endregion
}
