using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Widgets_WidgetActions : CMSAbstractWebPart, IPostBackEventHandler
{
    #region "Variables"

    private bool resetAllowed = true;
    private WebPartZoneInstance zoneInstance = null;
    private List<WebPartZoneInstance> zoneInstances = new List<WebPartZoneInstance>();
    private string addScript = String.Empty;
    private bool headerActionsLoaded = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets widget zone type.
    /// </summary>
    public string WidgetZoneID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WidgetZoneID"), String.Empty);
        }
        set
        {
            SetValue("WidgetZoneID", value);
        }
    }


    /// <summary>
    /// Gets or sets text for add button.
    /// </summary>
    public string AddButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AddButtonText"), String.Empty);
        }
        set
        {
            SetValue("AddButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets text for reset button.
    /// </summary>
    public string ResetButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResetButtonText"), String.Empty);
        }
        set
        {
            SetValue("ResetButtonText", value);
        }
    }


    /// <summary>
    /// Enables or disables reset button.
    /// </summary>
    public bool DisplayResetButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayResetButton"), true);
        }
        set
        {
            SetValue("DisplayResetButton", value);
        }
    }


    /// <summary>
    /// Enables or disables add widget button.
    /// </summary>
    public bool DisplayAddButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAddButton"), true);
        }
        set
        {
            SetValue("DisplayAddButton", value);
        }
    }


    /// <summary>
    /// Enables or disables confirmation for reset button.
    /// </summary>
    public bool ResetConfirmationRequired
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResetConfirmationRequired"), true);
        }
        set
        {
            SetValue("ResetConfirmationRequired", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (DocumentContext.CurrentPageInfo != null)
            {
                PageInfo pi = DocumentContext.CurrentPageInfo;

                // Make visible, visibility according to the current state will be set later (solves issue with changing visibility during postbacks)
                Visible = true;

                CMSPagePlaceholder parentPlaceHolder = PortalHelper.FindParentPlaceholder(this);

                // Nothing to render, nothing to do
                if ((!DisplayAddButton && !DisplayResetButton) ||
                    ((parentPlaceHolder != null) && (parentPlaceHolder.UsingDefaultPage || (parentPlaceHolder.PageInfo.DocumentID != pi.DocumentID))))
                {
                    Visible = false;
                    return;
                }

                var currentUser = MembershipContext.AuthenticatedUser;

                // Check security
                if (!AuthenticationHelper.IsAuthenticated() || !currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName))
                {
                    Visible = false;
                    resetAllowed = false;
                    return;
                }

                // Displaying - Widgets only in 'DashboardWidgets' mode, check dashboard name
                if ((PortalManager.ViewMode != ViewModeEnum.DashboardWidgets) || (String.IsNullOrEmpty(PortalContext.DashboardName)))
                {
                    Visible = false;
                    resetAllowed = false;
                    return;
                }

                // Find widget zone
                PageTemplateInfo pti = pi.UsedPageTemplateInfo;

                // ZodeID specified directly
                if (!String.IsNullOrEmpty(WidgetZoneID))
                {
                    zoneInstance = pti.TemplateInstance.GetZone(WidgetZoneID);
                }

                // For delete all variants all zones are necessary
                if (parentPlaceHolder != null)
                {
                    var zones = parentPlaceHolder.WebPartZones;
                    if (zones != null)
                    {
                        foreach (CMSWebPartZone zone in zones)
                        {
                            if ((zone.ZoneInstance != null) && (zone.ZoneInstance.WidgetZoneType == WidgetZoneTypeEnum.Dashboard))
                            {
                                zoneInstances.Add(zone.ZoneInstance);
                                if (zoneInstance == null)
                                {
                                    zoneInstance = zone.ZoneInstance;
                                }
                            }
                        }
                    }
                }

                // No suitable zones on the page, nothing to do
                if (zoneInstance == null)
                {
                    Visible = false;
                    resetAllowed = false;
                    return;
                }

                // Adding is enabled
                if (DisplayAddButton)
                {
                    btnAddWidget.Visible = true;
                    btnAddWidget.Text = GetAddWidgetButtonText();

                    int templateId = 0;
                    if (pi.UsedPageTemplateInfo != null)
                    {
                        templateId = pi.UsedPageTemplateInfo.PageTemplateId;
                    }

                    addScript = (PortalContext.ViewMode == ViewModeEnum.EditLive ? "OEDeactivateWebPartBorder({ webPartSpanId: $cmsj('.OnSiteMenuTable').parent().attr('id').replace('OE_OE_', 'OE_')}, null );" : String.Empty) + "NewWidget(new zoneProperties('" + zoneInstance.ZoneID + "', '" + pi.NodeAliasPath + "', '" + templateId + "')); return false;";
                    btnAddWidget.Attributes.Add("onclick", addScript);
                }

                // Reset is enabled
                if (DisplayResetButton)
                {
                    btnReset.Visible = true;
                    btnReset.Text = GetResetButtonText();
                    btnReset.Click += new EventHandler(btnReset_Click);

                    // Add confirmation if required
                    if (ResetConfirmationRequired)
                    {
                        btnReset.Attributes.Add("onclick", "if (!confirm(" + ScriptHelper.GetString(PortalHelper.LocalizeStringForUI("widgets.resetzoneconfirmtext")) + ")) return false;");
                    }
                }
            }
        }
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetupControl();

        if ((PagePlaceholder != null)
            && (PagePlaceholder.PortalManager != null)
            && (PagePlaceholder.PortalManager.CurrentEditMenu != null))
        {
            PagePlaceholder.PortalManager.CurrentEditMenu.OnBeforeReloadMenu += CurrentEditMenu_OnBeforeReloadMenu;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CssRegistration.RegisterBootstrap(Page);
    }


    /// <summary>
    /// Handles the OnBeforeReloadMenu event of the CurrentEditMenu control.
    /// </summary>
    protected void CurrentEditMenu_OnBeforeReloadMenu(object sender, EventArgs e)
    {
        if (!headerActionsLoaded)
        {
            headerActionsLoaded = true;

            // Register "Add widget button" and "Reset to default" button
            RegisterHeaderActionButtons();
        }
    }


    /// <summary>
    /// Handles reset button click. Resets zones of specified type to default settings.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        // Security check
        if (!DisplayResetButton || !resetAllowed)
        {
            return;
        }

        PageInfo pi = DocumentContext.CurrentPageInfo;

        if (pi == null)
        {
            return;
        }

        // Delete user personalization info
        PersonalizationInfo up = PersonalizationInfoProvider.GetDashBoardPersonalization(MembershipContext.AuthenticatedUser.UserID, PortalContext.DashboardName, PortalContext.DashboardSiteName);
        PersonalizationInfoProvider.DeletePersonalizationInfo(up);

        // Clear cached page template
        if (pi.UsedPageTemplateInfo != null)
        {
            CacheHelper.TouchKey("cms.pagetemplate|byid|" + pi.UsedPageTemplateInfo.PageTemplateId);
        }

        // Make redirect to see changes after load
        string url = RequestContext.CurrentURL;

        if (ViewMode.IsEdit(true) || ViewMode.IsEditLive())
        {
            // Ensure that the widgets will be loaded from the session layer (not from database) 
            url = URLHelper.UpdateParameterInUrl(url, "cmscontentchanged", "true");
        }

        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        base.ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Registers the header action buttons.
    /// </summary>
    private void RegisterHeaderActionButtons()
    {
        // Place actions to the main menu if required
        if (Visible)
        {
            // Try get current menu
            EditMenu em = PagePlaceholder.PortalManager.CurrentEditMenu;
            if (em != null)
            {
                // Add button
                if (DisplayAddButton)
                {
                    HeaderAction ha = new HeaderAction()
                    {
                        Enabled = true,
                        Text = GetAddWidgetButtonText(),
                        OnClientClick = addScript,
                        Tooltip = PortalHelper.LocalizeStringForUI("addwidget.tooltip"),
                        GenerateSeparatorBeforeAction = true,
                        ButtonStyle = ButtonStyle.Default
                    };

                    btnAddWidget.Visible = false;
                    em.AddExtraAction(ha);
                }

                // Reset button
                if (DisplayResetButton)
                {
                    HeaderAction ha = new HeaderAction
                    {
                        Enabled = true,
                        Text = GetResetButtonText(),
                        OnClientClick = "if (!confirm(" + ScriptHelper.GetString(PortalHelper.LocalizeStringForUI("widgets.resetzoneconfirmtext")) + ")) { return false; } else { " + ControlsHelper.GetPostBackEventReference(this, "reset") + " }",
                        Tooltip = PortalHelper.LocalizeStringForUI("resetwidget.tooltip"),
                        GenerateSeparatorBeforeAction = !DisplayAddButton,
                        ButtonStyle = ButtonStyle.Default
                    };

                    btnReset.Visible = false;
                    em.AddExtraAction(ha);
                }

                // Hide empty widget action panel
                pnlWidgetActions.Visible = false;
            }
        }
    }


    /// <summary>
    /// Gets the add widget button text.
    /// </summary>
    /// <returns></returns>
    private string GetAddWidgetButtonText()
    {
        return DataHelper.GetNotEmpty(AddButtonText, PortalHelper.LocalizeStringForUI("widgets.addwidget"));
    }


    /// <summary>
    /// Gets the reset button text.
    /// </summary>
    /// <returns></returns>
    private string GetResetButtonText()
    {
        return DataHelper.GetNotEmpty(ResetButtonText, PortalHelper.LocalizeStringForUI("widgets.resettodefault"));
    }

    #endregion


    #region "PostBack event"

    /// <summary>
    /// Raises the post back event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        SecurityHelper.LogScreenLockAction();

        switch (eventArgument)
        {
            case "reset":
                // Reset_click handler must be handled by PostabackEvent due to registering the reset button later in the page life cycle (when changing workflow steps)
                btnReset_Click(this, null);
                break;

            default:
                break;
        }
    }

    #endregion
}
