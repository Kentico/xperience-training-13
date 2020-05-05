using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Notifications.Web.UI;
using CMS.SiteProvider;


public partial class CMSModules_Notifications_Controls_NotificationSubscription_NotificationSubscription : CMSNotificationSubscription
{
    private object[,] controls;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if ((MembershipContext.AuthenticatedUser == null) || (MembershipContext.AuthenticatedUser.IsPublic()))
        {
            StopProcessing = true;
            Visible = false;
            return;
        }

        if (NotificationGateways.Count == 0)
        {
            StopProcessing = true;
            pnlSubscribe.Visible = false;
            lblError.Text = GetString("notifications.nogateway");
            lblError.Visible = true;
            return;
        }

        // Register show/hide JS
        string script = "function showHide(panelId, checkboxId) { " +
                        "    var panel = document.getElementById(panelId);" +
                        "    var chkBox = document.getElementById(checkboxId);" +
                        "    if ((panel != null) && (chkBox != null)) {" +
                        "        if (chkBox.checked) {" +
                        "            panel.style.display = 'block'" +
                        "        } else {" +
                        "            panel.style.display = 'none'" +
                        "        }" +
                        "    }" +
                        "}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "NotificationSubscriptionShowHide", ScriptHelper.GetScript(script));

        btnSubscribe.Text = GetString("notifications.subscribe");
        lblDescription.Text = HTMLHelper.HTMLEncode(EventDescription);

        // Load dynamically notification gateways forms
        controls = new object[NotificationGateways.Count, 3];
        for (int i = 0; i < NotificationGateways.Count; i++)
        {
            CMSNotificationGateway g = NotificationGateways[i];
            if ((g.NotificationGatewayObj != null) && (g.NotificationGatewayForm != null))
            {
                // Checkbox
                Panel pnlCheckbox = new Panel();
                pnlCheckbox.ID = g.NotificationGatewayObj.GatewayName + "_chkboxpanel";
                pnlCheckbox.CssClass = "NotificationSubscriptionCheckbox";
                CMSCheckBox chkSubscribe = new CMSCheckBox();
                if (NotificationGateways.Count == 1)
                {
                    chkSubscribe.Checked = true;
                }
                chkSubscribe.ID = g.NotificationGatewayObj.GatewayName;
                chkSubscribe.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(g.NotificationGatewayObj.GatewayDisplayName));
                pnlCheckbox.Controls.Add(chkSubscribe);
                pnlGateways.Controls.Add(pnlCheckbox);

                // Notification form
                Panel pnlForm = new Panel();
                pnlForm.ID = g.NotificationGatewayObj.GatewayName + "_formpanel";
                pnlForm.Controls.Add(g.NotificationGatewayForm);
                pnlGateways.Controls.Add(pnlForm);

                // Add JS for show/hide functionality
                chkSubscribe.Attributes.Add("onclick", "showHide('" + pnlForm.ClientID + "','" + chkSubscribe.ClientID + "');");

                // Store references in array (we need to set display attribute after the ViewState is loaded into checkboxes)
                controls[i, 0] = chkSubscribe;
                controls[i, 1] = pnlForm;
                controls[i, 2] = g;
            }
        }
    }


    /// <summary>
    /// Sets display attribute on prerender, because here the viewstate values are loaded to checkboxes.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        base.OnPreRender(e);

        // Hide the checkboxes if only one gateway is specified
        if (NotificationGateways.Count == 1)
        {
            CMSCheckBox chk = controls[0, 0] as CMSCheckBox;
            if (chk != null)
            {
                chk.Visible = false;
            }
        }
        else
        {
            for (int i = 0; i < NotificationGateways.Count; i++)
            {
                // Set the correct visibility of the panel
                CMSCheckBox chk = controls[i, 0] as CMSCheckBox;
                Panel pnl = controls[i, 1] as Panel;
                if ((pnl != null) && (chk != null))
                {
                    pnl.Attributes.Add("style", "display: " + (chk.Checked ? "block" : "none") + ";");
                }
            }
        }
    }


    /// <summary>
    /// For each notification gateway provider validates its notification form data.
    ///	If validation fails returns concatenated error messages from each form otherwise returns empty string.
    /// </summary>
    public override string Validate()
    {
        string errorMessage = String.Empty;

        // Check public user
        if (EventUserID > 0)
        {
            UserInfo ui = UserInfoProvider.GetUserInfo(EventUserID);
            if ((ui != null) && (ui.IsPublic()))
            {
                return GetString("notifications.subscribe.anonymous");
            }
        }
        else
        {
            if ((MembershipContext.AuthenticatedUser != null) && (MembershipContext.AuthenticatedUser.IsPublic()))
            {
                return GetString("notifications.subscribe.anonymous");
            }
        }

        for (int i = 0; i < NotificationGateways.Count; i++)
        {
            // Validate only loaded forms which are displayed
            CMSCheckBox chk = controls[i, 0] as CMSCheckBox;
            CMSNotificationGateway g = controls[i, 2] as CMSNotificationGateway;
            string err = g.NotificationGatewayForm.Validate();
            if ((g.NotificationGatewayForm != null) && (chk != null) && (chk.Checked) && !String.IsNullOrEmpty(err))
            {
                errorMessage += "<br />" + g.NotificationGatewayObj.GatewayDisplayName + ": " + err;
            }
        }

        // Remove first "<br />" which is not needed
        if (errorMessage != String.Empty)
        {
            errorMessage = errorMessage.Substring(6);
        }

        return errorMessage;
    }


    /// <summary>
    /// Calls Validate() method, if validation fails returns error message, 
    /// otherwise creates subscriptions and returns empty string.
    /// </summary>
    public override string Subscribe()
    {
        // Validate inputs
        string errorMessage = Validate();
        if (!String.IsNullOrEmpty(errorMessage))
        {
            return errorMessage;
        }

        // Get correct user ID
        int userId = 0;
        if (EventUserID > 0)
        {
            userId = EventUserID;
        }
        else
        {
            if (MembershipContext.AuthenticatedUser != null)
            {
                userId = MembershipContext.AuthenticatedUser.UserID;
            }
        }

        // Parse the notification template site and name
        NotificationTemplateInfo nti = null;
        string templateName = NotificationTemplateName;
        if (NotificationTemplateName != null)
        {
            string[] temp = NotificationTemplateName.Split(new char[] { '.' });
            if (temp.Length == 2)
            {
                SiteInfo tempSite = SiteInfoProvider.GetSiteInfo(temp[0]);
                if (tempSite != null)
                {
                    templateName = temp[1];
                    nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(templateName, tempSite.SiteID);
                }
            }
            else
            {
                nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(templateName, 0);
            }
        }

        bool hasSubscription = false;

        // Inputs are valid now, create the subscriptions
        for (int i = 0; i < NotificationGateways.Count; i++)
        {
            CMSCheckBox chk = controls[i, 0] as CMSCheckBox;
            Panel pnl = controls[i, 1] as Panel;
            CMSNotificationGateway g = controls[i, 2] as CMSNotificationGateway;
            if ((pnl != null) && (chk != null) && (g != null) && (chk.Checked))
            {
                // Register the subscriptions
                if (g.NotificationGatewayObj != null && g.NotificationGatewayForm != null)
                {
                    // Stores NotificationSubscriptionInfo objects
                    List<NotificationSubscriptionInfo> infos = new List<NotificationSubscriptionInfo>();
                    bool uniquenessFailed = false;
                    bool templateFailed = false;

                    foreach (NotificationSubscriptionInfo nsiTemplate in Subscriptions)
                    {
                        // Create new subscription and initialize it with default values
                        NotificationSubscriptionInfo nsi = new NotificationSubscriptionInfo();
                        nsi.SubscriptionEventDisplayName = EventDisplayName;
                        nsi.SubscriptionTarget = Convert.ToString(g.NotificationGatewayForm.Value);
                        nsi.SubscriptionEventCode = EventCode;
                        nsi.SubscriptionEventObjectID = EventObjectID;
                        nsi.SubscriptionEventSource = EventSource;
                        nsi.SubscriptionGatewayID = g.NotificationGatewayObj.GatewayID;
                        nsi.SubscriptionTime = DateTime.Now;
                        nsi.SubscriptionUserID = userId;
                        nsi.SubscriptionEventData1 = EventData1;
                        nsi.SubscriptionEventData2 = EventData2;
                        nsi.SubscriptionUseHTML = SubscriptionUseHTML;
                        nsi.SubscriptionSiteID = SubscriptionSiteID;
                        if (nti != null)
                        {
                            nsi.SubscriptionTemplateID = nti.TemplateID;
                        }

                        // Overwrite default values if these are specified in template subscription 
                        if (!String.IsNullOrEmpty(nsiTemplate.SubscriptionEventDisplayName))
                        {
                            nsi.SubscriptionEventDisplayName = nsiTemplate.SubscriptionEventDisplayName;
                        }
                        if (!String.IsNullOrEmpty(nsiTemplate.SubscriptionEventCode))
                        {
                            nsi.SubscriptionEventCode = nsiTemplate.SubscriptionEventCode;
                        }
                        if (!String.IsNullOrEmpty(nsiTemplate.SubscriptionEventSource))
                        {
                            nsi.SubscriptionEventSource = nsiTemplate.SubscriptionEventSource;
                        }
                        if (!String.IsNullOrEmpty(nsiTemplate.SubscriptionEventData1))
                        {
                            nsi.SubscriptionEventData1 = nsiTemplate.SubscriptionEventData1;
                        }
                        if (!String.IsNullOrEmpty(nsiTemplate.SubscriptionEventData2))
                        {
                            nsi.SubscriptionEventData2 = nsiTemplate.SubscriptionEventData2;
                        }
                        if (nsiTemplate.SubscriptionEventObjectID > 0)
                        {
                            nsi.SubscriptionEventObjectID = nsiTemplate.SubscriptionEventObjectID;
                        }
                        if (nsiTemplate.SubscriptionUserID > 0)
                        {
                            nsi.SubscriptionEventObjectID = nsiTemplate.SubscriptionUserID;
                        }
                        if (nsiTemplate.SubscriptionSiteID > 0)
                        {
                            nsi.SubscriptionSiteID = nsiTemplate.SubscriptionSiteID;
                        }
                        if (nsiTemplate.SubscriptionTemplateID > 0)
                        {
                            nsi.SubscriptionTemplateID = nsiTemplate.SubscriptionTemplateID;
                        }

                        // Check whether template is set
                        if (nsi.SubscriptionTemplateID <= 0)
                        {
                            templateFailed = true;
                            break;
                        }

                        // Check uniqueness (create only unique subscriptions)
                        var additionalWhere = new WhereCondition()
                            .WhereEquals("SubscriptionTarget", nsi.SubscriptionTarget)
                            .WhereEquals("SubscriptionGatewayID", nsi.SubscriptionGatewayID);

                        var whereConditionObj = NotificationSubscriptionInfoProvider.GetWhereConditionObject(nsi.SubscriptionEventSource, nsi.SubscriptionEventCode, nsi.SubscriptionEventObjectID, nsi.SubscriptionEventData1, nsi.SubscriptionEventData2, nsi.SubscriptionSiteID, additionalWhere);

                        bool subscriptionExists = NotificationSubscriptionInfoProvider.GetNotificationSubscriptions()
                            .Where(whereConditionObj)
                            .TopN(1)
                            .HasResults();

                        // Only if subscription is unique put it into the list
                        if (subscriptionExists)
                        {
                            uniquenessFailed = true;
                            break;
                        }
                        else
                        {
                            infos.Add(nsi);
                        }
                    }

                    if (uniquenessFailed)
                    {
                        return GetString("notifications.subscription.notunique");
                    }
                    else if (templateFailed)
                    {
                        return GetString("notifications.subscription.templatemissing");
                    }
                    else
                    {
                        // Save valid subscriptions into the DB
                        foreach (NotificationSubscriptionInfo nsi in infos)
                        {
                            NotificationSubscriptionInfoProvider.SetNotificationSubscriptionInfo(nsi);
                        }
                    }

                    // Clear the form after successful registration
                    g.NotificationGatewayForm.ClearForm();

                    hasSubscription = true;
                }
            }
        }

        if (hasSubscription)
        {
            if (NotificationGateways.Count > 1)
            {
                // Reset the state of the gateways
                for (int i = 0; i < NotificationGateways.Count; i++)
                {
                    CMSCheckBox chk = controls[i, 0] as CMSCheckBox;
                    Panel pnl = controls[i, 1] as Panel;
                    if ((chk != null) && (pnl != null))
                    {
                        chk.Checked = false;
                        pnl.Attributes.Add("style", "display: none;");
                    }
                }
            }

            return String.Empty;
        }
        else
        {
            return GetString("notifications.subscription.selectgateway");
        }
    }


    /// <summary>
    /// Handles the subscription process (displays error messages when something went wrong).
    /// </summary>
    protected void btnSubscribe_Click(object sender, EventArgs e)
    {
        string errorMessage = Subscribe();
        if (String.IsNullOrEmpty(errorMessage))
        {
            lblInfo.Text = GetString("notifications.subscription.successful");
            lblInfo.Visible = true;
        }
        else
        {
            // We can't use standard GetAlertScript, because of new line characters
            lblError.Text = errorMessage;
            lblError.Visible = true;
        }
    }
}
