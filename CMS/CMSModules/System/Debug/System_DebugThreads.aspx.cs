using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;

using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugThreads : CMSDebugPage
{
    #region "Variables"

    protected int index = 0;
    protected TimeSpan totalDuration = new TimeSpan(0);
    protected DateTime now = DateTime.Now;

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        now = DateTime.Now;

        btnRunDummy.Text = GetString("DebugThreads.Test");

        gridThreads.SetHeaders("", "unigrid.actions", "ThreadsLog.Context", "ThreadsLog.ThreadID", "ThreadsLog.Status", "ThreadsLog.Started", "ThreadsLog.Duration");
        gridFinished.SetHeaders("", "unigrid.actions", "ThreadsLog.Context", "ThreadsLog.ThreadID", "ThreadsLog.Status", "ThreadsLog.Started", "ThreadsLog.Finished", "ThreadsLog.Duration");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Cancel", ScriptHelper.GetScript(
            @"function CancelThread(threadGuid) {
                if (confirm(" + ScriptHelper.GetLocalizedString("ViewLog.CancelPrompt") + @")) {
                    document.getElementById('" + hdnGuid.ClientID + "').value = threadGuid;" +
            Page.ClientScript.GetPostBackEventReference(btnCancel, null) +
            @"}
              }"));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
        
        // Hide headers if there is nothing to show
        headThreadsRunning.Visible = gridThreads.HasControls();
        headThreadsFinished.Visible = gridFinished.HasControls();

        ScriptHelper.RegisterDialogScript(this);
    }


    protected void ReloadData()
    {
        LoadGrid(gridThreads, ThreadDebug.LiveThreadItems);
        LoadGrid(gridFinished, ThreadDebug.FinishedThreadItems);
    }


    /// <summary>
    /// Loads the grid with the data.
    /// </summary>
    /// <param name="grid">Grid to load</param>
    /// <param name="threads">List of threads</param>
    protected void LoadGrid(GridView grid, List<ThreadDebugItem> threads)
    {
        index = 0;

        // Ensure new collection for binding
        grid.DataSource = threads.OrderByDescending(thr => thr.ThreadStarted).ToList();

        // Bind the grid
        grid.DataBind();
    }


    protected void btnRunDummy_Click(object sender, EventArgs e)
    {
        LogContext.EnsureLog(Guid.NewGuid());

        CMSThread dummy = new CMSThread(RunTest);
        dummy.Start();

        Thread.Sleep(100);
        ReloadData();
    }


    private void RunTest()
    {
        for (int i = 0; i < 50; i++)
        {
            Thread.Sleep(100);
            LogContext.AppendLine("Sample log " + i);
        }
    }


    /// <summary>
    /// Gets the item index.
    /// </summary>
    protected int GetIndex()
    {
        return ++index;
    }


    /// <summary>
    /// Gets the duration of the thread.
    /// </summary>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    protected string GetDuration(object startTime, object endTime)
    {
        TimeSpan duration = ValidationHelper.GetDateTime(endTime, now).Subtract(ValidationHelper.GetDateTime(startTime, now));
        totalDuration = totalDuration.Add(duration);

        return GetDurationString(duration);
    }


    /// <summary>
    /// Gets the duration as formatted string.
    /// </summary>
    /// <param name="duration">Duration to get</param>
    protected string GetDurationString(TimeSpan duration)
    {
        string result = null;
        if (duration.TotalHours >= 1)
        {
            result += duration.Hours + ":";
            result += duration.Minutes.ToString().PadLeft(2, '0') + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else if (duration.TotalMinutes >= 1)
        {
            result += duration.Minutes + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else
        {
            result = duration.TotalSeconds.ToString("F3");
        }

        return result;
    }


    /// <summary>
    /// Gets the log action for the thread.
    /// </summary>
    /// <param name="hasLog">Log presence</param>
    /// <param name="threadGuid">Thread GUID</param>
    protected string GetLogAction(object hasLog, object threadGuid)
    {
        string result = null;

        bool logAvailable = ValidationHelper.GetBoolean(hasLog, false);
        if (logAvailable)
        {
            string url = UrlResolver.ResolveUrl("~/CMSModules/System/Debug/System_ViewLog.aspx");
            url = URLHelper.UpdateParameterInUrl(url, "threadGuid", threadGuid.ToString());
            if (WebFarmHelper.WebFarmEnabled)
            {
                url = URLHelper.UpdateParameterInUrl(url, "serverName", WebFarmHelper.ServerName);
            }

            var button = new CMSGridActionButton
            {
                IconCssClass = "icon-eye",
                IconStyle = GridIconStyle.Allow,
                ToolTip = GetString("General.View"),
                OnClientClick = "modalDialog('" + url + "', 'ThreadProgress'); return false;"
            };

            result = button.GetRenderedHTML();
        }

        return result;
    }


    /// <summary>
    /// Gets the debug action for the thread.
    /// </summary>
    /// <param name="requestGuid">Request GUID for the debug</param>
    protected string GetDebugAction(object requestGuid)
    {
        string result = null;

        if (DebugHelper.AnyDebugEnabled)
        {
            var rGuid = ValidationHelper.GetGuid(requestGuid, Guid.Empty);
            if (rGuid != Guid.Empty)
            {
                var logs = DebugHelper.FindRequestLogs(rGuid);
                if (logs != null)
                {
                    string url = LogControl.GetLogsUrl(rGuid);

                    var button = new CMSGridActionButton
                    {
                        IconCssClass = "icon-bug",
                        IconStyle = GridIconStyle.Allow,
                        ToolTip = GetString("General.Debug"),
                        OnClientClick = "modalDialog('" + url + "', 'ThreadDebug'); return false;"
                    };

                    result = button.GetRenderedHTML();
                }
            }
        }

        return result;
    }


    /// <summary>
    /// Gets the cancel action for the thread.
    /// </summary>
    /// <param name="threadGuid">Thread GUID</param>
    /// <param name="status">Status</param>
    protected string GetCancelAction(object threadGuid, object status)
    {
        string result = null;
        
        if (ValidationHelper.GetString(status, null) != "AbortRequested")
        {
            var button = new CMSGridActionButton
            {
                IconCssClass = "icon-times-circle",
                IconStyle = GridIconStyle.Critical,
                ToolTip = GetString("General.Cancel"),
                OnClientClick = "CancelThread('" + threadGuid + "'); return false;"
            };

            result = button.GetRenderedHTML();
        }
        
        return result;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Guid threadGuid = ValidationHelper.GetGuid(hdnGuid.Value, Guid.Empty);
        CMSThread thread = CMSThread.GetThread(threadGuid);
        if (thread != null)
        {
            thread.Stop();
        }
    }


    protected void timRefresh_Tick(object sender, EventArgs e)
    {
    }
}
