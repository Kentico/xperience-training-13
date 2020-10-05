using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_Debug : CMSUserControl
{
    private readonly List<LogControl> logControls = new List<LogControl>();


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ScriptHelper.RegisterModule(this, "CMS/DebugControl", pnlDebugContainer.ClientID);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        // Do not render if nothing is displayed
        foreach (var logControl in logControls)
        {
            if (logControl.Visible)
            {
                base.Render(writer);

                DebugContext.DebugPresentInResponse = true;

                break;
            }
        }
    }
}
