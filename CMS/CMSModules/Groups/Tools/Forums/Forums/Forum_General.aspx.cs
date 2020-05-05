using System;

using CMS.Community.Web.UI;
using CMS.Core;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.FORUMS, "GroupForumGeneral")]
public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_General : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        forumEdit.ForumID = QueryHelper.GetInteger("forumid", 0);
        forumEdit.OnCheckPermissions += forumEdit_OnCheckPermissions;
        forumEdit.IsLiveSite = false;
    }


    private void forumEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(forumEdit.ForumID);
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