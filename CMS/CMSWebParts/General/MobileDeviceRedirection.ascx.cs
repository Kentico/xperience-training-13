using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_General_MobileDeviceRedirection : CMSAbstractWebPart
{
    #region "Constants"

    private const string SETTINGS_DEVICE_AUTOMATIC = "automatic";
    private const string SETTINGS_DEVICE_SMALL = "small";
    private const string SETTINGS_DEVICE_LARGE = "large";

    #endregion


    #region "Properties"

    /// <summary>
    /// Small device redirection URL.
    /// </summary>
    public string SmallDeviceRedirectionURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SmallDeviceRedirectionURL"), string.Empty);
        }
        set
        {
            SetValue("SmallDeviceRedirectionURL", value);
        }
    }


    /// <summary>
    /// Large device redirection URL.
    /// </summary>
    public string LargeDeviceRedirectionURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LargeDeviceRedirectionURL"), string.Empty);
        }
        set
        {
            SetValue("LargeDeviceRedirectionURL", value);
        }
    }


    /// <summary>
    /// Redirect iPhone.
    /// </summary>
    public string RedirectIPhone
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectIPhone"), string.Empty);
        }
        set
        {
            SetValue("RedirectIPhone", value);
        }
    }


    /// <summary>
    /// Redirect Nokia.
    /// </summary>
    public string RedirectNokia
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectNokia"), string.Empty);
        }
        set
        {
            SetValue("RedirectNokia", value);
        }
    }


    /// <summary>
    /// Redirect BlackBerry.
    /// </summary>
    public string RedirectBlackBerry
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectBlackBerry"), string.Empty);
        }
        set
        {
            SetValue("RedirectBlackBerry", value);
        }
    }


    /// <summary>
    /// Redirect iPad.
    /// </summary>
    public string RedirectIPad
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectIPad"), string.Empty);
        }
        set
        {
            SetValue("RedirectIPad", value);
        }
    }


    /// <summary>
    /// Redirect Android.
    /// </summary>
    public string RedirectAndroid
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectAndroid"), string.Empty);
        }
        set
        {
            SetValue("RedirectAndroid", value);
        }
    }


    /// <summary>
    /// Always redirect.
    /// </summary>
    public bool AlwaysRedirect
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AlwaysRedirect"), false);
        }
        set
        {
            SetValue("AlwaysRedirect", value);
        }
    }


    /// <summary>
    /// Other small devices (User agent).
    /// </summary>
    public string OtherSmallDevices
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OtherSmallDevices"), string.Empty);
        }
        set
        {
            SetValue("OtherSmallDevices", value);
        }
    }


    /// <summary>
    /// Other large devices (User agent).
    /// </summary>
    public string OtherLargeDevices
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OtherLargeDevices"), string.Empty);
        }
        set
        {
            SetValue("OtherLargeDevices", value);
        }
    }

    /// <summary>
    /// Indicates whether current querystring values should be used for redirect URL
    /// </summary>
    public bool PreserveQueryString
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PreserveQueryString"), false);
        }
        set
        {
            SetValue("PreserveQueryString", value);
        }
    }

    #endregion


    #region "Webpart methods"

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
        if (!StopProcessing)
        {
            // Trim blank characters
            string rawSmallDeviceURL = SmallDeviceRedirectionURL.Trim();
            string rawLargeDeviceURL = LargeDeviceRedirectionURL.Trim();

            // Get redirect cookie
            string redirected = CookieHelper.GetValue(CookieName.MobileRedirected);

            if ((AlwaysRedirect || String.IsNullOrEmpty(redirected)) && PortalContext.ViewMode.IsLiveSite())
            {
                string redirectUrl = null;

                // Get user agent name or user agent string
#pragma warning disable CS0618 // Type or member is obsolete
                string userAgent = BrowserHelper.GetUserAgent();
#pragma warning restore CS0618 // Type or member is obsolete

                if (!string.IsNullOrEmpty(userAgent))
                {
                    UserAgentEnum userAgentEnum = GetUserAgentName(userAgent);

                    if (userAgentEnum != UserAgentEnum.Unknown)
                    {
                        redirectUrl = GetRedirectUrl(userAgent, userAgentEnum, rawSmallDeviceURL, rawLargeDeviceURL);
                    }
                    else
                    {
                        redirectUrl = CheckUsersRedirection(userAgent, rawSmallDeviceURL, rawLargeDeviceURL);
                    }

                    // Check if some address is specified
                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        string newURL = UrlResolver.ResolveUrl(redirectUrl);

                        newURL = EnsureQueryValues(newURL);

                        // If current URL is same as set, no redirection is done
                        if ((!RequestContext.CurrentURL.EqualsCSafe(newURL, true)) && (!URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL).EqualsCSafe(newURL, true)))
                        {
                            // Set redirected cookie
                            CookieHelper.SetValue(CookieName.MobileRedirected, "true", DateTimeHelper.ZERO_TIME);
                            URLHelper.ResponseRedirect(newURL);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Ensures combination of query string values with dependence on PreserveQueryString setting
    /// </summary>
    /// <param name="newUrl">newUrl</param>
    private string EnsureQueryValues(string newUrl)
    {
        if (PreserveQueryString)
        {
            // Get current QueryString
            string query = URLHelper.GetQuery(RequestContext.CurrentURL);
            if (!String.IsNullOrEmpty(query))
            {
                // Try combine with QueryString in new URL
                string newUrlQuery = URLHelper.GetQuery(newUrl);
                if (!String.IsNullOrEmpty(newUrlQuery))
                {
                    query = URLHelper.MergeQueryStrings(newUrlQuery, query, true);
                    newUrl = URLHelper.RemoveQuery(newUrl);
                }

                // Append query to the final URL
                newUrl = URLHelper.AppendQuery(newUrl, query);
            }
        }

        return newUrl;
    }


    /// <summary>
    /// Gets the redirect settings for specific user agent type
    /// </summary>
    /// <param name="userAgentEnum">User agent type</param>
    private string GetRedirectSettings(UserAgentEnum userAgentEnum)
    {
        switch (userAgentEnum)
        {
            case UserAgentEnum.BlackBerry:
                return RedirectBlackBerry;

            case UserAgentEnum.IPhone:
                return RedirectIPhone;

            case UserAgentEnum.IPad:
                return RedirectIPad;

            case UserAgentEnum.Nokia:
                return RedirectNokia;

            case UserAgentEnum.Android:
                return RedirectAndroid;
        }
        return String.Empty;
    }


    /// <summary>
    /// Returns redirect URL for specified user agent and redirection settings.
    /// </summary>
    protected string GetRedirectUrl(string userAgent, UserAgentEnum userAgentEnum, string smallUrl, string largeUrl)
    {
        switch (GetRedirectSettings(userAgentEnum))
        {
            case SETTINGS_DEVICE_AUTOMATIC:
                // Try get user defined redirect
                string url = CheckUsersRedirection(userAgent, smallUrl, largeUrl);
                // Return user defined redirect or URL defined by device type settings
                return (!string.IsNullOrEmpty(url)) ? url : GetAutomaticDeviceUrl(userAgentEnum, smallUrl, largeUrl);

            case SETTINGS_DEVICE_SMALL:
                return smallUrl;

            case SETTINGS_DEVICE_LARGE:
                return largeUrl;

            default:
                return string.Empty;
        }
    }


    /// <summary>
    /// Returns URL specified by automatic logic
    /// </summary>
    /// <param name="userAgentEnum">User agent enum type</param>
    /// <param name="smallUrl">Small URL</param>
    /// <param name="largeUrl">Large URL</param>
    private string GetAutomaticDeviceUrl(UserAgentEnum userAgentEnum, string smallUrl, string largeUrl)
    {
        switch (userAgentEnum)
        {
            case UserAgentEnum.IPad:
                return largeUrl;

            default:
                return smallUrl;
        }
    }


    /// <summary>
    /// Returns user agent enum for given user agent string.
    /// </summary>
    /// <param name="userAgent">User agent</param>
    protected UserAgentEnum GetUserAgentName(string userAgent)
    {
        userAgent = userAgent.ToLowerCSafe();
        foreach (UserAgentEnum userAgentType in Enum.GetValues(typeof(UserAgentEnum)))
        {
            if ((userAgentType != UserAgentEnum.Unknown) && userAgent.Contains(userAgentType.ToString().ToLowerCSafe()))
            {
                return userAgentType;
            }
        }

        return UserAgentEnum.Unknown;
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected string CheckUsersRedirection(string userAgent, string smallUrl, string largeUrl)
    {
        userAgent = userAgent.ToLowerCSafe();

        string[] largeDevices = OtherLargeDevices.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Check large devices
        if (largeDevices.Length > 0)
        {
            // Check all specified devices for match
            foreach (string t in largeDevices)
            {
                if (userAgent.Contains(t.ToLowerCSafe()))
                {
                    return largeUrl;
                }
            }
        }

        string[] smallDevices = OtherSmallDevices.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Check small devices
        if (smallDevices.Length > 0)
        {
            // Check all specified devices for match
            foreach (string t in smallDevices)
            {
                if (userAgent.Contains(t.ToLowerCSafe()))
                {
                    return smallUrl;
                }
            }
        }

        // Check if visitor use mobile device
#pragma warning disable CS0618 // Type or member is obsolete
        if (BrowserHelper.IsMobileDevice())
#pragma warning restore CS0618 // Type or member is obsolete
        {
            return smallUrl;
        }

        return string.Empty;
    }

    #endregion
}