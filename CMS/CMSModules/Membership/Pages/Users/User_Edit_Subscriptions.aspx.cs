using System;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_Edit_Subscriptions : CMSUsersPage
{
    protected int userId;
    protected int mSiteID;

    public override int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = SiteContext.CurrentSiteID;
            }

            return mSiteID;
        }
        set
        {
            elemSubscriptions.SiteID = mSiteID = value;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        userId = QueryHelper.GetInteger("userid", 0);
        if (userId > 0)
        {
            // Check that only global administrator can edit global administrator's accouns
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            EditedObject = ui;
            CheckUserAvaibleOnSite(ui);

            if (!CheckGlobalAdminEdit(ui))
            {
                elemSubscriptions.StopProcessing = true;
                elemSubscriptions.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            elemSubscriptions.UserID = userId;
            elemSubscriptions.IsLiveSite = false;
            elemSubscriptions.OnCheckPermissions += elemSubscriptions_OnCheckPermissions;
        }
        else
        {
            elemSubscriptions.StopProcessing = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Show site selector
        CurrentMaster.DisplaySiteSelectorPanel = true;

        if ((SiteID > 0) && !MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            CurrentMaster.DisplaySiteSelectorPanel = false;
            return;
        }

        // Set site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.OnlyRunningSites = false;
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        if (!RequestHelper.IsPostBack())
        {
            // If user is member of current site
            if (UserSiteInfoProvider.GetUserSiteInfo(userId, SiteID) != null)
            {
                // Force uniselector to preselect current site
                siteSelector.Value = SiteID;
            }

            // Force to load data
            siteSelector.Reload(true);
        }

        // Get truly selected item
        SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
        elemSubscriptions.ReloadData();
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);

        elemSubscriptions.ReloadData();
        updateContent.Update();
    }


    protected void elemSubscriptions_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("CMS.Users", CMSAdminControl.PERMISSION_MODIFY);
        }
    }
}