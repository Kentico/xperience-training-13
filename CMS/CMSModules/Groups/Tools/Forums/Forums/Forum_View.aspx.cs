using System;

using CMS.Community.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_Forums_Forums_Forum_View : CMSGroupForumPage
{
    protected int forumId = 0;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = QueryHelper.GetInteger("forumid", 0);
        ForumFlatView1.ForumID = forumId;
        InitializeMasterPage();
        ForumFlatView1.IsLiveSite = false;
    }


    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forums - Forum view";
    }
}