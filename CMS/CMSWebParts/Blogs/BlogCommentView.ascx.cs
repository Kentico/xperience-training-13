using System;

using CMS.Blogs;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Blogs_BlogCommentView : CMSAbstractWebPart
{
    #region "Properties" 

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            commentView.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), commentView.BlogProperties.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            commentView.BlogProperties.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates whether 'Edit' button should be displayed in comment view while editing comments on the live site.
    /// </summary>
    public bool ShowEditButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditButton"), commentView.BlogProperties.ShowEditButton);
        }
        set
        {
            SetValue("ShowEditButton", value);
            commentView.BlogProperties.ShowEditButton = value;
        }
    }


    /// <summary>
    /// Indicates whether 'Delete' button should be displayed in comment view while editing comments on the live site.
    /// </summary>
    public bool ShowDeleteButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDeleteButton"), commentView.BlogProperties.ShowDeleteButton);
        }
        set
        {
            SetValue("ShowDeleteButton", value);
            commentView.BlogProperties.ShowDeleteButton = value;
        }
    }


    /// <summary>
    /// Indicates whether user pictures should be displayed in comment detail.
    /// </summary>
    public bool EnableUserPictures
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableUserPictures"), commentView.BlogProperties.EnableUserPictures);
        }
        set
        {
            SetValue("EnableUserPictures", value);
            commentView.BlogProperties.EnableUserPictures = value;
        }
    }


    /// <summary>
    /// User picture max width.
    /// </summary>
    public int UserPictureMaxWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("UserPictureMaxWidth"), commentView.BlogProperties.UserPictureMaxWidth);
        }
        set
        {
            SetValue("UserPictureMaxWidth", value);
            commentView.BlogProperties.UserPictureMaxWidth = value;
        }
    }


    /// <summary>
    /// User picture max height.
    /// </summary>
    public int UserPictureMaxHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("UserPictureMaxHeight"), commentView.BlogProperties.UserPictureMaxHeight);
        }
        set
        {
            SetValue("UserPictureMaxHeight", value);
            commentView.BlogProperties.UserPictureMaxHeight = value;
        }
    }


    /// <summary>
    /// Blog comment separator.
    /// </summary>
    public string CommentSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CommentSeparator"), commentView.Separator);
        }
        set
        {
            SetValue("CommentSeparator", value);
            commentView.Separator = value;
        }
    }


    /// <summary>
    /// Gets or sets list of roles (separated by ';') which are allowed to report abuse (in combination with SecurityAccess.AuthorizedRoles).
    /// </summary>
    public string AbuseReportRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AbuseReportRoles"), commentView.AbuseReportRoles);
        }
        set
        {
            SetValue("AbuseReportRoles", value);
            commentView.AbuseReportRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the security access for report abuse link.
    /// </summary>
    public SecurityAccessEnum AbuseReportAccess
    {
        get
        {
            return (SecurityAccessEnum)ValidationHelper.GetInteger(GetValue("AbuseReportAccess"), (int)commentView.AbuseReportSecurityAccess);
        }
        set
        {
            SetValue("AbuseReportAccess", value);
            commentView.AbuseReportSecurityAccess = value;
        }
    }
    
    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            commentView.StopProcessing = true;
            commentView.BlogProperties.StopProcessing = true;
        }
        else
        {
            commentView.ControlContext = ControlContext;

            // Get current page info
            PageInfo currentPage = DocumentContext.CurrentPageInfo;

            bool selectOnlyPublished = (PageManager.ViewMode.IsLiveSite());

            // Get current document
            commentView.PostNode = DocumentContext.CurrentDocument;

            // Get current parent blog
            TreeNode blogNode = BlogHelper.GetParentBlog(currentPage.NodeAliasPath, SiteContext.CurrentSiteName, selectOnlyPublished);

            // If blog node exists, set comment view properties
            if (blogNode != null)
            {
                commentView.BlogProperties.AllowAnonymousComments = ValidationHelper.GetBoolean(blogNode.GetValue("BlogAllowAnonymousComments"), true);
                commentView.BlogProperties.ModerateComments = ValidationHelper.GetBoolean(blogNode.GetValue("BlogModerateComments"), false);
                commentView.BlogProperties.OpenCommentsFor = ValidationHelper.GetInteger(blogNode.GetValue("BlogOpenCommentsFor"), BlogProperties.OPEN_COMMENTS_ALWAYS);
                commentView.BlogProperties.SendCommentsToEmail = ValidationHelper.GetString(blogNode.GetValue("BlogSendCommentsToEmail"), "");
                commentView.BlogProperties.UseCaptcha = ValidationHelper.GetBoolean(blogNode.GetValue("BlogUseCAPTCHAForComments"), true);
                commentView.BlogProperties.RequireEmails = ValidationHelper.GetBoolean(blogNode.GetValue("BlogRequireEmails"), false);
                commentView.BlogProperties.EnableSubscriptions = ValidationHelper.GetBoolean(blogNode.GetValue("BlogEnableSubscriptions"), false);
                commentView.BlogProperties.CheckPermissions = CheckPermissions;
                commentView.BlogProperties.ShowDeleteButton = ShowDeleteButton;
                commentView.BlogProperties.ShowEditButton = ShowEditButton;
                commentView.BlogProperties.EnableUserPictures = EnableUserPictures;
                commentView.BlogProperties.UserPictureMaxHeight = UserPictureMaxHeight;
                commentView.BlogProperties.UserPictureMaxWidth = UserPictureMaxWidth;
                commentView.Separator = CommentSeparator;
                commentView.ReloadPageAfterAction = true;
                commentView.AbuseReportOwnerID = blogNode.NodeOwner;
                commentView.AbuseReportRoles = AbuseReportRoles;
                commentView.AbuseReportSecurityAccess = AbuseReportAccess;
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        commentView.ReloadComments();
    }
}