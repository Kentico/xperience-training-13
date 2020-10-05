using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;


public partial class CMSModules_WebAnalytics_Tools_Disabled : CMSWebAnalyticsPage
{
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }
    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Service.Resolve<IAnalyticsSettingEvaluator>().AnalyticsEnabled(SiteContext.CurrentSiteID))
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("default.aspx"));
        }
        else
        {
            ShowWarning(GetString("WebAnalytics.Disabled"));
        }
    }
}