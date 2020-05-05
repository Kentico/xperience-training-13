using System;
using System.Web;
using System.Web.UI;

using CMS.Community;
using CMS.Forums;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Community_Forums_GroupForumSearchResults : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether search results support forum selection.
    /// </summary>
    public bool EnableForumSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableForumSelection"), true);
        }
        set
        {
            SetValue("EnableForumSelection", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether usernam should be link to the user profile page.
    /// </summary>
    public bool RedirectToUserProfile
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RedirectToUserProfile"), forumElem.RedirectToUserProfile);
        }
        set
        {
            SetValue("RedirectToUserProfile", value);
            forumElem.RedirectToUserProfile = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether badge info should be displayed.
    /// </summary>
    public bool DisplayBadgeInfo
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBadgeInfo"), forumElem.DisplayBadgeInfo);
        }
        set
        {
            SetValue("DisplayBadgeInfo", value);
            forumElem.DisplayBadgeInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets no result text.
    /// </summary>
    public string NoResultsText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NoResultsText"), forumElem.SearchNoResults);
        }
        set
        {
            SetValue("NoResultsText", value);
            forumElem.SearchNoResults = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether avatars should be displayed.
    /// </summary>
    public bool EnableAvatars
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAvatars"), forumElem.EnableAvatars);
        }
        set
        {
            SetValue("EnableAvatars", value);
            forumElem.EnableAvatars = value;
        }
    }


    /// <summary>
    /// Gets or sets max side sizfe of avatar image.
    /// </summary>
    public int AvatarMaxSideSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("AvatarMaxSideSize"), forumElem.AvatarMaxSideSize);
        }
        set
        {
            SetValue("AvatarMaxSideSize", value);
            forumElem.AvatarMaxSideSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the group code name.
    /// </summary>
    public string ForumLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumViewMode"), "Flat");
        }
        set
        {
            SetValue("ForumViewMode", value);
            forumElem.ForumLayout = value;
        }
    }

    #endregion


    #region "Methods"

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
            forumElem.StopProcessing = true;
        }
        else
        {
            forumElem.ForumLayout = ForumLayout;
            forumElem.DisplayBadgeInfo = DisplayBadgeInfo;
            forumElem.RedirectToUserProfile = RedirectToUserProfile;
            forumElem.SearchNoResults = NoResultsText;

            if (EnableForumSelection)
            {
                forumElem.SearchInForums = QueryHelper.GetString("searchforums", "");
            }

            forumElem.EnableAvatars = EnableAvatars;
            forumElem.AvatarMaxSideSize = AvatarMaxSideSize;
            forumElem.SearchResult = true;
            if (CommunityContext.CurrentGroup != null)
            {
                forumElem.CommunityGroupID = CommunityContext.CurrentGroup.GroupID;
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        Visible = !StopProcessing;

        // If forum is not in search state, hide
        if (ForumContext.CurrentState != ForumStateEnum.Search)
        {
            Visible = false;
        }
    }

    #endregion
}