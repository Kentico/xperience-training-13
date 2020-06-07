using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.SiteProvider;

public partial class CMSModules_Forums_Controls_NewPost : ForumViewer
{
    #region "Private variables"

    private bool? mEnableSubscription;

    private const int POST_USERNAME_LENGTH = 200;
    private const int POST_SUBJECT_LENGTH = 450;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the user nickname if is available or username.
    /// </summary>
    public string UserName
    {
        get
        {
            if (!String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserNickName.Trim()))
            {
                return MembershipContext.AuthenticatedUser.UserNickName.Trim();
            }

            return Functions.GetFormattedUserName(MembershipContext.AuthenticatedUser.UserName, IsLiveSite);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the post form uses html editor or text area.
    /// </summary>
    public bool UseHTMLEditor
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Gets or sets the value that indicates whether subject field is displayed for thread replies.
    /// </summary>
    public bool DisplaySubjectForReply
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the email is required.
    /// </summary>
    public bool RequiresEmail
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the email is required only for public user.
    /// </summary>
    public bool RequiresEmailOnlyForPublic
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether subscription to current post is allowed.
    /// </summary>
    public bool AllowSubscription
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Gets or sets the value that indicates whether the signature for current post is allowed.
    /// </summary>
    public bool AllowSignature
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value that indicates whether preview is handled by external source.
    /// </summary>
    public bool UseExternalPreview
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether subscription is enabled.
    /// </summary>
    public new bool EnableSubscription
    {
        get
        {
            if (!mEnableSubscription.HasValue)
            {
                // Find nearest forum viewer control to get enables subscription value
                ForumViewer fv = (ForumViewer)ControlsHelper.GetParentControl(this, typeof(ForumViewer));
                if (fv != null)
                {
                    mEnableSubscription = fv.EnableSubscription;
                }
                else
                {
                    mEnableSubscription = true;
                }
            }

            return mEnableSubscription.Value;
        }
        set
        {
            mEnableSubscription = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when a preview is required.
    /// </summary>
    public event EventHandler OnPreview;

    /// <summary>
    /// Occurs when a post is saved and requires moderation.
    /// </summary>
    public event EventHandler OnModerationRequired;

    #endregion


    /// <summary>
    /// OnLoad override.
    /// </summary>
    /// <param name="e">Event args</param>
    protected override void OnLoad(EventArgs e)
    {
        txtEmail
            .EnableClientSideEmailFormatValidation("NewPostforum", "Forums_WebInterface_ForumNewPost.emailErrorMsg")
            .RegisterCustomValidator(rfvEmailRequired);

        CopyValuesFromParent(this);
        Initialize();
        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ForumInfo fi = ForumContext.CurrentForum;
        if (!fi.ForumHTMLEditor)
        {
            // WAI validation
            lblText.AssociatedControlClientID = ucBBEditor.TextArea.ClientID;
        }
    }


    /// <summary>
    /// Sets post text with dependence on current setting, use HTML or text area.
    /// </summary>
    /// <param name="text">Text to set</param>
    private void SetPostText(string text)
    {
        // Set HTML editor or textarea
        if (ForumContext.CurrentForum.ForumHTMLEditor)
        {
            htmlTemplateBody.ResolvedValue = text;
        }
        else
        {
            ucBBEditor.Text = text;
        }
    }


    /// <summary>
    /// Initialize resources, holders, controls etc.
    /// </summary>
    protected void Initialize()
    {
        #region "Settings of HTML editor"

        // Set HTML editor properties
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.EditorAreaCSS = "";
        htmlTemplateBody.ToolbarSet = "Forum";
        htmlTemplateBody.DisableObjectResizing = true; // Disable image resizing
        htmlTemplateBody.RemovePlugins.Add("contextmenu"); // Disable context menu
        htmlTemplateBody.MediaDialogConfig.UseFullURL = true;
        htmlTemplateBody.LinkDialogConfig.UseFullURL = true;

        #endregion


        #region "Resource strings"

        // Resources
        rfvSubject.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.subjectErrorMsg");
        lblText.Text = GetString("Forums_WebInterface_ForumNewPost.text");
        rfvText.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.textErrorMsg");
        rfvUserName.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.usernameErrorMsg");
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");
        btnPreview.Text = GetString("Forums_WebInterface_ForumNewPost.Preview");
        lblSubscribe.Text = GetString("Forums_WebInterface_ForumNewPost.Subscription");
        lblSignature.Text = GetString("Forums_WebInterface_ForumNewPost.Signature");
        lblAttachFile.Text = GetString("For.NewPost.Attach");
        lblNickName.Text = GetString("Forums_WebInterface_ForumNewPost.NickName");

        #endregion


        #region "Controls visibility"

        ForumInfo fi = ForumContext.CurrentForum;

        // Hide or display html editor/ text area
        if (fi.ForumHTMLEditor)
        {
            ucBBEditor.Visible = false;
            rfvText.Enabled = false;

            // Define customizable shortcuts
            Hashtable keystrokes = new Hashtable()
                                       {
                                           { "link", "CKEDITOR.CTRL + 76 /*L*/" },
                                           { "bold", "CKEDITOR.CTRL + 66 /*B*/" },
                                           { "italic", "CKEDITOR.CTRL + 73 /*I*/" },
                                           { "underline", "CKEDITOR.CTRL + 85 /*U*/" }
                                       };

            if (!fi.ForumEnableURL)
            {
                htmlTemplateBody.RemoveButtons.Add("InsertUrl");
                if (!fi.ForumEnableAdvancedURL)
                {
                    // Remove the keyborad shortcut for the link insertion
                    keystrokes.Remove("link");
                }
            }
            if (!fi.ForumEnableAdvancedURL)
            {
                htmlTemplateBody.RemoveButtons.Add("InsertLink");
            }
            if (!fi.ForumEnableImage)
            {
                htmlTemplateBody.RemoveButtons.Add("InsertImage");
            }
            if (!fi.ForumEnableAdvancedImage)
            {
                htmlTemplateBody.RemoveButtons.Add("InsertImageOrMedia");
            }
            if (!fi.ForumEnableQuote)
            {
                htmlTemplateBody.RemoveButtons.Add("InsertQuote");
            }
            if (!fi.ForumEnableFontBold)
            {
                htmlTemplateBody.RemoveButtons.Add("Bold");
                keystrokes.Remove("bold");
            }
            if (!fi.ForumEnableFontItalics)
            {
                htmlTemplateBody.RemoveButtons.Add("Italic");
                keystrokes.Remove("italic");
            }
            if (!fi.ForumEnableFontUnderline)
            {
                htmlTemplateBody.RemoveButtons.Add("Underline");
                keystrokes.Remove("underline");
            }
            if (!fi.ForumEnableFontStrike)
            {
                htmlTemplateBody.RemoveButtons.Add("Strike");
            }
            if (!fi.ForumEnableFontColor)
            {
                htmlTemplateBody.RemoveButtons.Add("TextColor");
                htmlTemplateBody.RemoveButtons.Add("BGColor");
            }

            // Generate keystrokes string for the CK Editor
            StringBuilder sb = new StringBuilder("[ [ CKEDITOR.ALT + 121 /*F10*/, 'toolbarFocus' ], [ CKEDITOR.ALT + 122 /*F11*/, 'elementsPathFocus' ], [ CKEDITOR.CTRL + 90 /*Z*/, 'undo' ], [ CKEDITOR.CTRL + 89 /*Y*/, 'redo' ], [ CKEDITOR.CTRL + CKEDITOR.SHIFT + 90 /*Z*/, 'redo' ], [ CKEDITOR.ALT + ( CKEDITOR.env.ie || CKEDITOR.env.webkit ? 189 : 109 ) /*-*/, 'toolbarCollapse' ], [ CKEDITOR.ALT + 48 /*0*/, 'a11yHelp' ]");
            string format = ", [ {0}, '{1}' ]";

            foreach (DictionaryEntry entry in keystrokes)
            {
                sb.Append(String.Format(format, entry.Value, entry.Key));
            }

            sb.Append("]");
            htmlTemplateBody.Keystrokes = sb.ToString();
        }
        else
        {
            ucBBEditor.IsLiveSite = IsLiveSite;
            ucBBEditor.ShowImage = fi.ForumEnableImage;
            ucBBEditor.ShowQuote = fi.ForumEnableQuote;
            ucBBEditor.ShowURL = fi.ForumEnableURL;
            ucBBEditor.ShowBold = fi.ForumEnableFontBold;
            ucBBEditor.ShowItalic = fi.ForumEnableFontItalics;
            ucBBEditor.ShowUnderline = fi.ForumEnableFontUnderline;
            ucBBEditor.ShowStrike = fi.ForumEnableFontStrike;
            ucBBEditor.ShowColor = fi.ForumEnableFontColor;
            ucBBEditor.ShowCode = fi.ForumEnableCodeSnippet;
            ucBBEditor.ShowAdvancedImage = fi.ForumEnableAdvancedImage;
            ucBBEditor.ShowAdvancedURL = fi.ForumEnableAdvancedURL;
            htmlTemplateBody.Visible = false;
        }

        if ((fi.ForumModerated) && (!ForumContext.UserIsModerator(fi.ForumID, CommunityGroupID)))
        {
            ShowInformation(GetString("forums.requiremoderation"));
        }

        bool userCanModerate = fi.ForumModerated && ForumContext.UserIsModerator(fi.ForumID, CommunityGroupID);
        if ((MembershipContext.AuthenticatedUser.IsPublic()) || (!ForumInfoProvider.IsAuthorizedPerForum(fi.ForumID, fi.ForumGroupID, "AttachFiles", fi.AllowAttachFiles, MembershipContext.AuthenticatedUser) && !userCanModerate))
        {
            plcAttachFile.Visible = false;
        }

        // If user can choose thread type and this is not reply, show the options
        if ((fi.ForumType == 0) && (ForumContext.CurrentReplyThread == null))
        {
            // Only thread can be set
            if ((ForumContext.CurrentState != ForumStateEnum.EditPost) || (ForumContext.CurrentPost.PostLevel == 0))
            {
                plcThreadType.Visible = true;
            }
        }

        // Hide or display subscription checkbox with dependence
        // on allow subscription property value and security
        if ((!AllowSubscription) || (!ForumInfoProvider.IsAuthorizedPerForum(fi.ForumID, fi.ForumGroupID, "Subscribe", fi.AllowSubscribe, MembershipContext.AuthenticatedUser)))
        {
            SubscribeHolder.Visible = false;
        }

        // Display signature if is allowed
        if (!AllowSignature)
        {
            plcSignature.Visible = false;
        }

        // Display username textbox if is change name allowed or label with user name
        if (fi.ForumAllowChangeName || MembershipContext.AuthenticatedUser.IsPublic() || ((ForumContext.CurrentForum != null) && (ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, ForumContext.CommunityGroupID))))
        {
            if (!RequestHelper.IsPostBack())
            {
                // Do not show 'public' for unauthenticated user
                if (!MembershipContext.AuthenticatedUser.IsPublic())
                {
                    txtUserName.Text = UserName;
                }
            }
            plcNickName.Visible = false;
        }
        else
        {
            if (ForumContext.CurrentMode != ForumMode.Edit)
            {
                lblNickNameValue.Text = HTMLHelper.HTMLEncode(UserName);
            }
            else
            {
                lblNickNameValue.Text = HTMLHelper.HTMLEncode(ForumContext.CurrentPost.PostUserName);
            }
            plcUserName.Visible = false;
        }

        // Prefill user email and reset the security code
        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
        }

        if (ForumContext.CurrentReplyThread != null)
        {
            string replyPrefix = GetString("forums.replyprefix");
            if (!ForumContext.CurrentReplyThread.PostSubject.StartsWithCSafe(replyPrefix))
            {
                txtSubject.Text = replyPrefix + ForumContext.CurrentReplyThread.PostSubject;
                txtSubject.Text = TextHelper.LimitLength(txtSubject.Text, POST_SUBJECT_LENGTH, "");
            }
            else
            {
                txtSubject.Text = ForumContext.CurrentReplyThread.PostSubject;
            }
            txtSubject.Text = txtSubject.Text;


            // New post - check max level for subscribcribtion
            if (ForumContext.CurrentReplyThread.PostLevel >= ForumPostInfoProvider.MaxPostLevel - 1)
            {
                SubscribeHolder.Visible = false;
            }
        }
        // Edit post - check max level for subscribcribtion
        else if ((ForumContext.CurrentPost != null) && (ForumContext.CurrentPost.PostLevel >= ForumPostInfoProvider.MaxPostLevel))
        {
            SubscribeHolder.Visible = false;
        }


        // Hide subscription if not enabled
        if (!EnableSubscription)
        {
            SubscribeHolder.Visible = false;
        }

        #endregion


        #region "Post Data"

        if (!RequestHelper.IsPostBack())
        {
            // Check whether current state is edit
            if (ForumContext.CurrentState == ForumStateEnum.EditPost)
            {
                txtEmail.Text = ForumContext.CurrentPost.PostUserMail;
                txtSignature.Text = ForumContext.CurrentPost.PostUserSignature;
                txtSubject.Text = ForumContext.CurrentPost.PostSubject;
                txtUserName.Text = ForumContext.CurrentPost.PostUserName;

                SetPostText(ForumContext.CurrentPost.PostText);


                radTypeDiscussion.Checked = true;

                if (ForumContext.CurrentPost.PostType == 1)
                {
                    radTypeQuestion.Checked = true;
                }
            }
            else if ((ForumContext.CurrentMode == ForumMode.Quote) && (ForumContext.CurrentReplyThread != null))
            {
                // Indicates whether wysiwyg editor is used
                bool isHtml = ForumContext.CurrentForum.ForumHTMLEditor;
                // Keeps post user name
                string userName = ForumContext.CurrentReplyThread.PostUserName;

                // Encode username for wysiwyg editor
                if (isHtml)
                {
                    userName = HTMLHelper.HTMLEncode(userName);
                }

                SetPostText(DiscussionMacroResolver.GetQuote(userName, ForumContext.CurrentReplyThread.PostText));

                // Set new line after
                if (isHtml)
                {
                    htmlTemplateBody.ResolvedValue += "<br /><br />";
                }
                else
                {
                    ucBBEditor.Text += "\n";
                }
            }
        }

        #endregion
    }


    /// <summary>
    /// OK click hadler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        #region "Security"

        // Check whether forum exists
        if (ForumContext.CurrentForum == null)
        {
            return;
        }

        // Check security
        bool securityCheck = true;
        switch (ForumContext.CurrentState)
        {
            case ForumStateEnum.NewThread:
                securityCheck = IsAvailable(ForumContext.CurrentForum, ForumActionType.NewThread);
                break;

            case ForumStateEnum.ReplyToPost:
                securityCheck = IsAvailable(ForumContext.CurrentForum, ForumActionType.Reply);
                break;

            case ForumStateEnum.EditPost:
                securityCheck = ForumContext.CurrentPost != null && IsAvailable(ForumContext.CurrentPost, ForumActionType.Edit);
                break;
        }

        if (!securityCheck)
        {
            ShowError(GetString("ForumNewPost.PermissionDenied"));
            return;
        }

        #region "Email field"

        // Check if email is set when it is required. Email format must be valid when it is set.
        var isEmailRequired = ForumContext.CurrentForum.ForumRequireEmail || chkSubscribe.Checked;
        var isEmailInvalid = (isEmailRequired && String.IsNullOrWhiteSpace(txtEmail.Text)) || !txtEmail.IsValid();
        if (isEmailInvalid)
        {
            ShowError(GetString("Forums_WebInterface_ForumNewPost.emailErrorMsg"));
            return;
        }

        #endregion


        #region "Subject"

        // Check whether subject is filled
        if (txtSubject.Text.Trim() == "")
        {
            ShowError(rfvSubject.ErrorMessage);
            return;
        }

        #endregion


        #region "Text"

        var validator = new Validator();
        string result;

        // Check post text in HTML editor or text area
        if (!ForumContext.CurrentForum.ForumHTMLEditor)
        {
            // Check whether post text is added in text area
            if ((result = validator.NotEmpty(DiscussionMacroResolver.RemoveTags(ucBBEditor.Text), rfvText.ErrorMessage).Result) != "")
            {
                ShowError(result);
                return;
            }
        }
        else
        {
            // Check whether post text is added in HTML editor
            if ((result = validator.NotEmpty(htmlTemplateBody.ResolvedValue, rfvText.ErrorMessage).Result) != "")
            {
                ShowError(result);
                return;
            }
        }

        #endregion


        #region "User name"

        // Check whether user name is filled if user name field is visible
        if (ForumContext.CurrentForum.ForumAllowChangeName || MembershipContext.AuthenticatedUser.IsPublic() || ((ForumContext.CurrentForum != null) && (ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, ForumContext.CommunityGroupID))))
        {
            validator = new Validator();

            if (!String.IsNullOrEmpty(result = validator.NotEmpty(txtUserName.Text, rfvUserName.ErrorMessage).Result))
            {
                ShowError(result);
                return;
            }
        }

        #endregion


        #endregion


        #region "Forum post properties"

        bool newPost = false;

        // Current forum info object
        ForumInfo fi = ForumContext.CurrentForum;

        // Forum post info object
        ForumPostInfo fp;

        // Get forum post info with dependence on current state
        if (ForumContext.CurrentState == ForumStateEnum.EditPost)
        {
            // Get existing object
            fp = ForumContext.CurrentPost;
            fp.PostLastEdit = DateTime.Now;
        }
        else
        {
            // Create new forum post info object
            fp = new ForumPostInfo();
            newPost = true;
        }

        fp.SiteId = fi.ForumSiteID;

        #region "Ad-hoc forum"

        if (IsAdHocForum && (ForumContext.CurrentForum.ForumID == 0))
        {
            if (DocumentContext.CurrentDocument == null)
            {
                ShowError(GetString("forums.documentdoesnotexist"));
                return;
            }

            fi.ForumGroupID = ForumGroupInfoProvider.GetAdHocGroupInfo(SiteID).GroupID;
            fi.ForumName = "AdHoc-" + Guid.NewGuid();
            fi.ForumDisplayName = TextHelper.LimitLength(DocumentContext.CurrentDocument.GetDocumentName(), POST_USERNAME_LENGTH, String.Empty);
            fi.ForumOpen = true;
            fi.ForumModerated = false;
            fi.ForumAccess = 040000;
            fi.ForumThreads = 0;
            fi.ForumPosts = 0;
            fi.ForumLogActivity = LogActivity;
            ForumInfoProvider.SetForumInfo(fi);

            ForumContext.CurrentForum.ForumID = fi.ForumID;
            ForumContext.ForumID = fi.ForumID;
            ForumID = fi.ForumID;
        }

        #endregion


        // Post forum
        fp.PostForumID = ForumContext.CurrentForum.ForumID;
        // Get forum post info with dependence on current state
        if (ForumContext.CurrentState != ForumStateEnum.EditPost)
        {
            // Post time
            fp.PostTime = DateTime.Now;
            // User IP address
            fp.PostInfo.IPAddress = RequestContext.UserHostAddress;
            // User agent
            fp.PostInfo.Agent = Request.UserAgent;
            // Post user id
            if (!MembershipContext.AuthenticatedUser.IsPublic())
            {
                fp.PostUserID = MembershipContext.AuthenticatedUser.UserID;
            }

            // Post signature
            fp.PostUserSignature = txtSignature.Text;
        }

        // Post subject
        fp.PostSubject = txtSubject.Text;
        // Post user email
        fp.PostUserMail = txtEmail.Text;


        // Post type
        int forumType = ForumContext.CurrentForum.ForumType;
        if (forumType == 0)
        {
            if (ForumContext.CurrentReplyThread == null)
            {
                // New thread - use type which user chosen
                fp.PostType = (radTypeDiscussion.Checked ? 0 : 1);
            }
            else
            {
                // Reply - use parent type
                fp.PostType = ForumContext.CurrentReplyThread.PostType;
            }
        }
        else
        {
            // Fixed type - use the forum setting
            fp.PostType = forumType - 1;
        }

        // Set username if change name is allowed
        if (fi.ForumAllowChangeName || MembershipContext.AuthenticatedUser.IsPublic() || ForumContext.UserIsModerator(fp.PostForumID, ForumContext.CommunityGroupID))
        {
            fp.PostUserName = TextHelper.LimitLength(txtUserName.Text, POST_USERNAME_LENGTH, "");
        }
        else
        {
            // Get forum post info with dependence on current state
            if (ForumContext.CurrentState != ForumStateEnum.EditPost)
            {
                fp.PostUserName = UserName;
            }
        }

        // Post parent id -> reply to
        if (ForumContext.CurrentReplyThread != null)
        {
            fp.PostParentID = ForumContext.CurrentReplyThread.PostId;

            // Check max relative level
            if ((MaxRelativeLevel > -1) && (ForumContext.CurrentReplyThread.PostLevel >= MaxRelativeLevel))
            {
                ShowError(GetString("Forums.MaxRelativeLevelError"));
                return;
            }
        }

        // Get post text from HTML editor if is enabled
        fp.PostText = ForumContext.CurrentForum.ForumHTMLEditor ? htmlTemplateBody.ResolvedValue : ucBBEditor.Text;

        // Approve post if forum is not moderated
        if (newPost)
        {
            if (!ForumContext.CurrentForum.ForumModerated)
            {
                fp.PostApproved = true;
            }
            else
            {
                if (ForumContext.UserIsModerator(fp.PostForumID, CommunityGroupID))
                {
                    fp.PostApproved = true;
                    fp.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                }
            }
        }

        // If signature is enabled then
        if (EnableSignature)
        {
            fp.PostUserSignature = MembershipContext.AuthenticatedUser.UserSignature;
        }

        #endregion


        if (!BadWordInfoProvider.CanUseBadWords(MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName))
        {
            // Prepare columns to check
            Dictionary<string, int> columns = new Dictionary<string, int>();
            columns.Add("PostText", 0);
            columns.Add("PostSubject", 450);
            columns.Add("PostUserSignature", 0);
            columns.Add("PostUserName", 200);

            // Perform bad words check
            string badMessage = BadWordsHelper.CheckBadWords(fp, columns, "PostApproved", "PostApprovedByUserID", fp.PostText, MembershipContext.AuthenticatedUser.UserID, () => { return ValidatePost(fp); });

            if (String.IsNullOrEmpty(badMessage))
            {
                if (!ValidatePost(fp))
                {
                    badMessage = GetString("ForumNewPost.EmptyBadWord");
                }
            }

            if (!String.IsNullOrEmpty(badMessage))
            {
                ShowError(badMessage);
                return;
            }
        }



        // Flood protection
        if (FloodProtectionHelper.CheckFlooding(SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            ShowError(GetString("General.FloodProtection"));
            return;
        }

        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        string baseUrl = ForumContext.CurrentForum.ForumBaseUrl;
        if (String.IsNullOrEmpty(baseUrl))
        {
            baseUrl = FriendlyBaseURL;
        }

        string unsubscriptionUrl = ForumContext.CurrentForum.ForumUnsubscriptionUrl;
        if (String.IsNullOrEmpty(unsubscriptionUrl))
        {
            unsubscriptionUrl = UnsubscriptionURL;
        }

        // USe parent post id for new post
        int subscibePostId = newPost ? fp.PostParentID : fp.PostId;

        // Check subscriptions
        if ((chkSubscribe.Checked) && (!String.IsNullOrEmpty(txtEmail.Text)) && (ForumSubscriptionInfoProvider.IsSubscribed(txtEmail.Text.Trim(), fp.PostForumID, subscibePostId)))
        {
            // Post of the forum is already subscribed to this email -> show an error
            chkSubscribe.Checked = false;
            ShowError(GetString("Forums.EmailAlreadySubscribed"));
            return;
        }

        // Save post object
        ForumPostInfoProvider.SetForumPostInfo(fp, baseUrl, unsubscriptionUrl);


        #region "Subscription"

        // If subscribe is checked create new subscription to the current post
        if ((chkSubscribe.Checked) && (!ForumSubscriptionInfoProvider.IsSubscribed(fp.PostUserMail, fp.PostForumID, fp.PostId)))
        {
            // Create new subscription info object
            ForumSubscriptionInfo fsi = new ForumSubscriptionInfo();
            // Set info properties
            fsi.SubscriptionForumID = fp.PostForumID;
            fsi.SubscriptionEmail = fp.PostUserMail;
            fsi.SubscriptionPostID = fp.PostId;
            fsi.SubscriptionUserID = fp.PostUserID;
            fsi.SubscriptionGUID = Guid.NewGuid();

            // Save subscription
            ForumSubscriptionInfoProvider.Subscribe(fsi, DateTime.Now, true, true);
        }

        #endregion


        bool moderationRequired = false;
        if ((!fp.PostApproved) && (!ForumContext.UserIsModerator(fp.PostForumID, CommunityGroupID)))
        {
            moderationRequired = true;
            if (OnModerationRequired != null)
            {
                OnModerationRequired(this, null);
            }
        }

        // Keep current user info
        var currentUser = MembershipContext.AuthenticatedUser;

        if (AuthenticationHelper.IsAuthenticated() && chkAttachFile.Checked && (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || ForumContext.CurrentForum.AllowAttachFiles != SecurityAccessEnum.Nobody))
        {
            // Redirect to the post attachments
            string attachmentUrl = GetURL(fp, ForumActionType.Attachment);
            if (moderationRequired)
            {
                attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "moderated", "1");
            }
            URLHelper.Redirect(UrlResolver.ResolveUrl(attachmentUrl));
        }
        else
        {
            if (!StopProcessing)
            {
                // Redirect back to the forum or forum thread
                URLHelper.Redirect(ClearURL());
            }
        }
    }


    /// <summary>
    /// Preview button click handler.
    /// </summary>
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        // If external preview is enabled fire OnPreview event
        if (UseExternalPreview)
        {
            #region "Forum properties"

            // Get forum post info with dependence on current state
            ForumPostInfo fp = ForumContext.CurrentState == ForumStateEnum.EditPost ? ForumContext.CurrentPost : new ForumPostInfo();

            // Post forum
            fp.PostForumID = ForumContext.CurrentForum.ForumID;
            // Post time
            fp.PostTime = DateTime.Now;
            // Post subject
            fp.PostSubject = txtSubject.Text;
            // Post user email
            fp.PostUserMail = txtEmail.Text;
            // Post signature
            fp.PostUserSignature = txtSignature.Text;

            // Post user id
            if (!MembershipContext.AuthenticatedUser.IsPublic())
            {
                fp.PostUserID = MembershipContext.AuthenticatedUser.UserID;
            }

            // Post user name
            if (ForumContext.CurrentForum.ForumAllowChangeName || MembershipContext.AuthenticatedUser.IsPublic() || (ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, ForumContext.CommunityGroupID)))
            {
                fp.PostUserName = TextHelper.LimitLength(txtUserName.Text, POST_USERNAME_LENGTH, "");
            }
            else
            {
                fp.PostUserName = UserName;
            }

            // Post parent id -> reply to
            if (ForumContext.CurrentReplyThread != null)
            {
                fp.PostParentID = ForumContext.CurrentReplyThread.PostId;
            }

            // Get post text from HTML editor if is enabled
            fp.PostText = ForumContext.CurrentForum.ForumHTMLEditor ? htmlTemplateBody.ResolvedValue : ucBBEditor.Text;

            #endregion


            if (OnPreview != null)
            {
                OnPreview(fp, null);
            }
        }
        else
        {
            pnlReplyPost.Visible = true;
            lblSubjectPreview.Text = HTMLHelper.HTMLEncode(txtSubject.Text);

            // Get forum text from HTML editor or text area
            if (ForumContext.CurrentForum.ForumHTMLEditor)
            {
                lblTextPreview.Text = htmlTemplateBody.ResolvedValue;
            }
            else
            {
                lblTextPreview.Text = HTMLHelper.HTMLEncodeLineBreaks(ucBBEditor.Text);
            }
        }
    }


    /// <summary>
    /// Cancel button click handler.
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(ClearURL());
    }


    private static bool ValidatePost(ForumPostInfo fpi)
    {
        if ((fpi.PostSubject == null) || (fpi.PostText == null) || (fpi.PostUserName == null))
        {
            return false;
        }

        return ((fpi.PostSubject.Trim() != "") && (fpi.PostText.Trim() != "") && (fpi.PostUserName.Trim() != ""));
    }
}
