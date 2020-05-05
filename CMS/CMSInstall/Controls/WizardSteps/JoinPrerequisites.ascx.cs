using System;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;
using CMS.WinServiceEngine;

public partial class CMSInstall_Controls_WizardSteps_JoinPrerequisites : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Indicates if scheduled tasks were manually stopped by a user.
    /// </summary>
    public bool TasksManuallyStopped
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["TasksManuallyStopped"], false);
        }
        set
        {
            ViewState["TasksManuallyStopped"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnStopTasks.Click += btnStopTasks_Click;
        spanScreenReader.Text = iconHelp.ToolTip = GetString("separationDB.enabledtasksjoin");
    }


    protected override void OnPreRender(EventArgs e)
    {
        DisplayTaskStatus();

        if (!SqlInstallationHelper.DatabaseIsSeparated())
        {
            DisplaySeparationError(GetString("separationDB.joinerror"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if DB join can be launched.
    /// </summary>
    public bool IsValidForDBJoin()
    {
        if (SchedulingHelper.EnableScheduler || SchedulingHelper.IsAnyTaskRunning())
        {
            DisplaySeparationError(GetString("separationDB.stoptaskserrorjoin"));
            return false;
        }

        if (!SqlInstallationHelper.DatabaseIsSeparated())
        {
            DisplaySeparationError(GetString("separationDB.joinerror"));
            return false;
        }

        // Test if separation process is not already started.
        if (DatabaseSeparationHelper.SeparationInProgress)
        {
            DisplaySeparationError(GetString("separationDB.processalreadystarted"));
            return false;
        }

        iconHelp.Visible = btnStopTasks.Visible = true;
        return true;
    }


    /// <summary>
    /// Checks if any tasks are running.
    /// </summary>
    private void DisplayTaskStatus()
    {
        if (SchedulingHelper.EnableScheduler || SchedulingHelper.IsAnyTaskRunning())
        {
            if (!btnStopTasks.Visible && TasksManuallyStopped && (hdnTurnedOff.Value != bool.TrueString))
            {
                DisplayStoppingTasks();
            }
            else
            {
                lblStatusValue.Text = "<span class=\"task-error\">" + GetString("general.enabled") + "</span>";
                plcTasks.Visible = true;
                lblTaskStatus.Visible = true;
                lblStatusValue.Visible = true;
                ltlStatus.Visible = false;
                iconHelp.Visible = btnStopTasks.Visible = true;
            }
        }
        else
        {
            if (TasksManuallyStopped)
            {
                lblTaskStatus.Visible = true;
                lblStatusValue.Visible = true;
                plcTasks.Visible = true;
                ltlStatus.Visible = false;
                ltlStatus.Text = null;
                lblStatusValue.Text = "<span class=\"task-success\">" + GetString("general.disabled") + "</span>";
                iconHelp.Visible = btnStopTasks.Visible = false;
            }
            else
            {
                plcTasks.Visible = true;
                lblTaskStatus.Visible = false;
                lblStatusValue.Visible = false;
                ltlStatus.Visible = false;
                iconHelp.Visible = btnStopTasks.Visible = false;
            }
            plcInfo.Visible = !plcSeparationError.Visible;
            hdnTurnedOff.Value = bool.TrueString;
        }
    }


    /// <summary>
    /// Displays that tasks are being stopped.
    /// </summary>
    private void DisplayStoppingTasks()
    {
        ltlStatus.Text = ScriptHelper.GetLoaderInlineHtml(GetString("general.disabling"));
        plcTasks.Visible = true;
        lblTaskStatus.Visible = true;
        lblStatusValue.Visible = true;
        ltlStatus.Visible = true;
        iconHelp.Visible = btnStopTasks.Visible = false;
    }


    /// <summary>
    /// Displays separation error.
    /// </summary>
    private void DisplaySeparationError(string error)
    {
        plcSeparationError.Visible = true;
        lblErrorTasks.Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength(error, 180));
        lblErrorTasks.ToolTip = error;
        plcInfo.Visible = false;
    }


    /// <summary>
    /// Stop tasks.
    /// </summary>
    void btnStopTasks_Click(object sender, EventArgs e)
    {
        // Stop tasks
        PersistentStorageHelper.SetValue("CMSSchedulerTasksEnabled", SettingsKeyInfoProvider.GetBoolValue("CMSSchedulerTasksEnabled"));
        if (SchedulingHelper.EnableScheduler)
        {
            SettingsKeyInfoProvider.SetGlobalValue("CMSSchedulerTasksEnabled", false);
        }

        WinServiceHelper.RestartService(WinServiceHelper.HM_SERVICE_BASENAME, false);

        // Display stopping progress
        iconHelp.Visible = btnStopTasks.Visible = false;
        DisplayStoppingTasks();
        TasksManuallyStopped = true;
    }

    #endregion
}
