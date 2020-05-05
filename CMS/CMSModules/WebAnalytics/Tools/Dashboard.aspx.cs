using System;

using CMS.Core;
using CMS.UIControls;


[UIElement(ModuleName.WEBANALYTICS, "Dashboard")]
public partial class CMSModules_WebAnalytics_Tools_Dashboard : DashboardPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        cmsDashboard.SetupSiteDashboard();
    }
}