using System;

using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_View : CMSGroupForumPage
{
    protected int forumGroupId = 0;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forum - Group view";

        forumGroupId = QueryHelper.GetInteger("forumgroupid", 0);
        ForumGroupInfo group = ForumGroupInfoProvider.GetForumGroupInfo(forumGroupId);
        if (group != null)
        {
            Forum1.CommunityGroupID = group.GroupGroupID;
            Forum1.GroupName = group.GroupName;
        }

        Forum1.IsLiveSite = false;
    }
}