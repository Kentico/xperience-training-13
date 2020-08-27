using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_SecurityLog : SecurityLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup header columns
            gridSec.SetHeaders("", "SecurityLog.UserName", "SecurityLog.Operation", "SecurityLog.Result", "SecurityLog.Resource", "SecurityLog.Name", "SecurityLog.Site", "General.Context");

            HeaderText = GetString("SecurityLog.Info");

            // Bind the data
            BindGrid(gridSec, dt);
        }
    }
}