using System;

using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

[UIElement(ModuleName.CMS, "Dashboard")]
public partial class CMSModules_Admin_AdministrationDashboard : DashboardPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get current user info
        var currentUser = MembershipContext.AuthenticatedUser;

        // Check whether user is global admin
        if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            cmsDashboard.SetupDashboard();
        }
        else
        {
            // For non-global admin redirect to access denied
            URLHelper.Redirect(AdministrationUrlHelper.GetAccessDeniedUrl("accessdeniedtopage.globaladminrequired"));
        }
    }
}