using System;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


[Title("ViewRequest.Title")]
public partial class CMSModules_System_Debug_System_ViewRequest : CMSDebugPage
{
    private Guid mGuid = Guid.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.IsDialog = false;

        mGuid = QueryHelper.GetGuid("guid", Guid.Empty);
        if (mGuid != Guid.Empty)
        {
            // Find the logs
            var logs = DebugHelper.FindRequestLogs(mGuid);
            if (logs != null)
            {
                plcLogs.Append("<div><strong>&nbsp;", HTMLHelper.HTMLEncode(logs.RequestURL), "</strong> (", logs.RequestTime.ToString("hh:MM:ss"), ")</div><br />");

                foreach (var debug in DebugHelper.RegisteredDebugs)
                {
                    // Check if the debug is enabled
                    if (debug.Enabled && !string.IsNullOrEmpty(debug.LogControl))
                    {
                        // Load the debug control
                        var log = LoadUserControl(debug.LogControl) as LogControl;
                        if (log != null)
                        {
                            log.Logs = logs;

                            plcLogs.Append(log);
                        }
                    }
                }
            }
        }
    }
}