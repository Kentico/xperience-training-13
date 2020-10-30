using System;
using System.Linq;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.Membership.Internal;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Header : CMSUserControl, ICallbackEventHandler
{
    #region "Constants and variables"

    protected const string SESSION_KEY_TECH_PREVIEW = "WRNShowTechPreview";
    protected const string SESSION_KEY_TRIAL = "WRNShowTrial";
    protected const string SESSION_KEY_SUBSCRIPTION_LICENCES = "WRNShowSubscriptionLicences";
    private const string VIRTUALCONTEXT_AUTHENTICATION_ROUTE = "/Kentico.VirtualContext/Authenticate?signInToken={0}";
    private const string SUBSCRIPTION_LICENSES_WARNING_ALREADY_CLOSED_TODAY = "Kentico.SubscriptionLicense.Closed";
    private const string GET_MVC_AUTHENTICATION_CALLBACK = "GET_MVC_AUTHENTICATION_CALLBACK";

    private string callbackResult = null;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ScriptHelper.RegisterModule(this, "CMS/Mole");

        // In case invalid site configuration skip VirtualContextAuhtenticator initialization
        if (!string.IsNullOrWhiteSpace(SiteContext.CurrentSite?.SitePresentationURL)
            && !string.IsNullOrWhiteSpace(SiteContext.CurrentSite?.DomainName))
        {
            // In case current user was not properly initialized redirect to the access denied page
            if (CurrentUser.UserAuthenticationGUID == Guid.Empty)
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetAccessDeniedUrl("membership.usersessioninvalid"));
            }

            ScriptHelper.RegisterModule(this, "CMS/VirtualContextAuthenticator", new
            {
                authenticationFrameUrl = GetMvcAuthenticationFrameUrl(),
                refreshInterval = Convert.ToInt32(new VirtualContextAuthenticationConfiguration().Validity.TotalSeconds)
            });

            var loadAuthenticationFrameCallback = Page.ClientScript.GetCallbackEventReference(this, "arg", "window.CMS.VirtualContextAuthenticator.loadAuthenticationFrame", "");
            var callbackScript = $"function raiseGetAuthenticationFrameUrlCallback() {{  var arg = '{GET_MVC_AUTHENTICATION_CALLBACK}'; {loadAuthenticationFrameCallback}; }}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "loadAuthenticationFrameCallback", callbackScript, true);
        }

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

        CheckSubscriptionLicences();
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

        EnsureStagingTaskGroupMenu();

        lnkDashboard.Attributes.Add("href", "#");
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlPwdExp.Visible = pwdExpiration.Visible;
    }

    #endregion


    #region "Methods"

    private string GetMvcAuthenticationFrameUrl()
    {
        return URLHelper.CombinePath(
                    string.Format(VIRTUALCONTEXT_AUTHENTICATION_ROUTE, new SecurityTokenManager<VirtualContextSignInConfiguration>().GetToken(CurrentUser)),
                    '/',
                    new PresentationUrlRetriever().RetrieveForAdministration(SiteContext.CurrentSiteName, LocalizationContext.PreferredCultureCode),
                    null);
    }


    private void CheckSubscriptionLicences()
    {
        // Hide message if requested by user
        if (!CheckWarningMessage(SESSION_KEY_SUBSCRIPTION_LICENCES))
        {
            pnlSubscriptionLicencesWarning.Visible = false;
            CacheHelper.Add(SUBSCRIPTION_LICENSES_WARNING_ALREADY_CLOSED_TODAY, true, CacheHelper.GetCacheDependency($"{LicenseKeyInfo.OBJECT_TYPE}|all"),
                DateTime.Now.AddDays(1), CacheConstants.NoSlidingExpiration);
            SessionHelper.Remove(SESSION_KEY_SUBSCRIPTION_LICENCES);
            return;
        }

        if (!AreSubscriptionLicensesValid(out int numberOfInvalidLicenses, out bool onlyWarning, out int daysToExpiration))
        {
            if (onlyWarning)
            {
                // Warning was already closed by someone and will be displayed after 24h again
                if (!CacheHelper.TryGetItem(SUBSCRIPTION_LICENSES_WARNING_ALREADY_CLOSED_TODAY, out bool _))
                {
                    pnlSubscriptionLicencesWarning.Visible = true;
                    ltlSubscriptionLicenceWarning.Text = numberOfInvalidLicenses > 1 ?
                        ResHelper.GetStringFormat("subscriptionlicenses.warning.multiple", UrlResolver.ResolveUrl(ApplicationUrlHelper.GetApplicationUrl("Licenses", "Licenses")), numberOfInvalidLicenses) :
                        ResHelper.GetStringFormat("subscriptionlicenses.warning.single", daysToExpiration);
                }
            }
            else
            {
                pnlSubscriptionLicencesError.Visible = true;
                ltlSubscriptionLicenceError.Text = numberOfInvalidLicenses > 1 ?
                    GetMultipleSubscriptionLicensesErrorMessage() :
                    GetSingleSubscriptionLicenseErrorMessage();
            }
        }

        string GetMultipleSubscriptionLicensesErrorMessage()
        {
            return daysToExpiration > 0
                ? ResHelper.GetStringFormat("subscriptionlicenses.error.multiple", UrlResolver.ResolveUrl(ApplicationUrlHelper.GetApplicationUrl("Licenses", "Licenses")), numberOfInvalidLicenses, daysToExpiration)
                : ResHelper.GetString("subscriptionlicenses.error.graceperiodexpired");
        }

        string GetSingleSubscriptionLicenseErrorMessage()
        {
            return daysToExpiration > 0
                ? ResHelper.GetStringFormat("subscriptionlicenses.error.single", daysToExpiration)
                : ResHelper.GetString("subscriptionlicenses.error.graceperiodexpired");
        }
    }


    private bool AreSubscriptionLicensesValid(out int numberOfInvalidLicenses, out bool onlyWarning, out int daysToExpiration)
    {
        numberOfInvalidLicenses = 0;
        onlyWarning = false;
        daysToExpiration = 0;

        var subscriptionLicenses = CacheHelper.Cache(cs =>
        {
            cs.CacheDependency = CacheHelper.GetCacheDependency($"{LicenseKeyInfo.OBJECT_TYPE}|all");

            return LicenseKeyInfoProvider.GetLicenseKeys()
                                         .TypedResult
                                         .Where(l => l.LicenseGuid != null);

        }, new CacheSettings(60, "Header", "AreSubscriptionLicensesValid"));

        var expiredLicenses = subscriptionLicenses.Where(license => DateIsLessThan30Days(license.ExpirationDateReal));

        if (expiredLicenses.Any())
        {
            numberOfInvalidLicenses = expiredLicenses.Count();
            daysToExpiration = expiredLicenses.Min(license => (license.ExpirationDateReal.Date - DateTime.Now.Date).Days);
            onlyWarning = false;

            return false;
        }


        // Subscription licenses have grace period of 30 days included in their real expiration date during which error message above is calculated.
        // From the customers point of view the warning is displayed 30 days before the license expires.
        // The license has not yet entered the grace period thats why we subtract the 30 day grace period to calculate 
        // expiration and remaining days for the warning message.
        var aboutToExpireLicenses = subscriptionLicenses.Where(license => DateIsLessThan30Days(license.ExpirationDateReal.AddDays(-LicenseListControlExtender.SUBSCRIPTION_LICENSE_EXPIRATION_GRACE_DAYS)));

        if (aboutToExpireLicenses.Any())
        {
            numberOfInvalidLicenses = aboutToExpireLicenses.Count();
            daysToExpiration = aboutToExpireLicenses.Min(license => (license.ExpirationDateReal.AddDays(-LicenseListControlExtender.SUBSCRIPTION_LICENSE_EXPIRATION_GRACE_DAYS).Date - DateTime.Now.Date).Days);
            onlyWarning = true;
            return false;
        }

        return true;

        bool DateIsLessThan30Days(DateTime licenseExpirationDate)
        {
            return (licenseExpirationDate.Date - DateTime.Now.Date).Days <= 30;
        }
    }


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

            ltlLicenseLimitations.Text = String.Format(GetString("header.ecommercefeatureexceeded"), skuCount, maxSKUCount);
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
        var cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveMessage", "");
        var callbackScript = $"function HideMessage(arg, context) {{{cbReference}; }}";
        ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "SetSessionFlag", callbackScript, true);
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
        SiteInfo si = SiteInfo.Provider.Get(siteId);
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
                var user = UserInfo.Provider.Get(MembershipContext.AuthenticatedUser.UserID);
                url = AuthenticationHelper.GetUserAuthenticationUrl(user, url, si.DomainName);
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
        return callbackResult;
    }


    /// <summary>
    /// Handles server call
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case GET_MVC_AUTHENTICATION_CALLBACK:
                callbackResult = GetMvcAuthenticationFrameUrl();
                break;

            default:
                SessionHelper.SetValue(eventArgument, false);
                break;
        }
    }

    #endregion
}
