using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_List : CMSGroupPollsPage
{
    protected int groupID = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get GroupID from query string
        groupID = QueryHelper.GetInteger("groupID", 0);

        CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_READ);

        if (CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE, false))
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("group.polls.newpoll");
            action.RedirectUrl = ResolveUrl("Polls_New.aspx") + "?groupid=" + groupID;
            CurrentMaster.HeaderActions.AddAction(action);

            pollsList.DeleteEnabled = true;
        }

        pollsList.OnEdit += new EventHandler(pollsList_OnEdit);
        pollsList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(pollsList_OnCheckPermissions);
        pollsList.GroupId = groupID;
        pollsList.IsLiveSite = false;
    }


    private void pollsList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckGroupPermissions(groupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Edit poll click handler.
    /// </summary>
    private void pollsList_OnEdit(object sender, EventArgs e)
    {
        string editActionUrl = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.Polls", "Groups.EditGroup.EditPoll", false, pollsList.SelectedItemID), "groupid", groupID.ToString());
        URLHelper.Redirect(editActionUrl);
    }
}