using System;

using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_General : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        groupEdit.GroupID = QueryHelper.GetInteger("forumgroupid", 0);
        groupEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(groupEdit_OnCheckPermissions);
        groupEdit.IsLiveSite = false;
    }


    private void groupEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(groupEdit.GroupID);
        if (fgi != null)
        {
            groupId = fgi.GroupGroupID;
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }
}