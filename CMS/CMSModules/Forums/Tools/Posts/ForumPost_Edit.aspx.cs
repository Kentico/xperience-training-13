using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Tools_Posts_ForumPost_Edit : CMSForumsPage
{
    private int postId = 0;
    private string listingParameter = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        postId = QueryHelper.GetInteger("postid", 0);
        ForumContext.CheckSite(0, 0, postId);
        CurrentMaster.PanelContent.CssClass = String.Empty;

        string listingPost = QueryHelper.GetString("listingpost", null);
        if (!String.IsNullOrEmpty(listingPost))
        {
            listingParameter = "&listingpost=" + HTMLHelper.HTMLEncode(listingPost);
        }

        postEdit.EditPostID = postId;

        postEdit.OnSaved += postEdit_OnSaved;
        postEdit.IsLiveSite = false;
        InitializeMasterPage();
    }


    protected void postEdit_OnSaved(object sender, EventArgs e)
    {
        ForumPostInfo forumPostObj = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (forumPostObj != null)
        {
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_tree'].location.href = 'ForumPost_Tree.aspx?postid=" + forumPostObj.PostId + "&forumid=" + forumPostObj.PostForumID + "';");
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=" + forumPostObj.PostId + listingParameter + "';");
        }
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        string currentForumPost = "";
        ForumPostInfo forumPostObj = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (forumPostObj != null)
        {
            currentForumPost = HTMLHelper.HTMLEncode(forumPostObj.PostSubject);
        }

        Title = "Forum Post edit";

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("ForumPost_Edit.ItemListLink"),
            RedirectUrl = ResolveUrl("~/CMSModules/Forums/Tools/Posts/ForumPost_View.aspx?postid=" + postId + listingParameter),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = currentForumPost,
        });

        PageTitle.TitleText = GetString("ForumPost_Edit.HeaderCaption");
        // Ensure correct breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.forums_forumpost"));
    }
}
