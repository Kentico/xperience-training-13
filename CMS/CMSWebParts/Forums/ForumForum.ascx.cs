using System;

using CMS.Forums;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Forums_ForumForum : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the site name which should be used for forum group selection.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
        }
        set
        {
            SetValue("SiteName", value);
            viewerForum.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the roles split by semicolon, which roles can report abuse, requires  security access state authorize roles.
    /// </summary>
    public string AbuseReportRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AbuseReportRoles"), "");
        }
        set
        {
            SetValue("AbuseReportRoles", value);
            viewerForum.AbuseReportRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the security access enum to abuse report (Authenticated users by default).
    /// </summary>
    public SecurityAccessEnum AbuseReportAccess
    {
        get
        {
            return (SecurityAccessEnum)Enum.ToObject(typeof(SecurityAccessEnum), ValidationHelper.GetInteger(GetValue("AbuseReportAccess"), (int)viewerForum.AbuseReportAccess));
        }
        set
        {
            SetValue("AbuseReportAccess", value);
            viewerForum.AbuseReportAccess = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether on site management is enabled.
    /// </summary>
    public bool OnSiteManagement
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnSiteManagement"), viewerForum.EnableOnSiteManagement);
        }
        set
        {
            SetValue("OnSiteManagement", value);
            viewerForum.EnableOnSiteManagement = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether subscription should be enabled.
    /// </summary>
    public bool EnableSubscription
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSubscription"), viewerForum.EnableSubscription);
        }
        set
        {
            SetValue("EnableSubscription", value);
            viewerForum.EnableSubscription = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether usernam should be link to the user profile page.
    /// </summary>
    public bool RedirectToUserProfile
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RedirectToUserProfile"), viewerForum.RedirectToUserProfile);
        }
        set
        {
            SetValue("RedirectToUserProfile", value);
            viewerForum.RedirectToUserProfile = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether badge info should be displayed.
    /// </summary>
    public bool DisplayBadgeInfo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBadgeInfo"), viewerForum.DisplayBadgeInfo);
        }
        set
        {
            SetValue("DisplayBadgeInfo", value);
            viewerForum.DisplayBadgeInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), viewerForum.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            viewerForum.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the user can add his signature to the post.
    /// </summary>
    public bool EnableSignature
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSignature"), viewerForum.EnableSignature);
        }
        set
        {
            SetValue("EnableSignature", value);
            viewerForum.EnableSignature = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the user can add the posts to his favorites.
    /// </summary>
    public bool EnableFavorites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableFavorites"), viewerForum.EnableFavorites);
        }
        set
        {
            SetValue("EnableFavorites", value);
            viewerForum.EnableFavorites = value;
        }
    }


    /// <summary>
    /// Gets or sets maximal size of the image in the post.
    /// </summary>
    public int AttachmentImageMaxSideSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("AttachmentImageMaxSideSize"), 100);
        }
        set
        {
            SetValue("AttachmentImageMaxSideSize", value);
            viewerForum.AttachmentImageMaxSideSize = AttachmentImageMaxSideSize;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to display image prviews in attachment list.
    /// </summary>
    public bool DisplayAttachmentImage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAttachmentImage"), viewerForum.DisplayAttachmentImage);
        }
        set
        {
            SetValue("DisplayAttachmentImage", value);
            viewerForum.DisplayAttachmentImage = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether forum generates friendly URLs.
    /// </summary>
    public bool UseFriendlyURL
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseFriendlyURL"), false);
        }
        set
        {
            SetValue("UseFriendlyURL", value);
            viewerForum.UseFriendlyURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum base URL without extension.
    /// </summary>
    public string FriendlyBaseURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FriendlyBaseURL"), "");
        }
        set
        {
            SetValue("FriendlyBaseURL", value);
            viewerForum.FriendlyBaseURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the friendly URL extension. For extension less URLs sets it to empty string.
    /// </summary>
    public string FriendlyURLExtension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FriendlyURLExtension"), viewerForum.FriendlyURLExtension);
        }
        set
        {
            SetValue("FriendlyURLExtension", value);
            viewerForum.FriendlyURLExtension = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether avatars should be displayed.
    /// </summary>
    public bool EnableAvatars
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAvatars"), viewerForum.EnableAvatars);
        }
        set
        {
            SetValue("EnableAvatars", value);
            viewerForum.EnableAvatars = value;
        }
    }


    /// <summary>
    /// Gets or sets max side size of avatar image.
    /// </summary>
    public int AvatarMaxSideSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("AvatarMaxSideSize"), viewerForum.AvatarMaxSideSize);
        }
        set
        {
            SetValue("AvatarMaxSideSize", value);
            viewerForum.AvatarMaxSideSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the thread view mode.
    /// </summary>
    public FlatModeEnum ThreadViewMode
    {
        get
        {
            switch (ValidationHelper.GetString(GetValue("ThreadViewMode"), ""))
            {
                case "newtoold":
                    return FlatModeEnum.NewestToOldest;
                case "oldtonew":
                    return FlatModeEnum.OldestToNewest;
                default:
                    return FlatModeEnum.Threaded;
            }
        }
        set
        {
            switch (value)
            {
                case FlatModeEnum.NewestToOldest:
                    SetValue("ThreadViewMode", "newtoold");
                    break;
                case FlatModeEnum.OldestToNewest:
                    SetValue("ThreadViewMode", "oldtonew");
                    break;
                default:
                    SetValue("ThreadViewMode", "threaded");
                    break;
            }
            viewerForum.ViewMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  public user should be redirected 
    /// to the logon page if hasn't permissions to required action
    /// </summary>
    public bool RedirectUnauthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseRedirectForUnauthorized"), viewerForum.RedirectUnauthorized);
        }
        set
        {
            SetValue("UseRedirectForUnauthorized", value);
            viewerForum.RedirectUnauthorized = value;
        }
    }


    /// <summary>
    /// Gets or sets the logon page URL.
    /// </summary>
    public string LogonPageURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LogonPageURL"), viewerForum.LogonPageURL);
        }
        set
        {
            SetValue("LogonPageURL", value);
            viewerForum.LogonPageURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the access denied page URL (URL where the user is redirected when 
    /// trying to access forum for which the user is unauthorized).
    /// </summary>
    public string AccessDeniedPageURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AccessDeniedPageURL"), viewerForum.AccessDeniedPageURL);
        }
        set
        {
            SetValue("AccessDeniedPageURL", value);
            viewerForum.AccessDeniedPageURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled for thread, this option is depend on select forum layout.
    /// </summary>
    public bool EnableThreadPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableThreadPaging"), viewerForum.EnableThreadPaging);
        }
        set
        {
            SetValue("EnableThreadPaging", value);
            viewerForum.EnableThreadPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the thread page size.
    /// </summary>
    public int ThreadPageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ThreadPageSize"), viewerForum.ThreadPageSize);
        }
        set
        {
            SetValue("ThreadPageSize", value);
            viewerForum.ThreadPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled for thread, this option is depend on select forum layout.
    /// </summary>
    public bool EnablePostsPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePostsPaging"), viewerForum.EnablePostsPaging);
        }
        set
        {
            SetValue("EnablePostsPaging", value);
            viewerForum.EnablePostsPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the thread page size.
    /// </summary>
    public int PostsPageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PostsPageSize"), viewerForum.PostsPageSize);
        }
        set
        {
            SetValue("PostsPageSize", value);
            viewerForum.PostsPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the show mode (available only for tree forum).
    /// </summary>
    public ShowModeEnum ShowMode
    {
        get
        {
            return ForumModes.GetShowMode(ValidationHelper.GetString(GetValue("ShowMode"), viewerForum.ShowMode.ToString()));
        }
        set
        {
            SetValue("ShowMode", value);
            viewerForum.ShowMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether tree is expanded (available only for tree view mode).
    /// </summary>
    public bool ExpandTree
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandTree"), viewerForum.ExpandTree);
        }
        set
        {
            SetValue("ExpandTree", value);
            viewerForum.ExpandTree = value;
        }
    }


    /// <summary>
    /// Gets forum layout based on current forum code name due to backward compatibility.
    /// </summary>
    private string LayoutCodeName
    {
        get
        {
            if ((PartInstance != null) && (PartInstance.WebPartType == "ForumTree"))
            {
                return "Tree";
            }
            else
            {
                return "Flat";
            }
        }
    }


    /// <summary>
    /// Gets or sets the group code name.
    /// </summary>
    public string ForumLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumViewMode"), LayoutCodeName);
        }
        set
        {
            SetValue("ForumViewMode", value);
            viewerForum.ForumLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum code name.
    /// </summary>
    public string ForumName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumName"), "");
        }
        set
        {
            SetValue("ForumName", value);
            viewerForum.ForumName = value;
        }
    }
    
    #endregion


    /// <summary>
    /// OnContent load override.
    /// </summary>
    public override void OnContentLoaded()
    {
        SetupControl();
        base.OnContentLoaded();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initialize control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            viewerForum.StopProcessing = true;
        }
        else
        {
            // Forum settings 
            viewerForum.SiteName = SiteName;
            viewerForum.ForumName = ForumName;
            viewerForum.ForumLayout = ForumLayout;

            // Friendly URLs
            viewerForum.UseFriendlyURL = UseFriendlyURL;
            viewerForum.FriendlyURLExtension = FriendlyURLExtension;
            viewerForum.FriendlyBaseURL = FriendlyBaseURL;

            // Post options
            viewerForum.DisplayAttachmentImage = DisplayAttachmentImage;
            viewerForum.AttachmentImageMaxSideSize = AttachmentImageMaxSideSize;
            viewerForum.EnableAvatars = EnableAvatars;
            viewerForum.AvatarMaxSideSize = AvatarMaxSideSize;
            viewerForum.ViewMode = ThreadViewMode;
            viewerForum.EnableFavorites = EnableFavorites;
            viewerForum.EnableSignature = EnableSignature;
            viewerForum.MaxRelativeLevel = MaxRelativeLevel;
            viewerForum.DisplayBadgeInfo = DisplayBadgeInfo;
            viewerForum.RedirectToUserProfile = RedirectToUserProfile;
            
            // Behaviour
            viewerForum.EnableSubscription = EnableSubscription;
            viewerForum.EnableOnSiteManagement = OnSiteManagement;
            viewerForum.RedirectUnauthorized = RedirectUnauthorized;
            viewerForum.LogonPageURL = LogonPageURL;
            viewerForum.AccessDeniedPageURL = AccessDeniedPageURL;

            // Abuse report
            viewerForum.AbuseReportAccess = AbuseReportAccess;
            viewerForum.AbuseReportRoles = AbuseReportRoles;


            //Paging
            viewerForum.EnableThreadPaging = EnableThreadPaging;
            viewerForum.ThreadPageSize = ThreadPageSize;
            viewerForum.EnablePostsPaging = EnablePostsPaging;
            viewerForum.PostsPageSize = PostsPageSize;

            // Tree forum properties
            viewerForum.ShowMode = ShowMode;
            viewerForum.ExpandTree = ExpandTree;

            if (UseUpdatePanel)
            {
                viewerForum.UseRedirectAfterAction = false;
            }
        }
    }


    /// <summary>
    /// OnPreRender - hide webpart if content is not displayed.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        Visible = viewerForum.Visible && !StopProcessing;
        base.OnPreRender(e);
    }
}