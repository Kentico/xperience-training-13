using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Newsletters_NewsletterUnsubscriptionWebPart : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the unsubscribed text.
    /// </summary>
    public string UnsubscribedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnsubscribedText"), "");
        }
        set
        {
            SetValue("UnsubscribedText", value);
        }
    }


    /// <summary>
    /// Gets or sets the unsubscribed text for when subscriber unsubscribes from all newsletters.
    /// </summary>
    public string UnsubscribedAllText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnsubscribedAllText"), "");
        }
        set
        {
            SetValue("UnsubscribedText", value);
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        var subscriptionService = Service.Resolve<ISubscriptionService>();
        var unsubscriptionProvider = Service.Resolve<IUnsubscriptionProvider>();
        var emailHashValidator = Service.Resolve<IEmailHashValidator>();

        int siteId = SiteContext.CurrentSiteID;

        // Get data from query string
        Guid subscriberGuid = QueryHelper.GetGuid("subscriberguid", Guid.Empty);
        Guid newsletterGuid = QueryHelper.GetGuid("newsletterguid", Guid.Empty);
        bool unsubscribeFromAll = QueryHelper.GetBoolean("unsubscribeFromAll", false);
        string email = QueryHelper.GetString("email", string.Empty);
        string hash = QueryHelper.GetString("hash", string.Empty);
        bool isInBackwardCompatibilityMode = (string.IsNullOrEmpty(hash) && (subscriberGuid != Guid.Empty));
        NewsletterContext.UnsubscriptionLinksBackwardCompatibilityMode = isInBackwardCompatibilityMode;

        // Email must be provided and hash must be valid when not in compatibility mode
        if (!isInBackwardCompatibilityMode && !emailHashValidator.ValidateEmailHash(hash, email))
        {
            lblError.Visible = true;
            lblError.Text = GetString("newsletter.unsubscribefailed");
            return;
        }

        // When backward compatibility mode is used, email is not provided in query string. Get it from subscriber or contact in this case.
        if (isInBackwardCompatibilityMode)
        {
            var subscriber = SubscriberInfo.Provider.Get(subscriberGuid, siteId);
            email = GetSubscriberEmail(subscriber, QueryHelper.GetInteger("contactid", 0));

            if (string.IsNullOrEmpty(email))
            {
                lblError.Visible = true;
                lblError.Text = GetString("Unsubscribe.NotSubscribed");
                return;
            }
        }

        if (!ValidationHelper.IsEmail(email))
        {
            lblError.Visible = true;
            lblError.Text = GetString("newsletter.unsubscribefailed");
            return;
        }

        int? issueId = GetIssueID(siteId);
        if (unsubscribeFromAll)
        {
            if (!unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email))
            {
               subscriptionService.UnsubscribeFromAllNewsletters(email, issueId);
            }

            DisplayConfirmationAllUnsubscribed();
        }
        else
        {
            var newsletter = NewsletterInfo.Provider.Get(newsletterGuid, siteId);
            if (newsletter == null)
            {
                lblError.Visible = true;
                lblError.Text = GetString("Unsubscribe.NewsletterDoesNotExist");
                return;
            }

            if (!unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(email, newsletter.NewsletterID))
            {
                subscriptionService.UnsubscribeFromSingleNewsletter(email, newsletter.NewsletterID, issueId, SendConfirmationEmail);
            }

            DisplayConfirmationSingleUnsubscribed();
        }
    }


    private static int? GetIssueID(int siteId)
    {
        var issue = IssueInfo.Provider.Get(QueryHelper.GetInteger("issueid", 0)) ??
            IssueInfo.Provider.Get(QueryHelper.GetGuid("issueguid", Guid.Empty), siteId);

        // When issue is not found, return null. When unsubscribing, 0 would fail when used as issue ID
        return issue != null ? (int?)issue.IssueID : null;
    }


    /// <summary>
    /// Returns email for given subscriber.
    /// When subscriber is of type <see cref="PredefinedObjectType.CONTACTGROUP"/>, <paramref name="contactID"/> must be specified otherwise email won't be found.
    /// </summary>
    private static string GetSubscriberEmail(SubscriberInfo subscriber, int contactID = 0)
    {
        var subscriberEmailRetriever = Service.Resolve<ISubscriberEmailRetriever>();
        string subscriberEmail = null;

        // Subscriber can be null for example when it was deleted.
        if (subscriber != null)
        {
            subscriberEmail = subscriberEmailRetriever.GetSubscriberEmail(subscriber.SubscriberID);
        }

        // When email was not found, use contact ID to retrieve an email
        if (string.IsNullOrEmpty(subscriberEmail))
        {
            subscriberEmail = subscriberEmailRetriever.GetEmailForContact(contactID);
        }

        return subscriberEmail;
    }


    /// <summary>
    /// Displays info label.
    /// </summary>
    private void DisplayConfirmationSingleUnsubscribed()
    {
        // Display confirmation message
        lblInfo.Visible = true;
        lblInfo.Text = String.IsNullOrEmpty(UnsubscribedText) ? GetString("Unsubscribe.Unsubscribed") : UnsubscribedText;
    }


    /// <summary>
    /// Displays info label.
    /// </summary>
    private void DisplayConfirmationAllUnsubscribed()
    {
        // Display confirmation message
        lblInfo.Visible = true;
        lblInfo.Text = String.IsNullOrEmpty(UnsubscribedAllText) ? GetString("emailmarketing.webpart.newsletterunsubscription.allconfirmation") : UnsubscribedAllText;
    }

    #endregion
}