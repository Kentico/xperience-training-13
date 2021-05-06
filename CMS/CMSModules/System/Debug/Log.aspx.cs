using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
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


    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get the debug settings
        var name = QueryHelper.GetString("name", "");

        Settings = DebugHelper.GetSettings(name);

        if (Settings == null)
        {
            btnClear.Visible = false;
            chkCompleteContext.Visible = false;

            return;
        }

        if (ShowLiveSiteData)
        {
            pnlHeaderActions.Parent.Controls.Clear();
        }

        await ReloadData().ConfigureAwait(false);
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
    private async Task ReloadData()
    {
        if (!Settings.Enabled)
        {
            ShowWarning(GetString("Debug" + Settings.Name + ".NotConfigured"));
        }
        else
        {
            plcLogs.Controls.Clear();

            var logs = ShowLiveSiteData ? await new LiveSiteDebugProcessor().GetLastLogsAsync(Settings.Name) : Settings.LastLogs;

            LoadLogs(logs);
        }
    }


    /// <summary>
    /// Loads the logs control for each request log and setups the control
    /// </summary>
    /// <param name="logs">List of request logs</param>
    private void LoadLogs(IEnumerable<RequestLog> logs)
    {
        if (logs == null)
        {
            return;
        }

        RequestLog lastLog = null;
        List<RequestLog> logsList = logs.ToList();

        // Load the logs
        for (int i = logsList.Count - 1; i >= 0; i--)
        {
            try
            {
                // Get the log
                var log = logsList[i];
                if (log != null)
                {
                    if ((log.Value != null) || !DataHelper.DataSourceIsEmpty(log.LogTable))
                    {
                        if (log.Settings == null)
                        {
                            log.Settings = Settings;
                        }

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
    protected async void btnClear_Click(object sender, EventArgs e)
    {
        Settings.LastLogs.Clear();

        mLogControls.Clear();

        await ReloadData().ConfigureAwait(false);
    }


    /// <summary>
    /// Clears the cache
    /// </summary>
    protected async void btnClearCache_Click(object sender, EventArgs e)
    {
        CacheHelper.ClearCache();

        ShowConfirmation(GetString("Administration-System.ClearCacheSuccess"));

        await ReloadData().ConfigureAwait(false);
    }
}
