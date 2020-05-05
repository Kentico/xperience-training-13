using System;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Tools_Forums_Forum_View : CMSForumsPage
{
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = QueryHelper.GetInteger("forumid", 0);
        ForumContext.CheckSite(0, forumId, 0);

        ForumFlatView1.ForumID = forumId;
        ForumFlatView1.IsLiveSite = false;
        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forums - Forum view";
    }
}