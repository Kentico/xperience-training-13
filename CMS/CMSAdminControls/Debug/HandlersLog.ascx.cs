using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_HandlersLog : HandlersLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup headers text
            gridStates.SetHeaders("", "HandlersLog.Name", "General.Context");

            HeaderText = GetString("HandlersLog.Info");

            // Bind the data
            BindGrid(gridStates, dt);
        }
    }
}