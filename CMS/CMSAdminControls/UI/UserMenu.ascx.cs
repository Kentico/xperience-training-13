using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Membership.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UserMenu : CMSUserControl
{
    #region "Page events"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }

        // Load JavaScript module
        ScriptHelper.RegisterModule(Page, "CMS/UserMenu", new
        {
            wrapperSelector = ".cms-navbar.js-user-menu-wrapper",
            checkChangesLinksSelector = ".js-check-changes"
        });
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        // Check if main actions are visible
        plcTopDivider.Visible = EnsureMainItems();

        // Check if bottom actions are visible
        plcBottomDivider.Visible = EnsureBottomItems();

        // Do not show uniselector open dialog buttons.
        // Custom link buttons are used to open the dialog.
        ucUsers.DialogButton.Visible = false;
        ucUICultures.DialogButton.Visible = false;
    }


    /// <summary>
    /// Ensures main items in menu.
    /// </summary>
    /// <returns>Returns true if at least one item is visible</returns>
    private bool EnsureMainItems()
    {
        bool topDividerVisible = EnsureImpersonation();
        topDividerVisible |= EnsureUICultures();

        return topDividerVisible;
    }


    /// <summary>
    /// Ensures bottom items in menu.
    /// </summary>
    /// <returns>Returns true if at least one item is visible</returns>
    private bool EnsureBottomItems()
    {
        return EnsureSignOut();
    }

    #endregion


    #region "Items"

    /// <summary>
    /// Checks Sign out link settings.
    /// </summary>
    private bool EnsureSignOut()
    {
        if (AuthenticationMode.IsWindowsAuthentication())
        {
            // Hide sign out link
            lnkSignOut.Visible = false;
            return false;
        }

        lnkSignOut.OnClientClick = "return CheckChanges();";

        return true;
    }


    /// <summary>
    /// Checks user impersonation settings.
    /// </summary>
    private bool EnsureImpersonation()
    {
        var userIsImpersonated = MembershipContext.CurrentUserIsImpersonated();

        // Show impersonate button for global admin only or impersonated user
        if (CookieHelper.AllowCookies && (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || userIsImpersonated))
        {
            lnkUsers.Visible = true;
            lnkUsers.OnClientClick = ucUsers.GetSelectionDialogScript();

            // Show all users except global administrators and public user, in CMSDesk show only site users
            ucUsers.WhereCondition = GetImpersonateWhereCondition().ToString(true);

            // Script for open uniselector modal dialog
            string impersonateScript = "function userImpersonateShowDialog () {US_SelectionDialog_" + ucUsers.ClientID + "()}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ImpersonateContextMenu", ScriptHelper.GetScript(impersonateScript));

            string userName = HTMLHelper.HTMLDecode(ValidationHelper.GetString(ucUsers.Value, String.Empty));
            if (userName != String.Empty)
            {
                // Get selected user info
                UserInfo iui = UserInfo.Provider.Get(userName);
                if (!iui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
                {
                    // Indicates whether user will be able to continue in the administration interface    
                    bool keepAdimUI = iui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName);

                    AuthenticationHelper.ImpersonateUser(iui, null, !keepAdimUI);
                    if (keepAdimUI)
                    {
                        PortalScriptHelper.RegisterAdminRedirectScript(Page);
                    }
                }
            }

            // Set visibility of Cancel impersonation item in menu
            plcCancelImpersonate.Visible = userIsImpersonated;

            // Hide impersonate action in menu if user is already impersonated
            plcImpersonate.Visible = !plcCancelImpersonate.Visible;

            return true;
        }

        // Hide impersonate action in menu
        plcImpersonate.Visible = false;
        plcCancelImpersonate.Visible = false;

        return false;
    }


    /// <summary>
    /// Returns <see cref="WhereCondition"/> valid for all users except global administrators and public user.
    /// In CMSDesk <see cref="WhereCondition"/> is valid only for site users.
    /// </summary>
    private WhereCondition GetImpersonateWhereCondition()
    {
        ObjectQuery<UserSiteInfo> siteUserIDs = GetSiteUserIDs();

        var typeInfo = UserInfo.TYPEINFO;
        
        return new WhereCondition()
            .WhereIn(typeInfo.IDColumn, siteUserIDs)
            .WhereLessOrEquals("UserPrivilegeLevel", (int)UserPrivilegeLevelEnum.Editor)
            .WhereTrue("UserEnabled")
            .WhereNotEquals(typeInfo.IDColumn, MembershipContext.AuthenticatedUser.UserID)
            .WhereNotEquals(typeInfo.CodeNameColumn, "public");
    }


    /// <summary>
    /// Returns <see cref="ObjectQuery"/> containing site user IDs.
    /// </summary>
    private ObjectQuery<UserSiteInfo> GetSiteUserIDs()
    {
        var dataQuery = UserSiteInfo.Provider
            .Get()
            .Column(UserSiteInfo.TYPEINFO.ParentIDColumn);

        if (IsCMSDesk)
        {
            dataQuery.WhereEquals(UserSiteInfo.TYPEINFO.SiteIDColumn, SiteContext.CurrentSiteID);
        }
        
        return dataQuery;
    }


    /// <summary>
    /// Checks UI culture settings.
    /// </summary>
    private bool EnsureUICultures()
    {
        // Show selector only if there are more UI cultures than one
        var count = CultureInfoProvider.GetUICultures().Count;
        if (count > 1)
        {
            lnkUICultures.Visible = true;
            lnkUICultures.OnClientClick = ucUICultures.GetSelectionDialogScript();

            string cultureName = ValidationHelper.GetString(ucUICultures.Value, String.Empty);
            if (!String.IsNullOrEmpty(cultureName))
            {
                var user = UserInfo.Provider.Get(MembershipContext.AuthenticatedUser.UserID);
                user.PreferredUICultureCode = cultureName;

                UserInfo.Provider.Set(user);

                // Enforce reload of authenticated user instance
                MembershipContext.AuthenticatedUser = null;

                // Set selected UI culture and refresh all pages
                CultureHelper.SetPreferredUICultureCode(cultureName);

                PortalScriptHelper.RegisterAdminRedirectScript(Page);
            }

            return true;
        }

        // Hide UI culture action in menu
        plcUICultures.Visible = false;
        return false;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// OnClick event handler for cancel impersonation.
    /// </summary>
    protected void lnkCancelImpersonate_OnClick(object sender, EventArgs e)
    {
        AuthenticationHelper.CancelImpersonation();

        if (AuthenticationMode.IsFormsAuthentication())
        {
            PortalScriptHelper.RegisterAdminRedirectScript(Page);
        }
        else
        {
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
    }


    /// <summary>
    /// OnClick event handler for sign out.
    /// </summary>
    protected void lnkSignOut_OnClick(object sender, EventArgs e)
    {
        // LiveID sign out URL is set if this LiveID session
        AuthenticationHelper.SignOut(UIHelper.GetSignOutUrl(SiteContext.CurrentSite));
    }

    #endregion
}
