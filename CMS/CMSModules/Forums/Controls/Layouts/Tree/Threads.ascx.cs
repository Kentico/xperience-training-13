using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Forums_Controls_Layouts_Tree_Threads : ForumViewer, IPostBackEventHandler
{
    #region "Private variables"

    private int mSelectedPost = 0;
    protected string mUnapproved = String.Empty;

    #endregion


    #region "Public properties"

    public int SelectedPost
    {
        get
        {
            return mSelectedPost;
        }
        set
        {
            mSelectedPost = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CopyValues(postTreeElem);

        // Hide selected area if forum is AdHoc
        if (IsAdHocForum)
        {
            plcHeader.Visible = false;
        }

        // Handle the Move topic mode
        if (ForumContext.CurrentMode != ForumMode.TopicMove)
        {
            plcMoveThread.Visible = false;
        }
        else
        {
            plcMoveThread.Visible = true;
            threadMove.TopicMoved += new EventHandler(TopicMoved);
        }

        if ((hdnSelected.Value == String.Empty) && (QueryHelper.Contains("moveto")))
        {
            hdnSelected.Value = QueryHelper.GetString("moveto", "");
            mSelectedPost = ValidationHelper.GetInteger(hdnSelected.Value, 0);
        }

        bool newThread = base.IsAvailable(null, ForumActionType.NewThread);
        bool newSubscription = base.IsAvailable(null, ForumActionType.SubscribeToForum);
        bool newFavorites = base.IsAvailable(null, ForumActionType.AddForumToFavorites);
        bool newBreadCrumbs = (ForumBreadcrumbs1.GenerateBreadcrumbs() != "");

        // Hide separators according to the link visibility
        // Each separator is hidden if the item preceeding the separator is invisible or
        // no item is behind the separator.
        if (!newThread || (!newSubscription && !newFavorites && !newBreadCrumbs))
        {
            plcActionSeparator.Visible = false;
        }
        if (!newSubscription || (!newFavorites && !newBreadCrumbs))
        {
            plcAddToFavoritesSeparator.Visible = false;
        }
        if (!newFavorites || !newBreadCrumbs)
        {
            plcBreadcrumbsSeparator.Visible = false;
        }

        // postTreeElem.ForumID = ForumContext.CurrentForum.ForumID;
        //postTreeElem.ShowMode = ShowModeEnum.TreeMode;

        // Unapproved posts are shown only if onsite mangement is enabled and user is moderator
        ForumInfo fi = ForumContext.CurrentForum;
        postTreeElem.SelectOnlyApproved = !((fi != null) && EnableOnSiteManagement && fi.ForumModerated && ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID));

        if (postTreeElem.ShowMode == ShowModeEnum.DynamicDetailMode)
        {
            // Set javascript for selected mode
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumTreeDynamic", ScriptHelper.GetScript("function SelectForumNode(nodeElem, selElem) { if (document.getElementById(nodeElem) != null) { document.getElementById(nodeElem).style.display = 'block';} " +
                                                                                                                    "if (document.getElementById(selElem) != null) {document.getElementById(selElem).style.display = 'none';} return false;}\n "));
        }
        else
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ForumTreeNormal", ScriptHelper.GetScript("function SelectForumNode(nodeElem) { return false; }\n "));
        }

        // Show post javascript
        string showScript = "function ShowPost(id){ document.getElementById('" + hdnSelected.ClientID + "').value = id; " + ControlsHelper.GetPostBackEventReference(this, "") + " }";
        //Register show post javascript code
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowForumPost", ScriptHelper.GetScript(showScript));

        // Don't use redirection after action
        UseRedirectAfterAction = false;

        int selected = ValidationHelper.GetInteger(hdnSelected.Value, ValidationHelper.GetInteger(Request[hdnSelected.UniqueID], 0));
        if (selected > 0)
        {
            ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(selected);
            if (fpi != null)
            {
                ucAbuse.ReportObjectID = fpi.PostId;
                ucAbuse.ReportTitle = ResHelper.GetString("Forums_WebInterface_ForumPost.AbuseReport", CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName)) + fpi.PostText;
                ucAbuse.CMSPanel.CommunityGroupID = CommunityGroupID;
                ucAbuse.CMSPanel.SecurityAccess = AbuseReportAccess;
                ucAbuse.CMSPanel.Roles = AbuseReportRoles;
            }
        }
    }


    /// <summary>
    /// Topic moved action handler.
    /// </summary>
    protected void TopicMoved(object sender, EventArgs e)
    {
        plcPostBody.Visible = false;
    }


    /// <summary>
    /// OnPrerender check whether.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        postTreeElem.Selected = ValidationHelper.GetInteger(hdnSelected.Value, SelectedPost);
        if ((mSelectedPost == 0) && (RequestHelper.IsAJAXRequest()))
        {
            mSelectedPost = postTreeElem.Selected;
        }

        if (mSelectedPost > 0)
        {
            ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(mSelectedPost);
            if ((fpi != null) && (ForumContext.CurrentForum != null) && (fpi.PostForumID == ForumContext.CurrentForum.ForumID))
            {
                plcPostPreview.Visible = true;
                ltlPostSubject.Text = HTMLHelper.HTMLEncode(fpi.PostSubject);
                ltlPostText.Text = ResolvePostText(fpi.PostText);
                ltlSignature.Text = GetSignatureArea(fpi, "<div class=\"SignatureArea\">", "</div>");
                ltlPostTime.Text = TimeZoneHelper.ConvertToUserTimeZone(fpi.PostTime, false, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite).ToString();
                ltlPostUser.Text = "<span class=\"PostUserName\">" + GetUserName(fpi) + "</span>";
                ltlAvatar.Text = AvatarImage(fpi);
                mUnapproved = !fpi.PostApproved ? " Unapproved" : "";

                if (ForumContext.CurrentForum.ForumEnableAdvancedImage)
                {
                    ltlPostText.AllowedControls = ControlsHelper.ALLOWED_FORUM_CONTROLS;
                }
                else
                {
                    ltlPostText.AllowedControls = "none";
                }

                attachmentDisplayer.ClearData();
                attachmentDisplayer.PostID = fpi.PostId;
                attachmentDisplayer.PostAttachmentCount = fpi.PostAttachmentCount;
                attachmentDisplayer.ReloadData();


                #region "Badge"

                if (DisplayBadgeInfo)
                {
                    // Set public badge if no badge is set
                    if (String.IsNullOrEmpty(ltlBadge.Text))
                    {
                        ltlBadge.Text = "<div class=\"Badge\">" + GetString("Forums.PublicBadge") + "</div>";
                    }
                }

                #endregion


                #region "Post actions"

                // Get the parent thread ID (for reply and quote)
                int threadId = ForumPostInfoProvider.GetPostRootFromIDPath(fpi.PostIDPath);

                ltlReply.Text = GetLink(fpi, GetString("Forums_WebInterface_ForumPost.replyLinkText"), "PostActionLink", ForumActionType.Reply, threadId);
                ltlQuote.Text = GetLink(fpi, GetString("Forums_WebInterface_ForumPost.quoteLinkText"), "PostActionLink", ForumActionType.Quote, threadId);
                ltlSubscribe.Text = GetLink(fpi, GetString("Forums_WebInterface_ForumPost.Subscribe"), "PostActionLink", ForumActionType.SubscribeToPost, threadId);

                ltlAnswer.Text = GetLink(fpi, GetString("general.yes"), "ActionLink", ForumActionType.IsAnswer);
                ltlNotAnswer.Text = GetLink(fpi, GetString("general.no"), "ActionLink", ForumActionType.IsNotAnswer);

                if (ltlAnswer.Text != String.Empty)
                {
                    ltlWasHelpful.Text = GetString("Forums_WebInterface_ForumPost.Washelpful") + "&nbsp;";
                    ltlAnswer.Text += "|";
                }

                ltlAddPostToFavorites.Text = GetLink(fpi, GetString("Forums_WebInterface_ForumPost.AddPostToFavorites"), "ActionLink", ForumActionType.AddPostToFavorites);

                ltlDelete.Text = GetLink(fpi, GetString("general.delete"), "ActionLink", ForumActionType.Delete);
                ltlEdit.Text = GetLink(fpi, GetString("general.edit"), "ActionLink", ForumActionType.Edit);
                ltlAttachments.Text = GetLink(fpi, GetString("general.attachments"), "ActionLink", ForumActionType.Attachment);

                #endregion


                // Hide separators
                if ((ltlReply.Text == "") || (ltlReply.Text != "" && ltlQuote.Text == ""))
                {
                    plcFirstSeparator.Visible = false;
                }

                if ((ltlSubscribe.Text == "") || (ltlSubscribe.Text != "" && ltlQuote.Text == ""))
                {
                    plcSecondSeparator.Visible = false;
                }

                if (ltlReply.Text != "" && ltlSubscribe.Text != "")
                {
                    plcFirstSeparator.Visible = true;
                }

                pnlManage.Visible = (EnableOnSiteManagement && (ForumContext.CurrentForum != null)) ? ForumContext.UserIsModerator(ForumContext.CurrentForum.ForumID, CommunityGroupID) : false;
                if (pnlManage.Visible)
                {
                    ltlApprove.Text = GetLink(fpi, GetString("general.approve"), "ActionLink", ForumActionType.Appprove);
                    ltlApproveAll.Text = GetLink(fpi, GetString("forums.approveall"), "ActionLink", ForumActionType.ApproveAll);
                    ltlReject.Text = GetLink(fpi, GetString("general.reject"), "ActionLink", ForumActionType.Reject);
                    ltlRejectAll.Text = GetLink(fpi, GetString("forums.rejectall"), "ActionLink", ForumActionType.RejectAll);
                    ltlSplit.Text = GetLink(fpi, GetString("forums.splitthread"), "ActionLink", ForumActionType.SplitThread);
                    ltlMove.Text = GetLink(fpi, GetString("forums.movethread"), "ActionLink", ForumActionType.MoveToTheOtherForum, threadId);
                }
            }
        }
        else
        {
            plcPostPreview.Visible = false;
        }

        if (ControlsHelper.IsInUpdatePanel(this))
        {
            ControlsHelper.GetUpdatePanel(this).Update();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns the link with added ThreadID parameter.
    /// </summary>
    private string GetLink(object data, object text, string cssClassName, ForumActionType type, int threadId)
    {
        string url = GetURL(data, type);
        if (url != "")
        {
            return "<a class=\"" + cssClassName + "\" href=\"" + URLHelper.AddParameterToUrl(url, "ThreadID", threadId.ToString()) + "\">" + text + "</a>";
        }

        return "";
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Gets post number.
    /// </summary>
    /// <param name="eventArgument">Argument of postback event</param>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        mSelectedPost = ValidationHelper.GetInteger(hdnSelected.Value, 0);

        // Call parent postback event handler if event argument is defined
        if (eventArgument != "")
        {
            string[] info = eventArgument.Split('$');
            if (info[0].ToLowerCSafe() == "delete")
            {
                UseRedirectAfterAction = true;
            }
            base.RaisePostBackEvent(eventArgument);
        }
    }

    #endregion
}
