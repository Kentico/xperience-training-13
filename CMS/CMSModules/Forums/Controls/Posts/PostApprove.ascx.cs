using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Posts_PostApprove : CMSAdminEditControl
{
    #region "Variables"

    // Current PostID
    private int mPostID = 0;
    // Viewer mode
    private string mMode = "approval";

    // User ID
    private int mUserID = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the post ID.
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
        }
    }


    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserID;
        }
        set
        {
            mUserID = value;
        }
    }


    /// <summary>
    /// Gets or sets mode of control
    /// </summary>
    public string Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Mode == "approval") || ((Mode == "subscription") && (CheckSubscription())))
        {
            // Load the data
            LoadData();
        }
        else
        {
            ShowError(GetString("editedobject.notexists"));
            pnlContent.Visible = false;
        }
    }


    /// <summary>
    /// Checks if current user has subscribed given post.
    /// </summary>
    /// <param name="postId">ID of forum post</param>
    /// <returns>true if user has subscriptions for given post, false if not</returns>
    private bool CheckSubscription()
    {
        if (UserID != MembershipContext.AuthenticatedUser.UserID)
        {
            // Check permissions
            if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
            {
                return false;
            }
        }
        var query = new ObjectQuery<ForumSubscriptionInfo>().Where(new WhereCondition().WhereNull("SubscriptionApproved").Or().WhereEquals("SubscriptionApproved", 1)).And().WhereEquals("SubscriptionUserID", UserID).And().WhereEquals("SubscriptionPostID", PostID).Column("SubscriptionID").TopN(1);

        return query.Any();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads the data.
    /// </summary>
    private void LoadData()
    {
        // Get the forum post
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(ValidationHelper.GetInteger(PostID, 0));
        ForumInfo fi = null;

        if (fpi != null)
        {
            fi = ForumInfoProvider.GetForumInfo(fpi.PostForumID);
        }
        else
        {
            return;
        }

        if (fi.ForumEnableAdvancedImage)
        {
            ltrText.AllowedControls = ControlsHelper.ALLOWED_FORUM_CONTROLS;
        }
        else
        {
            ltrText.AllowedControls = "none";
        }

        // Display converted datetime for live site
        lblDate.Text = TimeZoneUIMethods.ConvertDateTime(ValidationHelper.GetDateTime(fpi.PostTime, DateTimeHelper.ZERO_TIME), this).ToString();

        lblUser.Text = HTMLHelper.HTMLEncode(fpi.PostUserName);
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

        // Resolve the macros and display the post text
        ltrText.Text = dmh.ResolveMacros(fpi.PostText);
    }

    #endregion
}