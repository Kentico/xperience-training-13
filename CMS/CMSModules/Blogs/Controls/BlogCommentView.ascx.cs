using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.Blogs.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Blogs_Controls_BlogCommentView : CMSAdminEditControl
{
    private BlogProperties mBlogProperties = new BlogProperties();
    private string mCommentDetailControlPath = "~/CMSModules/Blogs/Controls/BlogCommentDetail.ascx";

    private bool isUserAuthorized;

    private TreeNode mPostNode;

    private string mAliasPath;
    private string mCulture;
    private string mSiteName;

    protected string mAbuseReportRoles = null;
    protected SecurityAccessEnum mAbuseReportSecurityAccess = SecurityAccessEnum.AllUsers;
    protected int mAbuseReportOwnerID = 0;


    /// <summary>
    /// FALSE - whole page is reloaded after any action occures (insert, edit, delete, approve comment), FALSE - only comment list is reloaded after action occures.
    /// </summary>
    public bool ReloadPageAfterAction
    {
        get;
        set;
    }


    /// <summary>
    /// Post alias path.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath ?? (mAliasPath = DocumentContext.CurrentPageInfo.NodeAliasPath);
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// Post culture.
    /// </summary>
    public string Culture
    {
        get
        {
            return mCulture ?? (mCulture = LocalizationContext.PreferredCultureCode);
        }
        set
        {
            mCulture = value;
        }
    }


    /// <summary>
    /// Post SiteName.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName ?? (mSiteName = SiteContext.CurrentSiteName);
        }
        set
        {
            mSiteName = value;
        }
    }


    /// <summary>
    /// Comment separator.
    /// </summary>
    public string Separator
    {
        get;
        set;
    }


    /// <summary>
    /// Post document node.
    /// </summary>
    public TreeNode PostNode
    {
        get
        {
            if (mPostNode != null)
            {
                return mPostNode;
            }

            SetContext();

            // Get the document
            TreeProvider tree = new TreeProvider();
            mPostNode = tree.SelectSingleNode(SiteName, AliasPath, Culture, false, "CMS.BlogPost");
            if ((mPostNode != null) && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
            {
                mPostNode = DocumentHelper.GetDocument(mPostNode, tree);
            }

            ReleaseContext();
            return mPostNode;
        }
        set
        {
            mPostNode = value;
        }
    }


    /// <summary>
    /// Blog properties.
    /// </summary>
    public BlogProperties BlogProperties
    {
        get
        {
            return mBlogProperties;
        }
        set
        {
            mBlogProperties = value;
        }
    }


    /// <summary>
    /// Comment detail control path.
    /// </summary>
    public string CommentDetailControlPath
    {
        get
        {
            return mCommentDetailControlPath;
        }
        set
        {
            mCommentDetailControlPath = value;
        }
    }


    /// <summary>
    /// Indicates whether post comments can be added to the post.
    /// </summary>
    public bool AreCommentsOpened
    {
        get
        {
            // Get current post document info            
            if (PostNode == null)
            {
                return false;
            }

            bool isOpened;
            var currentUser = MembershipContext.AuthenticatedUser;

            if (!ValidationHelper.GetBoolean(PostNode.GetValue("BlogPostAllowComments"), false))
            {
                // Comments are not allowed for current post
                isOpened = false;
            }
            else
            {
                // Check new comment dialog expiration
                switch (BlogProperties.OpenCommentsFor)
                {
                    case BlogProperties.OPEN_COMMENTS_ALWAYS:
                        isOpened = true;
                        break;

                    case BlogProperties.OPEN_COMMENTS_DISABLE:
                        isOpened = false;
                        break;

                    default:
                        DateTime postDate = ValidationHelper.GetDateTime(PostNode.GetValue("BlogPostDate"), DateTimeHelper.ZERO_TIME);
                        isOpened = (DateTime.Now <= postDate.AddDays(BlogProperties.OpenCommentsFor));

                        break;
                }

                if (currentUser.IsPublic())
                {
                    isOpened = (isOpened && BlogProperties.AllowAnonymousComments);
                }
            }

            return isOpened;
        }
    }


    /// <summary>
    /// Gets or sets list of roles (separated by ';') which are allowed to report abuse (in combination with SecurityAccess.AuthorizedRoles).
    /// </summary>
    public string AbuseReportRoles
    {
        get
        {
            return mAbuseReportRoles;
        }
        set
        {
            mAbuseReportRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the security access for report abuse link.
    /// </summary>
    public SecurityAccessEnum AbuseReportSecurityAccess
    {
        get
        {
            return mAbuseReportSecurityAccess;
        }
        set
        {
            mAbuseReportSecurityAccess = value;
        }
    }


    /// <summary>
    /// Gets or sets the owner ID (in combination with SecurityAccess.Owner).
    /// </summary>
    public int AbuseReportOwnerID
    {
        get
        {
            return mAbuseReportOwnerID;
        }
        set
        {
            mAbuseReportOwnerID = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (BlogProperties.StopProcessing)
        {
            Visible = false;
            return;
        }

        lblTitle.Text = GetString("Blog.CommentView.Comments");

        SetupControl();


        if (PostNode != null)
        {
            // Check permissions for blog
            if (BlogProperties.CheckPermissions && MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(PostNode, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed)
            {
                Visible = false;
            }

            if (Visible)
            {
                rptComments.ItemDataBound += rptComments_ItemDataBound;
                ctrlCommentEdit.OnAfterCommentSaved += ctrlCommentEdit_OnAfterCommentSaved;
                ctrlCommentEdit.OnBeforeCommentSaved += ctrlCommentEdit_OnBeforeCommentSaved;
                ctrlCommentEdit.UseCaptcha = BlogProperties.UseCaptcha;
                ctrlCommentEdit.EnableSubscriptions = BlogProperties.EnableSubscriptions;
                ctrlCommentEdit.RequireEmails = BlogProperties.RequireEmails;
                ctrlCommentEdit.ClearFormAfterSave = true;

                pnlSubscription.Visible = BlogProperties.EnableSubscriptions;
                plcBtnSubscribe.Visible = BlogProperties.EnableSubscriptions;
                elemSubscription.DocumentID = PostNode.DocumentID;
                elemSubscription.NodeID = PostNode.NodeID;
                elemSubscription.Culture = PostNode.DocumentCulture;
            }
        }

        // Make sure info label is displayed to the user when saved successfully
        if (QueryHelper.GetBoolean("saved", false))
        {
            ctrlCommentEdit.CommentSavedText = GetString("Blog.CommentView.CommentSaved");
        }

        if (Visible)
        {
            ReloadComments();
        }
    }


    private void ctrlCommentEdit_OnBeforeCommentSaved()
    {
        // Set information text after comment is saved
        if (!isUserAuthorized && BlogProperties.ModerateComments)
        {
            ctrlCommentEdit.CommentSavedText = GetString("Blog.CommentView.ModeratedCommentSaved");
        }
    }


    /// <summary>
    /// Reloads comment list after new comment is added.
    /// </summary>    
    private void ctrlCommentEdit_OnAfterCommentSaved(BlogCommentInfo commentObj)
    {
        // If comments are moderated there is no need to reload whole page and comment count immediately
        if (!isUserAuthorized && BlogProperties.ModerateComments)
        {
            // Reload comment list only
            ReloadComments();
        }
        else
        {
            string url = RequestContext.CurrentURL;

            if (commentObj.CommentApproved)
            {
                url = URLHelper.RemoveParameterFromUrl(url, "saveda");
                url = URLHelper.AddParameterToUrl(url, "saved", "1");
            }
            else
            {
                url = URLHelper.RemoveParameterFromUrl(url, "saved");
                url = URLHelper.AddParameterToUrl(url, "saveda", "1");
            }

            // Reload whole page
            URLHelper.Redirect(url);
        }
    }


    private void mBlogComment_OnCommentAction(string actionName, object actionArgument)
    {
        // Get comment ID
        int commentId = ValidationHelper.GetInteger(actionArgument, 0);
        BlogCommentInfo bci;
        
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Check 'Manage' permission
                if (!isUserAuthorized)
                {
                    AccessDenied("cms.blog", "Manage");
                }

                // Delete comment
                BlogCommentInfoProvider.DeleteBlogCommentInfo(commentId);

                ReloadData();

                break;

            case "approve":
                // Check 'Manage' permission
                if (!isUserAuthorized)
                {
                    AccessDenied("cms.blog", "Manage");
                }

                // Set comment as 'approved'
                bci = BlogCommentInfoProvider.GetBlogCommentInfo(commentId);
                var currentUser = MembershipContext.AuthenticatedUser;

                if ((bci != null) && (currentUser != null))
                {
                    bci.CommentApprovedByUserID = currentUser.UserID;
                    bci.CommentApproved = true;
                    BlogCommentInfoProvider.SetBlogCommentInfo(bci);
                }

                ReloadData();
                break;

            case "reject":
                // Check 'Manage' permission
                if (!isUserAuthorized)
                {
                    AccessDenied("cms.blog", "Manage");
                }

                // Set comment as 'rejected'
                bci = BlogCommentInfoProvider.GetBlogCommentInfo(commentId);
                if (bci != null)
                {
                    bci.CommentApprovedByUserID = 0;
                    bci.CommentApproved = false;
                    BlogCommentInfoProvider.SetBlogCommentInfo(bci);
                }

                ReloadData();
                break;
        }
    }


    /// <summary>
    /// Reload.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        // Reload whole page
        if (ReloadPageAfterAction)
        {
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
        // Reload comment list only
        else
        {
            ReloadComments();
        }
    }


    /// <summary>
    /// Reloads comment list.
    /// </summary>
    public void ReloadComments()
    {
        SetContext();

        pnlComment.Visible = AreCommentsOpened;

        if (PostNode != null)
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            // Check permissions for blog
            if (BlogProperties.CheckPermissions)
            {
                if (currentUser.IsAuthorizedPerDocument(PostNode, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed)
                {
                    Visible = false;
                    return;
                }
            }

            ctrlCommentEdit.PostDocumentId = PostNode.DocumentID;
            ctrlCommentEdit.PostNodeId = PostNode.NodeID;
            ctrlCommentEdit.PostCulture = PostNode.DocumentCulture;

            if (!BlogProperties.StopProcessing)
            {
                // Get parent blog
                bool selectOnlyPublished = PortalContext.ViewMode.IsLiveSite();
                TreeNode blogNode = BlogHelper.GetParentBlog(AliasPath, SiteName, selectOnlyPublished);

                // Determine whether user is authorized to manage comments
                isUserAuthorized = BlogHelper.IsUserAuthorizedToManageComments(blogNode);

                // Get all post comments
                rptComments.DataSource = BlogCommentInfoProvider.GetPostComments(PostNode.DocumentID, !isUserAuthorized, isUserAuthorized);
                rptComments.DataBind();
            }
        }

        ReleaseContext();
    }


    /// <summary>
    /// Adds comment list item.
    /// </summary>
    private void rptComments_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (lblInfo.Visible)
        {
            lblInfo.Visible = false;
        }

        if (e.Item.DataItem == null)
        {
            return;
        }

        // Create new comment info object
        var comment = new BlogCommentInfo(((DataRowView)e.Item.DataItem).Row);

        // Load 'BlogCommentDetail.ascx' control
        var commentControl = (BlogCommentDetail)LoadUserControl(mCommentDetailControlPath);

        // Set control data and properties
        commentControl.ID = "blogElem";
        commentControl.BlogProperties = mBlogProperties;
        commentControl.Comment = comment;
        commentControl.IsLiveSite = IsLiveSite;

        // Initialize comment 'Action buttons'
        commentControl.OnCommentAction += mBlogComment_OnCommentAction;
        commentControl.ShowRejectButton = (isUserAuthorized && comment.CommentApproved);
        commentControl.ShowApproveButton = (isUserAuthorized && !comment.CommentApproved);
        commentControl.ShowDeleteButton = (mBlogProperties.ShowDeleteButton && isUserAuthorized);
        commentControl.ShowEditButton = (mBlogProperties.ShowEditButton && isUserAuthorized);

        // Abuse report security properties 
        commentControl.AbuseReportSecurityAccess = AbuseReportSecurityAccess;
        commentControl.AbuseReportRoles = AbuseReportRoles;
        commentControl.AbuseReportOwnerID = AbuseReportOwnerID;

        // Add loaded control as comment list item
        e.Item.Controls.Clear();
        e.Item.Controls.Add(commentControl);
        if (Separator != null)
        {
            e.Item.Controls.Add(new LiteralControl(Separator));
        }
    }


    /// <summary>
    /// Initializes control.
    /// </summary>
    private void SetupControl()
    {
        // Show/hide appropriate control based on current selection form hidden field
        if (ValidationHelper.GetInteger(hdnSelSubsTab.Value, 0) == 0)
        {
            pnlComment.Style.Remove("display");
            pnlComment.Style.Add("display", "block");
            pnlSubscription.Style.Remove("display");
            pnlSubscription.Style.Add("display", "none");
        }
        else
        {
            pnlSubscription.Style.Remove("display");
            pnlSubscription.Style.Add("display", "block");
            pnlComment.Style.Remove("display");
            pnlComment.Style.Add("display", "none");
        }

        RegisterScripts();
    }


    /// <summary>
    /// Registers required scripts
    /// </summary>
    private void RegisterScripts()
    {

        btnLeaveMessage.Attributes.Add("onclick", "ShowSubscription(0, '" + hdnSelSubsTab.ClientID + "','" + pnlComment.ClientID + "','" +
                                                  pnlSubscription.ClientID + "'); return false; ");
        btnSubscribe.Attributes.Add("onclick", " ShowSubscription(1, '" + hdnSelSubsTab.ClientID + "','" + pnlComment.ClientID + "','" +
                                               pnlSubscription.ClientID + "'); return false; ");

        var script = @"
// Refreshes current page when comment properties are changed in modal dialog window
function RefreshBlogCommentPage() 
{         
    var url = window.location.href;
        
    // String ""#comments"" found in url -> trim it
    var charIndex = window.location.href.indexOf('#');
    if (charIndex != -1)
    {
        url = url.substring(0, charIndex);
    }
        
    // Refresh page content
    window.location.replace(url);       
}
    
// Switches between edit control and subscription control
function ShowSubscription(subs, hdnField, elemEdit, elemSubscr) {
    if (hdnField && elemEdit && elemSubscr) 
    {
        var hdnFieldElem = document.getElementById(hdnField);
        var elemEditElem = document.getElementById(elemEdit);
        var elemSubscrElem = document.getElementById(elemSubscr);
        if((hdnFieldElem!=null)&&(elemEditElem!=null)&&(elemSubscrElem!=null))
        {
            if (subs == 1) { // Show subscriber control
                elemEditElem.style.display = 'none';
                elemSubscrElem.style.display = 'block';
            }
            else
            {                // Show edit control
                elemEditElem.style.display = 'block';
                elemSubscrElem.style.display = 'none';
            }
            hdnFieldElem.value = subs;
        }
    }      
}";
    
       ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowSubscription" + ClientID, script, true);
    }
}
