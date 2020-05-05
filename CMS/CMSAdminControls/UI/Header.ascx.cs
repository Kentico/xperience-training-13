using System;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Header : CMSUserControl, ICallbackEventHandler
{
    #region "Constants"

    protected const string SESSION_KEY_TECH_PREVIEW = "WRNShowTechPreview";
    protected const string SESSION_KEY_TRIAL = "WRNShowTrial";
    private const string KENTICO_LICENSE_ULTIMATE_URL = "https://www.kentico.com/purchase/configure-your-license?edition=ultimate#editions";

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterModule(this, "CMS/Mole");

        ScriptHelper.RegisterModule(this, "CMS/BreadcrumbsPin", new
        {
            pinId = "js-single-object-dashboard-pin"
        });

        ScriptHelper.RegisterModule(this, "CMS/Header", new
        {
            selectorId = siteSelector.DropDownSingleSelect.ClientID,
            dashboardLinkId = lnkDashboard.ClientID
        });

        EnsureHideMessageCallback();

        CheckTrial();
        CheckEcommerceLicenseLimitations();

        // Display the techPreview info if there is a key in the web.config
        pnlTechPreview.Visible = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSUseTechnicalPreview"], false) && CheckWarningMessage(SESSION_KEY_TECH_PREVIEW);

        pwdExpiration.ShowChangePasswordLink = true;
        pwdExpiration.ExpirationTextBefore = GetString("passwordexpiration.expired");
        pwdExpiration.ExpirationWarningTextBefore = string.Format(GetString("passwordexpiration.willexpire"), MembershipContext.AuthenticatedUser.UserPasswordExpiration);

        string scHideWarning = @"
function HideWarning(id, key) {
    if(key) {
        HideMessage(key);
    }

    if(!id) {
        id = '" + pnlPwdExp.ClientID + @"';
    }

    var panel = $cmsj('#' + id);
    if(panel) {
        panel.hide();
        window.top.layouts[0].resizeAll();
    }
}

function ReceiveMessage() {}
";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "HideHeaderWarning", scHideWarning, true);

        // Site selector settings
        siteSelector.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;
        siteSelector.AllowAll = false;
        siteSelector.UniSelector.OnSelectionChanged += SiteSelector_OnSelectionChanged;
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.OnlyRunningSites = true;

        // Allow empty for not-existing current site
        if (SiteContext.CurrentSite == null)
        {
            siteSelector.AllowEmpty = true;
        }

        if (!RequestHelper.IsPostBack())
        {
            siteSelector.Value = SiteContext.CurrentSiteID;
        }

        // Show only assigned sites for not global admins
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            siteSelector.UserId = MembershipContext.AuthenticatedUser.UserID;
        }

        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function CheckChanges() {
    var innerFrame = parent.frames['cmsdesktop'];
    if (innerFrame.CheckChanges && !innerFrame.CheckChanges())
    {
        return false;
    }
    else
    {
        return true;
    }
}
");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "headerScript", ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterModule(this, "CMS/Breadcrumbs", new
        {
            moreBreadcrumbsText = GetString("breadcrumbs.more"),
            splitViewModeText = GetString("SplitMode.Compare")
        });

        EnsureSupportChat();
        EnsureStagingTaskGroupMenu();

        lnkDashboard.Attributes.Add("href", "#");
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlPwdExp.Visible = pwdExpiration.Visible;
    }

    #endregion


    #region "Methods"

    private void CheckTrial()
    {
        // Hide message if requested by user
        if (!CheckWarningMessage(SESSION_KEY_TRIAL))
        {
            pnlTrial.Visible = false;
            return;
        }

        string info = null;

        if (LicenseHelper.ApplicationExpires != DateTime.MinValue)
        {
            TimeSpan appExpiration = LicenseHelper.ApplicationExpires.Subtract(DateTime.Now);

            // Application expires
            if (CMSVersion.IsBetaVersion())
            {
                if (appExpiration.Ticks <= 0)
                {
                    info = GetString("Beta.AppExpired");
                }
                else
                {
                    info = string.Format(GetString("Beta.AppExpiresIn"), GetExpirationString(appExpiration.Days));
                }
            }
            else
            {
                if (appExpiration.Ticks <= 0)
                {
                    info = string.Format(GetString("Preview.AppExpired"), CMSVersion.VersionSuffix);
                }
                else
                {
                    info = string.Format(GetString("Preview.AppExpiresIn"), CMSVersion.VersionSuffix, GetExpirationString(appExpiration.Days));
                }
            }
        }
        // Check the license key for trial or free version
        else if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty) != string.Empty)
        {
            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(RequestContext.CurrentDomain);
            if ((lki != null) && (lki.Key.Length == LicenseKeyInfo.TRIAL_KEY_LENGTH) && (lki.ExpirationDateReal != LicenseKeyInfo.TIME_UNLIMITED_LICENSE))
            {
                TimeSpan expiration = lki.ExpirationDateReal.Subtract(DateTime.Now.AddDays(-1));
                // Trial version expiration date
                if (expiration.Ticks <= 0)
                {
                    info = GetString("Trial.Expired");
                }
                else
                {
                    info = string.Format(GetString("Trial.ExpiresIn"), GetExpirationString(expiration.Days));
                }
            }
            else if ((lki != null) && (lki.Edition == ProductEditionEnum.Free))
            {
                info = GetString("header.freeedition");
            }
        }

        ltlText.Text = info;
        pnlTrial.Visible = !string.IsNullOrEmpty(ltlText.Text);
    }


    private void CheckEcommerceLicenseLimitations()
    {
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            bool licenseOK = LicenseHelper.CheckLicenseLimitations(FeatureEnum.Ecommerce, out int skuCount, out int maxSKUCount);

            ltlLicenseLimitations.Text = String.Format(GetString("header.ecommercefeatureexceeded"), skuCount, maxSKUCount, KENTICO_LICENSE_ULTIMATE_URL);
            pnlLicenseLimitations.Visible = !licenseOK;
        }
    }


    /// <summary>
    /// Checks if warning message should be displayed.
    /// </summary>
    /// <param name="key">Session flag key</param>
    private bool CheckWarningMessage(string key)
    {
        return ValidationHelper.GetBoolean(SessionHelper.GetValue(key), true);
    }


    /// <summary>
    /// Ensures callback script to store state into session when message is hidden
    /// </summary>
    private void EnsureHideMessageCallback()
    {
        String cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveMessage", "");
        String callbackScript = "function HideMessage(arg, context) {" + cbReference + "; }";
        ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "SetSessionFlag", callbackScript, true);
    }


    /// <summary>
    /// Ensures that support chat control is inserted correctly into the header if the chat is module is available.
    /// </summary>
    private void EnsureSupportChat()
    {
        if ((ModuleEntryManager.IsModuleLoaded(ModuleName.CHAT)) && (SiteContext.CurrentSiteID > 0))
        {
            CMSUserControl supportChatControl = Page.LoadUserControl("~/CMSModules/Chat/Controls/SupportChatHeader.ascx") as CMSUserControl;
            if (supportChatControl != null)
            {
                plcSupportChat.Controls.Add(supportChatControl);
                plcSupportChat.Visible = true;
            }
        }
    }


    /// <summary>
    /// Ensures that StagingTaskGroupMenu control is inserted correctly into the header.
    /// </summary>
    private void EnsureStagingTaskGroupMenu()
    {
       if (StagingTaskInfoProvider.LoggingOfStagingTasksEnabled(SiteContext.CurrentSiteName))
       {
           CMSUserControl stagingTaskGroupMenu = Page.LoadUserControl("~/CMSAdminControls/UI/StagingTaskGroupMenu.ascx") as CMSUserControl;
           if (stagingTaskGroupMenu != null)
           {
               plcStagingTaskGroupContainer.Visible = true;
               plcStagingTaskGroup.Controls.Add(stagingTaskGroupMenu);
               plcStagingTaskGroup.Visible = true;
           }
       }
    }


    /// <summary>
    /// Gets expiration string according to remaining days to expiration.
    /// </summary>
    /// <param name="days">Days until expiration</param>
    private string GetExpirationString(int days)
    {
        // Check if more than one day till expiration
        if (days > 0)
        {
            return string.Format(GetString("general.validity.days"), days);
        }

        // Check if expiration occurs in less than a day
        if (days == 0)
        {
            return GetString("general.lessthanday");
        }

        return null;
    }

    #endregion


    #region "Control events"

    protected void SiteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Create URL
        int siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
        if (si != null)
        {
            string domain = si.DomainName.TrimEnd('/');
            string url = "~" + VirtualContext.CurrentURLPrefix + "/Admin/cmsadministration.aspx";

            if (domain.Contains("/"))
            {
                // Resolve application path
                url = url.Substring(1);
            }

            url = URLHelper.GetAbsoluteUrl(url, domain, null, null);

            // Check if single sign-on is turned on
            if (SettingsKeyInfoProvider.GetBoolValue("CMSAutomaticallySignInUser"))
            {
                var user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserID);
                url = AuthenticationHelper.GetUserAuthenticationUrl(user, url);
            }

            PortalScriptHelper.RegisterAdminRedirectScript(Page, url);
        }
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Gets callback result
    /// </summary>
    public string GetCallbackResult()
    {
        return null;
    }


    /// <summary>
    /// Handles server call
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        SessionHelper.SetValue(eventArgument, false);
    }

    #endregion
}
