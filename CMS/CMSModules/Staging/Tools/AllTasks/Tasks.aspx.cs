using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Staging", "All")]
public partial class CMSModules_Staging_Tools_AllTasks_Tasks : CMSStagingTasksPage
{
    #region "Properties"

    /// <summary>
    /// Event code suffix for task event names
    /// </summary>
    protected override string EventCodeSuffix
    {
        get
        {
            return "TASK";
        }
    }


    /// <summary>
    /// Grid with the task listing
    /// </summary>
    protected override UniGrid GridTasks
    {
        get
        {
            return gridTasks;
        }
    }


    /// <summary>
    /// Async control
    /// </summary>
    protected override AsyncControl AsyncControl
    {
        get
        {
            return ctlAsyncLog;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check 'Manage object tasks' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageAllTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageAllTasks");
        }

        CurrentMaster.DisplaySiteSelectorPanel = true;

        // Check enabled servers
        var isCallback = RequestHelper.IsCallback();
        if (!isCallback && !ServerInfoProvider.IsEnabledServer(SiteContext.CurrentSiteID))
        {
            ShowInformation(GetString("ObjectStaging.NoEnabledServer"));
            CurrentMaster.PanelHeader.Visible = false;
            plcContent.Visible = false;
            pnlFooter.Visible = false;
            return;
        }

        // Setup server dropdown
        selectorElem.DropDownList.AutoPostBack = true;
        selectorElem.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        // Set server ID
        SelectedServerID = ValidationHelper.GetInteger(selectorElem.Value, 0);

        // All servers
        if (SelectedServerID == UniSelector.US_ALL_RECORDS)
        {
            SelectedServerID = 0;
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        if (!isCallback)
        {
            // Check 'Manage object tasks' permission
            if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageAllTasks"))
            {
                RedirectToAccessDenied("cms.staging", "ManageAllTasks");
            }

            ucDisabledModule.TestAnyKey = true;
            ucDisabledModule.TestSettingKeys = "CMSStagingLogObjectChanges;CMSStagingLogDataChanges;CMSStagingLogChanges";
            ucDisabledModule.ParentPanel = pnlNotLogged;
            ucDisabledModule.InfoText = GetString("staging.disabledModule.allTasks");

            if (!ucDisabledModule.Check())
            {
                CurrentMaster.PanelHeader.Visible = false;
                plcContent.Visible = false;
                pnlFooter.Visible = false;
                return;
            }

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(this);

            // Setup title
            if (!ControlsHelper.CausedPostBack(btnSyncSelected, btnSyncAll))
            {
                plcContent.Visible = true;

                // Initialize buttons
                btnDeleteAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Tasks.ConfirmDeleteAll")) + ");";
                btnDeleteSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";
                btnSyncSelected.OnClientClick = "return !" + gridTasks.GetCheckSelectionScript();

                // Initialize grid
                gridTasks.ZeroRowsText = GetString("Tasks.NoTasks");
                gridTasks.OnDataReload += gridTasks_OnDataReload;
                gridTasks.ShowActionsMenu = true;
                gridTasks.Columns = "TaskID, TaskSiteID, TaskDocumentID, TaskNodeAliasPath, TaskTitle, TaskTime, TaskType, TaskObjectType, TaskObjectID, TaskRunning, (SELECT COUNT(*) FROM Staging_Synchronization WHERE SynchronizationTaskID = TaskID AND SynchronizationErrorMessage IS NOT NULL AND (SynchronizationServerID = @ServerID OR (@ServerID = 0 AND (@TaskSiteID = 0 OR SynchronizationServerID IN (SELECT ServerID FROM Staging_Server WHERE ServerSiteID = @TaskSiteID AND ServerEnabled=1))))) AS FailedCount";

                StagingTaskInfo ti = new StagingTaskInfo();
                gridTasks.AllColumns = SqlHelper.MergeColumns(ti.ColumnNames);

                pnlLog.Visible = false;
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlFooter.Visible = !gridTasks.IsEmpty;
    }


    /// <summary>
    /// Executes given action asynchronously
    /// </summary>
    /// <param name="action">Action to run</param>
    protected override void RunAsync(AsyncAction action)
    {
        base.RunAsync(action);

        pnlLog.Visible = true;
        plcContent.Visible = false;
    }

    #endregion


    #region "Grid events & methods"

    protected DataSet gridTasks_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Get the tasks
        var tasksQuery = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, completeWhere, currentOrder, currentTopN, columns, currentOffset, currentPageSize);
        var result = tasksQuery.Result;
        totalRecords = tasksQuery.TotalRecords;

        return result;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// All items synchronization.
    /// </summary>
    protected void SynchronizeAll(object parameter)
    {
        RunAction("Synchronization", "SYNCALLTASKS", SynchronizeAllInternal);
    }


    private string SynchronizeAllInternal()
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, GridTasks.CustomFilter.WhereCondition, "TaskID", -1, "TaskID");

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
    }


    /// <summary>
    /// Synchronization of selected items.
    /// </summary>
    /// <param name="list">List of selected items</param>
    public void SynchronizeSelected(List<String> list)
    {
        if (list == null)
        {
            return;
        }

        RunAction("Synchronization", "SYNCSELECTEDTASKS", () => SynchronizeSelectedInternal(list));
    }


    private string SynchronizeSelectedInternal(IEnumerable<string> list)
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(list);
    }


    /// <summary>
    /// Deletes selected tasks.
    /// </summary>
    protected void DeleteSelected(List<String> list)
    {
        if (list == null)
        {
            return;
        }

        RunAction("Deletion", "DELETESELECTEDTASKS", () => DeleteTasks(list));
    }


    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    protected void DeleteAll(object parameter)
    {
        RunAction("Deletion", "DELETEALLTASKS", DeleteAllInternal);
    }


    private string DeleteAllInternal()
    {
        AddLog(GetString("Synchronization.DeletingTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, gridTasks.CustomFilter.WhereCondition, "TaskID", -1, "TaskID, TaskTitle");

        DeleteTasks(ds);

        return null;
    }

    #endregion


    #region "Button handling"

    protected void btnSyncSelected_Click(object sender, EventArgs e)
    {
        if (gridTasks.SelectedItems.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");
            
            // Run asynchronous action
            RunAsync(p => SynchronizeSelected(gridTasks.SelectedItems));
        }
    }


    protected void btnSyncAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");
        CurrentInfo = GetString("Tasks.SynchronizationCanceled");

        // Run asynchronous action
        RunAsync(SynchronizeAll);
    }


    protected void btnDeleteAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");

        // Run asynchronous action
        RunAsync(DeleteAll);
    }


    protected void btnDeleteSelected_Click(object sender, EventArgs e)
    {
        if (gridTasks.SelectedItems.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");
            
            // Run asynchronous action
            RunAsync(p => DeleteSelected(gridTasks.SelectedItems));
        }
    }

    #endregion
}
