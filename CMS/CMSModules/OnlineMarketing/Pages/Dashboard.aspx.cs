using System;

using CMS.Core;
using CMS.UIControls;


[UIElement(ModuleName.ONLINEMARKETING, "OMDashBoard")]
public partial class CMSModules_OnlineMarketing_Pages_Dashboard : DashboardPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        cmsDashboard.SetupSiteDashboard();
    }
}