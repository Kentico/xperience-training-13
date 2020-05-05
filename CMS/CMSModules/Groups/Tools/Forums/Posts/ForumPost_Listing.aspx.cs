using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_Listing : CMSGroupForumPage
{
    private int postId = 0;
    private int forumId = 0;
    private ForumPostInfo postInfo = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentForumPost = "";

        string[] post = QueryHelper.GetString("postid", "").Split(';');
        postId = ValidationHelper.GetInteger(post[0], 0);
        if (post.Length >= 2)
        {
            forumId = ValidationHelper.GetInteger(post[1], 0);
            postListing.ForumId = forumId;
        }

        // Show current post
        postInfo = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (postInfo != null)
        {
            currentForumPost = HTMLHelper.HTMLEncode(postInfo.PostSubject);
            postListing.PostInfo = postInfo;
        }
        // If not post, show current forum
        else if (forumId > 0)
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
            if (fi != null)
            {
                currentForumPost = fi.ForumDisplayName;
            }
        }

        postListing.IsLiveSite = false;

        InitializeMasterPage(currentForumPost);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(string currentForumPost)
    {
        Title = "Forum post listing";
        PageTitle.TitleText = GetString("Forums.Listing.Title");
        if (postInfo != null)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("Forums.ParentPost");
            action.OnClientClick = "SelectPost(" + postInfo.PostParentID + ", " + postInfo.PostForumID + ");";
            action.RedirectUrl = null;
            CurrentMaster.HeaderActions.AddAction(action);
        }
    }
}