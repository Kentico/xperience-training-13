using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;

public partial class CMSModules_Forums_Controls_ForumDivider : ForumViewer
{
    #region "Private variables"

    // Default path to the forum layouts directory
    private const string defaultPath = "~/CMSModules/Forums/Controls/Layouts/";
    // Current control
    private Control ctrl;
    // Indicates starting mode, 0 - Group, 1 - Forum
    private int startingMode = -1;
    // Current forum state
    private ForumStateEnum currentState = ForumStateEnum.Unknown;
    // group name
    private string mGroupName = "";
    // forum name
    private string mForumName = "";
    // Indicates whether current control is search results control
    private bool mSearchResult;

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
    /// Gets or sets the value that indicates whether current control is search result.
    /// </summary>
    public bool SearchResult
    {
        get
        {
            return mSearchResult;
        }
        set
        {
            mSearchResult = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    public override int ForumID
    {
        get
        {
            return base.ForumID;
        }
        set
        {
            base.ForumID = value;
            startingMode = 1;
            ForumContext.ForumID = value;
        }
    }


    /// <summary>
    /// Gets or sets the group id.
    /// </summary>
    public override int GroupID
    {
        get
        {
            return base.GroupID;
        }
        set
        {
            if (!StopProcessing && !StopViewerLoading)
            {
                base.GroupID = value;
                ForumContext.GroupID = value;
                startingMode = 0;
                // Check whether forum is defined
                if ((ForumContext.CurrentForum != null) && (currentState != ForumStateEnum.Forums))
                {
                    // For nested level call request to single display
                    if ((ForumContext.CurrentGroup != null) && (ForumContext.CurrentForum.ForumGroupID == ForumContext.CurrentGroup.GroupID))
                    {
                        DisplayOnlyMe(this);
                        StopViewerLoading = true;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the community group id.
    /// </summary>
    public override int CommunityGroupID
    {
        get
        {
            return base.CommunityGroupID;
        }
        set
        {
            base.CommunityGroupID = value;
            ForumContext.CommunityGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets current group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            return mGroupName;
        }
        set
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(value, SiteID, CommunityGroupID);
            if (fgi != null)
            {
                GroupID = fgi.GroupID;
            }
            mGroupName = value;
        }
    }


    /// <summary>
    /// Gets or sets current forum name.
    /// </summary>
    public string ForumName
    {
        get
        {
            return mForumName;
        }
        set
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(value, SiteID, CommunityGroupID);
            if (fi != null)
            {
                ForumID = fi.ForumID;
            }

            if ((!String.IsNullOrEmpty(value)) && (value == "adhocforumgroup" || value == "ad_hoc_forum"))
            {
                IsAdHocForum = true;
                fi = ForumInfoProvider.GetForumInfoByDocument(DocumentContext.CurrentDocument.DocumentID);
                if (fi != null)
                {
                    ForumID = fi.ForumID;
                }
                else
                {
                    ForumID = -1;
                }
            }
            startingMode = 1;
            mForumName = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Collection of forum viewers.
    /// </summary>
    private static List<ForumViewer> ForumViewers
    {
        get
        {
            List<ForumViewer> viewers = RequestStockHelper.GetItem("ForumViewerCollection") as List<ForumViewer>;
            if (viewers == null)
            {
                viewers = new List<ForumViewer>();
                RequestStockHelper.Add("ForumViewerCollection", viewers);
            }

            return viewers;
        }
    }


    /// <summary>
    /// Gets or sets tha value that indicates whether forum viewers shouldn't be loaded.
    /// </summary>
    private static bool StopViewerLoading
    {
        get
        {
            return ValidationHelper.GetBoolean(RequestStockHelper.GetItem("ForumStopViewerLoading"), false);
        }
        set
        {
            RequestStockHelper.Add("ForumStopViewerLoading", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="state">Current state</param>
    public ForumStateEnum CheckPermissions(ForumStateEnum state)
    {
        // Return original state for selected types
        if ((state == ForumStateEnum.Forums) || (state == ForumStateEnum.Unknown) || (state == ForumStateEnum.Search))
        {
            return state;
        }

        // If forum doesn't exist display default
        if (ForumContext.CurrentForum == null)
        {
            return ForumStateEnum.Forums;
        }

        // If forum is closed => hide
        if ((!IsAdHocForum) && (!ForumContext.CurrentForum.ForumOpen))
        {
            return ForumStateEnum.Forums;
        }

        // Sets threads state for every action if forum is locked
        if (ForumContext.CurrentForum.ForumIsLocked)
        {
            switch (state)
            {
                case ForumStateEnum.NewSubscription:
                case ForumStateEnum.NewThread:
                case ForumStateEnum.ReplyToPost:
                case ForumStateEnum.SubscribeToPost:
                    return ForumStateEnum.Threads;
                // Allow attachment view for forum moderators
                case ForumStateEnum.Attachments:
                    if (ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID))
                    {
                        return state;
                    }
                    return ForumStateEnum.Threads;
            }
        }

        // If user is global admin, forum admin, community admin or moderator
        if (ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID))
        {
            return state;
        }

        // Sets thread state for locked post 
        if ((ForumContext.CurrentThread != null) && (ForumContext.CurrentThread.PostIsLocked))
        {
            if (!ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID))
            {
                switch (state)
                {
                    case ForumStateEnum.NewSubscription:
                    case ForumStateEnum.SubscribeToPost:
                    case ForumStateEnum.NewThread:
                    case ForumStateEnum.ReplyToPost:
                    case ForumStateEnum.Attachments:
                        return ForumStateEnum.Thread;
                }
            }
        }

        bool hasPermissions = true;

        // Check permissions for action
        switch (state)
        {
            case ForumStateEnum.ReplyToPost:
                hasPermissions = ForumInfoProvider.IsAuthorizedPerForum(ForumContext.CurrentForum.ForumID, ForumContext.CurrentForum.ForumGroupID, "Reply", ForumContext.CurrentForum.AllowReply, MembershipContext.AuthenticatedUser);
                break;
            case ForumStateEnum.NewThread:
                hasPermissions = ForumInfoProvider.IsAuthorizedPerForum(ForumContext.CurrentForum.ForumID, ForumContext.CurrentForum.ForumGroupID, "Post", ForumContext.CurrentForum.AllowPost, MembershipContext.AuthenticatedUser);
                break;
            case ForumStateEnum.Attachments:
                hasPermissions = ForumInfoProvider.IsAuthorizedPerForum(ForumContext.CurrentForum.ForumID, ForumContext.CurrentForum.ForumGroupID, "AttachFiles", ForumContext.CurrentForum.AllowAttachFiles, MembershipContext.AuthenticatedUser);
                break;
            case ForumStateEnum.TopicMove:
                hasPermissions = ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID);
                break;
            case ForumStateEnum.SubscribeToPost:
            case ForumStateEnum.NewSubscription:
                hasPermissions = ForumInfoProvider.IsAuthorizedPerForum(ForumContext.CurrentForum.ForumID, ForumContext.CurrentForum.ForumGroupID, "Subscribe", ForumContext.CurrentForum.AllowSubscribe, MembershipContext.AuthenticatedUser) && EnableSubscription;
                break;
            case ForumStateEnum.EditPost:
                hasPermissions = ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID) || (ForumContext.CurrentForum.ForumAuthorEdit && (ForumContext.CurrentPost != null && !MembershipContext.AuthenticatedUser.IsPublic() && (ForumContext.CurrentPost.PostUserID == MembershipContext.AuthenticatedUser.UserID)));
                break;
        }

        // Check ForumAccess permission
        if (ForumContext.CurrentForum != null)
        {
            hasPermissions = hasPermissions && ForumInfoProvider.IsAuthorizedPerForum(ForumContext.CurrentForum.ForumID, ForumContext.CurrentForum.ForumGroupID, "AccessToForum", ForumContext.CurrentForum.AllowAccess, MembershipContext.AuthenticatedUser);
        }

        // Check whether user has permissions for selected state
        if (!hasPermissions)
        {
            // Check whether public user should be redirected to logon page
            if (RedirectUnauthorized && MembershipContext.AuthenticatedUser.IsPublic())
            {
                URLHelper.Redirect(URLHelper.AddParameterToUrl(ResolveUrl(LogonPageURL), "returnurl", HttpUtility.UrlEncode(RequestContext.CurrentURL)));
            }
            else if (!String.IsNullOrEmpty(AccessDeniedPageURL))
            {
                URLHelper.Redirect(URLHelper.AddParameterToUrl(ResolveUrl(AccessDeniedPageURL), "returnurl", HttpUtility.UrlEncode(RequestContext.CurrentURL)));
            }
            // Sets state with dependence on current settings
            else
            {
                if (startingMode == 0)
                {
                    return ForumStateEnum.Forums;
                }
                else
                {
                    return ForumStateEnum.AccessDenied;
                }
            }
        }

        return state;
    }


    /// <summary>
    /// OnInit - handle single display request.
    /// </summary>
    /// <param name="e">Event args</param>
    protected override void OnInit(EventArgs e)
    {
        // Add current viewer to the viewers collection
        ForumViewers.Add(this);
        base.OnInit(e);
    }


    /// <summary>
    /// Page_Load - Set properties for current control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Check stop processing
            if (StopProcessing)
            {
                return;
            }

            EnsureUnsubscription();

            if (!SearchResult)
            {
                ForumContext.GroupID = GroupID;
                ForumContext.ForumID = ForumID;
                ForumContext.CommunityGroupID = CommunityGroupID;
            }

            // If search result control, check whether search is performed
            if (SearchResult && (ForumContext.CurrentState != ForumStateEnum.Search))
            {
                return;
            }

            // Get current state
            currentState = CheckPermissions(ForumContext.CurrentState);

            if (currentState != ForumStateEnum.Search)
            {
                // Group
                if (startingMode == 0)
                {
                    // Check whether forum is defined
                    if ((ForumContext.CurrentForum != null) && (currentState != ForumStateEnum.Forums))
                    {
                        // For nested level call request to single display
                        if ((ForumContext.CurrentGroup != null) && (ForumContext.CurrentForum.ForumGroupID == ForumContext.CurrentGroup.GroupID))
                        {
                            DisplayOnlyMe(this);
                        }
                        else
                        {
                            // Hide current forum because ids not match
                            return;
                        }
                    }
                }
                // Forum
                else if (startingMode == 1)
                {
                    // Hide all others forums
                    DisplayOnlyMe(this);
                }
                else
                {
                    // Hide forum because none of mandatory property was set
                    return;
                }
            }


            // Display correspondent control with dependence on current mode
            switch (currentState)
            {
                // Threads
                case ForumStateEnum.Threads:
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/Threads.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "threadsElem", ctrl);
                    break;

                // Thread
                case ForumStateEnum.Thread:
                    // Log thread views
                    if ((ForumContext.CurrentThread != null) && (!QueryHelper.Contains("tpage")))
                    {
                        ThreadViewCounter.LogThreadView(ForumContext.CurrentThread.PostId);
                    }
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/Thread.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "threadElem", ctrl);
                    break;

                // New post, reply or edit post
                case ForumStateEnum.NewThread:
                case ForumStateEnum.ReplyToPost:
                case ForumStateEnum.EditPost:
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/ThreadEdit.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "editElem", ctrl);
                    break;

                // Subscription to forum or subscription to post
                case ForumStateEnum.NewSubscription:
                case ForumStateEnum.SubscribeToPost:
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/SubscriptionEdit.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "subscriptionElem", ctrl);
                    break;

                // Forums
                case ForumStateEnum.Forums:
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/Forums.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "forumsElem", ctrl);
                    break;

                case ForumStateEnum.Attachments:
                    ctrl = LoadUserControl(defaultPath + ForumLayout + "/Attachments.ascx");
                    ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "attachmentElem", ctrl);
                    break;

                case ForumStateEnum.Search:
                    if (SearchResult)
                    {
                        ctrl = LoadUserControl(defaultPath + ForumLayout + "/SearchResults.ascx");
                        ctrl.ID = ControlsHelper.GetUniqueID(plcForum, "searchElem", ctrl);
                    }
                    else
                    {
                        return;
                    }
                    break;

                case ForumStateEnum.AccessDenied:
                    Visible = false;
                    return;

                // Unknown
                case ForumStateEnum.Unknown:
                    if (!SearchResult)
                    {
                        throw new Exception("[Forum divider]: Unknown forum state.");
                    }
                    return;
            }

            // Clear controls collection
            plcForum.Controls.Clear();

            // Add loaded control to the control collection
            plcForum.Controls.Add(ctrl);


            // Get forum viewer control
            ForumViewer frmv = ctrl as ForumViewer;

            // If control exists set forum properties
            if (frmv != null)
            {
                CopyValues(frmv);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    /// <summary>
    /// Redirect to the unsubscription page if current url contains valid unbscription information.
    /// </summary>
    protected void EnsureUnsubscription()
    {
        Guid unsubGuid = QueryHelper.GetGuid("forumsubguid", Guid.Empty);
        if (unsubGuid != Guid.Empty)
        {
            int forumId = QueryHelper.GetInteger("forumid", 0);
            if (forumId > 0)
            {
                ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(unsubGuid);
                if (fsi != null)
                {
                    ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
                    if ((fi != null) && (fsi.SubscriptionForumID == fi.ForumID))
                    {
                        URLHelper.Redirect("~/CMSModules/Forums/CMSPages/unsubscribe.aspx?forumsubguid=" + unsubGuid.ToString());
                    }
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Hide all forum viewers except the sender.
    /// </summary>
    /// <param name="sender">Forum instance which requires single display</param>
    private static void DisplayOnlyMe(Control sender)
    {
        var viewers = RequestStockHelper.GetItem("ForumViewerCollection") as List<ForumViewer>;
        if ((viewers != null) && (viewers.Count > 0))
        {
            var senderType = sender as ForumViewer;

            foreach (var forumViewer in viewers)
            {
                if ((forumViewer != null) && (forumViewer != senderType))
                {
                    forumViewer.StopProcessing = true;
                    forumViewer.Visible = false;
                }
            }
        }
    }

    #endregion

}