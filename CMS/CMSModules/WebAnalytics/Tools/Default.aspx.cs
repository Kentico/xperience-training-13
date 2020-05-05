using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.WebAnalytics.Web.UI;


public partial class CMSModules_WebAnalytics_Tools_Default : CMSWebAnalyticsPage
{
    protected override void OnPreRender(EventArgs e)
    {
        analyticsTree.Attributes["src"] = "Analytics_Statistics.aspx" + RequestContext.CurrentQueryString;

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        base.OnPreRender(e);
    }
}