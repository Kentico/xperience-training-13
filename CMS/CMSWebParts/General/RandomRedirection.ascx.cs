using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_RandomRedirection : CMSAbstractWebPart
{
    #region Webpart properties

    /// <summary>
    /// List of URLs to where webpart could redirect.
    /// </summary>
    public string RedirectionURLs
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectionURLs"), "");
        }
        set
        {
            SetValue("RedirectionURLs", value);
        }
    }

    #endregion


    #region Webpart methods

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
            if ((RedirectionURLs.Trim().Length > 0) &&
                PortalContext.ViewMode.IsLiveSite())
            {
                // Parse URLs string
                string[] URLs = RedirectionURLs.Trim().Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (URLs.Length > 0)
                {
                    // Generate random integer index to array
                    int rndID = new Random().Next(0, URLs.Length);
                    string newURL = UrlResolver.ResolveUrl(URLs[rndID].Trim());
                    if ((RequestContext.CurrentURL != newURL) &&
                        (URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL) != newURL))
                    {
                        URLHelper.ResponseRedirect(newURL);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}