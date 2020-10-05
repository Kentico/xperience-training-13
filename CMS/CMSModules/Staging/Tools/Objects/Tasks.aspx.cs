using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Staging", "Objects")]
public partial class CMSModules_Staging_Tools_Objects_Tasks : CMSStagingTasksPage
{
    #region "Constants & Variables"

    // Header action event name
    private const string SYNCHRONIZE_CURRENT = "SYNCCURRENT";

    private int synchronizedSiteId;
    private string objectType = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Event code suffix for task event names
    /// </summary>
    protected override string EventCodeSuffix
    {
        get
        {
            return "OBJECT";
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
    /// The task type that is used when cheking priviliges for links generated for the task grid.
    /// </summary>
    protected override string TaskType
    {
        get
        {
            return "Objects";
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check 'Manage objects tasks' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageObjectsTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageObjectsTasks");
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

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Setup server dropdown
        selectorElem.DropDownList.AutoPostBack = true;
        selectorElem.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

        // Set server ID
        SelectedServerID = ValidationHelper.GetInteger(selectorElem.Value, QueryHelper.GetInteger("serverId", 0));

        // All servers
        if (SelectedServerID == UniSelector.US_ALL_RECORDS)
        {
            SelectedServerID = 0;
            selectorElem.Value = UniSelector.US_ALL_RECORDS;
        }
        else
        {
            selectorElem.Value = SelectedServerID.ToString();
        }

        ltlScript.Text += ScriptHelper.GetScript("var currentServerId = " + SelectedServerID + ";");

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        if (!isCallback)
        {
            // Check 'Manage object tasks' permission
            if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageObjectsTasks"))
            {
                RedirectToAccessDenied("cms.staging", "ManageObjectsTasks");
            }

            synchronizedSiteId = QueryHelper.GetInteger("siteid", 0);
            CurrentSiteID = SiteContext.CurrentSiteID;
            
            ucDisabledModule.TestSettingKeys = "CMSStagingLogObjectChanges";
            ucDisabledModule.ParentPanel = pnlNotLogged;

            if (synchronizedSiteId == -1)
            {
                ucDisabledModule.InfoText = GetString("objectstaging.globalandsitenotlogged");
                ucDisabledModule.KeyScope = DisabledModuleScope.CurrentSiteAndGlobal;
                ucDisabledModule.GlobalButtonText = GetString("objectstaging.enableglobalandsiteobjects");
            }
            else if (synchronizedSiteId == 0)
            {
                ucDisabledModule.InfoText = GetString("objectstaging.globalnotlogged");
                ucDisabledModule.KeyScope = DisabledModuleScope.Global;
                ucDisabledModule.GlobalButtonText = GetString("objectstaging.enableglobalobjects");
            }
            else
            {
                ucDisabledModule.InfoText = GetString("ObjectStaging.SiteNotLogged");
                ucDisabledModule.KeyScope = DisabledModuleScope.Site;
                ucDisabledModule.SiteButtonText = GetString("objectstaging.enablesiteobjects");
            }

            // Check logging
            if (!ucDisabledModule.Check())
            {
                CurrentMaster.PanelHeader.Visible = false;
                plcContent.Visible = false;
                return;
            }

            // Get object type
            objectType = QueryHelper.GetString("objecttype", string.Empty);

            // Create "synchronize current" header action for tree root, nodes or objects with database representation
            if (String.IsNullOrEmpty(objectType) || objectType.StartsWith("##", StringComparison.Ordinal) || (ModuleManager.GetReadOnlyObject(objectType) != null))
            {
                HeaderActions.AddAction(new HeaderAction
                {
                    Text = GetString("ObjectTasks.SyncCurrent"),
                    EventName = SYNCHRONIZE_CURRENT
                });
            }

            // Add CSS class to panels wrapper in order it could be stacked
            CurrentMaster.PanelHeader.AddCssClass("header-container-multiple-panels");

            // Setup title
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Get the selected types
            ObjectTypeTreeNode selectedNode = StagingTaskInfoProvider.ObjectTree.FindNode(objectType, (synchronizedSiteId > 0));
            objectType = (selectedNode != null) ? selectedNode.GetObjectTypes(true) : string.Empty;
            if (!ControlsHelper.CausedPostBack(HeaderActions, btnSyncSelected, btnSyncAll))
            {
                // Register the dialog script
                ScriptHelper.RegisterDialogScript(this);

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
                TaskTypeCategories = TaskHelper.TASK_TYPE_CATEGORY_GENERAL + ";" + TaskHelper.TASK_TYPE_CATEGORY_OBJECTS + ";" + TaskHelper.TASK_TYPE_CATEGORY_DATA;
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
        string where = GetSiteWhere();
        WhereCondition mergedWhere = new WhereCondition(completeWhere).And().Where(where);

        var tasksQuery = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, mergedWhere.ToString(true), currentOrder, currentTopN, columns, currentOffset, currentPageSize);
        var result = tasksQuery.Result;
        totalRecords = tasksQuery.TotalRecords;

        return result;
    }


    /// <summary>
    /// Gets the basic where condition for the tasks.
    /// </summary>
    protected string GetSiteWhere()
    {
        string where = null;

        if (synchronizedSiteId > 0)
        {
            // Site tasks
            where = "TaskSiteID = " + synchronizedSiteId + " AND TaskType NOT IN (N'" + TaskTypeEnum.AddToSite + "', N'" + TaskTypeEnum.RemoveFromSite + "')";
        }
        else if (synchronizedSiteId == 0)
        {
            // Global tasks
            where = "TaskSiteID IS NULL OR TaskType IN (N'" + TaskTypeEnum.AddToSite + "', N'" + TaskTypeEnum.RemoveFromSite + "')";
        }

        return where;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "changeServer", ScriptHelper.GetScript("ChangeServer(" + SelectedServerID + ");"));
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// All items synchronization.
    /// </summary>
    protected void SynchronizeAll(object parameter)
    {
        RunAction("Synchronization", "SYNCALLOBJECTS", SynchronizeAllInternal);
    }


    private string SynchronizeAllInternal()
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        string where = new WhereCondition()
            .Where(GetSiteWhere())
            .And()
            .Where(gridTasks.CustomFilter.WhereCondition)
            .ToString(true);

        DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, @where, "TaskID", -1, "TaskID");

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
    }


    /// <summary>
    /// Synchronization of selected items.
    /// </summary>
    /// <param name="parameter">List of selected items</param>
    public void SynchronizeSelected(object parameter)
    {
        List<String> list = parameter as List<String>;
        if (list == null)
        {
            return;
        }

        RunAction("Synchronization", "SYNCSELECTEDOBJECT", () => SynchronizeSelectedInternal(list));
    }


    private string SynchronizeSelectedInternal(IEnumerable<string> list)
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(list);
    }


    /// <summary>
    /// Synchronizes the current object type.
    /// </summary>
    private void SynchronizeCurrent(object parameter)
    {
        RunAction("Synchronization", "SYNCCURRENTOBJECT", SynchronizeCurrentInternal);
    }


    private string SynchronizeCurrentInternal()
    {
        int sid = SelectedServerID;
        if (sid <= 0)
        {
            sid = SynchronizationInfoProvider.ENABLED_SERVERS;
        }

        string result = null;

        // Process all types
        string[] syncTypes = objectType.Split(';');
        foreach (string syncType in syncTypes)
        {
            if (syncType != string.Empty)
            {
                AddLog(String.Format(GetString("Synchronization.LoggingTasks"), syncType));

                // Scheduled tasks have disabled staging, they have to be dealt in extra manner
                if (syncType.Equals(TaskInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                {
                    CreateStagingTasksForScheduledTasks(synchronizedSiteId, CurrentSiteID);
                }
                else
                {
                    // Create update tasks
                    SynchronizationHelper.LogObjectChange(syncType, synchronizedSiteId, DateTimeHelper.ZERO_TIME, TaskTypeEnum.UpdateObject, true, false, false, false, false, CurrentSiteID, sid);
                }

                AddLog(GetString("Synchronization.RunningTasks"));

                // Get the tasks
                string where = GetSiteWhere();
                DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, syncType, @where, "TaskID", -1, "TaskID");

                // Run the synchronization
                result += StagingTaskRunner.RunSynchronization(ds);
            }
        }

        return result;
    }


    private void CreateStagingTasksForScheduledTasks(int objectSiteId, int currentSiteId)
    {
        var where = new WhereCondition().And(new WhereCondition().WhereNull("TaskType").Or().WhereNotEquals("TaskType", (int)ScheduledTaskTypeEnum.System));

        // Synchronize tree root (all objects from current site and all global objects)
        if (objectSiteId < 0)
        {
            where.WhereEqualsOrNull("TaskSiteID", currentSiteId);
        }
        // Synchronize global objects
        else if (objectSiteId == 0)
        {
            where.WhereNull("TaskSiteID");
        }
        // Synchronize site objects
        else
        {
            where.WhereEquals("TaskSiteID", objectSiteId);
        }

        using (new CMSActionContext() { AllowAsyncActions = false })
        {
            var tasks = TaskInfo.Provider.Get().Where(where).TypedResult;
            foreach (var task in tasks)
            {
                task.Generalized.StoreSettings();
                task.Generalized.LogSynchronization = SynchronizationTypeEnum.LogSynchronization;
                task.Generalized.LogEvents = true;

                TaskInfo.Provider.Set(task);

                task.Generalized.RestoreSettings();
            }
        }
    }


    /// <summary>
    /// Deletes selected tasks.
    /// </summary>
    protected void DeleteSelected(object parameter)
    {
        var list = parameter as List<String>;
        if (list == null)
        {
            return;
        }

        RunAction("Deletion", "DELETESELECTEDOBJECTS", () => DeleteTasks(list));
    }


    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    protected void DeleteAll(object parameter)
    {
        RunAction("Deletion", "DELETEALLOBJECTS", DeleteAllInternal);
    }


    private string DeleteAllInternal()
    {
        AddLog(GetString("Synchronization.DeletingTasks"));

        // Process all records
        var where = new WhereCondition()
            .Where(GetSiteWhere())
            .And()
            .Where(gridTasks.CustomFilter.WhereCondition)
            .ToString(true);

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, @where, "TaskID", -1, "TaskID, TaskTitle");

        DeleteTasks(ds);

        return null;
    }

    #endregion


    #region "Button handling"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == SYNCHRONIZE_CURRENT)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(SynchronizeCurrent);
        }
    }


    protected void btnSyncSelected_Click(object sender, EventArgs e)
    {
        var list = gridTasks.SelectedItems;
        if (list.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(p => SynchronizeSelected(list));
        }
    }


    protected void btnSyncAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");

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
        var list = gridTasks.SelectedItems;
        if (list.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");
            
            // Run asynchronous action
            RunAsync(p => DeleteSelected(list));
        }
    }

    #endregion
}
