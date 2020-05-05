using System;

using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_Security : CMSGroupPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PollSecurity.IsLiveSite = false;
        PollSecurity.ItemID = QueryHelper.GetInteger("pollid", 0);
        PollSecurity.OnCheckPermissions += PollSecurity_OnCheckPermissions;
        PollSecurity.Enabled = CheckGroupPermissions(QueryHelper.GetInteger("groupid",0), CMSAdminControl.PERMISSION_MANAGE, false);
    }


    private void PollSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        PollInfo pi = PollInfoProvider.GetPollInfo(PollSecurity.ItemID);
        int groupId = 0;

        if (pi != null)
        {
            groupId = pi.PollGroupID;
        }

        // Check permissions
        if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupId))
        {
            sender.StopProcessing = true;
        }
    }
}