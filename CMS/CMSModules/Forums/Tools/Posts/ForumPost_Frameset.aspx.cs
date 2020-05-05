using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Tools_Posts_ForumPost_Frameset : CMSForumsPage
{
    protected string postsTreeUrl = "ForumPost_Tree.aspx?forumid=";
    protected string postsEditUrl = "ForumPost_View.aspx?forumid=";
    protected string postFrameTree = "posts_tree";
    protected string postFrameEdit = "posts_edit";


    protected void Page_Load(object sender, EventArgs e)
    {
        int forumId = QueryHelper.GetInteger("forumid", 0);
        ForumContext.CheckSite(0, forumId, 0);

        if (QueryHelper.GetInteger("saved", 0) > 0)
        {
            postsTreeUrl += forumId.ToString() + "&saved=1";
            postsEditUrl += forumId.ToString() + "&saved=1";
        }
        else
        {
            postsTreeUrl += forumId.ToString();
            postsEditUrl += forumId.ToString();
        }

        frameTree.Attributes["name"] = postFrameTree;
        frameTree.Attributes["src"] = postsTreeUrl;

        frameEdit.Attributes["name"] = postFrameEdit;
        frameEdit.Attributes["src"] = postsEditUrl;

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        ScriptHelper.HideVerticalTabs(this);
    }
}
