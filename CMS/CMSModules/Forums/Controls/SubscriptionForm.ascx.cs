using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Protection;
using CMS.SiteProvider;


public partial class CMSModules_Forums_Controls_SubscriptionForm : ForumViewer
{
    #region "Properties"

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

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvEmailRequired.ErrorMessage = GetString("Forums_WebInterface_ForumNewPost.emailRequireErrorMsg");
        btnOk.Text = GetString("General.Ok");
        btnCancel.Text = GetString("General.Cancel");
        
        // Pre-fill user email
        if (!RequestHelper.IsPostBack())
        {
            txtSubscriptionEmail.Text = MembershipContext.AuthenticatedUser.Email;
        }

        txtSubscriptionEmail
            .EnableClientSideEmailFormatValidation("NewSubscription", "Forums_WebInterface_ForumNewPost.emailErrorMsg")
            .RegisterCustomValidator(rfvEmailRequired);
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        // Check input fields
        string result = new Validator()
            .NotEmpty(txtSubscriptionEmail.Text, rfvEmailRequired.ErrorMessage)
            .MatchesCondition(txtSubscriptionEmail, control => control.IsValid(), GetString("Forums_WebInterface_ForumNewPost.emailErrorMsg")).Result;

        if (result != String.Empty)
        {
            ShowError(result);
            return;
        }

        // For selected forum and only if subscription is enabled
        if ((ForumContext.CurrentForum != null) && ((ForumContext.CurrentState == ForumStateEnum.SubscribeToPost) || (ForumContext.CurrentState == ForumStateEnum.NewSubscription)))
        {
            // Check permissions
            if (!IsAvailable(ForumContext.CurrentForum, ForumActionType.SubscribeToForum))
            {
                ShowError(GetString("ForumNewPost.PermissionDenied"));
                return;
            }

            // Create new subscription
            ForumSubscriptionInfo fsi = new ForumSubscriptionInfo();
            fsi.SubscriptionForumID = ForumContext.CurrentForum.ForumID;
            fsi.SubscriptionEmail = HTMLHelper.HTMLEncode(txtSubscriptionEmail.Text.Trim());
            fsi.SubscriptionGUID = Guid.NewGuid();

            if (ForumContext.CurrentSubscribeThread != null)
            {
                fsi.SubscriptionPostID = ForumContext.CurrentSubscribeThread.PostId;
            }

            if (MembershipContext.AuthenticatedUser != null)
            {
                fsi.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
            }

            // Check whether user is not subscribed
            if (ForumSubscriptionInfoProvider.IsSubscribed(txtSubscriptionEmail.Text.Trim(), fsi.SubscriptionForumID, fsi.SubscriptionPostID))
            {
                ShowError(GetString("ForumSubscibe.SubscriptionExists"));
                return;
            }

            ForumSubscriptionInfoProvider.Subscribe(fsi, DateTime.Now, true, true);

            if (fsi.SubscriptionApproved)
            {
                ShowConfirmation(GetString("blog.subscription.beensubscribed"));
                LogSubscriptionActivity(fsi, ForumContext.CurrentForum);
            }
            else
            {
                string confirmation = GetString("general.subscribed.doubleoptin");
                int optInInterval = ForumGroupInfoProvider.DoubleOptInInterval(SiteContext.CurrentSiteName);
                if (optInInterval > 0)
                {
                    confirmation += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                }
                ShowConfirmation(confirmation);
            }
        }

        URLHelper.Redirect(ClearURL());
    }


    /// <summary>
    /// Cancel click handler.
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(ClearURL());
    }


    /// <summary>
    /// Logs activity.
    /// </summary>
    private void LogSubscriptionActivity(ForumSubscriptionInfo forumSubscriptionInfo, ForumInfo forumInfo)
    {
        if ((forumInfo == null) || (forumSubscriptionInfo == null) || !forumInfo.ForumLogActivity)
        {
            return;
        }
        
        Service.Resolve<ICurrentContactMergeService>().UpdateCurrentContactEmail(forumSubscriptionInfo.SubscriptionEmail, MembershipContext.AuthenticatedUser);
    }
}