using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_Newsletters_CustomSubscriptionForm : CMSAbstractWebPart
{
    #region "Variables"

    private bool mChooseMode;
    private readonly ISubscriptionService mSubscriptionService = Service.Resolve<ISubscriptionService>();
    private readonly IContactProvider mContactProvider = Service.Resolve<IContactProvider>();
    private bool dataFormSaved;
    private SubscriberInfo subscriber;
    private bool onBeforeSaveCalled;

    #endregion


    #region "Layout properties"

    /// <summary>
    /// Full alternative form name ('classname.formname') for newsletter subscriber.
    /// Default value is newsletter.subscriber.SubscriptionForm
    /// </summary>
    public string AlternativeForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeForm"), "newsletter.subscriber.SubscriptionForm");
        }
        set
        {
            SetValue("AlternativeForm", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether the CAPTCHA image should be displayed.
    /// </summary>
    public bool DisplayCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCaptcha"), false);
        }
        set
        {
            SetValue("DisplayCaptcha", value);
        }
    }


    /// <summary>
    /// Gets or sets the CAPTCHA label text.
    /// </summary>
    public string CaptchaText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CaptchaText"), "Webparts_Membership_RegistrationForm.Captcha");
        }
        set
        {
            SetValue("CaptchaText", value);
        }
    }


    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), ResHelper.LocalizeString("{$NewsletterSubscription.Submit$}"));
        }
        set
        {
            SetValue("ButtonText", value);
            btnSubmit.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets value which indicates if image button should be used instead of regular button.
    /// </summary>
    public bool UseImageButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseImageButton"), false);
        }
        set
        {
            SetValue("UseImageButton", value);
        }
    }


    /// <summary>
    /// Gets or sets image button URL.
    /// </summary>
    public string ImageButtonURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageButtonURL"), string.Empty);
        }
        set
        {
            SetValue("ImageButtonURL", value);
            btnImageSubmit.ImageUrl = value;
        }
    }


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

    #endregion


    #region "Other properties"

    /// <summary>
    /// Gets or sets value which indicates if authenticated users can subscribe to newsletter.
    /// </summary>
    public bool AllowUserSubscribers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowUserSubscribers"), false);
        }
        set
        {
            SetValue("AllowUserSubscribers", value);
        }
    }


    /// <summary>
    /// Gets or sets the newsletter code name.
    /// </summary>
    public string NewsletterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewsletterName"), string.Empty);
        }
        set
        {
            SetValue("NewsletterName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful subscription.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), string.Empty);
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful subscription.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether confirmation email will be sent.
    /// </summary>
    public bool SendConfirmationEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendConfirmationEmail"), true);
        }
        set
        {
            SetValue("SendConfirmationEmail", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after successful subscription.
    /// </summary>
    public string SubscriptionConfirmationMessage
    {
        get
        {
            string msg = ValidationHelper.GetString(GetValue("SubscriptionConfirmationMessage"), string.Empty);
            if (string.IsNullOrEmpty(msg))
            {
                msg = "NewsletterSubscription.Subscribed";
            }
            return GetString(msg);
        }
        set
        {
            SetValue("SubscriptionConfirmationMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after subscription failed.
    /// </summary>
    public string SubscriptionErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SubscriptionErrorMessage"), string.Empty);
        }
        set
        {
            SetValue("SubscriptionErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed if subscriber is already subscribed.
    /// </summary>
    public string MessageForAlreadySubscribed
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageForAlreadySubscribed"), string.Empty);
        }
        set
        {
            SetValue("MessageForAlreadySubscribed", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether this web part is used in other web part or user control.
    /// </summary>
    public bool ExternalUse
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether <see cref="AllowUserSubscribers"/> is required and current user is authenticated.
    /// </summary>
    private bool AllowUserSubscribersIsAuthenticated
    {
        get
        {
            return AllowUserSubscribers && AuthenticationHelper.IsAuthenticated();
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
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Page pre-render event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide default form submit button
        if (formElem != null)
        {
            formElem.SubmitButton.Visible = false;
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            formElem.StopProcessing = StopProcessing;
            Visible = false;
        }
        else
        {
            if (AllowUserSubscribersIsAuthenticated && (!string.IsNullOrEmpty(CurrentUser.Email)))
            {
                // Hide form for authenticated user who has an email
                formElem.StopProcessing = true;
                formElem.Visible = false;
            }
            else
            {
                // Get alternative form info
                AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeForm);
                if (afi != null)
                {
                    subscriber = new SubscriberInfo();

                    // Init the form
                    formElem.Data = subscriber;
                    formElem.FormInformation = FormHelper.GetFormInfo(AlternativeForm, true);
                    formElem.AltFormInformation = afi;
                    formElem.MessagesPlaceHolder = plcMess;
                    formElem.Visible = true;
                    formElem.ValidationErrorMessage = SubscriptionErrorMessage;
                    formElem.IsLiveSite = true;

                    // Reload form if not in PortalEngine environment
                    if (StandAlone)
                    {
                        formElem.ReloadData();
                    }
                }
                else
                {
                    lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeForm);
                    lblError.Visible = true;
                    pnlSubscription.Visible = false;
                }
            }

            // Init submit buttons
            if ((UseImageButton) && (!String.IsNullOrEmpty(ImageButtonURL)))
            {
                pnlButtonSubmit.Visible = false;
                pnlImageSubmit.Visible = true;
                btnImageSubmit.ImageUrl = ImageButtonURL;
            }
            else
            {
                pnlButtonSubmit.Visible = true;
                pnlImageSubmit.Visible = false;
                btnSubmit.Text = ButtonText;
            }

            lblInfo.CssClass = "EditingFormInfoLabel";
            lblError.CssClass = "EditingFormErrorLabel";

            if (formElem != null)
            {
                // Set the live site context
                formElem.ControlContext.ContextName = CMS.Base.Web.UI.ControlContext.LIVE_SITE;
            }

            InitNewsletterSelector();
        }
    }


    protected void InitNewsletterSelector()
    {
        // Show/hide newsletter list
        plcNwsList.Visible = NewsletterName.EqualsCSafe("nwsletuserchoose", true);

        if (!plcNwsList.Visible)
        {
            return;
        }
        mChooseMode = true;

        if ((!ExternalUse || !RequestHelper.IsPostBack()) && (chklNewsletters.Items.Count == 0))
        {
            DataSet ds = null;

            // Try to get data from cache
            using (var cs = new CachedSection<DataSet>(ref ds, CacheMinutes, true, CacheItemName, "newslettersubscription", SiteContext.CurrentSiteName))
            {
                if (cs.LoadData)
                {
                    // Get the data
                    ds = NewsletterInfoProvider.GetNewslettersForSite(SiteContext.CurrentSiteID).WhereEquals("NewsletterType", (int)EmailCommunicationTypeEnum.Newsletter).OrderBy("NewsletterDisplayName").Columns("NewsletterDisplayName", "NewsletterName");

                    // Add data to the cache
                    if (cs.Cached)
                    {
                        // Prepare cache dependency
                        cs.CacheDependency = CacheHelper.GetCacheDependency("newsletter.newsletter|all");
                    }

                    cs.Data = ds;
                }
            }

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Fill checkbox list with newsletters
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var displayName = ValidationHelper.GetString(dr["NewsletterDisplayName"], string.Empty);

                    var li = new ListItem(HTMLHelper.HTMLEncode(displayName), ValidationHelper.GetString(dr["NewsletterName"], string.Empty));
                    chklNewsletters.Items.Add(li);
                }
            }
        }
    }


    /// <summary>
    /// Submit button handler.
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (PortalContext.IsDesignMode(PageManager.ViewMode) || (HideOnCurrentPage) || (!IsVisible))
        {
            // Do not process
            return;
        }

        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        if (!IsValid())
        {
            return;
        }

        ContactInfo contact = SaveContact();

        if (mChooseMode)
        {
            ValidChoose(contact);
        }
        else
        {
            // Hide subscription form if subscription was successful
            pnlSubscription.Visible = !Save(NewsletterName, contact);
        }
    }


    /// <summary>
    /// Saves contact.
    /// </summary>
    private ContactInfo SaveContact()
    {
        ContactInfo contact = null;

        // Handle authenticated user when user subscription is allowed
        if (AllowUserSubscribersIsAuthenticated)
        {
            CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;
            contact = mContactProvider.GetContactForSubscribing(currentUser);
        }
        // Work with non-authenticated user or authenticated user when user subscription is disabled
        else if (SaveDataForm())
        {
            if (String.IsNullOrWhiteSpace(subscriber.SubscriberEmail))
            {
                lblError.Visible = true;
                lblError.Text = GetString("NewsletterSubscription.ErrorInvalidEmail");
                return null;
            }

            var firstName = TextHelper.LimitLength(subscriber.SubscriberFirstName, 100);
            var lastName = TextHelper.LimitLength(subscriber.SubscriberLastName, 100);

            contact = mContactProvider.GetContactForSubscribing(subscriber.SubscriberEmail, firstName, lastName);
        }

        return contact;
    }


    /// <summary>
    /// Indicates whether the basic properties contain a valid data.
    /// </summary>
    /// <returns>Returns true if the basic data are valid; otherwise, false</returns>
    private bool IsValid()
    {
        string errorText = null;
        bool result = true;

        if (mChooseMode)
        {
            if (chklNewsletters.SelectedIndex < 0)
            {
                errorText += GetString("NewsletterSubscription.NoneSelected") + "<br />";
                result = false;
            }
        }

        if (AllowUserSubscribersIsAuthenticated && (String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.Email)))
        {
            errorText += GetString("newslettersubscription.erroremptyemail") + "<br />";
            result = false;
        }

        // Assign validation result text.
        if (!string.IsNullOrEmpty(errorText))
        {
            lblError.Visible = true;
            lblError.Text = errorText;
        }

        return result;
    }


    /// <summary>
    /// Valid checkbox list, Indicates whether the subscriber is already subscribed to the selected newsletter.
    /// </summary>
    private void ValidChoose(ContactInfo contact)
    {
        if (contact == null)
        {
            return;
        }

        bool wasWrong = true;

        // Save selected items
        for (int i = 0; i < chklNewsletters.Items.Count; i++)
        {
            ListItem item = chklNewsletters.Items[i];
            if (item != null && item.Selected)
            {
                wasWrong = wasWrong & (!Save(item.Value, contact));
            }
        }

        // Check subscription
        if ((chklNewsletters.Items.Count > 0) && (!wasWrong))
        {
            lblInfo.Visible = true;
            lblInfo.Text += SubscriptionConfirmationMessage;

            // Hide subscription form after successful subscription
            pnlSubscription.Visible = false;
        }
    }


    /// <summary>
    /// Saves the data.
    /// </summary>
    private bool Save(string newsletterName, ContactInfo contact)
    {
        bool toReturn = false;

        if ((contact == null) || string.IsNullOrEmpty(newsletterName))
        {
            return false;
        }

        // Get newsletter info
        var newsletter = NewsletterInfo.Provider.Get(newsletterName, SiteContext.CurrentSiteID);
        if (newsletter != null)
        {
            try
            {
                // Check if subscriber is not marketable
                if (!mSubscriptionService.IsMarketable(contact, newsletter))
                {
                    toReturn = true;

                    if (!AllowUserSubscribersIsAuthenticated || String.IsNullOrEmpty(CurrentUser.Email))
                    {
                        if (!SaveDataForm())
                        {
                            return false;
                        }
                    }

                    mSubscriptionService.Subscribe(contact, newsletter, new SubscribeSettings()
                    {
                        SendConfirmationEmail = SendConfirmationEmail,
                        AllowOptIn = true,
                        RemoveUnsubscriptionFromNewsletter = true,
                        RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                        SourceSubscriber = subscriber
                    });

                    if (!mChooseMode)
                    {
                        // Display message about successful subscription
                        lblInfo.Visible = true;
                        lblInfo.Text = SubscriptionConfirmationMessage;
                    }
                }
                else
                {

                    // Init web part resolver
                    object[] data = new object[3];
                    data[0] = newsletter;
                    data[1] = subscriber;

                    MacroResolver resolver = ContextResolver;
                    resolver.SetNamedSourceData("Newsletter", newsletter);
                    resolver.SetNamedSourceData("Subscriber", subscriber);
                    if (AllowUserSubscribersIsAuthenticated)
                    {
                        data[2] = CurrentUser;
                        resolver.SetNamedSourceData("User", CurrentUser);
                    }
                    resolver.SetAnonymousSourceData(data);

                    lblInfo.Visible = true;
                    string message = null;

                    if (string.IsNullOrEmpty(MessageForAlreadySubscribed))
                    {
                        if (!mChooseMode)
                        {
                            message = GetString("NewsletterSubscription.SubscriberIsAlreadySubscribed");
                        }
                        else
                        {
                            message = string.Format("{0} {1}.<br />", GetString("NewsletterSubscription.SubscriberIsAlreadySubscribedXY"), HTMLHelper.HTMLEncode(newsletter.NewsletterDisplayName));
                        }
                    }
                    else
                    {
                        message = MessageForAlreadySubscribed;
                    }

                    // Info message - subscriber is already in site
                    if (!mChooseMode)
                    {
                        lblInfo.Text = message;
                    }
                    else
                    {
                        lblInfo.Text += message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }
        }
        else
        {
            lblError.Visible = true;
            lblError.Text = GetString("NewsletterSubscription.NewsletterDoesNotExist");
        }

        return toReturn;
    }


    /// <summary>
    /// Saves data from <see cref="formElem"/> to the <see cref="subscriber"/> field.
    /// </summary>
    private bool SaveDataForm()
    {
        if (!formElem.Visible)
        {
            return false;
        }

        if (!dataFormSaved)
        {
            dataFormSaved = true;
            try
            {
                formElem.OnBeforeSave += FormElem_OnBeforeSave;
                if (!formElem.SaveData(null, false) && !onBeforeSaveCalled)
                {
                    return false;
                }
            }
            finally
            {
                formElem.OnBeforeSave += FormElem_OnBeforeSave;
            }
        }

        return true;
    }


    private void FormElem_OnBeforeSave(object sender, EventArgs e)
    {
        var form = sender as BasicForm;
        if (form != null)
        {
            foreach (string columnName in form.Data.ColumnNames)
            {
                subscriber.SetValue(columnName, form.Data.GetValue(columnName));
            }
        }

        onBeforeSaveCalled = true;
    }


    /// <summary>
    /// Clears the cached items.
    /// </summary>
    public override void ClearCache()
    {
        string useCacheItemName = DataHelper.GetNotEmpty(CacheItemName, CacheHelper.BaseCacheKey + "|" + RequestContext.CurrentURL + "|" + ClientID);

        CacheHelper.ClearCache(useCacheItemName);
    }

    #endregion
}
