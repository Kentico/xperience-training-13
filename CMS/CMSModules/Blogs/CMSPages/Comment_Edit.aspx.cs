using System;

using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[CheckLicence(FeatureEnum.Blogs)]
public partial class CMSModules_Blogs_CMSPages_Comment_Edit : CMSLiveModalPage
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
            TreeNode blogNode = BlogHelper.GetParentBlog(commentObj.CommentPostDocumentID, true);

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

        ctrlCommentEdit.OnAfterCommentSaved += ctrlCommentEdit_OnAfterCommentSaved;

        btnOk.Click += btnOk_Click;
        btnOk.Text = GetString("general.saveandclose");
        btnOk.ValidationGroup = ctrlCommentEdit.ValidationGroup;

        PageTitle.TitleText = GetString("Blog.CommentEdit.Title");
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        ctrlCommentEdit.PerformAction();
    }


    protected void ctrlCommentEdit_OnAfterCommentSaved(BlogCommentInfo commentObj)
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.RefreshBlogCommentPage();CloseDialog();");
    }
}
