using System;
using System.Data;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_MySubscriptions : CMSAdminControl
{
    #region "Variables"

    private SubscriberInfo subscriber;
    private string selectorValue = string.Empty;
    private string currentValues = string.Empty;
    private bool mSendConfirmationEmail = true;
    private UserInfo userInfo;
    private int siteID;

    private readonly ISubscriptionService mSubscriptionService = Service.Resolve<ISubscriptionService>();
    private readonly IUnsubscriptionProvider mUnsubscriptionProvider = Service.Resolve<IUnsubscriptionProvider>();
    private readonly IContactProvider mContactProvider = Service.Resolve<IContactProvider>();

    #endregion


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
    /// Gets or sets the value that indicates whether send confirmation emails.
    /// </summary>
    public bool SendConfirmationEmail
    {
        get
        {
            return mSendConfirmationEmail;
        }
        set
        {
            mSendConfirmationEmail = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this control is visible.
    /// </summary>
    public bool ForcedVisible
    {
        get
        {
            return plcMain.Visible;
        }
        set
        {
            plcMain.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this control is used in other control.
    /// </summary>
    public bool ExternalUse
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the WebPart cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (siteID <= 0)
            {
                siteID = SiteContext.CurrentSiteID;
            }

            return siteID;
        }
        set
        {
            siteID = value;
        }
    }


    /// <summary>
    /// Gets or sets current user ID.
    /// </summary>
    public int UserID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if selector control is on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return false;
        }
        set
        {
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Last selector value.
    /// </summary>
    private string SelectorValue
    {
        get
        {
            if (string.IsNullOrEmpty(selectorValue))
            {
                // Try to get value from hidden field
                selectorValue = ValidationHelper.GetString(hdnValue.Value, string.Empty);
            }

            return selectorValue;
        }
        set
        {
            selectorValue = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load data.
    /// </summary>
    public void LoadData()
    {
        if (StopProcessing)
        {
            // Hide control
            Visible = false;
        }
        else
        {
            SetContext();

            InitializeUser();

            usNewsletters.WhereCondition = new WhereCondition().WhereEquals("NewsletterSiteID", SiteID).WhereEquals("NewsletterType", (int)EmailCommunicationTypeEnum.Newsletter).ToString(true);

            if (IsUserIdentified())
            {
                usNewsletters.Visible = true;

                InitializeSubscriber(userInfo, SiteID);

                LoadNewslettersForUser(userInfo);
            }
            else
            {
                usNewsletters.Visible = false;

                if ((UserID > 0) && (MembershipContext.AuthenticatedUser.UserID == UserID))
                {
                    ShowInformation(GetString("MySubscriptions.CannotIdentify"));
                }
                else
                {
                    if (!IsLiveSite)
                    {
                        // It's located in Admin/Users/Subscriptions
                        lblText.ResourceString = "MySubscriptions.EmailCommunicationDisabled";
                    }
                    else
                    {
                        ShowInformation(GetString("MySubscriptions.CannotIdentifyUser"));
                    }
                }
            }

            ReleaseContext();
        }
    }


    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerInvariant())
        {
            case "forcedvisible":
                ForcedVisible = ValidationHelper.GetBoolean(value, false);
                break;

            case "externaluse":
                ExternalUse = ValidationHelper.GetBoolean(value, false);
                break;

            case "cacheminutes":
                CacheMinutes = ValidationHelper.GetInteger(value, 0);
                break;

            case "reloaddata":
                // Special property which enables to call LoadData from MyAccount webpart
                LoadData();
                break;

            case "userid":
                UserID = ValidationHelper.GetInteger(value, 0);
                break;

            case "siteid":
                SiteID = ValidationHelper.GetInteger(value, 0);
                break;

            case "sendconfirmationemail":
                mSendConfirmationEmail = ValidationHelper.GetBoolean(value, true);
                break;
        }

        return true;
    }


    /// <summary>
    /// PageLoad.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        usNewsletters.OnSelectionChanged += usNewsletters_OnSelectionChanged;
        usNewsletters.IsLiveSite = IsLiveSite;

        if (ExternalUse)
        {
            LoadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!IsLiveSite)
        {
            // It's located in Admin/Users/Subscriptions
            headNewsletters.Visible = false;
            lblUnsubscribeFromAll.Visible = false;
            btnUsubscribeFromAll.Visible = false;
        }

        if (IsUserIdentified())
        {
            if (!IsLiveSite)
            {
                lblText.ResourceString = "mysubscriptions.selectorheading.thirdperson";
            }
            else
            {
                string email = GetVisitorEmail();

                bool isUnsubscribedFromAll = mUnsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email);

                if (isUnsubscribedFromAll)
                {
                    lblUnsubscribeFromAll.Text = GetString("mysubscriptions.unsubscribed.description");
                    btnUsubscribeFromAll.Text = GetString("mysubscriptions.unsubscribed.buttontext");
                }
                else
                {
                    lblUnsubscribeFromAll.Text = string.Format(GetString("mysubscriptions.notunsubscribed.description"), email);
                    btnUsubscribeFromAll.Text = GetString("mysubscriptions.notunsubscribed.buttontext");
                }
            }
        }

        // Preserve selected values
        hdnValue.Value = ValidationHelper.GetString(usNewsletters.Value, string.Empty);
    }


    protected void btnUnsubscribeFromAll_Click(object sender, EventArgs e)
    {
        if (IsUserIdentified() && IsLiveSite)
        {
            string email = GetVisitorEmail();

            var isUnsubscribed = mUnsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email);

            if (!isUnsubscribed)
            {
                mSubscriptionService.UnsubscribeFromAllNewsletters(email);
            }
            else
            {
                mUnsubscriptionProvider.RemoveUnsubscriptionsFromAllNewsletters(email);
            }
        }
    }


    private void usNewsletters_OnSelectionChanged(object sender, EventArgs e)
    {
        if (RaiseOnCheckPermissions("ManageSubscribers", this))
        {
            if (StopProcessing)
            {
                return;
            }
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (subscriber != null)
        {
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                var newsletterIds = newItems.Select(item => ValidationHelper.GetInteger(item, 0)).ToArray();

                foreach (var newsletterId in newsletterIds)
                {
                    mSubscriptionService.RemoveSubscription(subscriber.SubscriberID, newsletterId, SendConfirmationEmail);
                    LogUnsubscriptionActivity(newsletterId);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in newItems)
            {
                var newsletterId = ValidationHelper.GetInteger(item, 0);

                try
                {
                    var contact = mContactProvider.GetContactForSubscribing(userInfo);
                    var newsletter = NewsletterInfo.Provider.Get(newsletterId);
                    mSubscriptionService.Subscribe(contact, newsletter, new SubscribeSettings()
                    {
                        SendConfirmationEmail = SendConfirmationEmail,
                        AllowOptIn = true,
                        RemoveUnsubscriptionFromNewsletter = true,
                        RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    });
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException(ex.Source, "SUBSCRIBE", ex, SiteID);
                    // Can occur e.g. when newsletter is deleted while the user is selecting it for subscription.
                    // This is rare scenario, the main purpose of this catch is to avoid YSOD on the live site.
                }
            }
        }

        // Display information about successful (un)subscription
        ShowChangesSaved();
    }


    private void LoadNewslettersForUser(UserInfo user)
    {
        var subscriptionService = Service.Resolve<ISubscriptionService>();
        var contact = mContactProvider.GetContactForSubscribing(user);

        // Get selected newsletters
        DataSet ds = subscriptionService.GetAllActiveSubscriptions(contact.ContactID).Column("NewsletterID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "NewsletterID"));
        }

        // Load selected newsletters
        if (!RequestHelper.IsPostBack() || !string.IsNullOrEmpty(DataHelper.GetNewItemsInList(SelectorValue, currentValues)))
        {
            usNewsletters.Value = currentValues;
        }
    }


    private void InitializeUser()
    {
        // Get specified user if used instead of current user
        if (UserID > 0)
        {
            userInfo = UserInfo.Provider.Get(UserID);
        }
        else
        {
            userInfo = MembershipContext.AuthenticatedUser;
        }
    }


    /// <summary>
    /// Logs unsubscription activity if newsletter exists
    /// </summary>
    private void LogUnsubscriptionActivity(int newsletterId)
    {
        var newsletter = NewsletterInfo.Provider.Get(newsletterId);
        if (newsletter == null)
        {
            return;
        }

        var activityLogger = Service.Resolve<INewslettersActivityLogger>();
        activityLogger.LogUnsubscribeFromSingleNewsletterActivity(newsletter);
    }


    private void InitializeSubscriber(UserInfo user, int siteId)
    {
        var contact = mContactProvider.GetContactForSubscribing(user);
        subscriber = SubscriberInfoProvider.GetSubscriberInfo(ContactInfo.OBJECT_TYPE, contact.ContactID, siteId);
    }


    private bool IsUserIdentified()
    {
        return (userInfo != null) && !userInfo.IsPublic() && ValidationHelper.IsEmail(userInfo.Email);
    }


    private string GetVisitorEmail()
    {
        return mContactProvider.GetContactForSubscribing(userInfo).ContactEmail;
    }

    #endregion
}