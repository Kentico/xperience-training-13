using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Notifications_Administration_Users_User_Edit_Notifications : CMSUsersPage
{
    private int mUserId;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check the license
        string domain = RequestContext.CurrentDomain;
        if (domain != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(domain, FeatureEnum.Notifications);
        }

        // Check "read" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Read"))
        {
            RedirectToAccessDenied("CMS.Users", "Read");
        }

        mUserId = QueryHelper.GetInteger("userid", 0);

        // Check that only global administrator can edit global administrator's accouns
        if (mUserId > 0)
        {
            UserInfo ui = UserInfoProvider.GetUserInfo(mUserId);
            EditedObject = ui;

            if (!CheckGlobalAdminEdit(ui))
            {
                userNotificationsElem.Visible = false;

                // Show error message
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));

                return;
            }
        }

        userNotificationsElem.UserId = mUserId;
        userNotificationsElem.SiteID = SiteID;
        userNotificationsElem.ZeroRowsText = GetString("notifications.administration.userhasnonotifications");
        userNotificationsElem.OnCheckPermissions += userNotificationsElem_OnCheckPermissions;
    }


    protected void userNotificationsElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        var cui = MembershipContext.AuthenticatedUser;
        if ((cui == null) || ((mUserId != cui.UserID) && !cui.IsAuthorizedPerResource("CMS.Users", permissionType)))
        {
            RedirectToAccessDenied("CMS.Users", permissionType);
        }
    }
}