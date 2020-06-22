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


[UIElement("CMS.Staging", "TaskGroup")]
public partial class CMSModules_Staging_Tools_TaskGroup_TaskGroup : CMSStagingTasksPage
{
    #region "Variables"

    private TaskGroupInfo mCurrentTaskGroup;

    #endregion


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


    /// <summary>
    /// Current task group we get from UIContext, it does not matter if we get here from UI or via link generated from UILinkHelper.
    /// </summary>
    protected TaskGroupInfo CurrentTaskGroup
    {
        get
        {
            if (mCurrentTaskGroup == null)
            {
                mCurrentTaskGroup = TaskGroupInfo.Provider.Get(UIContext.ObjectID);
            }

            return mCurrentTaskGroup;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Loads the page.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckPermissions();
        CurrentMaster.DisplaySiteSelectorPanel = true;

        // Check enabled servers
        var isCallback = RequestHelper.IsCallback();
        if (!isCallback && !ServerInfoProvider.IsEnabledServer(SiteContext.CurrentSiteID))
        {
            ShowInformation(GetString("ObjectStaging.NoEnabledServer"));
            HideUI();
            return;
        }

        InitServerSelector();
        mCurrentTaskGroup = TaskGroupInfo.Provider.Get(UIContext.ObjectID);

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        if (!isCallback)
        {
            CheckPermissions();

            PrepareDisabledModuleInfo();

            if (!ucDisabledModule.Check())
            {
                HideUI();
                return;
            }

            ScriptHelper.RegisterDialogScript(this);

            // Setup title
            if (!ControlsHelper.CausedPostBack(btnSyncSelected, btnSyncAll))
            {
                plcContent.Visible = true;

                InitButtons();
                InitUniGrid();
                TaskGroupSelectorEnabled = false;

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

    private DataSet gridTasks_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        completeWhere = new WhereCondition(completeWhere).And().WhereIn("TaskID", TaskGroupTaskInfo.Provider.Get().WhereEquals("TaskGroupID", CurrentTaskGroup.TaskGroupID).Column("TaskID")).ToString(true);

        // Get the tasks
        var tasksQuery = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, completeWhere, currentOrder, currentTopN, columns, currentOffset, currentPageSize);
        var result = tasksQuery.Result;
        totalRecords = tasksQuery.TotalRecords;

        return result;
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// All items synchronization.
    /// </summary>
    private void SynchronizeAll(object parameter)
    {
        RunAction("Synchronization", "SYNCALLTASKS", SynchronizeAllInternal);
    }


    /// <summary>
    /// All items synchronization.
    /// </summary>
    private string SynchronizeAllInternal()
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, GridTasks.CustomFilter.WhereCondition, "TaskID", -1, "TaskID")
                                            .WhereIn("TaskID", TaskGroupTaskInfo.Provider.Get().WhereEquals("TaskGroupID", CurrentTaskGroup.TaskGroupID).Column("TaskID"));

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
    }


    /// <summary>
    /// Synchronization of selected items.
    /// </summary>
    /// <param name="list">List of selected items</param>
    private void SynchronizeSelected(List<String> list)
    {
        if (list == null)
        {
            return;
        }

        RunAction("Synchronization", "SYNCSELECTEDTASKS", () => SynchronizeSelectedInternal(list));
    }


    /// <summary>
    /// Synchronization of selected items.
    /// </summary>
    /// <param name="list">List of selected items</param>
    private string SynchronizeSelectedInternal(IEnumerable<string> list)
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Run the synchronization
        var result = StagingTaskRunner.RunSynchronization(list);

        return result;
    }


    /// <summary>
    /// Deletes selected tasks.
    /// </summary>
    private void DeleteSelected(List<String> list)
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
    private void DeleteAll(object parameter)
    {
        RunAction("Deletion", "DELETEALLTASKS", DeleteAllInternal);
    }


    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    private string DeleteAllInternal()
    {
        AddLog(GetString("Synchronization.DeletingTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectTaskList(CurrentSiteID, SelectedServerID, GridTasks.CustomFilter.WhereCondition, "TaskID", -1, "TaskID, TaskTitle")
                                            .WhereIn("TaskID", TaskGroupTaskInfo.Provider.Get().WhereEquals("TaskGroupID", CurrentTaskGroup.TaskGroupID).Column("TaskID"));

        DeleteTasks(ds);

        return null;
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Handles sync selected button's click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnSyncSelected_Click(object sender, EventArgs e)
    {
        if (gridTasks.SelectedItems.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(p => SynchronizeSelected(gridTasks.SelectedItems));
        }
    }


    /// <summary>
    /// Handles sync all button's click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnSyncAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");
        CurrentInfo = GetString("Tasks.SynchronizationCanceled");

        // Run asynchronous action
        RunAsync(SynchronizeAll);
    }


    /// <summary>
    /// Handles delete all button's click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnDeleteAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");

        // Run asynchronous action
        RunAsync(DeleteAll);
    }


    /// <summary>
    /// Handles delete selected button's click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
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


    #region "Private Methods"

    /// <summary>
    /// Inits server selector events and properties.
    /// </summary>
    private void InitServerSelector()
    {
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
    }


    /// <summary>
    /// Check if user has sufficient permissions.
    /// </summary>
    private void CheckPermissions()
    {
        // Check 'Manage object tasks' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageAllTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageAllTasks");
        }
    }


    /// <summary>
    /// Inits unigrid events and data.
    /// </summary>
    private void InitUniGrid()
    {
        // Initialize grid
        gridTasks.ZeroRowsText = GetString("Tasks.NoTasks");
        gridTasks.OnDataReload += gridTasks_OnDataReload;
        gridTasks.ShowActionsMenu = true;
        gridTasks.Columns = "TaskID, TaskSiteID, TaskDocumentID, TaskNodeAliasPath, TaskTitle, TaskTime, TaskType, TaskObjectType, TaskObjectID, TaskRunning, (SELECT COUNT(*) FROM Staging_Synchronization WHERE SynchronizationTaskID = TaskID AND SynchronizationErrorMessage IS NOT NULL AND (SynchronizationServerID = @ServerID OR (@ServerID = 0 AND (@TaskSiteID = 0 OR SynchronizationServerID IN (SELECT ServerID FROM Staging_Server WHERE ServerSiteID = @TaskSiteID AND ServerEnabled=1))))) AS FailedCount";
        gridTasks.WhereCondition = new WhereCondition().WhereIn("TaskID", TaskGroupInfo.Provider.Get().WhereEquals("TaskGroupID", CurrentTaskGroup.TaskGroupID).Column("TaskID")).ToString(true);
        StagingTaskInfo ti = new StagingTaskInfo();
        gridTasks.AllColumns = SqlHelper.MergeColumns(ti.ColumnNames);
    }


    /// <summary>
    /// Inits buttons that synchronize and delete tasks in grid.
    /// </summary>
    private void InitButtons()
    {
        // Initialize buttons
        btnDeleteAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Tasks.ConfirmDeleteAll")) + ");";
        btnDeleteSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";
        btnSyncSelected.OnClientClick = "return !" + gridTasks.GetCheckSelectionScript();
    }


    /// <summary>
    /// Inits disabled module info if user does not have setting keys.
    /// </summary>
    private void PrepareDisabledModuleInfo()
    {
        ucDisabledModule.TestAnyKey = true;
        ucDisabledModule.TestSettingKeys = "CMSStagingLogObjectChanges;CMSStagingLogDataChanges;CMSStagingLogChanges";
        ucDisabledModule.ParentPanel = pnlNotLogged;
        ucDisabledModule.InfoText = GetString("staging.disabledModule.allTasks");
    }


    /// <summary>
    /// Hides UI if user does not have sufficient permissions.
    /// </summary>
    private void HideUI()
    {
        CurrentMaster.PanelHeader.Visible = false;
        plcContent.Visible = false;
        pnlFooter.Visible = false;
    }

    #endregion
}
