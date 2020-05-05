using System;
using System.Web.UI;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_SubscriptionApproval : CMSUserControl
{
    #region "Private variables"

    private string mSubscriptionHash;
    private string mRequestTime;
    private BoardSubscriptionInfo mSubscriptionObject;
    private BoardInfo mSubscriptionSubject;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets successful approval text.
    /// </summary>
    public string SuccessfulConfirmationText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets unsuccessful approval text.
    /// </summary>
    public string UnsuccessfulConfirmationText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation info label text.
    /// </summary>
    public string ConfirmationInfoText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button text.
    /// </summary>
    public string ConfirmationButtonText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation label CSS class.
    /// </summary>
    public string ConfirmationTextCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button CSS class.
    /// </summary>
    public string ConfirmationButtonCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Get message board subscription object
    /// </summary>
    private BoardSubscriptionInfo SubscriptionObject
    {
        get
        {
            return mSubscriptionObject ?? (mSubscriptionObject = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(mSubscriptionHash));
        }
    }


    /// <summary>
    /// Get subject of subscription
    /// </summary>
    public BoardInfo SubscriptionSubject
    {
        get
        {
            if ((mSubscriptionSubject == null) && (SubscriptionObject != null))
            {
                mSubscriptionSubject = BoardInfoProvider.GetBoardInfo(SubscriptionObject.SubscriptionBoardID);
            }

            return mSubscriptionSubject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // If StopProcessing flag is set, do nothing
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        // Validate hash
        if (!QueryHelper.ValidateHash("hash", "aliaspath", new HashSettings("")))
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
        }

        // Get data from query string
        mSubscriptionHash = QueryHelper.GetString("boardsubscriptionhash", string.Empty);
        mRequestTime = QueryHelper.GetString("datetime", string.Empty);
            
        bool controlPb = false;

        if (RequestHelper.IsPostBack())
        {
            Control pbCtrl = ControlsHelper.GetPostBackControl(Page);
            if (pbCtrl == btnConfirm)
            {
                controlPb = true;
            }
        }

        // Setup controls
        SetupControls(!controlPb);

        if (!controlPb)
        {
            CheckAndSubscribe(mSubscriptionHash, mRequestTime, true);
        }
    }


    /// <summary>
    /// Button confirmation click event
    /// </summary>
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        CheckAndSubscribe(mSubscriptionHash, mRequestTime, false);
    }


    /// <summary>
    /// Initialize controls properties
    /// </summary>
    private void SetupControls(bool forceReload)
    {
        lblInfo.CssClass = DataHelper.GetNotEmpty(ConfirmationTextCssClass, "InfoLabel");
        btnConfirm.CssClass = ConfirmationButtonCssClass;

        if (forceReload)
        {
            btnConfirm.Text = DataHelper.GetNotEmpty(ConfirmationButtonText, GetString("general.subscription_confirmbutton"));
            lblInfo.Text = DataHelper.GetNotEmpty(ConfirmationInfoText, GetString("general.subscription_confirmtext"));
        }
    }


    /// <summary>
    /// Check that subscription hash is valid and subscription didn't expire
    /// </summary>
    /// <param name="subscriptionHash">Subscription hash to check</param>
    /// <param name="requestTime">Date time of subscription request</param>
    /// <param name="checkOnly">Indicates if only check will be performed</param>
    private void CheckAndSubscribe(string subscriptionHash, string requestTime, bool checkOnly)
    {
        // Get date and time
        DateTime datetime = DateTimeHelper.ZERO_TIME;

        // Get date and time
        if (!string.IsNullOrEmpty(requestTime))
        {
            try
            {
                datetime = DateTimeUrlFormatter.Parse(requestTime);
            }
            catch
            {
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_failed")));
                return;
            }
        }

        // Initialize opt-in result
        OptInApprovalResultEnum result = OptInApprovalResultEnum.NotFound;

        // Check only data consistency
        if (checkOnly)
        {
            if (SubscriptionObject != null)
            {
                // Validate hash 
                result = BoardSubscriptionInfoProvider.ValidateHash(SubscriptionObject, subscriptionHash, SiteContext.CurrentSiteName, datetime);
                if ((result == OptInApprovalResultEnum.Success) && (SubscriptionObject.SubscriptionApproved))
                {
                    result = OptInApprovalResultEnum.NotFound;
                }
            }
        }
        else
        {
            // Try to approve subscription
            result = BoardSubscriptionInfoProvider.ApproveSubscription(SubscriptionObject, subscriptionHash, false, SiteContext.CurrentSiteName, datetime);
        }

        // Process result
        switch (result)
        {
            // Approving subscription was successful
            case OptInApprovalResultEnum.Success:
                if (!checkOnly)
                {
                    ShowInfo(DataHelper.GetNotEmpty(SuccessfulConfirmationText, GetString("general.subscription_approval")));
                    Service.Resolve<ICurrentContactMergeService>().UpdateCurrentContactEmail(SubscriptionObject.SubscriptionEmail, MembershipContext.AuthenticatedUser);
                }
                break;

            // Subscription failed due to parameters
            case OptInApprovalResultEnum.Failed:
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_failed")));
                break;

            case OptInApprovalResultEnum.TimeExceeded:
                BoardSubscriptionInfoProvider.DeleteBoardSubscriptionInfo(SubscriptionObject);
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_timeexceeded")));
                break;

            // Subscription not found
            default:
                DisplayError(DataHelper.GetNotEmpty(UnsuccessfulConfirmationText, GetString("general.subscription_invalid")));
                break;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Display error message
    /// </summary>
    /// <param name="errorText">Error text to display</param>
    private void DisplayError(string errorText)
    {
        lblInfo.CssClass = "ErrorLabel";
        lblInfo.Text = errorText;
        btnConfirm.Visible = false;
    }


    /// <summary>
    /// Display information message
    /// </summary>
    /// <param name="infoText">Information to display</param>
    private void ShowInfo(string infoText)
    {
        lblInfo.CssClass = "InfoLabel";
        lblInfo.Text = infoText;
        btnConfirm.Visible = false;
    }

    #endregion
}