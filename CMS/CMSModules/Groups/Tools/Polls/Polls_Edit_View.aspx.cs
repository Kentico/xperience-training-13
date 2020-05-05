using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_View : CMSGroupPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get PollID and GroupID from querystring
        int pollId = QueryHelper.GetInteger("pollid", 0);
        int groupId = QueryHelper.GetInteger("groupid", 0);

        PollInfo pi = PollInfoProvider.GetPollInfo(pollId);
        EditedObject = pi;

        if ((pi != null) && (pi.PollGroupID == groupId))
        {
            PollView.PollCodeName = pi.PollCodeName;
            PollView.PollSiteID = pi.PollSiteID;
            PollView.PollGroupID = pi.PollGroupID;
            PollView.CountType = CountTypeEnum.Percentage;
            PollView.ShowGraph = true;
            PollView.ShowResultsAfterVote = true;
            // Check permissions during voting if user hasn't got 'Manage' permission
            PollView.CheckPermissions = (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE));
            PollView.CheckVoted = false;
            PollView.HideWhenNotAuthorized = false;
            PollView.CheckOpen = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        PollView.VoteButton.ButtonStyle = ButtonStyle.Primary;
    }
}