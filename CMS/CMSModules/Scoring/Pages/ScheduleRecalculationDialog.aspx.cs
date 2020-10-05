using System;
using System.Threading.Tasks;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;


[Title("om.score.recalculate")]
public partial class CMSModules_Scoring_Pages_ScheduleRecalculationDialog : CMSModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        QueryHelper.ValidateHash("hash");
        ScoreInfo scoreInfo = ScoreInfo.Provider.Get(QueryHelper.GetInteger("scoreID", 0));

        if (scoreInfo == null)
        {
            ShowError(GetString("general.objectnotfound"));
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            LoadValuesFromExistingScore(scoreInfo);
        }

        string tooltipTextKey = scoreInfo.ScorePersonaID > 0 ? "persona.recalculationwarninglong" : "om.score.recalculationwarninglong";
        
        ShowWarning(GetString("om.score.recalculationwarning"), null, GetString(tooltipTextKey));

        Save += (s, ea) => ScheduleRecalculation(scoreInfo);
    }


    public void radGroupRecalculate_CheckedChanged(object sender, EventArgs e)
    {
        calendarControl.Enabled = radLater.Checked;
    }


    protected void ScheduleRecalculation(ScoreInfo scoreInfo)
    {
        // Validate input
        if (radLater.Checked && !(calendarControl.SelectedDateTime > DateTime.Now))
        {
            ShowError(GetString("om.score.recalculationscheduledinvaliddate"));
            return;
        }

        if (!scoreInfo.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(scoreInfo.TypeInfo.ModuleName, "modify");
        }

        if (radLater.Checked && (calendarControl.SelectedDateTime > DateTime.Now))
        {
            StartRecalculationLater(scoreInfo);
        }
        else if (radNow.Checked)
        {
            StartRecalculationNow(scoreInfo);
        }

        ScriptHelper.RegisterWOpenerScript(this);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshPage", ScriptHelper.GetScript("wopener.RefreshPage(); CloseDialog();"));
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads controls with values from existing score.
    /// </summary>
    /// <param name="score"></param>
    private void LoadValuesFromExistingScore(ScoreInfo score)
    {
        if (score.ScoreScheduledTaskID > 0)
        {
            TaskInfo taskInfo = TaskInfo.Provider.Get(score.ScoreScheduledTaskID);
            if ((taskInfo != null) && taskInfo.TaskEnabled)
            {
                radLater.Checked = true;
                calendarControl.Enabled = true;
                calendarControl.SelectedDateTime = taskInfo.TaskNextRunTime;
            }
        }
    }


    /// <summary>
    /// Recalculates score at time that user specified.
    /// </summary>
    /// <param name="score">Score to recalculate</param>
    private void StartRecalculationLater(ScoreInfo score)
    {
        // Set info for scheduled task
        var task = ScoreInfoProvider.EnsureScheduledTask(score, String.Empty, TaskInfoProvider.NO_TIME, false, false);
        task.TaskNextRunTime = calendarControl.SelectedDateTime;
        task.TaskDeleteAfterLastRun = true;
        task.TaskEnabled = true;
        TaskInfo.Provider.Set(task);

        // Update score
        score.ScoreScheduledTaskID = task.TaskID;
        ScoreInfo.Provider.Set(score);
    }


    /// <summary>
    /// Immediately recalculates score.
    /// </summary>
    /// <param name="score">Score to recalculate</param>
    private void StartRecalculationNow(ScoreInfo score)
    {
        if (score.ScoreStatus == ScoreStatusEnum.Recalculating)
        {
            // Score is already being recalculated
            return;
        }

        // Delete already scheduled task, it is not needed
        if (score.ScoreScheduledTaskID > 0)
        {
            TaskInfo.Provider.Get(score.ScoreScheduledTaskID)?.Delete();
            score.ScoreScheduledTaskID = 0;
        }

        // Set score as recalculating before running the async recalculator, so the change is displayed at the UI immediatelly
        ScoreInfoProvider.MarkScoreAsRecalculating(score);

        // Recalculate the score
        ScoreAsyncRecalculator recalculator = new ScoreAsyncRecalculator(score);
        Task result = recalculator.RunAsync();
        LogTaskException(result);
    }


    /// <summary>
    /// Logs exceptions thrown within task runtime.
    /// </summary>
    /// <param name="task">Task which exceptions should be logged</param>
    private void LogTaskException(Task task)
    {
        task.ContinueWith(CMSThread.Wrap(
            (Task t) =>
                Service.Resolve<IEventLogService>().LogException("Score",
                    "SCORE_RECALCULATION",
                    t.Exception)
            ),
            TaskContinuationOptions.OnlyOnFaulted);
    }

    #endregion
}
