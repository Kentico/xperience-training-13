using System;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Tools_Forums_Forum_Moderators : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int forumID = QueryHelper.GetInteger("forumid", 0);
        ForumContext.CheckSite(0, forumID, 0);

        forumModerators.ForumID = forumID;
        forumModerators.IsLiveSite = false;
    }
}