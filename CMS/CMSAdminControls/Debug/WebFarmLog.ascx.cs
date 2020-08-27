using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_WebFarmLog : WebFarmLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            gridQueries.SetHeaders("", "WebFarmLog.TaskType", "WebFarmLog.Target", "WebFarmLog.TextData", "General.Context");

            HeaderText = GetString("WebFarmLog.Info");

            // Bind the data
            BindGrid(gridQueries, dt);
        }
    }
}