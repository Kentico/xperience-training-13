using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_GoogleServices_GoogleAnalytics : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or set tracking code for google analytics.
    /// </summary>
    public string TrackingCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackingCode"), "");
        }
        set
        {
            SetValue("TrackingCode", value);
        }
    }


    /// <summary>
    /// Get or set type of analytics tracking
    /// Single domain – value 0
    /// One domain with multiple subdomains – value 1
    /// Multiple top-level domains – value 2
    /// </summary>
    public int TrackingType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TrackingType"), 0);
        }
        set
        {
            SetValue("TrackingType", value);
        }
    }


    /// <summary>
    /// Indicates if asynchronous version of script should be used.
    /// </summary>
    public bool UseAsyncScript
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseAsyncScript"), false);
        }
        set
        {
            SetValue("UseAsyncScript", value);
        }
    }


    /// <summary>
    /// Main website domain to be used for tracking.
    /// </summary>
    public string MainWebsiteDomain
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MainWebsiteDomain"), null);
        }
        set
        {
            SetValue("MainWebsiteDomain", value);
        }
    }

    #endregion


    #region "Webpart events"

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
            if (PortalContext.ViewMode.IsLiveSite())
            {
                string googleScript = string.Empty;

                // Register necessary Google scripts
                if (UseAsyncScript)
                {
                    googleScript = "(function() {\n" +
                                      "var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;\n" +
                                      "\nga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';\n" +
                                      "var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);\n" +
                                      "})();";
                }
                else
                {
                    googleScript = "var gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");\n" +
                                   "document.write(unescape(\"%3Cscript src='\" + gaJsHost + \"google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E\"));";
                }


                // Register final script to a page
                ScriptHelper.RegisterStartupScript(this, typeof(string), "GoogleAnalyticsScript", ScriptHelper.GetScript(googleScript));

                // Create custom tracking script
                string trackingScript = "if ( (parent == null) || (!parent.IsCMSDesk) ) {\ntry { \n";
                if (UseAsyncScript)
                {
                    trackingScript += "var _gaq = _gaq || [];\n" +
                                      "_gaq.push(['_setAccount', '" + TrackingCode + "']);\n";
                }
                else
                {
                    trackingScript += "var pageTracker = _gat._getTracker('" + TrackingCode + "');\n";
                }

                switch (TrackingType)
                {
                    // One domain with multiple subdomains
                    case 1:
                        if (UseAsyncScript)
                        {
                            trackingScript += "_gaq.push(['_setDomainName', '" + GetMainDomain() + "']);\n";
                        }
                        else
                        {
                            trackingScript += "pageTracker._setDomainName('" + GetMainDomain() + "');\n";
                        }
                        break;

                    // Multiple top-level domains 
                    case 2:
                        if (UseAsyncScript)
                        {
                            trackingScript += "_gaq.push(['_setDomainName', '" + GetMainDomain() + "']);\n" +
                                              "_gaq.push(['_setAllowLinker', true]);\n";
                        }
                        else
                        {
                            trackingScript += "pageTracker._setDomainName('" + GetMainDomain() + "');\n" +
                                              "pageTracker._setAllowLinker(true);\n";
                        }
                        break;

                    // Single domain
                    default:
                        break;
                }

                if (UseAsyncScript)
                {
                    trackingScript += "_gaq.push(['_trackPageview']);\n";
                }
                else
                {
                    trackingScript += "pageTracker._trackPageview();\n";
                }

                trackingScript += "} catch(err) {}\n}";

                // Register final script to a page
                ScriptHelper.RegisterStartupScript(this, typeof(string), "GoogleAnalyticsTracking" + "_" + ClientID, ScriptHelper.GetScript(trackingScript));
            }
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Gets main domain part from current domain.
    /// </summary>
    /// <returns>Main domain part</returns>
    private string GetMainDomain()
    {
        // Check if custom property should be used
        if (string.IsNullOrEmpty(MainWebsiteDomain))
        {
            string currentDomain = RequestContext.CurrentDomain;
            string[] domainParts = currentDomain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            // Check if the domain name consists of more than 2 domain levels
            if (domainParts.Length > 2)
            {
                currentDomain = string.Join(".", domainParts, 1, domainParts.Length - 1);
            }

            return currentDomain;
        }

        return MainWebsiteDomain;
    }

    #endregion
}
