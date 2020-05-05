using System;

using CMS.Community;
using CMS.UIControls;


public partial class CMSModules_Groups_CMSPages_JoinTheGroup : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        groupJoinElem.JoinButton = btnJoin;
        groupJoinElem.CancelButton = btnCancel;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("Groups.JoinTheGroup");
        Title = GetString("Groups.JoinTheGroup");

        if (CommunityContext.CurrentGroup != null)
        {
            groupJoinElem.Group = CommunityContext.CurrentGroup;
        }
    }
}