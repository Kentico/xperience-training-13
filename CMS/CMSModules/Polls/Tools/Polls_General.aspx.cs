using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


[UIElement("CMS.Polls", "EditPoll.General")]
public partial class CMSModules_Polls_Tools_Polls_General : CMSPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int pollId = QueryHelper.GetInteger("pollid", 0);
        PollInfo pi = PollInfoProvider.GetPollInfo(pollId);
        EditedObject = pi;

        // Check global and site read permmision
        CheckPollsReadPermission(pi.PollSiteID);

        PollProperties.ItemID = pollId;
        PollProperties.IsLiveSite = false;
        PollProperties.OnSaved += PollProperties_OnSaved;

        PollProperties.Enabled = CheckPollsModifyPermission(pi.PollSiteID, false);

        if (pi.PollSiteID > 0)
        {
            PollProperties.SiteID = pi.PollSiteID;
            PollProperties.GroupID = pi.PollGroupID;
        }
    }


    protected void PollProperties_OnSaved(object sender, EventArgs e)
    {
        // Refresh header with display name
        ScriptHelper.RefreshTabHeader(Page, ((PollInfo)EditedObject).PollDisplayName);
    }
}
