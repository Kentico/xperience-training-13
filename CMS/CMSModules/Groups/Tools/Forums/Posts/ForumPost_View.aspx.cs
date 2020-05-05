using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_View : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        postView.PostID = QueryHelper.GetInteger("postid", 0);
        postView.Reply = QueryHelper.GetInteger("reply", 0);
        postView.ForumID = QueryHelper.GetInteger("forumId", 0);
        postView.ListingPost = QueryHelper.GetString("listingpost", String.Empty);
        postView.IsLiveSite = false;

        // Register back to listing script
        if (!String.IsNullOrEmpty(postView.ListingPost))
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "BackToListing", ScriptHelper.GetScript(
                "function BackToListing() { location.href = '" + ResolveUrl("~/CMSModules/Groups/Tools/Forums/Posts/ForumPost_Listing.aspx?postid=" + ScriptHelper.GetString(postView.ListingPost, false)) + "'; }\n"));
        }

        postView.OnCheckPermissions += CheckGroupPermissions;

        InitializeMasterPage();
    }


    protected void CheckGroupPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;

        if (postView.PostID == 0)
        {
            groupId = GetGroupIdFromForum(postView.ForumID);
        }
        else
        {
            var post = ForumPostInfoProvider.GetForumPostInfo(postView.PostID);
            if (post != null)
            {
                groupId = GetGroupIdFromForum(post.PostForumID);
            }
        }        

        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private int GetGroupIdFromForum(int forumId)
    {
        var forum = ForumInfoProvider.GetForumInfo(forumId);
        if (forum != null)
        {
            var forumGroup = ForumGroupInfoProvider.GetForumGroupInfo(forum.ForumGroupID);
            if (forumGroup != null)
            {
                return forumGroup.GroupGroupID;
            }
        }

        return 0;
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
