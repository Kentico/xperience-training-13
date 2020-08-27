using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_Log : CMSDebugPage
{
    private readonly List<LogControl> mLogControls = new List<LogControl>();


    /// <summary>
    /// Debug settings
    /// </summary>
    public DebugSettings Settings
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the debug settings
        var name = QueryHelper.GetString("name", "");

        Settings = DebugHelper.GetSettings(name);

        if (Settings == null)
        {
            btnClear.Visible = false;
            chkCompleteContext.Visible = false;

            return;
        }

        ReloadData();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (Settings != null)
        {
            if (mLogControls.Count > 0)
            {
                double totalDuration = 0;

                // Summarize all logs
                foreach (var log in mLogControls)
                {
                    totalDuration += log.TotalDuration;
                }

                if (totalDuration > 0)
                {
                    lblInfo.Text = String.Format(GetString("RequestLog.Total"), totalDuration, mLogControls.Count);
                }
            }
            else if (Settings.Enabled)
            {
                lblInfo.Text = GetString("RequestLog.NotFound");
            }
        }

        base.Render(writer);
    }


    /// <summary>
    /// Reloads the debug data
    /// </summary>
    protected void ReloadData()
    {
        if (!Settings.Enabled)
        {
            ShowWarning(GetString("Debug" + Settings.Name + ".NotConfigured"));
        }
        else
        {
            plcLogs.Controls.Clear();

            var logs = Settings.LastLogs;

            LoadLogs(logs);
        }
    }


    /// <summary>
    /// Loads the logs control for each request log and setups the control
    /// </summary>
    /// <param name="logs">List of request logs</param>
    private void LoadLogs(List<RequestLog> logs)
    {
        RequestLog lastLog = null;

        // Load the logs
        for (int i = logs.Count - 1; i >= 0; i--)
        {
            try
            {
                // Get the log
                var log = logs[i];
                if (log != null)
                {
                    if ((log.Value != null) || !DataHelper.DataSourceIsEmpty(log.LogTable))
                    {
                        // Load the control
                        var logCtrl = LoadLogControl(log, Settings.LogControl, i);

                        logCtrl.PreviousLog = lastLog;
                        logCtrl.ShowCompleteContext = chkCompleteContext.Checked;

                        // Add to the output
                        plcLogs.Controls.Add(logCtrl);

                        mLogControls.Add(logCtrl);

                        lastLog = log;
                    }
                }
            }
            catch
            {
                // Suppress error
            }
        }
    }


    /// <summary>
    /// Clears the current logs
    /// </summary>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Settings.LastLogs.Clear();

        mLogControls.Clear();

        ReloadData();
    }


    /// <summary>
    /// Clears the cache
    /// </summary>
    protected void btnClearCache_Click(object sender, EventArgs e)
    {
        CacheHelper.ClearCache();

        ShowConfirmation(GetString("Administration-System.ClearCacheSuccess"));

        ReloadData();
    }
}