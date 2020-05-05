using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_LiveControls_Posts : CMSAdminItemsControl, IPostBackEventHandler
{
    #region "Variables"

    private int mForumId;
    private int postId;
    private ForumPostInfo post;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the Forum ID.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);


        #region "Security"

        postEdit.OnCheckPermissions += postEdit_OnCheckPermissions;
        postNew.OnCheckPermissions += postNew_OnCheckPermissions;

        #endregion


        postNew.OnInsertPost += postNew_OnInsertPost;
        postNew.OnPreview += postNew_OnPreview;
        postEdit.OnPreview += postEdit_OnPreview;
        postEdit.OnCancelClick += postEdit_OnCancelClick;
        postEdit.OnSaved += postEdit_OnSaved;

        // Set forum
        treeElem.ForumID = mForumId;
        postNew.ForumID = mForumId;
        postEdit.ForumID = mForumId;

        // Get post ID
        postId = ValidationHelper.GetInteger(hdnPost.Value, 0);
        if (postId > 0)
        {
            postEdit.EditPostID = postId;
            post = ForumPostInfoProvider.GetForumPostInfo(postId);
        }

        // Unigrid settings
        UniGrid.Visible = false;
        UniGrid.Query = "";
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        // Initialize page elements
        titleViewElem.TitleText = GetString("ForumPost_View.PostTitleText");
        titleEditElem.TitleText = GetString("ForumPost_Edit.HeaderCaption");
        
        lnkBackHidden.Click += lnkBackHidden_Click;

        if (post != null)
        {
            InitializeBreadcrumbs(post.PostSubject);
            InitializeMenu();
        }

        // Add handlers
        treeElem.OnGetStatusIcons += treeElem_OnGetPostIconUrl;

        actionsElem.ActionPerformed += actionsElem_ActionPerformed;
    }


    #region "Security handlers"

    private void postNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void postEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// Unigrid External bound event handler.
    /// </summary>
    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Display link instead of title
        if (sourceName.ToLowerCSafe() == "title")
        {
            if (parameter != DBNull.Value)
            {
                DataRowView row = (DataRowView)parameter;

                // Get info
                Guid attachmentGuid = ValidationHelper.GetGuid(row["AttachmentGUID"], Guid.Empty);

                if (attachmentGuid != Guid.Empty)
                {
                    string url = URLHelper.GetAbsoluteUrl("~/CMSPages/GetForumAttachment.aspx?fileguid=" + attachmentGuid);
                    string title = ValidationHelper.GetString(row["AttachmentFileName"], "");

                    // Create link to post attachment
                    HyperLink link = new HyperLink();
                    link.NavigateUrl = url;
                    link.Target = "_blank";
                    link.Text = HTMLHelper.HTMLEncode(title);
                    link.ToolTip = url;
                    return link;
                }
            }
        }

        return parameter.ToString();
    }


    /// <summary>
    /// Unigrid Action event handler.
    /// </summary>
    private void UniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
            {
                return;
            }

            ForumAttachmentInfoProvider.DeleteForumAttachmentInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    protected void postNew_OnPreview(object sender, EventArgs e)
    {
        // Display new preview
        DisplayControl("newpreview");
    }


    private void postEdit_OnCancelClick(object sender, EventArgs e)
    {
        // Display view
        DisplayControl("view");
    }


    protected void postEdit_OnPreview(object sender, EventArgs e)
    {
        // Display edit preview
        DisplayControl("editpreview");
    }


    protected void postEdit_OnSaved(object sender, EventArgs e)
    {
        // Display new view
        postView.PostData = null;
        DisplayControl("view");
    }


    protected void postNew_OnInsertPost(object sender, EventArgs e)
    {
        // Set properties
        postId = postNew.EditPostID;
        treeElem.Selected = postId;
        postEdit.EditPostID = postId;
        postView.PostID = postId;
        postNew.ReplyToPostID = 0;

        // Save post to database
        post = ForumPostInfoProvider.GetForumPostInfo(postId);
        hdnPost.Value = postId.ToString();
        DisplayControl("view");
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        hdnPost.Value = null;
        postId = 0;
        post = null;
        DisplayControl("new");
    }


    /// <summary>
    /// Initializes the menu.
    /// </summary>
    protected void InitializeMenu()
    {
        actionsElem.ActionsList.Clear();

        HeaderAction edit = new HeaderAction();
        edit.Text = GetString("general.edit");
        edit.Tooltip = GetString("ForumPost_View.EditToolTip");
        edit.CommandName = "edit";
        actionsElem.AddAction(edit);

        HeaderAction delete = new HeaderAction();
        delete.Text = GetString("general.delete");
        delete.Tooltip = GetString("ForumPost_View.DeleteToolTip");
        delete.CommandName = "delete";
        delete.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("ForumPost_View.DeleteConfirmation")) + ")){return false;}";
        actionsElem.AddAction(delete);

        HeaderAction reply = new HeaderAction();
        reply.Text = GetString("ForumPost_View.IconReply");
        reply.Tooltip = GetString("ForumPost_View.ReplyToolTip");
        reply.CommandName = "reply";
        actionsElem.AddAction(reply);

        if (post.PostLevel == 0)
        {
            HeaderAction lockAction = new HeaderAction();
            lockAction.Text = GetString("ForumPost_View.IconLock");
            lockAction.Tooltip = GetString("ForumPost_View.LockToolTip");
            lockAction.CommandName = "lockunlock";
            actionsElem.AddAction(lockAction);

            HeaderAction unlock = new HeaderAction();
            unlock.Text = GetString("ForumPost_View.IconUnLock");
            unlock.Tooltip = GetString("ForumPost_View.UnLockToolTip");
            unlock.CommandName = "lockunlock";
            unlock.Visible = ((post != null) && post.PostIsLocked);
            actionsElem.AddAction(unlock);

            HeaderAction stick = new HeaderAction();
            stick.Text = GetString("ForumPost_View.IconStick");
            stick.Tooltip = GetString("ForumPost_View.StickToolTip");
            stick.CommandName = "stick";
            stick.Visible = ((post != null) && (post.PostStickOrder <= 0));
            actionsElem.AddAction(stick);

            HeaderAction unStick = new HeaderAction();
            unStick.Text = GetString("ForumPost_View.IconUnStick");
            unStick.Tooltip = GetString("ForumPost_View.UnStickToolTip");
            unStick.CommandName = "unstick";
            unStick.Visible = ((post != null) && (post.PostStickOrder > 0));
            actionsElem.AddAction(unStick);
        }
        else
        {
            HeaderAction split = new HeaderAction();
            split.Text = GetString("ForumPost_View.IconSplit");
            split.Tooltip = GetString("ForumPost_View.SplitToolTip");
            split.CommandName = "split";
            actionsElem.AddAction(split);
        }

        // Approve
        HeaderAction approveReject = new HeaderAction();

        if (!post.PostApproved)
        {
            approveReject.Text = GetString("general.approve");
            approveReject.Tooltip = GetString("ForumPost_View.ApproveToolTip");
            approveReject.CommandName = "approve";
        }
        else
        {
            approveReject.Text = GetString("general.reject");
            approveReject.Tooltip = GetString("ForumPost_View.RejectToolTip");
            approveReject.CommandName = "reject";
        }

        actionsElem.AddAction(approveReject);

        HeaderAction approveSubtree = new HeaderAction();

        // Approve subtree
        if (!post.PostApproved)
        {
            approveSubtree.Text = GetString("ForumPost_View.IconApproveSubTree");
            approveSubtree.Tooltip = GetString("ForumPost_View.ApproveSubTreeToolTip");
            approveSubtree.CommandName = "approvesubtree";
        }
        else
        {
            approveSubtree.Text = GetString("ForumPost_View.IconRejectSubTree");
            approveSubtree.Tooltip = GetString("ForumPost_View.IconRejectSubTree");
            approveSubtree.CommandName = "rejectsubtree";
        }

        actionsElem.AddAction(approveSubtree);
    }


    /// <summary>
    /// Initializes breadcrumb items.
    /// </summary>
    /// <param name="forumSubject">Forum subject displayed in breadcrumbs</param>
    private void InitializeBreadcrumbs(string forumSubject)
    {
        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("general.view"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = forumSubject,
        });
    }


    /// <summary>
    /// Handle actions.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        switch (e.CommandName.ToLowerCSafe())
        {
            case "stick":

                ForumPostInfoProvider.StickThread(post);

                // Get the post object with updated info
                post = ForumPostInfoProvider.GetForumPostInfo(post.PostId);
                DisplayControl("view");
                break;

            case "unstick":

                ForumPostInfoProvider.UnstickThread(post);

                // Get the post object with updated info
                post = ForumPostInfoProvider.GetForumPostInfo(post.PostId);
                DisplayControl("view");
                break;

            case "split":

                ForumPostInfoProvider.SplitThread(post);

                // Get the post object with updated info
                post = ForumPostInfoProvider.GetForumPostInfo(post.PostId);
                DisplayControl("view");
                break;

            case "lockunlock":
                // Lock or unlock post
                post.PostIsLocked = !post.PostIsLocked;
                ForumPostInfoProvider.SetForumPostInfo(post);
                DisplayControl("view");
                break;

            case "edit":
                // Edit
                DisplayControl("edit");
                break;

            case "delete":
                // Delete post
                ForumPostInfoProvider.DeleteForumPostInfo(postId);
                postNew.ClearForm();
                DisplayControl("new");
                break;

            case "reply":
                // Reply
                DisplayControl("reply");
                break;

            case "approve":
                // Approve action
                if (MembershipContext.AuthenticatedUser != null)
                {
                    post.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    post.PostApproved = true;
                    ForumPostInfoProvider.SetForumPostInfo(post);
                }

                DisplayControl("view");
                break;

            case "reject":
                // Reject action
                post.PostApprovedByUserID = 0;
                post.PostApproved = false;
                ForumPostInfoProvider.SetForumPostInfo(post);

                DisplayControl("view");
                break;

            case "approvesubtree":
                // Approve subtree
                if ((post != null) && (MembershipContext.AuthenticatedUser != null))
                {
                    post.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                    post.PostApproved = true;
                    ForumPostInfoProvider.SetForumPostInfo(post);

                    DataSet ds = ForumPostInfoProvider.GetChildPosts(post.PostId);

                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        // All posts under current post
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            ForumPostInfo mfpi = new ForumPostInfo(dr);
                            if (!mfpi.PostApproved)
                            {
                                mfpi.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                                mfpi.PostApproved = true;
                                ForumPostInfoProvider.SetForumPostInfo(mfpi);
                            }
                        }
                    }

                    DisplayControl("view");
                }

                break;

            case "rejectsubtree":
                // Reject subtree
                if (post != null)
                {
                    post.PostApprovedByUserID = 0;
                    post.PostApproved = false;
                    ForumPostInfoProvider.SetForumPostInfo(post);

                    DataSet ds = ForumPostInfoProvider.GetChildPosts(post.PostId);

                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        // All posts under current post
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            ForumPostInfo mfpi = new ForumPostInfo(dr);
                            if (mfpi.PostApproved)
                            {
                                mfpi.PostApprovedByUserID = 0;
                                mfpi.PostApproved = false;
                                ForumPostInfoProvider.SetForumPostInfo(mfpi);
                            }
                        }
                    }
                    DisplayControl("view");
                }

                break;
        }

        hdnPost.Value = postId.ToString();
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        // Display view control
        DisplayControl("view");
    }


    protected string treeElem_OnGetPostIconUrl(ForumPostTreeNode node)
    {
        if (node == null)
        {
            return null;
        }

        if (!ValidationHelper.GetBoolean(((DataRow)node.ItemData)["PostApproved"], false))
        {
            return UIHelper.GetAccessibleIconTag("NodeLink icon-circle tn color-red-70", GetString("general.notapproved"));
        }

        return null;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (ControlsHelper.IsInUpdatePanel(this))
        {
            ControlsHelper.UpdateCurrentPanel(this);
        }

        // Register script for show post
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumPostPostback_value", ScriptHelper.GetScript(
            "function ShowPost(showId){ \n" +
            "    var hidden = document.getElementById('" + hdnPost.ClientID + "'); \n" +
            "    if (hidden != null) { \n" +
            "    hidden.value = showId; \n" +
            "    } \n" + ControlsHelper.GetPostBackEventReference(this, "showid") +
            "} \n"));
    }


    /// <summary>
    /// Displays only specified control. Other controls hides.
    /// </summary>
    /// <param name="selectedControl">Specified control</param>
    private void DisplayControl(string selectedControl)
    {
        // Tree element
        treeElem.ForumID = mForumId;
        treeElem.SelectOnlyApproved = false;
        treeElem.Selected = postId;

        // Hide elements
        plcPostEdit.Visible = false;
        plcPostView.Visible = false;
        plcPostNew.Visible = false;

        switch (selectedControl.ToLowerCSafe())
        {
            case "view":
                // View mode
                if ((post != null) && (post.PostAttachmentCount > 0))
                {
                    string where = "(AttachmentPostID = " + postId + ")";

                    // Load unigrid
                    UniGrid.WhereCondition = where;
                    UniGrid.Query = "Forums.ForumAttachment.selectall";
                    UniGrid.Columns = "AttachmentID,AttachmentFileName,AttachmentFileSize,AttachmentGUID";
                    UniGrid.Visible = true;
                    UniGrid.ReloadData();
                }

                if (post != null)
                {
                    InitializeMenu();
                    actionsElem.ReloadData();
                }

                postNew.ForumID = mForumId;
                postView.PostID = postId;
                postView.PostData = null;
                plcPostView.Visible = true;
                postView.ReloadData();
                break;

            case "edit":
                // Edit mode
                postEdit.ForumID = mForumId;
                postEdit.EditPostID = postId;
                postEdit.ReloadData();
                plcPostEdit.Visible = true;
                break;

            case "reply":
                // Reply mode
                plcPostNew.Visible = true;
                postNew.ReplyToPostID = postId;
                postNew.ReloadData();
                break;

            case "newpreview":
                // New preview mode
                plcPostEdit.Visible = false;
                plcPostView.Visible = false;
                plcPostNew.Visible = true;
                break;

            case "editpreview":
                // Edit preview mode
                plcPostEdit.Visible = true;
                plcPostView.Visible = false;
                plcPostNew.Visible = false;
                break;

            default:
                // Default view mode
                postNew.ClearForm();
                if (postId > 0)
                {
                    postNew.ReplyToPostID = postId;
                }
                else
                {
                    postNew.ReplyToPostID = 0;
                }

                plcPostNew.Visible = true;
                postNew.ReloadData();

                break;
        }
    }


    #region IPostBackEventHandler Members

    public void RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument.ToLowerCSafe())
        {
            case "showid":
                if (postId <= 0)
                {
                    ReloadData();
                }
                else
                {
                    DisplayControl("view");
                }
                break;
        }
    }

    #endregion
}
