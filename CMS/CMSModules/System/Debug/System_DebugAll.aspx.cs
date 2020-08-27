using System;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugAll : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("DebugAll.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!RequestDebug.Settings.Enabled)
        {
            ShowWarning(GetString("DebugRequests.NotConfigured"));
        }
        else
        {
            plcLogs.Controls.Clear();

            var requestLogs = RequestDebug.Settings.LastLogs;

            RequestLog lastLog = null;

            for (int i = requestLogs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the request log
                    var log = requestLogs[i];
                    if (log != null)
                    {
                        // Load the control only if there is more than only request log
                        var logs = log.ParentLogs;
                        if (logs != null)
                        {
                            AllLog logCtrl = (AllLog)LoadLogControl(log, "~/CMSAdminControls/Debug/AllLog.ascx", i);

                            logCtrl.PreviousLog = lastLog;
                            logCtrl.Logs = logs;
                            logCtrl.ShowCompleteContext = chkCompleteContext.Checked;

                            // Add to the output
                            plcLogs.Append(logCtrl);

                            lastLog = log;
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        DebugHelper.ClearLogs();

        ReloadData();
    }
}