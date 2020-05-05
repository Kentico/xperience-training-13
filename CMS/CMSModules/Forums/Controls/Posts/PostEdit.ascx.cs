using System;
using System.Collections;

using CMS.Base;

using System.Text;
using System.Threading;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Posts_PostEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mForumID = 0;
    private int mEditPostID = 0;
    private ForumInfo fi = null;
    private ForumPostInfo editPi = null;
    private ForumPostInfo replyPi = null;
    private bool mRedirectEnabled = true;
    private bool mDisableCancelButton = false;

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
    /// Gets or sets the value that indicates whether redirect is enabled.
    /// </summary>
    public bool RedirectEnabled
    {
        get
        {
            return mRedirectEnabled;
        }
        set
        {
            mRedirectEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets forum id.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumID;
        }
        set
        {
            mForumID = value;
        }
    }


    /// <summary>
    /// Gets or sets post id of reply post.
    /// </summary>
    public int ReplyToPostID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ReplyToPostID"], 0);
        }
        set
        {
            ViewState["ReplyToPostID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets post id of post which should be edited.
    /// </summary>
    public int EditPostID
    {
        get
        {
            return mEditPostID;
        }
        set
        {
            mEditPostID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether cancel button should be disabled.
    /// </summary>
    public bool DisableCancelButton
    {
        get
        {
            return mDisableCancelButton;
        }
        set
        {
            mDisableCancelButton = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        txtEmail
            .EnableClientSideEmailFormatValidation("NewPostforum", "Forums_WebInterface_ForumNewPost.emailErrorMsg")
            .RegisterCustomValidator(rfvEmailRequired);

        bool process = (Visible && !StopProcessing);

        fi = ForumInfoProvider.GetForumInfo(ForumID);
        replyPi = ForumPostInfoProvider.GetForumPostInfo(ReplyToPostID);
        editPi = ForumPostInfoProvider.GetForumPostInfo(EditPostID);

        if ((fi == null) && (editPi != null))
        {
            fi = ForumInfoProvider.GetForumInfo(editPi.PostForumID);
            ForumID = fi.ForumID;
        }

        // Check whether the post still exists
        if (EditPostID > 0)
        {
            EditedObject = editPi;
        }

        // Check whether the post still exists
        if ((ForumID > 0) && (fi == null))
        {
            RedirectToInformation("editedobject.notexists");
        }


        #region "HTML Editor properties"

        // Set HTML editor properties
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.EditorAreaCSS = "";
        htmlTemplateBody.ToolbarSet = "Forum";
        htmlTemplateBody.DisableObjectResizing = true; // Disable image resizing
        htmlTemplateBody.RemovePlugins.Add("contextmenu"); // Disable context menu
        htmlTemplateBody.IsLiveSite = IsLiveSite;
        htmlTemplateBody.MediaDialogConfig.UseFullURL = true;
        htmlTemplateBody.LinkDialogConfig.UseFullURL = true;

        #endregion


        if (fi != null)
        {
            if ((fi.ForumType == 0) && (replyPi == null))
            {
                plcThreadType.Visible = true;
            }

            if (fi.ForumRequireEmail)
            {
                rfvEmailRequired.Enabled = true;
                rfvEmailRequired.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.emailRequireErrorMsg");
            }


            #region "Forum text"

            rfvText.Enabled = !fi.ForumHTMLEditor;
            htmlTemplateBody.Visible = fi.ForumHTMLEditor;
            ucBBEditor.Visible = !fi.ForumHTMLEditor;

            if (fi.ForumHTMLEditor)
            {
                // Define customizable shortcuts
                Hashtable keystrokes = new Hashtable()
                                           {
                                               { "link", "CKEDITOR.CTRL + 76 /*L*/" },
                                               { "bold", "CKEDITOR.CTRL + 66 /*B*/" },
                                               { "italic", "CKEDITOR.CTRL + 73 /*I*/" },
                                               { "underline", "CKEDITOR.CTRL + 85 /*U*/" }
                                           };

                // Register script for HTML Editor forum buttons control
                if (!fi.ForumEnableURL)
                {
                    htmlTemplateBody.RemoveButtons.Add("InsertUrl");
                    if (!fi.ForumEnableAdvancedURL)
                    {
                        // Remove the keyboard shortcut for the link insertion
                        keystrokes.Remove("link");
                    }
                }
                if (!fi.ForumEnableImage)
                {
                    htmlTemplateBody.RemoveButtons.Add("InsertImage");
                }
                if (!fi.ForumEnableQuote)
                {
                    htmlTemplateBody.RemoveButtons.Add("InsertQuote");
                }
                if (!fi.ForumEnableAdvancedURL)
                {
                    htmlTemplateBody.RemoveButtons.Add("InsertLink");
                }
                if (!fi.ForumEnableAdvancedImage)
                {
                    htmlTemplateBody.RemoveButtons.Add("InsertImageOrMedia");
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
                ucBBEditor.ShowBold = fi.ForumEnableFontBold;
                ucBBEditor.ShowItalic = fi.ForumEnableFontItalics;
                ucBBEditor.ShowUnderline = fi.ForumEnableFontUnderline;
                ucBBEditor.ShowStrike = fi.ForumEnableFontStrike;
                ucBBEditor.ShowCode = fi.ForumEnableCodeSnippet;
                ucBBEditor.ShowColor = fi.ForumEnableFontColor;
                ucBBEditor.ShowURL = fi.ForumEnableURL;
                ucBBEditor.ShowAdvancedImage = fi.ForumEnableAdvancedImage;
                ucBBEditor.ShowAdvancedURL = fi.ForumEnableAdvancedURL;

                // WAI validation
                lblText.AssociatedControlClientID = ucBBEditor.TextArea.ClientID;
            }

            #endregion
        }

        // Do not show subscribe checkbox if no post can't be added under the post
        if ((replyPi != null) && (replyPi.PostLevel >= ForumPostInfoProvider.MaxPostLevel - 1))
        {
            plcSubscribe.Visible = false;
        }


        #region "Resources"

        rfvSubject.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.subjectErrorMsg");
        lblText.Text = GetString("Forums_WebInterface_ForumNewPost.text");
        rfvText.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.textErrorMsg");
        rfvUserName.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.usernameErrorMsg");
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");
        btnPreview.Text = GetString("Forums_WebInterface_ForumNewPost.Preview");
        lblSubscribe.Text = GetString("Forums_WebInterface_ForumNewPost.Subscription");
        lblSignature.Text = GetString("Forums_WebInterface_ForumNewPost.Signature");
        lblPostIsAnswerLabel.Text = GetString("ForumPost_Edit.PostIsAnswer");
        lblPostIsNotAnswerLabel.Text = GetString("ForumPost_Edit.PostIsNotAnswer");

        #endregion


        if (!IsLiveSite && !RequestHelper.IsPostBack() && process)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// When post is added.
    /// </summary>
    public event EventHandler OnInsertPost;


    /// <summary>
    /// When preview is requested.
    /// </summary>
    public event EventHandler OnPreview;


    /// <summary>
    /// Btn OK handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (editPi == null)
        {
            editPi = ForumPostInfoProvider.GetForumPostInfo(EditPostID);
        }

        // Sets the current or parent post id
        int subscibePostId = 0;

        if (editPi != null)
        {
            editPi.PostLastEdit = DateTime.Now;
            editPi.PostIsAnswer = ValidationHelper.GetInteger(txtPostIsAnswer.Text, editPi.PostIsAnswer);
            editPi.PostIsNotAnswer = ValidationHelper.GetInteger(txtPostIsNotAnswer.Text, editPi.PostIsNotAnswer);
        }
        else
        {
            // Create new post
            editPi = new ForumPostInfo();

            // Set as reply
            if (replyPi != null)
            {
                editPi.PostParentID = replyPi.PostId;
                subscibePostId = replyPi.PostId;
            }

            editPi.PostUserID = MembershipContext.AuthenticatedUser.UserID;
            editPi.PostForumID = fi.ForumID;
            editPi.SiteId = fi.ForumSiteID;
            editPi.PostTime = DateTime.Now;
            editPi.PostApproved = true;
            editPi.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
        }


        #region "Security"

        string result = new Validator()
            .NotEmpty(txtSubject.Text, rfvSubject.ErrorMessage)
            .NotEmpty(txtUserName, rfvUserName.ErrorMessage)
            .Result;

        // Check if is some text in TextArea or in HTMLEditor
        if (result == "")
        {
            if (fi.ForumHTMLEditor)
            {
                if (htmlTemplateBody.ResolvedValue.Trim() == "")
                {
                    result = rfvText.ErrorMessage;
                }
                editPi.PostText = htmlTemplateBody.ResolvedValue;
            }
            else
            {
                if (DiscussionMacroResolver.RemoveTags(ucBBEditor.Text).Trim() == "")
                {
                    result = rfvText.ErrorMessage;
                }
                editPi.PostText = ucBBEditor.Text;
            }
        }

        if (result == "")
        {
            var isEmailRequired = fi.ForumRequireEmail || chkSubscribe.Checked;
            var isEmailInvalid = (isEmailRequired || (txtEmail.Text != String.Empty)) && !txtEmail.IsValid();
            if (isEmailInvalid)
            {
                ShowError(GetString("Forums_WebInterface_ForumNewPost.emailErrorMsg"));
                if (chkSubscribe.Checked && String.IsNullOrEmpty(txtEmail.Text))
                {
                    ShowError(GetString("Forums.Emailsubscribe"));
                }
                Visible = true;
                return;
            }
        }

        #endregion


        // Check subscriptions
        if ((chkSubscribe.Checked) && (!String.IsNullOrEmpty(txtEmail.Text)) && (ForumSubscriptionInfoProvider.IsSubscribed(txtEmail.Text.Trim(), editPi.PostForumID, subscibePostId)))
        {
            // Post of the forum is already subscribed to this email -> show an error
            result = GetString("Forums.EmailAlreadySubscribed");
            chkSubscribe.Checked = false;
        }

        if (String.IsNullOrEmpty(result))
        {
            if (fi.ForumType == 0)
            {
                editPi.PostType = (radTypeQuestion.Checked) ? 1 : 0;
            }

            editPi.PostUserName = TextHelper.LimitLength(txtUserName.Text, POST_USERNAME_LENGTH, "");
            editPi.PostSubject = TextHelper.LimitLength(txtSubject.Text, POST_SUBJECT_LENGTH, "");
            editPi.PostUserMail = txtEmail.Text;
            editPi.PostUserSignature = txtSignature.Text;

            ForumPostInfoProvider.SetForumPostInfo(editPi);
            EditPostID = editPi.PostId;


            #region "Subscription"

            if ((chkSubscribe.Checked) && (!String.IsNullOrEmpty(editPi.PostUserMail)))
            {
                ForumSubscriptionInfo fsi = new ForumSubscriptionInfo();
                fsi.SubscriptionForumID = ForumID;
                fsi.SubscriptionEmail = editPi.PostUserMail;
                fsi.SubscriptionPostID = editPi.PostId;
                fsi.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
                fsi.SubscriptionGUID = Guid.NewGuid();

                ForumSubscriptionInfoProvider.Subscribe(fsi, DateTime.Now, true, true);
            }

            #endregion


            ClearForm();

            if (OnInsertPost != null)
            {
                OnInsertPost(this, null);
            }

            RaiseOnSaved();
        }
        else
        {
            ShowError(result);
            return;
        }
    }


    public event EventHandler OnCancelClick;


    /// <summary>
    /// Cancel click handler.
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();

        if (OnCancelClick != null)
        {
            OnCancelClick(this, null);
        }

        if (RedirectEnabled)
        {
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
    }


    /// <summary>
    /// Preview click handler.
    /// </summary>
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        ForumPostInfo fp = new ForumPostInfo();

        fp.PostUserMail = txtEmail.Text;
        fp.PostUserName = TextHelper.LimitLength(txtUserName.Text, POST_USERNAME_LENGTH, "");
        fp.PostText = ucBBEditor.Text;

        if (fi.ForumHTMLEditor)
        {
            fp.PostText = htmlTemplateBody.ResolvedValue;
        }

        fp.PostTime = DateTime.Now;
        fp.PostSubject = txtSubject.Text;
        fp.PostUserSignature = txtSignature.Text;
        fp.PostForumID = (editPi != null) ? editPi.PostForumID : ForumID;
        fp.PostUserID = (editPi != null) ? editPi.PostUserID : MembershipContext.AuthenticatedUser.UserID;

        if (replyPi != null)
        {
            fp.PostLevel = replyPi.PostLevel + 1;
        }

        ForumPost1.DisplayOnly = true;
        ForumPost1.PostData = fp;

        pnlReplyPost.Visible = true;
        lblHeader.Text = GetString("Forums_ForumNewPost_Header.Preview");

        // Fire the event
        if (OnPreview != null)
        {
            OnPreview(this, null);
        }
    }


    /// <summary>
    /// Clear from fields.
    /// </summary>
    public override void ClearForm()
    {
        ucBBEditor.Text = "";
        txtSubject.Text = "";
        txtEmail.Text = "";
        txtUserName.Text = "";
        txtSignature.Text = "";
        htmlTemplateBody.ResolvedValue = "";
        pnlReplyPost.Visible = false;
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        fi = ForumInfoProvider.GetForumInfo(ForumID);
        replyPi = ForumPostInfoProvider.GetForumPostInfo(ReplyToPostID);
        editPi = ForumPostInfoProvider.GetForumPostInfo(EditPostID);
        var cui = MembershipContext.AuthenticatedUser;

        // Edit post
        if (editPi != null)
        {
            ForumPost1.PostID = editPi.PostId;
            ForumPost1.DisplayOnly = true;

            if (DisableCancelButton)
            {
                btnCancel.Enabled = false;
            }
            ucBBEditor.Text = editPi.PostText;
            htmlTemplateBody.ResolvedValue = editPi.PostText;
            txtSubject.Text = editPi.PostSubject;
            txtSignature.Text = editPi.PostUserSignature;
            txtEmail.Text = editPi.PostUserMail;
            txtUserName.Text = editPi.PostUserName;
            plcIsAnswer.Visible = true;
            plcIsNotAnswer.Visible = true;
            txtPostIsAnswer.Text = editPi.PostIsAnswer.ToString();
            txtPostIsNotAnswer.Text = editPi.PostIsNotAnswer.ToString();

            if ((fi != null) && (fi.ForumType == 0) && (editPi.PostLevel == 0))
            {
                if (editPi.PostType == 0)
                {
                    radTypeDiscussion.Checked = true;
                }
                else
                {
                    radTypeQuestion.Checked = true;
                }
            }
            else
            {
                plcThreadType.Visible = false;
            }
        }
        else
        {
            // Reply to post
            if (replyPi != null)
            {
                pnlReplyPost.Visible = true;
                ForumPost1.PostID = replyPi.PostId;
                ForumPost1.DisplayOnly = true;

                plcThreadType.Visible = false;
                txtSignature.Text = cui.UserSignature;
                txtEmail.Text = cui.Email;

                if (!String.IsNullOrEmpty(cui.UserNickName))
                {
                    txtUserName.Text = cui.UserNickName.Trim();
                }
                else
                {
                    txtUserName.Text = Functions.GetFormattedUserName(cui.UserName, IsLiveSite);
                }

                if (!replyPi.PostSubject.StartsWithCSafe(GetString("Forums.ReplyPrefix")))
                {
                    txtSubject.Text = GetString("Forums.ReplyPrefix") + replyPi.PostSubject;
                    txtSubject.Text = TextHelper.LimitLength(txtSubject.Text, POST_SUBJECT_LENGTH, "");
                }
                else
                {
                    txtSubject.Text = replyPi.PostSubject;
                }
                btnCancel.Enabled = true;
            }
            // Create new thread
            else
            {
                btnCancel.Enabled = false;
                txtSignature.Text = cui.UserSignature;
                txtEmail.Text = cui.Email;

                if (!String.IsNullOrEmpty(cui.UserNickName))
                {
                    txtUserName.Text = cui.UserNickName.Trim();
                }
                else
                {
                    txtUserName.Text = Functions.GetFormattedUserName(cui.UserName, IsLiveSite);
                }
            }
        }
    }
}