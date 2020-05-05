using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Blogs_Controls_NewBlog : CMSUserControl
{
    #region "Variables"

    private string mBlogParentPath = "";
    private string mBlogSideColumnText = "";
    private Guid mBlogTeaser = Guid.Empty;
    private int mBlogOpenCommentsFor = -1; // blog is opened "Always" by default
    private string mBlogSendCommentsToEmail = "";
    private bool mBlogAllowAnonymousComments = true;
    private string mBlogModerators = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Path in the content tree where new blog should be created.
    /// </summary>
    public string BlogParentPath
    {
        get
        {
            return mBlogParentPath;
        }
        set
        {
            mBlogParentPath = value;
        }
    }


    /// <summary>
    /// Indicates if user should be redirected to the blog after the blog it is created.
    /// </summary>
    public bool RedirectToNewBlog
    {
        get;
        set;
    }


    /// <summary>
    /// Blog side column text.
    /// </summary>
    public string BlogSideColumnText
    {
        get
        {
            return mBlogSideColumnText;
        }
        set
        {
            mBlogSideColumnText = value;
        }
    }


    /// <summary>
    /// Blog teaser.
    /// </summary>
    public Guid BlogTeaser
    {
        get
        {
            return mBlogTeaser;
        }
        set
        {
            mBlogTeaser = value;
        }
    }


    /// <summary>
    /// Email address where new comments should be sent.
    /// </summary>
    public string BlogSendCommentsToEmail
    {
        get
        {
            return mBlogSendCommentsToEmail;
        }
        set
        {
            mBlogSendCommentsToEmail = value;
        }
    }


    /// <summary>
    /// Indicates if blog comments are opened (0 - not opened, -1 - always opened, X - number of days the comments are opened after the post is published).
    /// </summary>
    public int BlogOpenCommentsFor
    {
        get
        {
            return mBlogOpenCommentsFor;
        }
        set
        {
            mBlogOpenCommentsFor = value;
        }
    }


    /// <summary>
    /// Indicates if new comments require to be moderated before publishing.
    /// </summary>
    public bool BlogModerateComments
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if security control should be used when inserting new comment.
    /// </summary>
    public bool BlogUseCAPTCHAForComments
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates anonymous users can insert comments.
    /// </summary>
    public bool BlogAllowAnonymousComments
    {
        get
        {
            return mBlogAllowAnonymousComments;
        }
        set
        {
            mBlogAllowAnonymousComments = value;
        }
    }


    /// <summary>
    /// Users which are allowed to moderate blog comments. Format [username1];[username2];...
    /// </summary>
    public string BlogModerators
    {
        get
        {
            return mBlogModerators;
        }
        set
        {
            mBlogModerators = value;
        }
    }


    /// <summary>
    /// Page template which is applied to a new blog. If not specified, page template of the parent document is applied.
    /// </summary>
    public string NewBlogTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether permissions are to be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Initialize controls
            lblName.Text = GetString("Blogs.NewBlog.Name");
            lblDescription.Text = GetString("Blogs.NewBlog.Description");
            btnOk.Text = GetString("General.OK");
            rfvName.ErrorMessage = GetString("Blogs.NewBlog.NameEmpty");
            btnOk.Click += btnOk_Click;
        }
    }


    private void btnOk_Click(object sender, EventArgs e)
    {
        // Validate all required data for new blog
        string errorMessage = ValidateData();

        if (!LicenseHelper.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Blogs, ObjectActionEnum.Insert))
        {
            errorMessage = GetString("cmsdesk.bloglicenselimits");
        }

        // Get current user
        var user = MembershipContext.AuthenticatedUser;

        if (errorMessage == "")
        {
            // Get parent node for new blog
            TreeProvider tree = new TreeProvider(user);
            TreeNode parent = tree.SelectSingleNode(SiteContext.CurrentSiteName, BlogParentPath.TrimEnd('%'), TreeProvider.ALL_CULTURES);
            if (parent != null)
            {
                DataClassInfo blogClass = DataClassInfoProvider.GetDataClassInfo("CMS.Blog");
                if (blogClass == null)
                {
                    return;
                }

                // Check if blog is allowed in selected location
                if (!DocumentHelper.IsDocumentTypeAllowed(parent, blogClass.ClassID))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("Content.ChildClassNotAllowed");
                    return;
                }

                if (!CheckPermissions || user.IsAuthorizedToCreateNewDocument(parent, "cms.blog"))
                {
                    // Check if blog description allows empty value
                    FormInfo formInfo = new FormInfo(blogClass.ClassFormDefinition);
                    FormFieldInfo fieldInfo = formInfo.GetFormField("BlogDescription");
                    if ((fieldInfo != null) && !fieldInfo.AllowEmpty && String.IsNullOrWhiteSpace(txtDescription.Text))
                    {
                        lblError.Visible = true;
                        lblError.Text = GetString("blogs.newblog.descriptionempty");
                        return;
                    }

                    // Initialize and create new blog node
                    var blogNode = TreeNode.New("cms.blog", tree);
                    blogNode.SetValue("BlogName", txtName.Text.Trim());
                    blogNode.SetValue("BlogDescription", txtDescription.Text.Trim());
                    blogNode.SetValue("BlogAllowAnonymousComments", BlogAllowAnonymousComments);
                    blogNode.SetValue("BlogModerateComments", BlogModerateComments);
                    blogNode.SetValue("BlogOpenCommentsFor", BlogOpenCommentsFor);
                    blogNode.SetValue("BlogSendCommentsToEmail", BlogSendCommentsToEmail);
                    blogNode.SetValue("BlogSideColumnText", BlogSideColumnText);
                    blogNode.SetValue("BlogUseCAPTCHAForComments", BlogUseCAPTCHAForComments);
                    blogNode.SetValue("BlogModerators", BlogModerators);
                    if (BlogTeaser == Guid.Empty)
                    {
                        blogNode.SetValue("BlogTeaser", null);
                    }
                    else
                    {
                        blogNode.SetValue("BlogTeaser", BlogTeaser);
                    }

                    blogNode.SetValue("NodeOwner", user.UserID);
                    blogNode.DocumentName = txtName.Text.Trim();
                    blogNode.DocumentCulture = LocalizationContext.PreferredCultureCode;
                    DocumentHelper.InsertDocument(blogNode, parent, tree);

                    if (RedirectToNewBlog)
                    {
                        // Redirect to the new blog
                        URLHelper.Redirect(UrlResolver.ResolveUrl(DocumentURLProvider.GetUrl(blogNode)));
                    }
                    else
                    {
                        // Display info message
                        lblInfo.Visible = true;
                        lblInfo.Text = GetString("General.ChangesSaved");
                    }
                }
                else
                {
                    // Not authorized to create blog
                    errorMessage = GetString("blogs.notallowedtocreate");
                }
            }
            else
            {
                // Parent node was not found
                errorMessage = GetString("Blogs.NewBlog.PathNotFound");
            }
        }

        if (errorMessage == "")
        {
            return;
        }

        // Display error message
        lblError.Visible = true;
        lblError.Text = errorMessage;
    }


    /// <summary>
    /// Validates form data and returns error message if some error occurs.
    /// </summary>
    private string ValidateData()
    {
        if (txtName.Text.Trim() == "")
        {
            // Blog name is empty
            return rfvName.ErrorMessage;
        }
        if (BlogParentPath.TrimEnd('%') == "")
        {
            // Path where blog should be created is empty
            return GetString("Blogs.NewBlog.PathEmpty");
        }
        if (MembershipContext.AuthenticatedUser.IsPublic())
        {
            // Anonymous user is not allowed to create blog
            return GetString("Blogs.NewBlog.AnonymousUser");
        }
        return "";
    }
}