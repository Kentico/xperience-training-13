using System;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSMessages_SiteOffline : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set service unavailable state
        Response.StatusCode = 503;

        // Set title
        titleElem.TitleText = GetString("Error.SiteOffline");
        SiteInfo currentSite = SiteContext.CurrentSite;
        if (currentSite != null)
        {
            if (currentSite.SiteIsOffline)
            {
                // Site is offline
                if (!String.IsNullOrEmpty(currentSite.SiteOfflineMessage))
                {
                    lblInfo.Text = MacroResolver.Resolve(currentSite.SiteOfflineMessage);
                }
                else
                {
                    lblInfo.Text = GetString("error.siteisoffline");
                }
            }
            else
            {
                // Redirect to the root
                URLHelper.ResponseRedirect("~/");
            }
        }
    }
}