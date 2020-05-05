using System;

using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[CheckLicence(FeatureEnum.Blogs)]
public partial class CMSModules_Blogs_Controls_Comment_Edit : CMSModalPage
{
    protected int commentId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        commentId = QueryHelper.GetInteger("commentID", 0);

        // Get comment info
        BlogCommentInfo commentObj = BlogCommentInfoProvider.GetBlogCommentInfo(commentId);
        EditedObject = commentObj;

        if (commentObj != null)
        {
            // Get parent blog            
            TreeNode blogNode = BlogHelper.GetParentBlog(commentObj.CommentPostDocumentID, false);

            // Check site ID of edited blog
            if ((blogNode != null) && (blogNode.NodeSiteID != SiteContext.CurrentSiteID))
            {
                EditedObject = null;
            }

            bool isAuthorized = BlogHelper.IsUserAuthorizedToManageComments(blogNode);

            // Check "manage" permission
            if (!isAuthorized)
            {
                RedirectToAccessDenied("cms.blog", "Manage");
            }

            ctrlCommentEdit.CommentId = commentId;
        }

        Save += (s, ea) => ctrlCommentEdit.PerformAction();

        ctrlCommentEdit.IsLiveSite = false;
        ctrlCommentEdit.OnAfterCommentSaved += ctrlCommentEdit_OnAfterCommentSaved;

        PageTitle.TitleText = GetString("Blog.CommentEdit.Title");
    }


    protected void ctrlCommentEdit_OnAfterCommentSaved(BlogCommentInfo commentObj)
    {
        // Get filter parameters
        string filterParams = "?user=" + QueryHelper.GetText("user", "") + "&comment=" + QueryHelper.GetText("comment", "") +
                              "&approved=" + QueryHelper.GetText("approved", "") + "&isspam=" + QueryHelper.GetText("isspam", "");

        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBlogCommentPage(" + ScriptHelper.GetString(filterParams) + ",'" + QueryHelper.GetBoolean("usepostback", false) + "');CloseDialog();");
    }
}
