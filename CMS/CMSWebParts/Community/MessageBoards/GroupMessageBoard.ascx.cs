using System;

using CMS.Community;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.PortalEngine.Web.UI;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.PortalEngine;

public partial class CMSWebParts_Community_MessageBoards_GroupMessageBoard : CMSAbstractWebPart
{
    #region "Private fields"

    private string mWebPartName = "";
    private BoardInfo mBoardObj = null;
    private GroupInfo mCurrentGroup = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current web part name.
    /// </summary>
    private string BoardDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardDisplayName"), "");
        }
        set
        {
            SetValue("BoardDisplayName", value);
        }
    }


    /// <summary>
    /// Current web part name.
    /// </summary>
    private string WebPartName
    {
        get
        {
            if (mWebPartName == "")
            {
                mWebPartName = ValidationHelper.GetString(GetValue("WebPartControlID"), "");
            }

            return mWebPartName;
        }
    }


    /// <summary>
    /// Current group from community context.
    /// </summary>
    private GroupInfo CurrentGroup
    {
        get
        {
            if (mCurrentGroup == null)
            {
                mCurrentGroup = CommunityContext.CurrentGroup;
            }

            return mCurrentGroup;
        }
    }


    /// <summary>
    /// Current message board.
    /// </summary>
    private BoardInfo BoardObj
    {
        get
        {
            if (mBoardObj == null)
            {
                // Get the current group info and obtain the related board info
                if (CurrentGroup != null)
                {
                    mBoardObj = BoardInfoProvider.GetBoardInfoForGroup(GetBoardName(WebPartName), CurrentGroup.GroupID);
                }
            }
            return mBoardObj;
        }
    }

    #endregion


    #region "Public properties"

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
            msgBoard.StopProcessing = value;
        }
    }


    /// <summary>
    /// No messages text.
    /// </summary>
    public string NoMessagesText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NoMessagesText"), "");
        }
        set
        {
            SetValue("NoMessagesText", value);
        }
    }


    /// <summary>
    /// Default board moderators.
    /// </summary>
    public string BoardModerators
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardModerators"), "");
        }
        set
        {
            SetValue("BoardModerators", value);
        }
    }


    /// <summary>
    /// Transformation used to display board message text.
    /// </summary>
    public string MessageTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageTransformation"), "");
        }
        set
        {
            SetValue("MessageTransformation", value);
        }
    }


    /// <summary>
    /// Indicates whether the EDIT button should be displayed.
    /// </summary>
    public bool ShowEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEdit"), false);
        }
        set
        {
            SetValue("ShowEdit", value);
        }
    }


    /// <summary>
    /// Indicates whether the DELETE button should be displayed.
    /// </summary>
    public bool ShowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDelete"), false);
        }
        set
        {
            SetValue("ShowDelete", value);
        }
    }


    /// <summary>
    /// Indicates whether the APPROVE button should be displayed.
    /// </summary>
    public bool ShowApprove
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowApprove"), false);
        }
        set
        {
            SetValue("ShowApprove", value);
        }
    }


    /// <summary>
    /// Indicates whether the REJECT button should be displayed.
    /// </summary>
    public bool ShowReject
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowReject"), false);
        }
        set
        {
            SetValue("ShowReject", value);
        }
    }


    /// <summary>
    /// Indicates whether the subscriptions should be enabled.
    /// </summary>
    public bool BoardEnableSubscriptions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardEnableSubscriptions"), false);
        }
        set
        {
            SetValue("BoardEnableSubscriptions", value);
        }
    }


    /// <summary>
    /// Indicates whether the permissions should be checked.
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
            msgBoard.BoardProperties.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates whether the existing messages should be displayed to anonymous user.
    /// </summary>
    public bool EnableAnonymousRead
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAnonymousRead"), false);
        }
        set
        {
            SetValue("EnableAnonymousRead", value);
        }
    }


    /// <summary>
    /// Board unsubscription URL.
    /// </summary>
    public string BoardUnsubscriptionUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardUnsubscriptionUrl"), "");
        }
        set
        {
            SetValue("BoardUnsubscriptionUrl", value);
        }
    }


    /// <summary>
    /// Board base URL.
    /// </summary>
    public string BoardBaseUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardBaseUrl"), "");
        }
        set
        {
            SetValue("BoardBaseUrl", value);
        }
    }


    /// <summary>
    /// Indicates whether board is opened.
    /// </summary>
    public bool BoardOpened
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardOpened"), false);
        }
        set
        {
            SetValue("BoardOpened", value);
        }
    }


    /// <summary>
    /// Indicates type of board access.
    /// </summary>
    public SecurityAccessEnum BoardAccess
    {
        get
        {
            return (SecurityAccessEnum)ValidationHelper.GetInteger(GetValue("BoardAccess"), 0);
        }
        set
        {
            SetValue("BoardAccess", value);
        }
    }


    /// <summary>
    /// Indicates the board message post requires e-mail.
    /// </summary>
    public bool BoardRequireEmails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardRequireEmails"), false);
        }
        set
        {
            SetValue("BoardRequireEmails", value);
        }
    }


    /// <summary>
    /// Indicates whether the board is moderated.
    /// </summary>
    public bool BoardModerated
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardModerated"), false);
        }
        set
        {
            SetValue("BoardModerated", value);
        }
    }


    /// <summary>
    /// Indicates whether the CAPTCHA should be used.
    /// </summary>
    public bool BoardUseCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardUseCaptcha"), false);
        }
        set
        {
            SetValue("BoardUseCaptcha", value);
        }
    }


    /// <summary>
    /// Board opened from.
    /// </summary>
    public DateTime BoardOpenedFrom
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("BoardOpenedFrom"), DateTimeHelper.ZERO_TIME);
        }
        set
        {
            SetValue("BoardOpenedFrom", value);
        }
    }


    /// <summary>
    /// Board opened to.
    /// </summary>
    public DateTime BoardOpenedTo
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("BoardOpenedTo"), DateTimeHelper.ZERO_TIME);
        }
        set
        {
            SetValue("BoardOpenedTo", value);
        }
    }


    /// <summary>
    /// Indicates whether logging activity is performed.
    /// </summary>
    public bool BoardLogActivity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BoardLogActivity"), false);
        }
        set
        {
            SetValue("BoardLogActivity", value);
        }
    }


    /// <summary>
    /// Enables/disables content rating
    /// </summary>
    public bool EnableContentRating
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableContentRating"), false);
        }
        set
        {
            SetValue("EnableContentRating", value);
        }
    }


    /// <summary>
    /// Gets or sets type of content rating scale.
    /// </summary>
    public string RatingType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RatingType"), "StarsAJAX");
        }
        set
        {
            SetValue("RatingType", value);
        }
    }


    /// <summary>
    /// Gets or sets max value/length of content rating scale
    /// </summary>
    public int MaxRatingValue
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRatingValue"), 10);
        }
        set
        {
            SetValue("MaxRatingValue", value);
        }
    }


    /// <summary>
    /// Gest or sest the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            msgBoard.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, msgBoard.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            msgBoard.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            msgBoard.CacheMinutes = value;
        }
    }

    #endregion


    protected void Page_Init(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControls();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControls();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        // If the web part shouldn't proceed further
        if (StopProcessing)
        {
            msgBoard.BoardProperties.StopProcessing = true;
            Visible = false;
        }
        else
        {
            // Set the message board transformation
            msgBoard.MessageTransformation = MessageTransformation;

            // Set buttons
            msgBoard.BoardProperties.ShowApproveButton = ShowApprove;
            msgBoard.BoardProperties.ShowDeleteButton = ShowDelete;
            msgBoard.BoardProperties.ShowEditButton = ShowEdit;
            msgBoard.BoardProperties.ShowRejectButton = ShowReject;

            msgBoard.FormResourcePrefix = ResourcePrefix;

            // Set caching
            msgBoard.CacheItemName = CacheItemName;
            msgBoard.CacheMinutes = CacheMinutes;
            msgBoard.CacheDependencies = CacheDependencies;

            msgBoard.OrderBy = "MessageInserted DESC";

            // Use board properties
            if (BoardObj != null)
            {
                msgBoard.BoardProperties.BoardAccess = BoardObj.BoardAccess;
                msgBoard.BoardProperties.BoardOwner = "group";
                msgBoard.BoardProperties.BoardName = BoardObj.BoardName;
                msgBoard.BoardProperties.BoardDisplayName = BoardObj.BoardDisplayName;

                msgBoard.BoardProperties.BoardUnsubscriptionUrl = BoardInfoProvider.GetUnsubscriptionUrl(BoardObj.BoardUnsubscriptionURL, SiteContext.CurrentSiteName);
                msgBoard.BoardProperties.BoardBaseUrl = (string.IsNullOrEmpty(BoardObj.BoardBaseURL)) ? ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardBaseURL"), "") : BoardObj.BoardBaseURL;
                msgBoard.BoardProperties.BoardEnableSubscriptions = BoardObj.BoardEnableSubscriptions;
                msgBoard.BoardProperties.BoardOpened = BoardObj.BoardOpened;
                msgBoard.BoardProperties.BoardRequireEmails = BoardObj.BoardRequireEmails;
                msgBoard.BoardProperties.BoardModerated = BoardObj.BoardModerated;
                msgBoard.BoardProperties.BoardUseCaptcha = BoardObj.BoardUseCaptcha;
                msgBoard.BoardProperties.BoardOpenedFrom = BoardObj.BoardOpenedFrom;
                msgBoard.BoardProperties.BoardOpenedTo = BoardObj.BoardOpenedTo;
                msgBoard.BoardProperties.BoardEnableAnonymousRead = EnableAnonymousRead;
                msgBoard.BoardProperties.EnableContentRating = EnableContentRating;
                msgBoard.BoardProperties.CheckPermissions = CheckPermissions;
                msgBoard.BoardProperties.RatingType = RatingType;
                msgBoard.BoardProperties.MaxRatingValue = MaxRatingValue;
                msgBoard.MessageBoardID = BoardObj.BoardID;
                msgBoard.NoMessagesText = NoMessagesText;
            }
            // Use default properties
            else
            {
                // If the board is group and information on current group wasn't supplied hide the web part
                if (CurrentGroup == null)
                {
                    Visible = false;
                }
                else
                {
                    // Default board- document related continue
                    msgBoard.BoardProperties.BoardAccess = BoardAccess;
                    msgBoard.BoardProperties.BoardOwner = "group";
                    msgBoard.BoardProperties.BoardName = GetBoardName(WebPartName);

                    string boardDisplayName = null;
                    if (!String.IsNullOrEmpty(BoardDisplayName))
                    {
                        boardDisplayName = BoardDisplayName;
                    }
                    // Use predefined display name format
                    else
                    {
                        boardDisplayName = DocumentContext.CurrentPageInfo.GetDocumentName();
                    }
                    // Limit display name length
                    msgBoard.BoardProperties.BoardDisplayName = TextHelper.LimitLength(boardDisplayName, 250, "");

                    msgBoard.BoardProperties.BoardUnsubscriptionUrl = BoardInfoProvider.GetUnsubscriptionUrl(BoardUnsubscriptionUrl, SiteContext.CurrentSiteName);
                    msgBoard.BoardProperties.BoardBaseUrl = (string.IsNullOrEmpty(BoardBaseUrl)) ? ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardBaseURL"), "") : BoardBaseUrl;
                    msgBoard.BoardProperties.BoardEnableSubscriptions = BoardEnableSubscriptions;
                    msgBoard.BoardProperties.BoardOpened = BoardOpened;
                    msgBoard.BoardProperties.BoardRequireEmails = BoardRequireEmails;
                    msgBoard.BoardProperties.BoardModerated = BoardModerated;
                    msgBoard.BoardProperties.BoardModerators = BoardModerators;
                    msgBoard.BoardProperties.BoardUseCaptcha = BoardUseCaptcha;
                    msgBoard.BoardProperties.BoardOpenedFrom = BoardOpenedFrom;
                    msgBoard.BoardProperties.BoardOpenedTo = BoardOpenedTo;
                    msgBoard.BoardProperties.BoardEnableAnonymousRead = EnableAnonymousRead;
                    msgBoard.BoardProperties.BoardLogActivity = BoardLogActivity;
                    msgBoard.BoardProperties.EnableContentRating = EnableContentRating;
                    msgBoard.BoardProperties.CheckPermissions = CheckPermissions;
                    msgBoard.BoardProperties.RatingType = RatingType;
                    msgBoard.BoardProperties.MaxRatingValue = MaxRatingValue;
                    msgBoard.MessageBoardID = 0;
                    msgBoard.NoMessagesText = NoMessagesText;
                }
            }
        }
    }


    /// <summary>
    /// Returns board name according board type.
    /// </summary>
    /// <param name="webPartName">Name of the web part</param>
    private string GetBoardName(string webPartName)
    {
        return BoardInfoProvider.GetMessageBoardName(webPartName, BoardOwnerTypeEnum.Group, CurrentGroup.GroupGUID.ToString());
    }

    #endregion
}