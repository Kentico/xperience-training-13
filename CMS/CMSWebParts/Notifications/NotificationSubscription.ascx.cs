using System;

using CMS.Helpers;
using CMS.Notifications.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Notifications_NotificationSubscription : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Determines whether the users are subscribed to site specific event or global event.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "-");
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the format of the subscription (HTML/Plaintext)
    /// </summary>
    public bool SubscriptionUseHTML
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SubscriptionUseHTML"), false);
        }
        set
        {
            SetValue("SubscriptionUseHTML", value);
        }
    }


    /// <summary>
    /// Event data field 1.
    /// </summary>
    public string EventData1
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventData1"), String.Empty);
        }
        set
        {
            SetValue("EventData1", value);
        }
    }


    /// <summary>
    /// Event data field 2.
    /// </summary>
    public string EventData2
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventData2"), String.Empty);
        }
        set
        {
            SetValue("EventData2", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which will be displayed above the notification gateway forms.
    /// </summary>
    public string EventDescription
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventDescription"), String.Empty);
        }
        set
        {
            SetValue("EventDescription", value);
        }
    }


    /// <summary>
    /// Gets or sets the code names of the notification gateways separated with semicolon.
    /// </summary>
    public string GatewayNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GatewayNames"), String.Empty);
        }
        set
        {
            SetValue("GatewayNames", value);
        }
    }


    /// <summary>
    /// Gets or sets the notification template code name.
    /// </summary>
    public string NotificationTemplateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NotificationTemplateName"), String.Empty);
        }
        set
        {
            SetValue("NotificationTemplateName", value);
        }
    }


    /// <summary>
    /// Gets or sets the event source.
    /// </summary>
    public string EventSource
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventSource"), String.Empty);
        }
        set
        {
            SetValue("EventSource", value);
        }
    }


    /// <summary>
    /// Gets or sets the event code.
    /// </summary>
    public string EventCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventCode"), String.Empty);
        }
        set
        {
            SetValue("EventCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the event object ID.
    /// </summary>
    public int EventObjectID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("EventObjectID"), -1);
        }
        set
        {
            SetValue("EventObjectID", value);
        }
    }


    /// <summary>
    /// Gets or sets localizable string or plain text which describes event and which is visible to the users.
    /// </summary>
    public string EventDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventDisplayName"), String.Empty);
        }
        set
        {
            SetValue("EventDisplayName", value);
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            subscriptionElem.StopProcessing = true;
        }
        else
        {
            // Inititalize subscriptionElem control
            subscriptionElem.Subscriptions = new[] { new NotificationSubscriptionInfo() };
            subscriptionElem.EventDescription = EventDescription;
            subscriptionElem.GatewayNames = GatewayNames;
            subscriptionElem.NotificationTemplateName = NotificationTemplateName;
            subscriptionElem.EventCode = EventCode;
            subscriptionElem.EventSource = EventSource;
            subscriptionElem.EventDisplayName = EventDisplayName;
            subscriptionElem.EventObjectID = EventObjectID;
            subscriptionElem.EventData1 = EventData1;
            subscriptionElem.EventData2 = EventData2;
            subscriptionElem.SubscriptionUseHTML = SubscriptionUseHTML;

            // If "#current#" is set, then get current site ID
            if (SiteName == "#current#")
            {
                subscriptionElem.SubscriptionSiteID = SiteContext.CurrentSiteID;
            }
            // If "-" as global is not set, then try to find the site
            else if (SiteName != "-")
            {
                // Try to find given site
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteName);
                if (si != null)
                {
                    subscriptionElem.SubscriptionSiteID = si.SiteID;
                }
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}