using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Staging", "Data")]
public partial class CMSModules_Staging_Tools_Data_Tasks : CMSStagingTasksPage
{
    #region "Protected variables"

    // Header action event name
    private const string SYNCHRONIZE_CURRENT = "SYNCCURRENTDATA";
    private const string SYNCHRONIZE_COMPLETE = "COMPLETESYNC";

    private const string BASE_TASK_WHERE = "TaskSiteID IS NULL";

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
            return "DATA";
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
            return "Data";
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check 'Manage data tasks' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageDataTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageDataTasks");
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
            if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageDataTasks"))
            {
                RedirectToAccessDenied("cms.staging", "ManageDataTasks");
            }

            ucDisabledModule.KeyScope = DisabledModuleScope.Global;
            ucDisabledModule.TestSettingKeys = "CMSStagingLogDataChanges";
            ucDisabledModule.InfoText = GetString("DataStaging.TaskSeparator");
            ucDisabledModule.ParentPanel = pnlNotLogged;

            // Check logging
            if (!ucDisabledModule.Check())
            {
                CurrentMaster.PanelHeader.Visible = false;
                plcContent.Visible = false;
                pnlFooter.Visible = false;
                return;
            }

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(this);

            // Get object type
            objectType = QueryHelper.GetString("objecttype", string.Empty);
            if (!String.IsNullOrEmpty(objectType))
            {
                // Create header action
                HeaderActions.AddAction(new HeaderAction
                {
                    Text = GetString("ObjectTasks.SyncCurrent"),
                    EventName = SYNCHRONIZE_CURRENT
                });

                // Add CSS class to panels wrapper in order it could be stacked
                CurrentMaster.PanelHeader.AddCssClass("header-container-multiple-panels");
            }

            // Create header action
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("Tasks.CompleteSync"),
                EventName = SYNCHRONIZE_COMPLETE,
                ButtonStyle = ButtonStyle.Default
            });

            // Setup title
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");
            if (!ControlsHelper.CausedPostBack(HeaderActions, btnSyncSelected, btnSyncAll))
            {
                plcContent.Visible = true;

                // Initialize buttons
                btnDeleteAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Tasks.ConfirmDeleteAll")) + ");";
                btnDeleteSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";
                btnSyncSelected.OnClientClick = "return !" + gridTasks.GetCheckSelectionScript();

                // Initialize grid
                gridTasks.OrderBy = "TaskTime";
                gridTasks.ZeroRowsText = GetString("Tasks.NoTasks");
                gridTasks.OnDataReload += gridTasks_OnDataReload;
                gridTasks.ShowActionsMenu = true;
                gridTasks.Columns = "TaskID, TaskSiteID, TaskDocumentID, TaskNodeAliasPath, TaskTitle, TaskTime, TaskType, TaskObjectType, TaskObjectID, TaskRunning, (SELECT COUNT(*) FROM Staging_Synchronization WHERE SynchronizationTaskID = TaskID AND SynchronizationErrorMessage IS NOT NULL AND (SynchronizationServerID = @ServerID OR (@ServerID = 0 AND (@TaskSiteID = 0 OR SynchronizationServerID IN (SELECT ServerID FROM Staging_Server WHERE ServerSiteID = @TaskSiteID AND ServerEnabled=1))))) AS FailedCount";
                StagingTaskInfo ti = new StagingTaskInfo();
                gridTasks.AllColumns = SqlHelper.MergeColumns(ti.ColumnNames);

                pnlLog.Visible = false;
                TaskTypeCategories = TaskHelper.TASK_TYPE_CATEGORY_GENERAL + ";" + TaskHelper.TASK_TYPE_CATEGORY_DATA;
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
        bool classesFound;
        string where = GetAllTasksWhere(out classesFound);

        WhereCondition mergedWhere = new WhereCondition(completeWhere).And().Where(where);

        DataSet ds = null;

        // There are some custom tables assigned to the site, get the data
        if (classesFound || !string.IsNullOrEmpty(objectType))
        {
            var tasksQuery = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, mergedWhere.ToString(true), currentOrder, currentTopN, columns, currentOffset, currentPageSize);
            ds = tasksQuery.Result;
            totalRecords = tasksQuery.TotalRecords;
        }
        else
        {
            totalRecords = -1;
        }

        return ds;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "changeServer", ScriptHelper.GetScript("ChangeServer(" + SelectedServerID + ");"));
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Complete synchronization.
    /// </summary>
    public void SynchronizeComplete(object parameter)
    {
        RunAction("Synchronization", "SYNCCOMPLETEDATA", SynchronizeCompleteInternal);
    }


    private string SynchronizeCompleteInternal()
    {
        int sid = SelectedServerID;
        if (sid <= 0)
        {
            sid = SynchronizationInfoProvider.ENABLED_SERVERS;
        }

        var objectTypes = GetObjectTypes();

        AddLog(String.Format(GetString("Synchronization.LoggingTasks"), objectTypes.Join("', '")));

        // Create update tasks
        SynchronizationHelper.LogObjectChange(objectTypes.Join(";"), 0, DateTimeHelper.ZERO_TIME, TaskTypeEnum.UpdateObject, true, false, false, false, false, CurrentSiteID, sid);

        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        DataSet ds = GetStagingDataTasks();

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
    }


    private List<string> GetObjectTypes()
    {
        // Get custom tables object types
        var objectTypes = new List<string>();

        DataSet dsTables = CustomTableHelper.GetCustomTableClasses(CurrentSiteID).Column("ClassName");
        if (!DataHelper.DataSourceIsEmpty(dsTables))
        {
            DataTable table = dsTables.Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                objectTypes.Add(CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString()));
            }
        }

        return objectTypes;
    }


    /// <summary>
    /// All items synchronization.
    /// </summary>
    protected void SynchronizeAll(object parameter)
    {
        RunAction("Synchronization", "SYNCALLDATA", SynchronizeAllInternal);
    }


    private string SynchronizeAllInternal()
    {
        string where = new WhereCondition()
            .Where(GetAllTasksWhere())
            .And()
            .Where(gridTasks.CustomFilter.WhereCondition)
            .ToString(true);

        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, where, "TaskID", -1, "TaskID");

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

        RunAction("Synchronization", "SYNCSELECTEDDATA", () => SynchronizeSelectedInternal(list));
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
        RunAction("Synchronization", SYNCHRONIZE_CURRENT, SynchronizeCurrentInternal);
    }


    private string SynchronizeCurrentInternal()
    {
        int sid = SelectedServerID;
        if (sid <= 0)
        {
            sid = SynchronizationInfoProvider.ENABLED_SERVERS;
        }

        AddLog(String.Format(GetString("Synchronization.LoggingTasks"), objectType));

        // Create update tasks
        SynchronizationHelper.LogObjectChange(objectType, 0, DateTimeHelper.ZERO_TIME, TaskTypeEnum.UpdateObject, true, false, false, false, false, CurrentSiteID, sid);

        AddLog(GetString("Synchronization.RunningTasks"));

        // Get the tasks
        DataSet ds = GetStagingDataTasks();

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
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

        RunAction("Deletion", "DELETESELECTEDDATA", () => DeleteTasks(list));
    }
    

    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    protected void DeleteAll(object parameter)
    {
        RunAction("Deletion", "DELETEALLDATA", DeleteAllInternal);
    }


    private string DeleteAllInternal()
    {
        AddLog(GetString("Synchronization.DeletingTasks"));

        var where = new WhereCondition()
            .Where(GetAllTasksWhere())
            .And()
            .Where(gridTasks.CustomFilter.WhereCondition)
            .ToString(true);

        DeleteTasks(where);

        return null;
    }


    private string GetAllTasksWhere()
    {
        bool classesSelected;

        return GetAllTasksWhere(out classesSelected);
    }


    private string GetAllTasksWhere(out bool classesFound)
    {
        string where = BASE_TASK_WHERE;
        string classWhere = string.Empty;

        classesFound = false;

        // Ensure only data task selection
        if (string.IsNullOrEmpty(objectType))
        {
            DataSet dsTables = CustomTableHelper.GetCustomTableClasses(CurrentSiteID).Column("ClassName");
            if (!DataHelper.DataSourceIsEmpty(dsTables))
            {
                foreach (DataRow dr in dsTables.Tables[0].Rows)
                {
                    classWhere += "N'" + SqlHelper.EscapeQuotes(CustomTableItemProvider.GetObjectType(dr["ClassName"].ToString())) + "',";
                }

                classWhere = classWhere.TrimEnd(',');
                classesFound = true;
            }

            where = SqlHelper.AddWhereCondition(where, "TaskObjectType IN (" + classWhere + ")");
        }

        return where;
    }


    /// <summary>
    /// Deletes tasks based on the given where condition
    /// </summary>
    /// <param name="where">Where condition</param>
    protected void DeleteTasks(string where)
    {
        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, where, "TaskID", 0, "TaskID, TaskTitle");

        DeleteTasks(ds);
    }


    private DataSet GetStagingDataTasks()
    {
        return StagingTaskInfoProvider.SelectObjectTaskList(CurrentSiteID, SelectedServerID, objectType, BASE_TASK_WHERE, "TaskID", -1, "TaskID");
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
        else if (e.CommandName == SYNCHRONIZE_COMPLETE)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(SynchronizeComplete);
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
