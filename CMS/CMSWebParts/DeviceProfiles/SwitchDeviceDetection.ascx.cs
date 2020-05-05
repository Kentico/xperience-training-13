using System;

using CMS.Core;
using CMS.DeviceProfiles;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_DeviceProfiles_SwitchDeviceDetection : CMSAbstractWebPart
{

    private bool? mShowDesktopVersion = null;
    
    #region Properties

    /// <summary>
    /// Gets or sets LinkContentMobile property.
    /// </summary>
    public string LinkContentMobile
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LinkContentMobile"), String.Empty);
        }
        set
        {
            SetValue("LinkContentMobile", value);
        }
    }


    /// <summary>
    /// Gets or sets LinkContentDesktop property.
    /// </summary>
    public string LinkContentDesktop
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LinkContentDesktop"), String.Empty);
        }
        set
        {
            SetValue("LinkContentDesktop", value);
        }
    }


    /// <summary>
    /// Indicates if visitor wants to show desktop version of the page
    /// </summary>
    public bool ShowDesktopVersion
    {
        get
        {
            if (!mShowDesktopVersion.HasValue)
            {
                mShowDesktopVersion = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.ShowDesktopVersion), false);
            }
            return mShowDesktopVersion.Value;
        }
        set
        {
            CookieHelper.SetValue(CookieName.ShowDesktopVersion, value.ToString(), DateTime.Now.AddYears(1));
            mShowDesktopVersion = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        var cookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();

        // Check if current device is desktop
        if ((cookieLevelProvider.GetCurrentCookieLevel() < CookieLevel.Essential) || (String.IsNullOrEmpty(DeviceContext.CurrentDeviceProfileName) && String.IsNullOrEmpty(DeviceProfileInfoProvider.GetOriginalCurrentDevicProfileName(CurrentSite.SiteName))))
        {
            pnlContent.Visible = false;
            return;
        } 

        bool useDesktop = ViewMode.IsLiveSite() ? ShowDesktopVersion : String.IsNullOrEmpty(DeviceContext.CurrentDeviceProfileName);
        lnkLink.Text = ContextResolver.ResolveMacros(useDesktop ? LinkContentDesktop : LinkContentMobile);
    }


    protected void lnkLink_OnClick(object sender, EventArgs e)
    {
        if (ViewMode != ViewModeEnum.LiveSite)
        {
            return;
        }
        ShowDesktopVersion = ShowDesktopVersion ? false : true;
        URLHelper.Redirect(RequestContext.CurrentURL);
    }

}
