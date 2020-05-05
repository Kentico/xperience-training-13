using System;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Notifications.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;

public partial class CMSWebParts_Notifications_ContentSubscription : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the code names of the notification gateways separated with semicolon.
    /// </summary>
    public string GatewayNames
    {
        get
        {
            string names = ValidationHelper.GetString(GetValue("GatewayNames"), "");
            if (names == "")
            {
                names = "CMS.EmailGateway";
            }
            return names;
        }
        set
        {
            SetValue("GatewayNames", value);
        }
    }


    /// <summary>
    /// Indicates if notification e-mail is sent when specified document is created.
    /// </summary>
    public bool CreateEventEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CreateEventEnabled"), false);
        }
        set
        {
            SetValue("CreateEventEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets localizable string or plain text which describes CREATE event and which is visible to the users.
    /// </summary>
    public string CreateEventDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CreateEventDisplayName"), "");
        }
        set
        {
            SetValue("CreateEventDisplayName", value);
        }
    }


    /// <summary>
    /// Indicates if notification e-mail is sent when specified document is deleted.
    /// </summary>
    public bool DeleteEventEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DeleteEventEnabled"), false);
        }
        set
        {
            SetValue("DeleteEventEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets localizable string or plain text which describes DELETE event and which is visible to the users.
    /// </summary>
    public string DeleteEventDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DeleteEventDisplayName"), "");
        }
        set
        {
            SetValue("DeleteEventDisplayName", value);
        }
    }


    /// <summary>
    /// Indicates if notification e-mail is sent when specified document is updated.
    /// </summary>
    public bool UpdateEventEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UpdateEventEnabled"), false);
        }
        set
        {
            SetValue("UpdateEventEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets localizable string or plain text which describes UPDATE event and which is visible to the users.
    /// </summary>
    public string UpdateEventDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpdateEventDisplayName"), "");
        }
        set
        {
            SetValue("UpdateEventDisplayName", value);
        }
    }


    /// <summary>
    /// Gets or sets the description of the event.
    /// </summary>
    public string EventDescription
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventDescription"), "");
        }
        set
        {
            SetValue("EventDescription", value);
        }
    }


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
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), "");
        }
        set
        {
            SetValue("Path", value);
        }
    }


    /// <summary>
    /// Gets or sets the notification template code name for CREATE event.
    /// </summary>
    public string CreateEventTemplateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CreateEventTemplateName"), "");
        }
        set
        {
            SetValue("CreateEventTemplateName", value);
        }
    }


    /// <summary>
    /// Gets or sets the notification template code name for DELETE event.
    /// </summary>
    public string DeleteEventTemplateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DeleteEventTemplateName"), "");
        }
        set
        {
            SetValue("DeleteEventTemplateName", value);
        }
    }


    /// <summary>
    /// Gets or sets the notification template code name for UPDATE event.
    /// </summary>
    public string UpdateEventTemplateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UpdateEventTemplateName"), "");
        }
        set
        {
            SetValue("UpdateEventTemplateName", value);
        }
    }


    /// <summary>
    /// Gets or sets the document types.
    /// </summary>
    public string DocumentTypes
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentTypes"), "");
        }
        set
        {
            SetValue("DocumentTypes", value);
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
            // Build the actions string
            string actionsString = (CreateEventEnabled ? "|CREATEDOC" : "") +
                                   (DeleteEventEnabled ? "|DELETEDOC" : "") +
                                   (UpdateEventEnabled ? "|UPDATEDOC" : "");

            actionsString = actionsString.TrimStart('|');

            // Get the actions
            string[] actions = actionsString.Split(new char[] { '|' });
            if (actions.Length > 0)
            {
                // Inititalize subscriptionElem control
                subscriptionElem.GatewayNames = GatewayNames;
                subscriptionElem.EventSource = "Content";
                subscriptionElem.EventDescription = EventDescription;
                subscriptionElem.EventObjectID = 0;
                subscriptionElem.EventData1 = (String.IsNullOrEmpty(Path) ? "/%" : MacroResolver.ResolveCurrentPath(Path));
                subscriptionElem.EventData2 = DocumentTypes;
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

                // Initialize SubscriptionInfo objects
                NotificationSubscriptionInfo[] subscriptions = new NotificationSubscriptionInfo[actions.Length];
                for (int i = 0; i < actions.Length; i++)
                {
                    NotificationSubscriptionInfo nsi = new NotificationSubscriptionInfo();
                    nsi.SubscriptionEventCode = actions[i];

                    // Get correct template name and event display name
                    string currentDisplayName = string.Empty;
                    string currentTemplateName = string.Empty;
                    switch (actions[i].ToLowerCSafe())
                    {
                        case "createdoc":
                            currentDisplayName = CreateEventDisplayName;
                            currentTemplateName = CreateEventTemplateName;
                            break;
                        case "deletedoc":
                            currentDisplayName = DeleteEventDisplayName;
                            currentTemplateName = DeleteEventTemplateName;
                            break;
                        case "updatedoc":
                            currentDisplayName = UpdateEventDisplayName;
                            currentTemplateName = UpdateEventTemplateName;
                            break;
                    }

                    // Get correct template
                    NotificationTemplateInfo nti = GetTemplateInfo(currentTemplateName);
                    if (nti != null)
                    {
                        nsi.SubscriptionTemplateID = nti.TemplateID;
                    }

                    if (String.IsNullOrEmpty(currentDisplayName))
                    {
                        nsi.SubscriptionEventDisplayName = TextHelper.LimitLength(String.Format(GetString("notifications.contentsubscription.name"),
                                                                         (String.IsNullOrEmpty(Path) ? "/%" : Path),
                                                                         (String.IsNullOrEmpty(DocumentTypes) ? GetString("notifications.contentsubscription.alldoctypes") : DocumentTypes),
                                                                         actions[i]), 250, wholeWords: true, cutLocation: CutTextEnum.Middle);
                    }
                    else
                    {
                        nsi.SubscriptionEventDisplayName = currentDisplayName;
                    }
                    subscriptions[i] = nsi;
                }
                subscriptionElem.Subscriptions = subscriptions;
            }
        }
    }


    /// <summary>
    /// Parses the notification template site and name and returns proper Info object.
    /// </summary>
    private NotificationTemplateInfo GetTemplateInfo(string templateName)
    {
        if (!string.IsNullOrEmpty(templateName))
        {
            // Get current site name
            string siteName = CurrentSiteName;

            // If SiteName is not "#current#" or "-" get site name from property
            if (!(SiteName.EqualsCSafe("#current#", true) || (SiteName == "-")))
            {
                siteName = SiteName;
            }

            if (templateName.StartsWithCSafe(siteName + ".", true))
            {
                // Remove site name from template name
                templateName = templateName.Remove(0, siteName.Length + 1);

                // Site template
                SiteInfo tempSite = SiteInfoProvider.GetSiteInfo(siteName);
                if (tempSite != null)
                {
                    return NotificationTemplateInfoProvider.GetNotificationTemplateInfo(templateName, tempSite.SiteID);
                }
            }
            else
            {
                // Global template
                return NotificationTemplateInfoProvider.GetNotificationTemplateInfo(templateName);
            }
        }

        return null;
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