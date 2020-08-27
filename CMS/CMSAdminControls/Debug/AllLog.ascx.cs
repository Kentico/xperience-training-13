using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_AllLog : AllLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (!StopProcessing)
        {
            var dt = MergeLogs(Logs, Page, ShowCompleteContext);

            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                // Setup header texts
                gridDebug.SetHeaders("", "AllLog.DebugType", "AllLog.Information", "AllLog.Result", "General.Context", "AllLog.TotalDuration", "AllLog.Duration");

                // Bind the data
                BindGrid(gridDebug, dt);
            }
        }
    }
}