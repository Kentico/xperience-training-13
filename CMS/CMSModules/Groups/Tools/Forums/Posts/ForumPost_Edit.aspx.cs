using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_Edit : CMSGroupForumPage
{
    private int postId = 0;
    private string listingParameter = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        string currentForumPost = "";

        // get forumPost id from querystring
        postId = QueryHelper.GetInteger("postid", 0);
        ForumPostInfo forumPostObj = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (forumPostObj != null)
        {
            currentForumPost = HTMLHelper.HTMLEncode(forumPostObj.PostSubject);
        }

        CurrentMaster.PanelContent.CssClass = String.Empty;

        string listingPost = QueryHelper.GetString("listingpost", null);
        if (!String.IsNullOrEmpty(listingPost))
        {
            listingParameter = "&listingpost=" + HTMLHelper.HTMLEncode(listingPost);
        }

        postEdit.EditPostID = postId;
        postEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(postEdit_OnCheckPermissions);
        postEdit.OnSaved += new EventHandler(postEdit_OnSaved);
        postEdit.IsLiveSite = false;

        InitializeMasterPage(currentForumPost);
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


    protected void postEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(postEdit.EditPostID);
        if (fpi != null)
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(fpi.PostForumID);
            if (fi != null)
            {
                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
                if (fgi != null)
                {
                    groupId = fgi.GroupGroupID;
                }
            }
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(string currentForumPost)
    {
        Title = "Forum Post edit";

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("ForumPost_Edit.ItemListLink"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Forums/Posts/ForumPost_View.aspx?postid=" + postId + listingParameter),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = currentForumPost,
        });

        PageTitle.TitleText = GetString("ForumPost_Edit.HeaderCaption");
    }
}
