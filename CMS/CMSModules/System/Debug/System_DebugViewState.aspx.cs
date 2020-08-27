using System;
using System.Data;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugViewState : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("Debug.ClearLog");

        ReloadData();
    }


    protected void ReloadData()
    {
        if (!ViewStateDebug.Settings.Enabled)
        {
            ShowWarning(GetString("DebugViewState.NotConfigured"));
        }
        else
        {
            plcLogs.Controls.Clear();

            var logs = ViewStateDebug.Settings.LastLogs;

            RequestLog lastLog = null;

            for (int i = logs.Count - 1; i >= 0; i--)
            {
                try
                {
                    // Get the log
                    var log = logs[i];
                    if (log != null)
                    {
                        // Load the table
                        DataTable dt = log.LogTable;
                        if (!DataHelper.DataSourceIsEmpty(dt))
                        {
                            // Load the control
                            ViewStateLog logCtrl = (ViewStateLog)LoadLogControl(log, "~/CMSAdminControls/Debug/ViewState.ascx", i);

                            logCtrl.PreviousLog = lastLog;
                            logCtrl.DisplayTotalSize = false;
                            logCtrl.DisplayOnlyDirty = chkOnlyDirty.Checked;

                            // Add to the output
                            plcLogs.Controls.Add(logCtrl);

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
        ViewStateDebug.Settings.LastLogs.Clear();
        ReloadData();
    }
}