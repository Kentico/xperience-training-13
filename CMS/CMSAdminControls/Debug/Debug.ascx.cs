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

        foreach (var debug in DebugHelper.RegisteredDebugs)
        {
            // Check if the debug is enabled for live pages
            if (debug.Live && !string.IsNullOrEmpty(debug.LogControl))
            {
                // Load the debug control
                var log = LoadUserControl(debug.LogControl) as LogControl;
                if (log != null)
                {
                    logControls.Add(log);

                    plcDebugs.Append(log);
                }
            }
        }
      
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
