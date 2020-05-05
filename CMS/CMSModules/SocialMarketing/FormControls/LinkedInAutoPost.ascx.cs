using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.SocialMarketing.Web.UI;

public partial class CMSModules_SocialMarketing_FormControls_LinkedInAutoPost : SocialMarketingAutoPostControl
{
    #region "Private fields"

    private string mDefaultValue;
    private bool mPublishedWhileEditing;
    private bool mPostInfoSet;
    private LinkedInPostInfo mPostInfo;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets edited LinkedIn post info object. Value of the control representing ID of the post is updated when a new post object is set.
    /// </summary>
    private LinkedInPostInfo PostInfo
    {
        get
        {
            if (!mPostInfoSet)
            {
                var document = Document;
                if ((mPostInfo == null) && (document != null) && (document.DocumentGUID != Guid.Empty) && IsFeatureAvailable)
                {
                    mPostInfo = LinkedInPostInfoProvider.GetLinkedInPostInfosByDocumentGuid(document.DocumentGUID, SiteIdentifier).ToList().FirstOrDefault(x => x.LinkedInPostIsCreatedByUser);
                }
                mPostInfoSet = true;
            }

            return mPostInfo;
        }
        set
        {
            mPostInfo = value;
            mPostInfoSet = true;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the editing form is displayed or not.
    /// </summary>
    private bool DisplayForm
    {
        get
        {
            return chkPostToLinkedIn.Checked;
        }
        set
        {
            chkPostToLinkedIn.Checked = value;
            pnlForm.Visible = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value of this control. Value is an integer representing the ID of the LinkedIn post
    /// </summary>
    public override object Value
    {
        get
        {
            if ((PostInfo != null) && (PostInfo.LinkedInPostID > 0))
            {
                return PostInfo.LinkedInPostID;
            }

            return null;
        }
        set
        {
            if ((value == null) || !IsFeatureAvailable)
            {
                return;
            }

            // Try to get post info ID
            int postId = ValidationHelper.GetInteger(value, 0);
            if (postId > 0)
            {
                PostInfo = LinkedInPostInfoProvider.GetLinkedInPostInfo(postId);

                return;
            }

            string defaultValue = value.ToString();
            if (!String.IsNullOrWhiteSpace(defaultValue))
            {
                // Consider given value as default post text
                mDefaultValue = defaultValue;
            }
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            SetUnderlyingControlsEnabledState(value);
            if (!value && !StopProcessing && (PostInfo == null) && IsFeatureAvailable)
            {
                // Control gets disabled and post info does not exist - initialize inner controls to its default values
                InitializeControls();
            }
        }
    }


    /// <summary>
    /// Gets the controls messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Checks if entered data are valid.
    /// </summary>
    public override bool IsValid()
    {
        if (!chkPostToLinkedIn.Checked || (PostInfo != null) && !PostInfo.IsEditable)
        {
            return true;
        }

        if (String.IsNullOrWhiteSpace(txtPost.Text))
        {
            ValidationError = ResHelper.GetString("sm.linkedin.posts.msg.postempty");

            return false;
        }

        if (ValidationHelper.GetInteger(companySelector.Value, 0) <= 0)
        {
            ValidationError = ResHelper.GetString("sm.linkedin.posts.msg.selectaccount");
            return false;
        }

        if (txtPost.Text.Length > LinkedInHelper.COMPANY_SHARE_COMMENT_MAX_LENGTH)
        {
            ValidationError = String.Format(GetString("basicform.invalidlength"), LinkedInHelper.COMPANY_SHARE_COMMENT_MAX_LENGTH);
            return false;
        }

        return true;
    }

    #endregion


    #region "Life-cycle methods and event handlers"

    /// <summary>
    /// OnInit event.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;

        if (!IsFeatureAvailable || !HasUserReadPermission)
        {
            StopProcessing = true;

            return;
        }

        if (Form != null)
        {
            Form.OnBeforeDataRetrieval += Form_OnBeforeDataRetrieval;
            Form.OnAfterSave += Form_OnAfterSave;
        }

        InitializeControls();
        if (PostInfo != null)
        {
            // Post must be loaded into form on every postback, because BasicForm is re-instantiated 
            // after a new document is created and data posted to server are lost. 
            // This must be done in PageInit before ViewState and post data are loaded.
            LoadPostDataIntoControl(PostInfo);
        }
    }


    /// <summary>
    /// OnPreRender event.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Social marketing functionality is not available for this license or user
            Enabled = false;

            if (!IsFeatureAvailable)
            {
                ShowWarning(ResHelper.GetString("sm.nolicense"));
            }
            else if (!HasUserReadPermission)
            {
                ShowWarning(ResHelper.GetString("sm.linkedin.posts.noreadpermission"));
            }

            return;
        }

        if (!HasUserModifyPermission)
        {
            // User cannot edit or create post
            ShowWarning(ResHelper.GetString("sm.linkedin.posts.nomodifypermission"));
            SetUnderlyingControlsEnabledState(false);
        }

        publishDateTime.Enabled = !chkPostAfterDocumentPublish.Checked;

        if ((PostInfo != null) && !PostInfo.IsEditable)
        {
            if (mPublishedWhileEditing || (RequestHelper.IsPostBack() && !FormDataEqualsPostInfo(PostInfo)))
            {
                // Data in form has changed, but post can not be modified
                AddWarning(GetString("sm.linkedin.autopost.publishedwhileediting"));
                LoadPostDataIntoControl(PostInfo);
            }
            else if (!RequestHelper.IsPostBack() && PostInfo.IsFaulty)
            {
                // Show error on first load. This must be done on PagePreRender, 
                // because errors added on PageInit event are ignored.
                ShowPostPublishState(PostInfo);
            }

            // Form will be disabled because post is published or faulty and cannot be edited
            SetUnderlyingControlsEnabledState(false);
        }

        // Register bootstrap tooltip over help icons
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");
    }


    /// <summary>
    /// Form OnBeforeDataRetrieval.
    /// </summary>
    private void Form_OnBeforeDataRetrieval(object sender, EventArgs eventArgs)
    {
        if (!HasUserModifyPermission)
        {
            return;
        }

        if (chkPostToLinkedIn.Checked)
        {
            if ((PostInfo == null) || PostInfo.IsEditable)
            {
                // Post does not exist or can be modified
                PostInfo = SetControlDataIntoPost(PostInfo);
                LinkedInPostInfoProvider.SetLinkedInPostInfo(PostInfo);
            }
            else if (!FormDataEqualsPostInfo(PostInfo))
            {
                mPublishedWhileEditing = true;
            }
        }
        else
        {
            // Checkbox post to LinkedIn is not checked
            if (PostInfo != null)
            {
                if (PostInfo.IsEditable)
                {
                    // Existing post has to be deleted 
                    LinkedInPostInfoProvider.DeleteLinkedInPostInfo(PostInfo);
                    PostInfo = null;
                    InitializeControls();
                }
                else
                {
                    mPublishedWhileEditing = true;
                }
            }
        }
    }


    /// <summary>
    /// Form OnAfterSave event.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        if ((PostInfo == null) || !HasUserModifyPermission)
        {
            return;
        }

        if (Document != null)
        {
            if (!PostInfo.LinkedInPostDocumentGUID.HasValue)
            {
                PostInfo.LinkedInPostDocumentGUID = Document.DocumentGUID;
            }

            if (PostInfo.LinkedInPostPostAfterDocumentPublish && !IsUnderWorkflow && !PostInfo.IsPublished)
            {
                // Post will be scheduled on time when document gets published
                PostInfo.LinkedInPostScheduledPublishDateTime = Document.DocumentPublishFrom;
            }

            if (PostInfo.HasChanged)
            {
                LinkedInPostInfoProvider.SetLinkedInPostInfo(PostInfo);
            }
        }

        // Check whether the post should be published or scheduled now (post WON'T be published/scheduled if it should be published with the document which is under workflow)
        if (!PostInfo.IsPublished && !(PostInfo.LinkedInPostPostAfterDocumentPublish && IsUnderWorkflow))
        {
            PublishPost(PostInfo.LinkedInPostID);
        }
        LoadPostDataIntoControl(PostInfo);
    }


    /// <summary>
    /// Checkbox chkPostToLinkedIn OnCheckedChanged event.
    /// </summary>
    protected void chkPostToLinkedIn_OnCheckedChanged(object sender, EventArgs e)
    {
        DisplayForm = chkPostToLinkedIn.Checked;
    }


    /// <summary>
    /// Checkbox chkPostAfterDocumentPublish OnCheckedChanged event.
    /// </summary>
    protected void chkPostAfterDocumentPublish_OnCheckedChanged(object sender, EventArgs e)
    {
        if (chkPostAfterDocumentPublish.Checked)
        {
            publishDateTime.Value = null;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes inner controls to its default state.
    /// </summary>
    private void InitializeControls()
    {
        chkPostAfterDocumentPublish.Checked = false;

        string content = String.Empty;
        if (!String.IsNullOrWhiteSpace(mDefaultValue))
        {
            // Default content given as control's string Value
            content = mDefaultValue;
        }
        else if ((FieldInfo != null) && !String.IsNullOrWhiteSpace(FieldInfo.DefaultValue))
        {
            // Default content from field info
            content = FieldInfo.DefaultValue;
        }
        txtPost.Text = content;
        txtPost.MaxLength = (int)LinkedInHelper.COMPANY_SHARE_COMMENT_MAX_LENGTH;

        if (!chkPostAfterDocumentPublish.Checked)
        {
            publishDateTime.Value = DateTime.Now;
        }

        companySelector.ObjectSiteName = SiteIdentifier;
        LinkedInAccountInfo defaultAccountInfo = LinkedInAccountInfoProvider.GetDefaultLinkedInAccount(SiteIdentifier);
        if (defaultAccountInfo != null)
        {
            companySelector.Value = defaultAccountInfo.LinkedInAccountID;
        }

        urlShortenerSelector.SiteID = SiteIdentifier;
        urlShortenerSelector.Value = URLShortenerHelper.GetDefaultURLShortenerForSocialNetwork(SocialNetworkTypeEnum.LinkedIn, SiteIdentifier);
        chkPostAfterDocumentPublish.Visible = (Document != null);
        campaingSelector.Value = null;
        campaingSelector.ObjectSiteName = SiteIdentifier;

        if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
        {
            // Post must be created because field doesn't allow an empty value
            chkPostToLinkedIn.Enabled = false;
            DisplayForm = true;
        }
    }


    /// <summary>
    /// Loads post data into the inner controls.
    /// </summary>
    /// <param name="post">LinkedIn post that will be loaded.</param>
    private void LoadPostDataIntoControl(LinkedInPostInfo post)
    {
        companySelector.Value = post.LinkedInPostLinkedInAccountID;
        txtPost.Text = post.LinkedInPostComment;

        if (post.LinkedInPostPostAfterDocumentPublish)
        {
            chkPostAfterDocumentPublish.Checked = true;
            publishDateTime.Enabled = false;
        }
        publishDateTime.Value = post.LinkedInPostScheduledPublishDateTime;

        campaingSelector.Value = post.LinkedInPostCampaignID;
        urlShortenerSelector.Value = post.LinkedInPostURLShortenerType;

        DisplayForm = true;
        ShowPostPublishState(post);
    }


    /// <summary>
    /// Sets data retrieved from inner controls into new (or given) LinkedIn post object.
    /// </summary>
    /// <param name="post">LinkedIn post object that will be set from inner controls.</param>
    /// <returns>LinkedIn post containing data retrieved from inner controls.</returns>
    private LinkedInPostInfo SetControlDataIntoPost(LinkedInPostInfo post = null)
    {
        if (post == null)
        {
            post = new LinkedInPostInfo();
        }

        post.LinkedInPostLinkedInAccountID = ValidationHelper.GetInteger(companySelector.Value, 0);
        post.LinkedInPostComment = txtPost.Text;
        post.LinkedInPostURLShortenerType = (URLShortenerTypeEnum)urlShortenerSelector.Value;
        post.LinkedInPostPostAfterDocumentPublish = chkPostAfterDocumentPublish.Checked;
        post.LinkedInPostScheduledPublishDateTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        post.LinkedInPostCampaignID = ValidationHelper.GetInteger(campaingSelector.Value, 0);
        post.LinkedInPostSiteID = SiteIdentifier;
        post.LinkedInPostIsCreatedByUser = true;

        return post;
    }


    /// <summary>
    /// Shows given LinkedIn post's publish state.
    /// </summary>
    /// <param name="post">LinkedIn post.</param>
    private void ShowPostPublishState(LinkedInPostInfo post)
    {
        string message = LinkedInPostInfoProvider.GetPostPublishStateMessage(post, MembershipContext.AuthenticatedUser, CurrentSite);

        if (String.IsNullOrEmpty(message))
        {
            return;
        }

        if (post.IsFaulty)
        {
            var errorMessage = FormatErrorMessage(message);
            ShowError(errorMessage);
        }
        else if (post.IsPublished)
        {
            ShowConfirmation(message);
        }
        else
        {
            ShowInformation(message);
        }
    }


    private string FormatErrorMessage(string message)
    {
        return String.Format("<strong>{0}</strong>: {1}", GetString("sm.facebook.autopost"), message);
    }


    /// <summary>
    /// Publishes LinkedIn post with given identifier.
    /// </summary>
    /// <param name="postId">LinkedIn post identifier.</param>
    private void PublishPost(int postId)
    {
        try
        {
            LinkedInPostInfoProvider.TryCancelScheduledPublishLinkedInPost(postId);
            LinkedInPostInfoProvider.PublishLinkedInPost(postId);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogWarning("Social marketing - LinkedIn post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                String.Format("An error occurred while publishing the LinkedIn post with ID {0}.", postId));
        }
    }


    /// <summary>
    /// Disables or enables all inner controls.
    /// </summary>
    /// <param name="enabled">Controls will be enabled if true; disabled otherwise.</param>
    private void SetUnderlyingControlsEnabledState(bool enabled)
    {
        chkPostToLinkedIn.Enabled = enabled;
        companySelector.Enabled = enabled;
        txtPost.Enabled = enabled;
        urlShortenerSelector.Enabled = enabled;
        publishDateTime.Enabled = enabled;
        campaingSelector.Enabled = enabled;
        chkPostAfterDocumentPublish.Enabled = enabled;
    }


    /// <summary>
    /// Indicates whether data in the form are equals to the given post info or not.
    /// </summary>
    /// <param name="post">Post that is compared with form data.</param>
    /// <returns>True if form data are equals to given post, false otherwise.</returns>
    private bool FormDataEqualsPostInfo(LinkedInPostInfo post)
    {
        if (!chkPostToLinkedIn.Checked)
        {
            return false;
        }

        DateTime? formScheduledPublishTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        if (formScheduledPublishTime == DateTimeHelper.ZERO_TIME)
        {
            formScheduledPublishTime = null;
        }
        return String.Equals(post.LinkedInPostComment, txtPost.Text, StringComparison.InvariantCulture)
            && post.LinkedInPostScheduledPublishDateTime == formScheduledPublishTime
            && post.LinkedInPostLinkedInAccountID == ValidationHelper.GetInteger(companySelector.Value, 0)
            && post.LinkedInPostURLShortenerType == (URLShortenerTypeEnum)urlShortenerSelector.Value
            && post.LinkedInPostPostAfterDocumentPublish == chkPostAfterDocumentPublish.Checked
            && ValidationHelper.GetInteger(post.LinkedInPostCampaignID, 0) == ValidationHelper.GetInteger(campaingSelector.Value, 0)
            && post.LinkedInPostSiteID == SiteIdentifier;
    }
    
    #endregion
}
