using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Newsletters;
using CMS.PortalEngine.Web.UI;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_Newsletters_NewsletterSubscriptionWebPart : CMSAbstractWebPart
{
    #region "Variables"

    private bool chooseMode;
    private bool visibleFirstName = true;
    private bool visibleLastName = true;
    private bool visibleEmail = true;
    private readonly ISubscriptionService mSubscriptionService = Service.Resolve<ISubscriptionService>();
    private readonly IContactProvider mContactProvider = Service.Resolve<IContactProvider>();

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether first name will be displayed.
    /// </summary>
    public bool DisplayFirstName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstName"), false);
        }
        set
        {
            SetValue("DisplayFirstName", value);
        }
    }


    /// <summary>
    /// Gets or sets the first name text.
    /// </summary>
    public string FirstNameText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FirstNameText"), GetString("NewsletterSubscription.FirstName"));
        }
        set
        {
            SetValue("FirstNameText", value);
            lblFirstName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether last name will be displayed.
    /// </summary>
    public bool DisplayLastName
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayLastName"), false);
        }
        set
        {
            SetValue("DisplayLastName", value);
        }
    }


    /// <summary>
    /// Gets or sets the last name text.
    /// </summary>
    public string LastNameText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LastNameText"), GetString("NewsletterSubscription.LastName"));
        }
        set
        {
            SetValue("LastNameText", value);
            lblLastName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the newsletter code name.
    /// </summary>
    public string NewsletterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewsletterName"), "");
        }
        set
        {
            SetValue("NewsletterName", value);
        }
    }


    /// <summary>
    /// Gets or sets the e-mail text.
    /// </summary>
    public string EmailText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EmailText"), GetString("NewsletterSubscription.Email"));
        }
        set
        {
            SetValue("EmailText", value);
            lblEmail.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), GetString("NewsletterSubscription.Submit"));
        }
        set
        {
            SetValue("ButtonText", value);
            btnSubmit.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful subscription.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
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
    /// Gets or sets the value that indicates whether this webpart is used in other webpart or user control.
    /// </summary>
    public bool ExternalUse
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the captcha label text.
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
    /// Gets or sets value that indicates whether the captcha image should be displayed.
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
    /// Gets or sets value which indicates if image button should be used instead of regular one.
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
            return ValidationHelper.GetString(GetValue("ImageButtonURL"), "");
        }
        set
        {
            SetValue("ImageButtonURL", value);
            btnImageSubmit.ImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID of the logon form.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            lblFirstName.SkinID = value;
            lblLastName.SkinID = value;
            lblEmail.SkinID = value;
            txtFirstName.SkinID = value;
            txtLastName.SkinID = value;
            txtEmail.SkinID = value;
            btnSubmit.SkinID = value;
        }
    }

    #endregion


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


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        SetVisibility();
    }


    /// <summary>
    /// Sets visibility of controls.
    /// </summary>
    protected void SetVisibility()
    {
        plcFirstName.Visible = visibleFirstName;
        plcLastName.Visible = visibleLastName;
        plcEmail.Visible = visibleEmail;
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        lblFirstName.Text = FirstNameText;
        lblLastName.Text = LastNameText;
        lblEmail.Text = EmailText;

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

        // Display labels only if user is logged in and property AllowUserSubscribers is set to true
        if (AllowUserSubscribers && AuthenticationHelper.IsAuthenticated())
        {
            visibleFirstName = false;
            visibleLastName = false;
            visibleEmail = false;
        }
        // Otherwise display text-boxes
        else
        {
            visibleFirstName = true;
            visibleLastName = true;
            visibleEmail = true;
        }

        // Hide first name field if not required
        if (!DisplayFirstName)
        {
            visibleFirstName = false;
        }
        // Hide last name field if not required
        if (!DisplayLastName)
        {
            visibleLastName = false;
        }

        // Show/hide newsletter list
        plcNwsList.Visible = NewsletterName.EqualsCSafe("nwsletuserchoose", true);
        if (plcNwsList.Visible)
        {
            chooseMode = true;

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
                    ListItem li = null;
                    string displayName = null;

                    // Fill checkbox list with newsletters
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        displayName = ValidationHelper.GetString(dr["NewsletterDisplayName"], string.Empty);

                        li = new ListItem(HTMLHelper.HTMLEncode(displayName), ValidationHelper.GetString(dr["NewsletterName"], string.Empty));
                        chklNewsletters.Items.Add(li);
                    }
                }
            }
        }

        // Set SkinID properties
        if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (string.IsNullOrEmpty(ValidationHelper.GetString(Page.StyleSheetTheme, string.Empty))))
        {
            string skinId = SkinID;
            if (!string.IsNullOrEmpty(skinId))
            {
                lblFirstName.SkinID = skinId;
                lblLastName.SkinID = skinId;
                lblEmail.SkinID = skinId;
                txtFirstName.SkinID = skinId;
                txtLastName.SkinID = skinId;
                txtEmail.SkinID = skinId;
                btnSubmit.SkinID = skinId;
            }
        }
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    /// <param name="page">Web Forms page</param>
    public override void ApplyStyleSheetSkin(Page page)
    {
        string skinId = SkinID;
        if (!string.IsNullOrEmpty(skinId))
        {
            lblFirstName.SkinID = skinId;
            lblLastName.SkinID = skinId;
            lblEmail.SkinID = skinId;
            txtFirstName.SkinID = skinId;
            txtLastName.SkinID = skinId;
            txtEmail.SkinID = skinId;
            btnSubmit.SkinID = skinId;
        }

        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Indicates whether the control form fields contain a valid data.
    /// </summary>
    /// <returns>Returns true if the form data are valid; otherwise, false</returns>
    private bool IsValid()
    {
        string errorText = null;
        bool result = true;

        // If not allowing user subscribing or if user is not logged in
        if (!(AllowUserSubscribers && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated()))
        {
            // First name validation
            if (DisplayFirstName)
            {
                if (txtFirstName.Text == null || txtFirstName.Text.Trim().Length == 0)
                {
                    errorText += GetString("NewsletterSubscription.ErrorEmptyFirstName") + "<br />";
                    result = false;
                }
            }

            // Last name
            if (DisplayLastName)
            {
                if (txtLastName.Text == null || txtLastName.Text.Trim().Length == 0)
                {
                    errorText += GetString("NewsletterSubscription.ErrorEmptyLastName") + "<br />";
                    result = false;
                }
            }

            // E-mail address validation
            if (String.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.IsValid())
            {
                errorText += GetString("NewsletterSubscription.ErrorInvalidEmail") + "<br />";
                result = false;
            }
        }
        // If allowing user subscribing and user is logged in and user don't have filled in e-mail
        else if ((AllowUserSubscribers && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated()) && (String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.Email)))
        {
            errorText += GetString("newslettersubscription.erroremptyemail") + "<br />";
            result = false;
        }

        if (chooseMode)
        {
            if (chklNewsletters.SelectedIndex < 0)
            {
                errorText += GetString("NewsletterSubscription.NoneSelected") + "<br />";
                result = false;
            }
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
    private void ValidChoose()
    {
        ContactInfo contact = SaveContact();
        ClearAndHideContactForm();

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
            lblInfo.Text += GetString("NewsletterSubscription.Subscribed");
        }
        else
        {
            plcNwsList.Visible = true;

            if (DisplayFirstName && !(AllowUserSubscribers && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated()))
            {
                visibleFirstName = true;
            }

            if (DisplayLastName && !(AllowUserSubscribers && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated()))
            {
                visibleLastName = true;
            }

            if (!((AllowUserSubscribers && (MembershipContext.AuthenticatedUser != null) && AuthenticationHelper.IsAuthenticated()) && (!String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.Email))))
            {
                visibleEmail = true;
            }

            if ((UseImageButton) && (!String.IsNullOrEmpty(ImageButtonURL)))
            {
                pnlButtonSubmit.Visible = false;
                pnlImageSubmit.Visible = true;
            }
            else
            {
                pnlButtonSubmit.Visible = true;
                pnlImageSubmit.Visible = false;
            }
        }
    }


    /// <summary>
    /// Submit button handler.
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        if (IsValid())
        {
            if (chooseMode)
            {
                ValidChoose();
            }
            else
            {
                ContactInfo contact = SaveContact();
                ClearAndHideContactForm();
                Save(NewsletterName, contact);
            }
        }
    }


    /// <summary>
    /// Saves contact.
    /// </summary>
    /// <returns>Contact info object</returns>
    private ContactInfo SaveContact()
    {
        ContactInfo contact;
        CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

        // Handle authenticated user when user subscription is allowed
        if (AllowUserSubscribers && AuthenticationHelper.IsAuthenticated() && (currentUser != null))
        {
            contact = mContactProvider.GetContactForSubscribing(currentUser);
        }
        // Work with non-authenticated user or authenticated user when user subscription is disabled
        else
        {
            var firstName = TextHelper.LimitLength(txtFirstName.Text.Trim(), 100);
            var lastName = TextHelper.LimitLength(txtLastName.Text.Trim(), 100);
            contact = mContactProvider.GetContactForSubscribing(txtEmail.Text.Trim(), firstName, lastName);
        }
        return contact;
    }


    /// <summary>
    /// Saves the data.
    /// </summary>
    private bool Save(string newsletterName, ContactInfo contact)
    {
        bool toReturn = false;
        int siteId = SiteContext.CurrentSiteID;

        // Check if subscriber info object exists
        if (string.IsNullOrEmpty(newsletterName) || (contact == null))
        {
            return false;
        }

        // Get newsletter info
        NewsletterInfo newsletter = NewsletterInfo.Provider.Get(newsletterName, siteId);
        if (newsletter != null)
        {
            try
            {
                // Check if contact is not marketable
                if (!mSubscriptionService.IsMarketable(contact, newsletter))
                {
                    toReturn = true;

                    mSubscriptionService.Subscribe(contact, newsletter, new SubscribeSettings()
                    {
                        SendConfirmationEmail = SendConfirmationEmail,
                        AllowOptIn = true,
                        RemoveUnsubscriptionFromNewsletter = true,
                        RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    });

                    // Info message
                    if (!chooseMode)
                    {
                        lblInfo.Visible = true;
                        lblInfo.Text = GetString("NewsletterSubscription.Subscribed");
                    }
                }
                else
                {
                    // Info message - subscriber is allready in site
                    if (!chooseMode)
                    {
                        lblInfo.Visible = true;
                        lblInfo.Text = GetString("NewsletterSubscription.SubscriberIsAlreadySubscribed");
                    }
                    else
                    {
                        lblInfo.Visible = true;
                        lblInfo.Text += GetString("NewsletterSubscription.SubscriberIsAlreadySubscribedXY") + " " + HTMLHelper.HTMLEncode(newsletter.NewsletterDisplayName) + ".<br />";
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
    /// Clears the cached items.
    /// </summary>
    public override void ClearCache()
    {
        string useCacheItemName = DataHelper.GetNotEmpty(CacheItemName, CacheHelper.BaseCacheKey + "|" + RequestContext.CurrentURL + "|" + ClientID);

        CacheHelper.ClearCache(useCacheItemName);
    }


    private void ClearAndHideContactForm()
    {
        // Hide all
        visibleLastName = false;
        visibleFirstName = false;
        visibleEmail = false;

        pnlButtonSubmit.Visible = false;
        pnlImageSubmit.Visible = false;

        plcNwsList.Visible = false;

        // Clear the form
        txtEmail.Text = string.Empty;
        txtFirstName.Text = string.Empty;
        txtLastName.Text = string.Empty;
    }
}