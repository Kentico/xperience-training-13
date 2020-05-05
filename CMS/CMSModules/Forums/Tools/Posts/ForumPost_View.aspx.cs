using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Tools_Posts_ForumPost_View : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int postID = QueryHelper.GetInteger("postid", 0);
        int forumID = QueryHelper.GetInteger("forumId", 0);
        ForumContext.CheckSite(0, forumID, postID);

        postView.PostID = postID;
        postView.Reply = QueryHelper.GetInteger("reply", 0);
        postView.ForumID = forumID;
        postView.ListingPost = QueryHelper.GetString("listingpost", String.Empty);
        postView.IsLiveSite = false;

        // Register back to listing script
        if (!String.IsNullOrEmpty(postView.ListingPost))
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "BackToListing", ScriptHelper.GetScript(
                "function BackToListing() { location.href = '" + ResolveUrl("~/CMSModules/Forums/Tools/Posts/ForumPost_Listing.aspx?postid=" + ScriptHelper.GetString(postView.ListingPost, false)) + "'; }\n"));
        }

        // Initialize master page
        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes MasterPage.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forum Post View";
        string listingParam = null;

        if (!String.IsNullOrEmpty(postView.ListingPost))
        {
            listingParam = "+ '&listingpost=" + HTMLHelper.HTMLEncode(postView.ListingPost) + "'";
        }

        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditPost",
                                               ScriptHelper.GetScript("function EditPost(postId) { " +
                                                                      "if ( postId != 0 ) { parent.frames['posts_edit'].location.href = 'ForumPost_Edit.aspx?postid=' + postId" + listingParam + ";}}"));
    }
}
