using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.MacroEngine;

public partial class CMSWebParts_Blogs_NewBlog : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Path in the content tree where new blog should be created.
    /// </summary>
    public string BlogParentPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BlogParentPath"), "");
        }
        set
        {
            SetValue("BlogParentPath", value);
        }
    }


    /// <summary>
    /// Indicates if user should be redirected to the blog after the blog is created.
    /// </summary>
    public bool RedirectToNewBlog
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RedirectToNewBlog"), false);
        }
        set
        {
            SetValue("RedirectToNewBlog", value);
        }
    }


    /// <summary>
    /// Blog side columnt text.
    /// </summary>
    public string BlogSideColumnText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BlogSideColumnText"), "");
        }
        set
        {
            SetValue("BlogSideColumnText", value);
        }
    }


    /// <summary>
    /// Blog teaser.
    /// </summary>
    public Guid BlogTeaser
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("BlogTeaser"), Guid.Empty);
        }
        set
        {
            SetValue("BlogTeaser", value);
        }
    }


    /// <summary>
    /// Email address where new comments should be sent.
    /// </summary>
    public string BlogSendCommentsToEmail
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BlogSendCommentsToEmail"), "");
        }
        set
        {
            SetValue("BlogSendCommentsToEmail", value);
        }
    }


    /// <summary>
    /// Indicates if blog comments are opened (0 - not opened, -1 - always opened, X - number of days the comments are opened after the post is published).
    /// </summary>
    public int BlogOpenCommentsFor
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("BlogOpenCommentsFor"), -1);
        }
        set
        {
            SetValue("BlogOpenCommentsFor", value);
        }
    }


    /// <summary>
    /// Indicates if new comments requir to be moderated before publishing.
    /// </summary>
    public bool BlogModerateComments
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BlogModerateComments"), false);
        }
        set
        {
            SetValue("BlogModerateComments", value);
        }
    }


    /// <summary>
    /// Indicates if security control should be used when inserting new comment.
    /// </summary>
    public bool BlogUseCAPTCHAForComments
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BlogUseCAPTCHAForComments"), false);
        }
        set
        {
            SetValue("BlogUseCAPTCHAForComments", value);
        }
    }


    /// <summary>
    /// Indicates anonymous users can insert comments.
    /// </summary>
    public bool BlogAllowAnonymousComments
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BlogAllowAnonymousComments"), false);
        }
        set
        {
            SetValue("BlogAllowAnonymousComments", value);
        }
    }


    /// <summary>
    /// User which are allowed to moderate blog comments. Format [username1];[username2];...
    /// </summary>
    public string BlogModerators
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BlogModerators"), "");
        }
        set
        {
            SetValue("BlogModerators", value);
        }
    }


    /// <summary>
    /// Page template which is applied to a new blog. If not specified, page template of the parent document is applied.
    /// </summary>
    public string NewBlogTemplate
    {
        get
        {
            object value = GetValue("NewBlogTemplate");
            if (value == null)
            {
                return null;
            }
            else
            {
                return Convert.ToString(value);
            }
        }
        set
        {
            SetValue("NewBlogTemplate", value);
        }
    }


    /// <summary>
    /// Indicates if permissions are to be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
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
            newBlog.StopProcessing = true;
        }
        else
        {
            // Set new blog properties
            newBlog.BlogParentPath = MacroResolver.ResolveCurrentPath(BlogParentPath);
            newBlog.RedirectToNewBlog = RedirectToNewBlog;
            newBlog.BlogAllowAnonymousComments = BlogAllowAnonymousComments;
            newBlog.BlogModerateComments = BlogModerateComments;
            newBlog.BlogOpenCommentsFor = BlogOpenCommentsFor;
            newBlog.BlogSendCommentsToEmail = BlogSendCommentsToEmail;
            newBlog.BlogSideColumnText = BlogSideColumnText;
            newBlog.BlogTeaser = BlogTeaser;
            newBlog.BlogUseCAPTCHAForComments = BlogUseCAPTCHAForComments;
            newBlog.BlogModerators = BlogModerators;
            newBlog.NewBlogTemplate = NewBlogTemplate;
            newBlog.CheckPermissions = CheckPermissions;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}