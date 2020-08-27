using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_MacroLog : MacroLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup header texts
            gridMacros.SetHeaders("", "MacroLog.Expression", "MacroLog.Result", "MacroLog.IdentityAndUser", "General.Context", "MacroLog.Duration");

            HeaderText = GetString("MacroLog.Info");

            // Bind the data
            BindGrid(gridMacros, dt);
        }
    }
}