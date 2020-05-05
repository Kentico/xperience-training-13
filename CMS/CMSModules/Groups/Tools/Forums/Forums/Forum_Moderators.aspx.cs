using System;

using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_Moderators : CMSGroupForumPage
{
    protected int forumId = 0;
    protected ForumInfo forum = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = QueryHelper.GetInteger("forumid", 0);
        if (forumId == 0)
        {
            return;
        }

        forumModerators.ForumID = forumId;
        forumModerators.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumModerators_OnCheckPermissions);
        forumModerators.IsLiveSite = false;
    }


    private void forumModerators_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(QueryHelper.GetInteger("forumid", 0));
        if (fi != null)
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
            if (fgi != null)
            {
                groupId = fgi.GroupGroupID;
            }
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }
}