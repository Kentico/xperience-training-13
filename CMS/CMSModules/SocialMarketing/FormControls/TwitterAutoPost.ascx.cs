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

public partial class CMSModules_SocialMarketing_FormControls_TwitterAutoPost : SocialMarketingAutoPostControl
{
    #region "Private fields"

    private string mDefaultValue;
    private bool mPublishedWhileEditing;
    private bool mTweetInfoSet;
    private TwitterPostInfo mTweetInfo;


    /// <summary>
    /// This value is used for backward compatibility after update from previous version of CMS.
    /// </summary>
    private SocialMarketingBackCompatibilityHelper.SocialMarketingPublishingElement? mBackCompatibilityValue;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets edited Twitter post info object. Value of the control representing ID of the post is updated when a new post object is set.
    /// </summary>
    private TwitterPostInfo TweetInfo
    {
        get
        {
            if (!mTweetInfoSet)
            {
                var document = Document;
                if ((mTweetInfo == null) && (document != null) && (document.DocumentGUID != Guid.Empty) && IsFeatureAvailable)
                {
                    mTweetInfo = TwitterPostInfoProvider.GetTwitterPostInfosByDocumentGuid(document.DocumentGUID, SiteIdentifier).ToList().FirstOrDefault(x => x.TwitterPostIsCreatedByUser);
                }
                mTweetInfoSet = true;
            }

            return mTweetInfo;
        }
        set
        {
            mTweetInfo = value;
            mTweetInfoSet = true;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the editing form is displayed or not.
    /// </summary>
    private bool DisplayForm
    {
        get
        {
            return chkPostToTwitter.Checked;
        }
        set
        {
            chkPostToTwitter.Checked = value;
            pnlForm.Visible = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value of this control. Value is an integer representing the ID of the Twitter post
    /// </summary>
    public override object Value
    {
        get
        {
            if ((TweetInfo != null) && (TweetInfo.TwitterPostID > 0))
            {
                return TweetInfo.TwitterPostID;
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
                TweetInfo = TwitterPostInfoProvider.GetTwitterPostInfo(postId);

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
                if ((mBackCompatibilityValue.Value.SocialNetworkType != SocialNetworkTypeEnum.Twitter)
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
            if (!value && !StopProcessing && (TweetInfo == null) && IsFeatureAvailable)
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
        if (!chkPostToTwitter.Checked || (TweetInfo != null) && !TweetInfo.IsEditable)
        {
            return true;
        }

        if (!tweetContent.EditingControl.IsValid())
        {
            ValidationError = tweetContent.EditingControl.ValidationError;

            return false;
        }

        if (ValidationHelper.GetInteger(channelSelector.Value, 0) <= 0)
        {
            ValidationError = ResHelper.GetString("sm.twitter.posts.msg.selectaccount");
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
        if (TweetInfo != null)
        {
            // Post must be loaded into form on every postback, because BasicForm is re-instantiated 
            // after a new document is created and data posted to server are lost. 
            // This must be done in PageInit before ViewState and post data are loaded.
            LoadPostDataIntoControl(TweetInfo);
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
                ShowWarning(ResHelper.GetString("sm.twitter.posts.noreadpermission"));
            }

            return;
        }

        if (!HasUserModifyPermission)
        {
            // User cannot edit or create post
            ShowWarning(ResHelper.GetString("sm.twitter.posts.nomodifypermission"));
            SetUnderlyingControlsEnabledState(false);
        }
        else if (mBackCompatibilityValue.HasValue && (TweetInfo == null) && chkPostToTwitter.Checked)
        {
            // User has modify permission and form is in compatibility mode
            ShowWarning(ResHelper.GetString("sm.twitter.posts.backcompatibility"));
        }

        publishDateTime.Enabled = !chkPostAfterDocumentPublish.Checked;

        if ((TweetInfo != null) && !TweetInfo.IsEditable)
        {
            if (mPublishedWhileEditing || (RequestHelper.IsPostBack() && !FormDataEqualsPostInfo(TweetInfo)))
            {
                // Data in form has changed, but post can not be modified
                AddWarning(GetString("sm.twitter.autopost.publishedwhileediting"));
                LoadPostDataIntoControl(TweetInfo);
            }
            else if (!RequestHelper.IsPostBack() && TweetInfo.IsFaulty)
            {
                // Show error on first load. This must be done on PagePreRender, 
                // because errors added on PageInit event are ignored.
                ShowPostPublishState(TweetInfo);
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

        if (chkPostToTwitter.Checked)
        {
            if ((TweetInfo == null) || TweetInfo.IsEditable)
            {
                // Post does not exist or can be modified
                TweetInfo = SetControlDataIntoPost(TweetInfo);
                TwitterPostInfoProvider.SetTwitterPostInfo(TweetInfo);
            }
            else if (!FormDataEqualsPostInfo(TweetInfo))
            {
                mPublishedWhileEditing = true;
            }
        }
        else
        {
            // Checkbox post to Twitter is not checked
            if (TweetInfo != null)
            {
                if (TweetInfo.IsEditable)
                {
                    // Existing post has to be deleted 
                    TwitterPostInfoProvider.DeleteTwitterPostInfo(TweetInfo);
                    TweetInfo = null;
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
        if (TweetInfo == null || !HasUserModifyPermission)
        {
            return;
        }

        if (Document != null)
        {
            if (!TweetInfo.TwitterPostDocumentGUID.HasValue)
            {
                TweetInfo.TwitterPostDocumentGUID = Document.DocumentGUID;
            }

            if (TweetInfo.TwitterPostPostAfterDocumentPublish && !IsUnderWorkflow && !TweetInfo.IsPublished)
            {
                // Tweet will be scheduled on time when document gets published
                TweetInfo.TwitterPostScheduledPublishDateTime = Document.DocumentPublishFrom;
            }

            if (TweetInfo.HasChanged)
            {
                TwitterPostInfoProvider.SetTwitterPostInfo(TweetInfo);
            }
        }

        // Check whether the tweet should be published or scheduled now (tweet WON'T be published/scheduled if it should be published with the document which is under workflow)
        if (!TweetInfo.IsPublished && !(TweetInfo.TwitterPostPostAfterDocumentPublish && IsUnderWorkflow))
        {
            PublishPost(TweetInfo.TwitterPostID);
        }
        LoadPostDataIntoControl(TweetInfo);
    }


    /// <summary>
    /// Checkbox chkPostToTwitter OnCheckedChanged event.
    /// </summary>
    protected void chkPostToTwitter_OnCheckedChanged(object sender, EventArgs e)
    {
        DisplayForm = chkPostToTwitter.Checked;
    }


    /// <summary>
    /// Checkbox chkPostAfterDocumentPublish OnCheckedChanged event.
    /// </summary>
    protected void chkPostAfterDocumentPublish_OnCheckedChanged(object sender, EventArgs e)
    {
        publishDateTime.Enabled = !chkPostAfterDocumentPublish.Checked;
        if (chkPostAfterDocumentPublish.Checked)
        {
            publishDateTime.Value = null;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes inner controls. Post data are loaded when editing post.
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
        tweetContent.Text = content;

        if (!chkPostAfterDocumentPublish.Checked)
        {
            publishDateTime.Value = DateTime.Now;
        }

        channelSelector.ObjectSiteName = SiteIdentifier;
        TwitterAccountInfo defaultAccountInfo = TwitterAccountInfoProvider.GetDefaultTwitterAccount(SiteIdentifier);
        if (defaultAccountInfo != null)
        {
            channelSelector.Value = defaultAccountInfo.TwitterAccountID;
        }
        
        urlShortenerSelector.SiteID = SiteIdentifier;
        urlShortenerSelector.Value = URLShortenerHelper.GetDefaultURLShortenerForSocialNetwork(SocialNetworkTypeEnum.Twitter, SiteIdentifier);
        chkPostAfterDocumentPublish.Visible = (Document != null);
        campaingSelector.Value = null;
        campaingSelector.ObjectSiteName = SiteIdentifier;

        if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
        {
            // Post must be created because field doesn't allow an empty value
            chkPostToTwitter.Enabled = false;
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
    /// <param name="post">Twitter post that will be loaded.</param>
    private void LoadPostDataIntoControl(TwitterPostInfo post)
    {
        channelSelector.Value = post.TwitterPostTwitterAccountID;
        tweetContent.Value = post.TwitterPostText;

        if (post.TwitterPostPostAfterDocumentPublish)
        {
            chkPostAfterDocumentPublish.Checked = true;
            publishDateTime.Enabled = false;
        }
        publishDateTime.Value = post.TwitterPostScheduledPublishDateTime;

        campaingSelector.Value = post.TwitterPostCampaignID;
        urlShortenerSelector.Value = post.TwitterPostURLShortenerType;

        DisplayForm = true;
        ShowPostPublishState(post);
    }


    /// <summary>
    /// Sets data retrieved from inner controls into new (or given) Twitter post object.
    /// </summary>
    /// <param name="post">Twitter post object that will be set from inner controls.</param>
    /// <returns>Twitter post containing data retrieved from inner controls.</returns>
    private TwitterPostInfo SetControlDataIntoPost(TwitterPostInfo post = null)
    {
        if (post == null)
        {
            post = new TwitterPostInfo();
        }

        post.TwitterPostTwitterAccountID = ValidationHelper.GetInteger(channelSelector.Value, 0);
        post.TwitterPostText = (string)tweetContent.Value;
        post.TwitterPostURLShortenerType = (URLShortenerTypeEnum)urlShortenerSelector.Value;
        post.TwitterPostPostAfterDocumentPublish = chkPostAfterDocumentPublish.Checked;
        post.TwitterPostScheduledPublishDateTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        post.TwitterPostCampaignID = ValidationHelper.GetInteger(campaingSelector.Value, 0);
        post.TwitterPostSiteID = SiteIdentifier;
        post.TwitterPostIsCreatedByUser = true;

        return post;
    }


    /// <summary>
    /// Shows given Twitter post's publish state.
    /// </summary>
    /// <param name="post">Twitter post.</param>
    private void ShowPostPublishState(TwitterPostInfo post)
    {
        string message = TwitterPostInfoProvider.GetPostPublishStateMessage(post, MembershipContext.AuthenticatedUser, CurrentSite);

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
    /// Publishes Twitter post with given identifier.
    /// </summary>
    /// <param name="postId">Twitter post identifier.</param>
    private void PublishPost(int postId)
    {
        try
        {
            TwitterPostInfoProvider.TryCancelScheduledPublishTwitterPost(postId);
            TwitterPostInfoProvider.PublishTwitterPost(postId);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogWarning("Social marketing - Twitter post", "PUBLISHPOST", ex, SiteContext.CurrentSiteID,
                String.Format("An error occurred while publishing the Twitter post with ID {0}.", postId));
        }
    }


    /// <summary>
    /// Disables or enables all inner controls.
    /// </summary>
    /// <param name="enabled">Controls will be enabled if true; disabled otherwise.</param>
    private void SetUnderlyingControlsEnabledState(bool enabled)
    {
        chkPostToTwitter.Enabled = enabled;
        channelSelector.Enabled = enabled;
        tweetContent.Enabled = enabled;
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
    private bool FormDataEqualsPostInfo(TwitterPostInfo post)
    {
        if (!chkPostToTwitter.Checked)
        {
            return false;
        }

        DateTime? formScheduledPublishTime = ValidationHelper.GetDateTime(publishDateTime.Value, DateTimeHelper.ZERO_TIME);
        if (formScheduledPublishTime == DateTimeHelper.ZERO_TIME)
        {
            formScheduledPublishTime = null;
        }
        return String.Equals(post.TwitterPostText, (string)tweetContent.Value, StringComparison.InvariantCulture)
            && post.TwitterPostScheduledPublishDateTime == formScheduledPublishTime
            && post.TwitterPostTwitterAccountID == ValidationHelper.GetInteger(channelSelector.Value, 0)
            && post.TwitterPostURLShortenerType == (URLShortenerTypeEnum)urlShortenerSelector.Value
            && post.TwitterPostPostAfterDocumentPublish == chkPostAfterDocumentPublish.Checked
            && ValidationHelper.GetInteger(post.TwitterPostCampaignID, 0) == ValidationHelper.GetInteger(campaingSelector.Value, 0)
            && post.TwitterPostSiteID == SiteIdentifier;
    }
    
    #endregion
}
