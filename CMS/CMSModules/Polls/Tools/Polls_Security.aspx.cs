using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.Polls;
using CMS.UIControls;


[UIElement("CMS.Polls", "EditPoll.Security")]
public partial class CMSModules_Polls_Tools_Polls_Security : CMSPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int pollid = QueryHelper.GetInteger("pollid", 0);
        PollInfo pi = PollInfoProvider.GetPollInfo(pollid);
        EditedObject = pi;

        // Check global and site read permmision
        CheckPollsReadPermission(pi.PollSiteID);

        PollSecurity.Enabled = CheckPollsModifyPermission(pi.PollSiteID, false);

        PollSecurity.ItemID = pollid;
        PollSecurity.IsLiveSite = false;
        PollSecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PollSecurity_OnCheckPermissions);
    }


    /// <summary>
    /// Check permissions event handler.
    /// </summary>
    private void PollSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Polls", permissionType))
        {
            sender.StopProcessing = true;
        }
    }
}