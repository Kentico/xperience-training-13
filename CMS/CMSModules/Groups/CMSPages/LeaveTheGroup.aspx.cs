using System;

using CMS.Community;
using CMS.UIControls;


public partial class CMSModules_Groups_CMSPages_LeaveTheGroup : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        groupLeaveElem.LeaveButton = btnLeave;
        groupLeaveElem.CancelButton = btnCancel;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("Groups.LeaveTheGroup");
        Title = GetString("Groups.LeaveTheGroup");

        if (CommunityContext.CurrentGroup != null)
        {
            groupLeaveElem.Group = CommunityContext.CurrentGroup;
        }
    }
}