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

public partial class CMSModules_SocialMarketing_FormControls_FacebookAutoPost : SocialMarketingAutoPostControl
{
    #region "Private fields"

    private string mDefaultValue;
    private bool mPublishedWhileEditing;
    private bool mPostInfoSet;
    private FacebookPostInfo mPostInfo;


    /// <summary>
    /// This value is used for backward compatibility after update from previous version of CMS.
    /// </summary>
    private SocialMarketingBackCompatibilityHelper.SocialMarketingPublishingElement? mBackCompatibilityValue;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets edited Facebook post info object. Value of the control representing ID of the post is updated when a new post object is set.
    /// </summary>
    private FacebookPostInfo PostInfo
    {
        get
        {
            if (!mPostInfoSet)
            {
                var document = Document;
                if ((document != null) && (document.DocumentGUID != Guid.Empty) && IsFeatureAvailable)
                {
                    mPostInfo = FacebookPostInfoProvider.GetFacebookPostInfosByDocumentGuid(document.DocumentGUID, SiteIdentifier).ToList().FirstOrDefault(x => x.FacebookPostIsCreatedByUser);
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
            return chkPostToFacebook.Checked;
        }
        set
        {
            chkPostToFacebook.Checked = value;
            pnlForm.Visible = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value of this control. Value is an integer representing the ID of the Facebook post
    /// </summary>
    public override object Value
    {
        get
        {
            if ((PostInfo != null) && (PostInfo.FacebookPostID > 0))
            {
                return PostInfo.FacebookPostID;
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
                PostInfo = FacebookPostInfoProvider.GetFacebookPostInfo(postId);

                return;
            }

            string defaultValue = value.ToString();
            if (String.IsNullOrWhiteSpace(defaultValue))
            {
                return;
            }

            try
            {
                // Try to get value from previous CMS version
                mBackCompatibilityValue = SocialMarketingBackCompatibilityHelper.DeserializePublishingElement(defaultValue);
                if ((mBackCompatibilityValue.Value.SocialNetworkType != SocialNetworkTypeEnum.Facebook)
                    || mBackCompatibilityValue.Value.IsPublished
                    || String.IsNullOrWhiteSpace(mBackCompatibilityValue.Value.Template))
                {
                    mBackCompatibilityValue = null;
                }
            }
            catch
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
        if (!chkPostToFacebook.Checked || (PostInfo != null) && !PostInfo.IsEditable)
        {
            return true;
        }

        if (String.IsNullOrWhiteSpace(txtPost.Text))
        {
            ValidationError = ResHelper.GetString("sm.facebook.posts.msg.postempty");

            return false;
        }

        if (ValidationHelper.GetInteger(pageSelector.Value, 0) <= 0)
        {
            ValidationError = ResHelper.GetString("sm.facebook.posts.msg.selectaccount");
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
                ShowWarning(ResHelper.GetString("sm.facebook.posts.noreadpermission"));
            }

            return;
        }

        if (!HasUserModifyPermission)
        {
            // User cannot edit or create post
            ShowWarning(ResHelper.GetString("sm.facebook.posts.nomodifypermission"));
            SetUnderlyingControlsEnabledState(false);
        }
        else if (mBackCompatibilityValue.HasValue && (PostInfo == null) && chkPostToFacebook.Checked)
        {
            // User has modify permission and form is in compatibility mode
            ShowWarning(ResHelper.GetString("sm.facebook.posts.backcompatibility"));
        }

        publishDateTime.Enabled = !chkPostAfterDocumentPublish.Checked;

        if ((PostInfo != null) && !PostInfo.IsEditable)
        {
            if (mPublishedWhileEditing || (RequestHelper.IsPostBack() && !FormDataEqualsPostInfo(PostInfo)))
            {
                // Data in form has changed, but post can not be modified
                AddWarning(GetString("sm.facebook.autopost.publishedwhileediting"));
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

        if (chkPostToFacebook.Checked)
        {
            if ((PostInfo == null) || PostInfo.IsEditable)
            {
                // Post does not exist or can be modified
                PostInfo = SetControlDataIntoPost(PostInfo);
                FacebookPostInfoProvider.SetFacebookPostInfo(PostInfo);
            }
            else if (!FormDataEqualsPostInfo(PostInfo))
            {
                mPublishedWhileEditing = true;
            }
        }
        else
        {
            // Checkbox post to Facebook is not checked
            if (PostInfo != null)
            {
                if (PostInfo.IsEditable)
                {
                    // Existing post has to be deleted 
                    FacebookPostInfoProvider.DeleteFacebookPostInfo(PostInfo);
                    PostInfo = null;
                    InitializeControls();
                }
                else
                {
                    mPublishedWhileEditing = true;
                }
            }
            else if (mBackCompatibilityValue.HasValue)
            {
                // Form has to be cleaned up from old value
                mBackCompatibilityValue = null;
                InitializeControls();
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
            if (!PostInfo.FacebookPostDocumentGUID.HasValue)
            {
                PostInfo.FacebookPostDocumentGUID = Document.DocumentGUID;
            }

            if (PostInfo.FacebookPostPostAfterDocumentPublish && !IsUnderWorkflow && !PostInfo.IsPublished)
            {
                // Post will be scheduled on time when document gets published
                PostInfo.FacebookPostScheduledPublishDateTime = Document.DocumentPublishFrom;
            }

            if (PostInfo.HasChanged)
            {
                FacebookPostInfoProvider.SetFacebookPostInfo(PostInfo);
            }
        }

        // Check whether the post should be published or scheduled now (post WON'T be published/scheduled if it should be published with the document which is under workflow)
        if (!PostInfo.IsPublished && !(PostInfo.FacebookPostPostAfterDocumentPublish && IsUnderWorkflow))
        {
            PublishPost(PostInfo.FacebookPostID);
        }
        LoadPostDataIntoControl(PostInfo);
    }


    /// <summary>
    /// Checkbox chkPostToFacebook OnCheckedChanged event.
    /// </summary>
    protected void chkPostToFacebook_OnCheckedChanged(object sender, EventArgs e)
    {
        DisplayForm = chkPostToFacebook.Checked;
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
        if (mBackCompatibilityValue.HasValue)
        {
            // Pre fill form with old data
            chkPostAfterDocumentPublish.Checked = mBackCompatibilityValue.Value.AutoPostAfterPublishing;
            content = mBackCompatibilityValue.Value.Template;
        }
        else if (!String.IsNullOrWhiteSpace(mDefaultValue))
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

        if (!chkPostAfterDocumentPublish.Checked)
        {
            publishDateTime.Value = DateTime.Now;
        }

        pageSelector.ObjectSiteName = SiteIdentifier;
        FacebookAccountInfo defaultAccountInfo = FacebookAccountInfoProvider.GetDefaultFacebookAccount(SiteIdentifier);
        if (defaultAccountInfo != null)
        {
            pageSelector.Value = defaultAccountInfo.FacebookAccountID;
        }
        
        urlShortenerSelector.SiteID = SiteIdentifier;
        urlShortenerSelector.Value = URLShortenerHelper.GetDefaultURLShortenerForSocialNetwork(SocialNetworkTypeEnum.Facebook, SiteIdentifier);
        chkPostAfterDocumentPublish.Visible = (Document != null);
        campaingSelector.Value = null;
        campaingSelector.ObjectSiteName = SiteIdentifier;

        if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
        {
            // Post must be created because field doesn't allow an empty value
            chkPostToFacebook.Enabled = false;
            DisplayForm = true;
        }
        else
        {
            // Form is displayed if it is pre filled with old data
            DisplayForm = mBackCompatibilityValue.HasValue;
        }
    }


    /// <summary>
    /// Loads post data into the inner controls.
    /// </summary>
    /// <param name="post">Facebook post that will be loaded.</param>
    private void LoadPostDataIntoControl(FacebookPostInfo post)
    {
        pageSelector.Value = post.FacebookPostFacebookAccountID;
        txtPost.Text = post.FacebookPostText;

        if (post.FacebookPostPostAfterDocumentPublish)
        {
            chkPostAfterDocumentPublish.Checked = true;
            publishDateTime.Enabled = false;
        }
        publishDateTime.Value = post.FacebookPostScheduledPublishDateTime;

        campaingSelector.Value = post.FacebookPostCampaignID;
        urlShortenerSelector.Value = post.FacebookPostURLShortenerType;

        DisplayForm = true;
        ShowPostPublishState(post);
    }


    /// <summary>
    /// Sets data retrieved from inner controls into new (or given) Facebook post object.
    /// </summary>
    /// <param name="post">Facebook post object that will be set from inner controls.</param>
    /// <returns>Facebook post containing data retrieved from inner controls.</returns>
    private FacebookPostInfo SetControlDataIntoPost(FacebookPostInfo post = null)
    {
        if (post == null)
        {
            post = new FacebookPostInfo();
        }

        post.FacebookPostFacebookAccountID = ValidationHelper.GetInteger(pageSelector.Value, 0);
        post.FacebookPostText = txtPost.Text;
        post.FacebookPostURLShortenerType = (URLShortenerTypeEnum)urlShortenerSelector.Value;
        post.FacebookPostPostAfterDocumentPublish = chkPostAfterDocumentPublish.Checked;
        post.FacebookPostScheduledPublishDateTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        post.FacebookPostCampaignID = ValidationHelper.GetInteger(campaingSelector.Value, 0);
        post.FacebookPostSiteID = SiteIdentifier;
        post.FacebookPostIsCreatedByUser = true;

        return post;
    }


    /// <summary>
    /// Shows given Facebook post's publish state.
    /// </summary>
    /// <param name="post">Facebook post.</param>
    private void ShowPostPublishState(FacebookPostInfo post)
    {
        string message = FacebookPostInfoProvider.GetPostPublishStateMessage(post, MembershipContext.AuthenticatedUser, CurrentSite);

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
            ShowConfirmation(message, true);
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
    /// Publishes Facebook post with given identifier.
    /// </summary>
    /// <param name="postId">Facebook post identifier.</param>
    private void PublishPost(int postId)
    {
        try
        {
            FacebookPostInfoProvider.TryCancelScheduledPublishFacebookPost(postId);
            FacebookPostInfoProvider.PublishFacebookPost(postId);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogWarning("Social marketing - Facebook post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                String.Format("An error occurred while publishing the Facebook post with ID {0}.", postId));
        }
    }


    /// <summary>
    /// Disables or enables all inner controls.
    /// </summary>
    /// <param name="enabled">Controls will be enabled if true; disabled otherwise.</param>
    private void SetUnderlyingControlsEnabledState(bool enabled)
    {
        chkPostToFacebook.Enabled = enabled;
        pageSelector.Enabled = enabled;
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
    private bool FormDataEqualsPostInfo(FacebookPostInfo post)
    {
        if (!chkPostToFacebook.Checked)
        {
            return false;
        }

        DateTime? formScheduledPublishTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        if (formScheduledPublishTime == DateTimeHelper.ZERO_TIME)
        {
            formScheduledPublishTime = null;
        }
        return String.Equals(post.FacebookPostText, txtPost.Text, StringComparison.InvariantCulture)
            && post.FacebookPostScheduledPublishDateTime == formScheduledPublishTime
            && post.FacebookPostFacebookAccountID == ValidationHelper.GetInteger(pageSelector.Value, 0)
            && post.FacebookPostURLShortenerType == (URLShortenerTypeEnum)urlShortenerSelector.Value
            && post.FacebookPostPostAfterDocumentPublish == chkPostAfterDocumentPublish.Checked
            && ValidationHelper.GetInteger(post.FacebookPostCampaignID, 0) == ValidationHelper.GetInteger(campaingSelector.Value, 0)
            && post.FacebookPostSiteID == SiteIdentifier;
    }


    #endregion
}
