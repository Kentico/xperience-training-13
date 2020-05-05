using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_MessageBoards_Controls_NewSubscription : CMSUserControl
{
    #region "Private variables"

    private BoardProperties mBoardProperties = null;
    private int mBoardID = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ForumId.
    /// </summary>
    public int BoardID
    {
        get
        {
            return mBoardID;
        }
        set
        {
            mBoardID = value;
        }
    }


    /// <summary>
    /// Properties passed from the upper control.
    /// </summary>
    public BoardProperties BoardProperties
    {
        get
        {
            return mBoardProperties;
        }
        set
        {
            mBoardProperties = value;
        }
    }


    /// <summary>
    /// Placeholder for messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string valGroup = UniqueID;

        lblEmail.ResourceString = "board.subscription.email";
        btnOk.ResourceString = "board.subscription.subscribe";
        btnOk.ValidationGroup = valGroup;

        rfvEmailRequired.ErrorMessage = GetString("board.subscription.noemail");
        rfvEmailRequired.ValidationGroup = valGroup;

        txtEmail
            .EnableClientSideEmailFormatValidation(valGroup, "board.messageedit.revemail")
            .RegisterCustomValidator(rfvEmailRequired);
    }


    /// <summary>
    /// Pre-fill user e-mail.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            if (txtEmail.Text.Trim() == String.Empty && !String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.Email))
            {
                txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
            }
        }
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
        string email = txtEmail.Text.Trim();

        string result = new Validator()
            .NotEmpty(email, rfvEmailRequired.ErrorMessage)
            .IsEmail(email, GetString("general.correctemailformat"), checkLength: true)
            .Result;

        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
            return;
        }
        
        // Try to create a new board
        BoardInfo boardInfo = null;
        if (BoardID == 0)
        {
            // Create new message board according to webpart properties
            boardInfo = new BoardInfo(BoardProperties);
            BoardInfoProvider.SetBoardInfo(boardInfo);

            // Update information on current message board
            BoardID = boardInfo.BoardID;

            // Set board-role relationship                
            BoardRoleInfoProvider.SetBoardRoles(BoardID, BoardProperties.BoardRoles);

            // Set moderators
            BoardModeratorInfoProvider.SetBoardModerators(BoardID, BoardProperties.BoardModerators);
        }

        if (BoardID > 0)
        {
            // Check for duplicit e-mails
            DataSet ds = BoardSubscriptionInfoProvider.GetSubscriptions("(SubscriptionApproved <> 0) AND (SubscriptionBoardID=" + BoardID +
                                                                        ") AND (SubscriptionEmail='" + SqlHelper.GetSafeQueryString(email, false) + "')", null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ShowError(GetString("board.subscription.emailexists"));
                return;
            }
            BoardSubscriptionInfo bsi = new BoardSubscriptionInfo();
            bsi.SubscriptionBoardID = BoardID;
            bsi.SubscriptionEmail = email;
            if ((MembershipContext.AuthenticatedUser != null) && !MembershipContext.AuthenticatedUser.IsPublic())
            {
                bsi.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
            }
            BoardSubscriptionInfoProvider.Subscribe(bsi, DateTime.Now, true, true);

            // Clear form
            txtEmail.Text = "";
            if (boardInfo == null)
            {
                boardInfo = BoardInfoProvider.GetBoardInfo(BoardID);
            }

            // If subscribed, log activity
            if (bsi.SubscriptionApproved)
            {
                ShowConfirmation(GetString("board.subscription.beensubscribed"));
                Service.Resolve<ICurrentContactMergeService>().UpdateCurrentContactEmail(bsi.SubscriptionEmail, MembershipContext.AuthenticatedUser);
            }
            else
            {
                string confirmation = GetString("general.subscribed.doubleoptin");
                int optInInterval = BoardInfoProvider.DoubleOptInInterval(SiteContext.CurrentSiteName);
                if (optInInterval > 0)
                {
                    confirmation += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                }
                ShowConfirmation(confirmation);
            }
        }
    }

    #endregion
}