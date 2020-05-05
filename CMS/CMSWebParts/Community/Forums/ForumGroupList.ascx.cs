using System;
using System.Data;
using System.Web.UI;

using CMS.Community;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_Forums_ForumGroupList : CMSAbstractWebPart
{
    #region "Variables"

    private ForumViewer ForumDivider1 = new ForumViewer();
    private DataSet forumGroups;

    #endregion


    #region "Public properties"

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
            ForumDivider1.AbuseReportRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the security access enum to abuse report (Authenticated users by default).
    /// </summary>
    public SecurityAccessEnum AbuseReportAccess
    {
        get
        {
            return (SecurityAccessEnum)Enum.ToObject(typeof(SecurityAccessEnum), ValidationHelper.GetInteger(GetValue("AbuseReportAccess"), (int)ForumDivider1.AbuseReportAccess));
        }
        set
        {
            SetValue("AbuseReportAccess", value);
            ForumDivider1.AbuseReportAccess = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether on site management is enabled.
    /// </summary>
    public bool OnSiteManagement
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnSiteManagement"), ForumDivider1.EnableOnSiteManagement);
        }
        set
        {
            SetValue("OnSiteManagement", value);
            ForumDivider1.EnableOnSiteManagement = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether subscription should be enabled.
    /// </summary>
    public bool EnableSubscription
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSubscription"), ForumDivider1.EnableSubscription);
        }
        set
        {
            SetValue("EnableSubscription", value);
            ForumDivider1.EnableSubscription = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether username should be link to the user profile page.
    /// </summary>
    public bool RedirectToUserProfile
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RedirectToUserProfile"), ForumDivider1.RedirectToUserProfile);
        }
        set
        {
            SetValue("RedirectToUserProfile", value);
            ForumDivider1.RedirectToUserProfile = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether badge info should be displayed.
    /// </summary>
    public bool DisplayBadgeInfo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBadgeInfo"), ForumDivider1.DisplayBadgeInfo);
        }
        set
        {
            SetValue("DisplayBadgeInfo", value);
            ForumDivider1.DisplayBadgeInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), ForumDivider1.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            ForumDivider1.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the user can add his signature to the post.
    /// </summary>
    public bool EnableSignature
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSignature"), ForumDivider1.EnableSignature);
        }
        set
        {
            SetValue("EnableSignature", value);
            ForumDivider1.EnableSignature = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the user can add the posts to his favorites.
    /// </summary>
    public bool EnableFavorites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableFavorites"), ForumDivider1.EnableFavorites);
        }
        set
        {
            SetValue("EnableFavorites", value);
            ForumDivider1.EnableFavorites = value;
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
            ForumDivider1.AttachmentImageMaxSideSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to display image prviews in attachment list.
    /// </summary>
    public bool DisplayAttachmentImage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAttachmentImage"), ForumDivider1.DisplayAttachmentImage);
        }
        set
        {
            SetValue("DisplayAttachmentImage", value);
            ForumDivider1.DisplayAttachmentImage = value;
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
            ForumDivider1.UseFriendlyURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum friendly base URL without extension.
    /// </summary>
    public string BaseURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BaseURL"), "");
        }
        set
        {
            SetValue("BaseURL", value);
            ForumDivider1.BaseURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum friendly base URL without extension.
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
            ForumDivider1.FriendlyBaseURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum unsubscription URL.
    /// </summary>
    public string UnsubscriptionURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnsubscriptionURL"), "");
        }
        set
        {
            SetValue("UnsubscriptionURL", value);
            ForumDivider1.UnsubscriptionURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the friendly URL extension. For extension less URLs sets it to empty string.
    /// </summary>
    public string FriendlyURLExtension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FriendlyURLExtension"), ForumDivider1.FriendlyURLExtension);
        }
        set
        {
            SetValue("FriendlyURLExtension", value);
            ForumDivider1.FriendlyURLExtension = value;
        }
    }


    /// <summary>
    /// Gets or sets the group code name.
    /// </summary>
    public string ForumLayout
    {
        get
        {
            string layout = ValidationHelper.GetString(GetValue("ForumViewMode"), "Flat");

            // Backward compatibility
            if (layout == "1")
            {
                return "Flat";
            }
            else if (layout == "0")
            {
                return "Tree";
            }

            return layout;
        }
        set
        {
            SetValue("ForumViewMode", value);
            ForumDivider1.ForumLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether avatars should be displayed.
    /// </summary>
    public bool EnableAvatars
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAvatars"), ForumDivider1.EnableAvatars);
        }
        set
        {
            SetValue("EnableAvatars", value);
            ForumDivider1.EnableAvatars = value;
        }
    }


    /// <summary>
    /// Gets or sets max side sizfe of avatar image.
    /// </summary>
    public int AvatarMaxSideSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("AvatarMaxSideSize"), ForumDivider1.AvatarMaxSideSize);
        }
        set
        {
            SetValue("AvatarMaxSideSize", value);
            ForumDivider1.AvatarMaxSideSize = value;
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
            ForumDivider1.ViewMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether public user should be redirected 
    /// to the logon page if hasn't permissions to required action
    /// </summary>
    public bool RedirectUnauthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseRedirectForUnauthorized"), ForumDivider1.RedirectUnauthorized);
        }
        set
        {
            SetValue("UseRedirectForUnauthorized", value);
            ForumDivider1.RedirectUnauthorized = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the forums for which the user has no permissions
    /// are visible in the list of forums in forum group.
    /// </summary>
    public bool HideForumForUnauthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideForumForUnauthorized"), ForumDivider1.HideForumForUnauthorized);
        }
        set
        {
            SetValue("HideForumForUnauthorized", value);
            ForumDivider1.HideForumForUnauthorized = value;
        }
    }


    /// <summary>
    /// Gets or sets the logon page URL.
    /// </summary>
    public string LogonPageURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LogonPageURL"), ForumDivider1.LogonPageURL);
        }
        set
        {
            SetValue("LogonPageURL", value);
            ForumDivider1.LogonPageURL = value;
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
            return ValidationHelper.GetString(GetValue("AccessDeniedPageURL"), ForumDivider1.AccessDeniedPageURL);
        }
        set
        {
            SetValue("AccessDeniedPageURL", value);
            ForumDivider1.AccessDeniedPageURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled for thread, this option is depend on select forum layout.
    /// </summary>
    public bool EnableThreadPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableThreadPaging"), ForumDivider1.EnableThreadPaging);
        }
        set
        {
            SetValue("EnableThreadPaging", value);
            ForumDivider1.EnableThreadPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the thread page size.
    /// </summary>
    public int ThreadPageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ThreadPageSize"), ForumDivider1.ThreadPageSize);
        }
        set
        {
            SetValue("ThreadPageSize", value);
            ForumDivider1.ThreadPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled for thread, this option is depend on select forum layout.
    /// </summary>
    public bool EnablePostsPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePostsPaging"), ForumDivider1.EnablePostsPaging);
        }
        set
        {
            SetValue("EnablePostsPaging", value);
            ForumDivider1.EnablePostsPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the thread page size.
    /// </summary>
    public int PostsPageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PostsPageSize"), ForumDivider1.PostsPageSize);
        }
        set
        {
            SetValue("PostsPageSize", value);
            ForumDivider1.PostsPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the show mode (available only for tree forum).
    /// </summary>
    public ShowModeEnum ShowMode
    {
        get
        {
            return ForumModes.GetShowMode(ValidationHelper.GetString(GetValue("ShowMode"), ForumDivider1.ShowMode.ToString()));
        }
        set
        {
            SetValue("ShowMode", value);
            ForumDivider1.ShowMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether tree is expanded (available only for tree view mode).
    /// </summary>
    public bool ExpandTree
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandTree"), ForumDivider1.ExpandTree);
        }
        set
        {
            SetValue("ExpandTree", value);
            ForumDivider1.ExpandTree = value;
        }
    }

    #endregion


    #region "Group list properties"

    /// <summary>
    /// Gets or sets the community group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            string groupName = ValidationHelper.GetString(GetValue("GroupName"), "");
            if ((string.IsNullOrEmpty(groupName) || groupName == GroupInfoProvider.CURRENT_GROUP) && (CommunityContext.CurrentGroup != null))
            {
                return CommunityContext.CurrentGroup.GroupName;
            }
            return groupName;
        }
        set
        {
            SetValue("GroupName", value);
        }
    }


    /// <summary>
    /// Gets or sets the separator which should be displayed between groups.
    /// </summary>
    public string GroupsSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupsSeparator"), "");
        }
        set
        {
            SetValue("GroupsSeparator", value);
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
    /// Reloads control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Pre-render override for setting visiblity.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        Visible = !DataHelper.DataSourceIsEmpty(forumGroups);
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Clear placeholder first
            plcHolder.Controls.Clear();

            // Group settings
            ForumDivider1.ForumLayout = ForumLayout;

            // Friendly URLs
            ForumDivider1.UseFriendlyURL = UseFriendlyURL;
            ForumDivider1.FriendlyURLExtension = FriendlyURLExtension;
            ForumDivider1.FriendlyBaseURL = FriendlyBaseURL;

            // Post options
            ForumDivider1.DisplayAttachmentImage = DisplayAttachmentImage;
            ForumDivider1.AttachmentImageMaxSideSize = AttachmentImageMaxSideSize;
            ForumDivider1.EnableAvatars = EnableAvatars;
            ForumDivider1.AvatarMaxSideSize = AvatarMaxSideSize;
            ForumDivider1.ViewMode = ThreadViewMode;
            ForumDivider1.EnableFavorites = EnableFavorites;
            ForumDivider1.EnableSignature = EnableSignature;
            ForumDivider1.MaxRelativeLevel = MaxRelativeLevel;
            ForumDivider1.DisplayBadgeInfo = DisplayBadgeInfo;
            ForumDivider1.RedirectToUserProfile = RedirectToUserProfile;
            ForumDivider1.BaseURL = BaseURL;
            ForumDivider1.UnsubscriptionURL = UnsubscriptionURL;
            
            // Behaviour
            ForumDivider1.EnableSubscription = EnableSubscription;
            ForumDivider1.EnableOnSiteManagement = OnSiteManagement;
            ForumDivider1.HideForumForUnauthorized = HideForumForUnauthorized;
            ForumDivider1.RedirectUnauthorized = RedirectUnauthorized;
            ForumDivider1.LogonPageURL = LogonPageURL;
            ForumDivider1.AccessDeniedPageURL = AccessDeniedPageURL;

            // Abuse report
            ForumDivider1.AbuseReportAccess = AbuseReportAccess;
            ForumDivider1.AbuseReportRoles = AbuseReportRoles;

            //Paging
            ForumDivider1.EnableThreadPaging = EnableThreadPaging;
            ForumDivider1.ThreadPageSize = ThreadPageSize;
            ForumDivider1.EnablePostsPaging = EnablePostsPaging;
            ForumDivider1.PostsPageSize = PostsPageSize;

            // Tree forum properties
            ForumDivider1.ShowMode = ShowMode;
            ForumDivider1.ExpandTree = ExpandTree;

            if (CommunityContext.CurrentGroup != null)
            {
                ForumDivider1.CommunityGroupID = CommunityContext.CurrentGroup.GroupID;
            }

            if (UseUpdatePanel)
            {
                ForumDivider1.UseRedirectAfterAction = false;
            }

            string path = ResolveUrl("~/CMSModules/Forums/Controls/ForumDivider.ascx");
            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            if (gi != null)
            {
                var innerQuery = new ObjectQuery<ForumInfo>().WhereEquals("ForumOpen", 1).WhereEquals("ForumGroupID", new QueryColumn("GroupID")).Column(new CountColumn("ForumID"));

                forumGroups = ForumGroupInfoProvider.GetForumGroups()
                                                    .WhereEquals("GroupGroupID", gi.GroupID)
                                                    .WhereGreaterThan(innerQuery, 0)
                                                    .OrderBy("GroupOrder");
                if (!DataHelper.DataSourceIsEmpty(forumGroups))
                {
                    int ctrlId = 0;
                    bool wasAdded = false;

                    // Load all the groups
                    foreach (DataRow dr in forumGroups.Tables[0].Rows)
                    {
                        Control ctrl = LoadUserControl(path);
                        if (ctrl != null)
                        {
                            ctrl.ID = "groupElem" + ctrlId;
                            ForumViewer frmv = ctrl as ForumViewer;
                            if (frmv != null)
                            {
                                // Copy values
                                ForumDivider1.CopyValues(frmv);
                                if (wasAdded && !String.IsNullOrEmpty(GroupsSeparator))
                                {
                                    plcHolder.Controls.Add(new LiteralControl(GroupsSeparator));
                                }

                                frmv.GroupID = ValidationHelper.GetInteger(dr["GroupID"], 0);
                                plcHolder.Controls.Add(frmv);
                                wasAdded = true;
                            }
                        }

                        ctrlId++;
                    }
                }
            }
        }
    }
}