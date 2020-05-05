using System;

using CMS.BannerManagement;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;

public partial class CMSWebParts_BannerManagement_BannerRotator : CMSAbstractWebPart
{
    #region "Private Fields"

    private BannerInfo banner;
    // "Cache" view mode
    private ViewModeEnum currentViewMode = PortalContext.ViewMode;
    // True if banner is taken from viewstate (request was POST). In that case impressions should not be counted
    private bool bannerReused;

    #endregion


    #region "Properties"

    private bool HideIfBannerNotFound
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideIfBannerNotFound"), true);
        }
    }


    private bool KeepPreviousBannerOnPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("KeepPreviousBannerOnPostBack"), true);
        }
    }


    private int? BannerIDViewState
    {
        get
        {
            return ViewState["BannerID"] as int?;
        }
        set
        {
            ViewState["BannerID"] = value;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string bannerCategoryCodeName = ValidationHelper.GetString(GetValue("BannerCategoryCodeName"), "");

        BannerCategoryInfo bannerCategory = BannerCategoryInfoProvider.GetBannerCategoryInfoFromSiteOrGlobal(bannerCategoryCodeName, SiteContext.CurrentSiteID);

        if ((bannerCategory == null) || (bannerCategory.BannerCategoryEnabled == false))
        {
            Visible = !HideIfBannerNotFound;
            return;
        }

        if (RequestHelper.IsPostBack() && KeepPreviousBannerOnPostBack && BannerIDViewState.HasValue)
        {
            bannerReused = true;
            banner = BannerInfoProvider.GetBannerInfo(BannerIDViewState.Value);
        }

        // If random banner should be picked or banner from viewstate was not found
        if (banner == null)
        {
            bannerReused = false;
            // Get random banner from selected category. Decrement hits left for this banner only if page is displayed on the live site.
            banner = BannerInfoProvider.GetRandomValidBanner(bannerCategory.BannerCategoryID, (currentViewMode.IsLiveSite()));
        }

        // Exits if no banner was found
        if (banner == null)
        {
            Visible = !HideIfBannerNotFound;
            return;
        }
        
        // Store banner id in the viewstate if the same banner should be used if request is postback
        if (KeepPreviousBannerOnPostBack)
        {
            BannerIDViewState = banner.BannerID;
        }

        string width = ValidationHelper.GetString(GetValue("Width"), "");
        string height = ValidationHelper.GetString(GetValue("Height"), "");
        string anchorClass = ValidationHelper.GetString(GetValue("AnchorClass"), "");
        bool fakeLink = ValidationHelper.GetBoolean(GetValue("FakeLink"), true);

        if (width != "")
        {
            lnkBanner.Style["width"] = width;
        }
        if (height != "")
        {
            lnkBanner.Style["height"] = height;
        }

        lnkBanner.CssClass = string.Format("CMSBanner {0}", anchorClass).Trim();

        lnkBanner.Visible = true;



        // Do not set link if we are not on the live site.
        if (currentViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
        {
            // Link pointing to our custom handler which logs click and redirects
            string bannerRedirectURL = URLHelper.ResolveUrl("~/CMSModules/BannerManagement/CMSPages/BannerRedirect.ashx?bannerID=" + banner.BannerID);

            if (fakeLink)
            {
                // By default href attribute will be set to 'nice' URL
                lnkBanner.Attributes.Add("href", URLHelper.ResolveUrl(banner.BannerURL));

                // After clicking href will be set to URL pointing to custom handler which counts clicks
                lnkBanner.Attributes.Add("onclick", string.Format("this.href='{0}';", bannerRedirectURL));

                // GECKO doesn't count middle mouse click as click, so onmouseup (or down) needs to be added
                lnkBanner.Attributes.Add("onmouseup", string.Format("this.href='{0}';", bannerRedirectURL));
            }
            else
            {
                // If faking links is disabled, set href to redirect url
                lnkBanner.Attributes.Add("href", bannerRedirectURL);
            }

            // Add target="_blank" attribute if link should be opened in new window
            if (banner.BannerBlank)
            {
                lnkBanner.Target = "_blank";
            }
        }

        if (banner.BannerType == BannerTypeEnum.Image)
        {
            BannerImageAttributes bannerImageAttributes = BannerManagementHelper.DeserializeBannerImageAttributes(banner.BannerContent);

            imgBanner.AlternateText = bannerImageAttributes.Alt;
            imgBanner.ToolTip = bannerImageAttributes.Title;
            imgBanner.CssClass = bannerImageAttributes.Class;
            imgBanner.Style.Value = HTMLHelper.HTMLEncode(bannerImageAttributes.Style);

            imgBanner.ImageUrl = URLHelper.ResolveUrl(bannerImageAttributes.Src);

            imgBanner.Visible = true;
            ltrBanner.Visible = false;
        }
        else
        {
            string text = MacroResolver.Resolve(banner.BannerContent);

            ltrBanner.Text = HTMLHelper.ResolveUrls(text, null, false);
            imgBanner.Visible = false;
            ltrBanner.Visible = true;

            if (banner.BannerType == BannerTypeEnum.HTML)
            {
                ControlsHelper.ResolveDynamicControls(this);
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Log hit only if we are on the live site and banner is not reused from previous request
        if ((currentViewMode == CMS.PortalEngine.ViewModeEnum.LiveSite) && (banner != null) && !bannerReused)
        {
            string currentSiteName = SiteContext.CurrentSiteName;

            if (AnalyticsHelper.AnalyticsEnabled(currentSiteName))
            {
                if (AnalyticsHelper.JavascriptLoggingEnabled(SiteContext.CurrentSiteName))
                {
                    WebAnalyticsServiceScriptsRenderer.RegisterLogBannerHitCall(Page, banner.BannerID);
                }
                else
                {
                    HitLogProvider.LogHit("bannerhit", currentSiteName, null, null, banner.BannerID);
                }
            }
        }
    }

    #endregion
}
