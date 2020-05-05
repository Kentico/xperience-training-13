using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;


public partial class CMSModules_Forums_Controls_Posts_ForumPost : ForumViewer
{
    #region "Variables"

    private int mPostID;

    #endregion


    #region "Public properties"



    /// <summary>
    /// Current post id.
    /// </summary>
    public int PostID
    {
        get
        {
            return mPostID;
        }
        set
        {
            mPostID = value;
            PostInfoInternal = null;
        }
    }


    /// <summary>
    /// Gets or sets post info object for internal purposes.
    /// </summary>
    private ForumPostInfo PostInfoInternal
    {
        get
        {
            return (ForumPostInfo)GetValue("PostInfo");
        }
        set
        {
            SetValue("PostInfo", value);
        }
    }


    /// <summary>
    /// Gets or sets post info object. Post with id defined by PostID is retrieved at first request.
    /// </summary>
    public ForumPostInfo PostInfo
    {
        get
        {
            var postInfo = PostInfoInternal;
            if ((postInfo == null) && (PostID > 0))
            {
                postInfo = ForumPostInfoProvider.GetForumPostInfo(PostID);
                if (postInfo != null)
                {
                    mPostID = postInfo.PostId;
                }
            }

            PostInfoInternal = postInfo;
            return postInfo;
        }
        set
        {
            PostInfoInternal = value;
            if (PostInfoInternal != null)
            {
                mPostID = PostInfoInternal.PostId;
            }
        }
    }


    /// <summary>
    /// Gets or sets post data object.
    /// </summary>
    public IDataContainer PostData
    {
        get;
        set;
    }


    public bool DisplayOnly
    {
        get;
        set;
    }

    #endregion


    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ReloadData();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        ForumPostInfo fpi = null;
        ForumInfo fi;


        #region "Load data"

        if (PostData != null)
        {
            fpi = ForumPostInfo.New(PostData);
        }

        if (fpi == null)
        {
            fpi = PostInfo;
        }

        if (fpi != null)
        {
            PostData = fpi;
            fi = ForumInfoProvider.GetForumInfo(fpi.PostForumID);
        }
        else
        {
            return;
        }

        #endregion


        if (fi.ForumEnableAdvancedImage)
        {
            ltlText.AllowedControls = ControlsHelper.ALLOWED_FORUM_CONTROLS;
        }
        else
        {
            ltlText.AllowedControls = "none";
        }

        lnkUserName.Text = HTMLHelper.HTMLEncode(fpi.PostUserName);

        // Display converted datetime for live site
        lblDate.Text = " (" + TimeZoneUIMethods.ConvertDateTime(ValidationHelper.GetDateTime(fpi.PostTime, DateTimeHelper.ZERO_TIME), this).ToString() + ")";

        lblSubject.Text = HTMLHelper.HTMLEncode(fpi.PostSubject);
        DiscussionMacroResolver dmh = new DiscussionMacroResolver();
        dmh.EnableBold = fi.ForumEnableFontBold;
        dmh.EnableItalics = fi.ForumEnableFontItalics;
        dmh.EnableStrikeThrough = fi.ForumEnableFontStrike;
        dmh.EnableUnderline = fi.ForumEnableFontUnderline;
        dmh.EnableCode = fi.ForumEnableCodeSnippet;
        dmh.EnableColor = fi.ForumEnableFontColor;
        dmh.EnableImage = fi.ForumEnableImage || fi.ForumEnableAdvancedImage;
        dmh.EnableQuote = fi.ForumEnableQuote;
        dmh.EnableURL = fi.ForumEnableURL || fi.ForumEnableAdvancedURL;
        dmh.MaxImageSideSize = fi.ForumImageMaxSideSize;
        dmh.QuotePostText = GetString("DiscussionMacroResolver.QuotePostText");

        if (fi.ForumHTMLEditor)
        {
            dmh.EncodeText = false;
            dmh.ConvertLineBreaksToHTML = false;
        }
        else
        {
            dmh.EncodeText = true;
            dmh.ConvertLineBreaksToHTML = true;
        }

        ltlText.Text = "<div class=\"PostText\">" + dmh.ResolveMacros(fpi.PostText) + "</div>";

        userAvatar.Text = AvatarImage(fpi);

        if (DisplayBadgeInfo)
        {
            // Set public badge if no badge is set
            if (String.IsNullOrEmpty(ltlBadge.Text))
            {
                ltlBadge.Text = "<div class=\"Badge\">" + GetString("Forums.PublicBadge") + "</div>";
            }
        }

        if (EnableSignature)
        {
            if (fpi.PostUserSignature.Trim() != "")
            {
                plcSignature.Visible = true;
                ltrSignature.Text = HTMLHelper.HTMLEncode(fpi.PostUserSignature);
            }
        }

        if (!DisplayOnly)
        {
            string threadId = ForumPostInfoProvider.GetPostRootFromIDPath(ValidationHelper.GetString(GetData(PostData, "PostIdPath"), "")).ToString();

            // Reply
            if (IsAvailable(PostData, ForumActionType.Reply))
            {
                lnkReply.Visible = true;
                lnkReply.Text = GetString("Forums_WebInterface_ForumPost.replyLinkText");
                lnkReply.NavigateUrl = URLHelper.UpdateParameterInUrl(GetURL(PostData, ForumActionType.Reply), "threadid", threadId);
            }
            else
            {
                lnkReply.Visible = false;
            }

            // Quote
            if (IsAvailable(PostData, ForumActionType.Quote))
            {
                lnkQuote.Visible = true;
                lnkQuote.Text = GetString("Forums_WebInterface_ForumPost.quoteLinkText");
                lnkQuote.NavigateUrl = URLHelper.UpdateParameterInUrl(GetURL(PostData, ForumActionType.Quote), "threadid", threadId);
            }
            else
            {
                lnkQuote.Visible = false;
            }

            // Display subscribe link
            if (IsAvailable(PostData, ForumActionType.SubscribeToPost))
            {
                lnkSubscribe.Visible = true;
                lnkSubscribe.Text = GetString("Forums_WebInterface_ForumPost.Subscribe");
                lnkSubscribe.NavigateUrl = URLHelper.UpdateParameterInUrl(GetURL(PostData, ForumActionType.SubscribeToPost), "threadid", threadId);
            }
            else
            {
                lnkSubscribe.Visible = false;
            }

            lnkUserName.CssClass = "PostUserLink";

            if (!String.IsNullOrEmpty(fpi.PostUserMail) && (fi.ForumDisplayEmails))
            {
                lnkUserName.NavigateUrl = "mailto:" + HTMLHelper.HTMLEncode(fpi.PostUserMail) + "?subject=" + HTMLHelper.HTMLEncode(fpi.PostSubject);
                lnkUserName.CssClass = "PostUser";
            }
        }

        // Display action panel only if reply to link or subscription link are visible
        plcActions.Visible = ((lnkReply.Visible || lnkSubscribe.Visible || lnkQuote.Visible) && !DisplayOnly);

        if ((lnkReply.Visible) && (lnkQuote.Visible || lnkSubscribe.Visible))
        {
            lblActionSeparator.Visible = true;
        }

        if ((lnkQuote.Visible) && (lnkSubscribe.Visible))
        {
            lblActionSeparator2.Visible = true;
        }
    }
}