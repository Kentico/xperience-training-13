using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


[UIElement("CMS.Staging", "Documents")]
public partial class CMSModules_Staging_Tools_Tasks_Tasks : CMSStagingTasksPage
{
    #region "Variables"

    // Header actions event names
    private const string SYNCHRONIZE_CURRENT = "SYNCCURRENT";
    private const string SYNCHRONIZE_SUBTREE = "SYNCSUBTREE";
    private const string SYNCHRONIZE_COMPLETE = "SYNCCOMPLETE";

    protected bool allowView = true;

    private string aliasPath = "/";

    #endregion


    #region "Properties"

    /// <summary>
    /// Event code suffix for task event names
    /// </summary>
    protected override string EventCodeSuffix
    {
        get
        {
            return "DOC";
        }
    }


    /// <summary>
    /// Grid with the task listing
    /// </summary>
    protected override UniGrid GridTasks
    {
        get
        {
            return tasksUniGrid;
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
            return "Documents";
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check 'Manage servers' permission
        if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageDocumentsTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageDocumentsTasks");
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

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        if (!isCallback)
        {
            int nodeId = QueryHelper.GetInteger("stagingnodeid", 0);

            aliasPath = "/";

            // Get the document node
            if (nodeId > 0)
            {
                TreeProvider tree = new TreeProvider(CurrentUser);
                TreeNode node = tree.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES);
                if (node != null)
                {
                    aliasPath = node.NodeAliasPath;
                }
            }

            // Setup title
            ucDisabledModule.TestSettingKeys = "CMSStagingLogChanges";
            ucDisabledModule.InfoText = GetString("ContentStaging.TaskSeparator");
            ucDisabledModule.ParentPanel = pnlNotLogged;

            // Check logging
            if (!ucDisabledModule.Check())
            {
                CurrentMaster.PanelHeader.Visible = false;
                plcContent.Visible = false;
                pnlFooter.Visible = false;
                return;
            }

            // Create header actions
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("Tasks.SyncCurrent"),
                EventName = SYNCHRONIZE_CURRENT
            });

            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("Tasks.SyncSubtree"),
                EventName = SYNCHRONIZE_SUBTREE,
                ButtonStyle = ButtonStyle.Default
            });

            // Create header action
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("Tasks.CompleteSync"),
                EventName = SYNCHRONIZE_COMPLETE,
                ButtonStyle = ButtonStyle.Default
            });

            // Add CSS class to panels wrapper in order it could be stacked
            CurrentMaster.PanelHeader.AddCssClass("header-container-multiple-panels");

            if (!ControlsHelper.CausedPostBack(HeaderActions, btnSyncSelected, btnSyncAll))
            {
                // Check 'Manage servers' permission
                if (!CurrentUser.IsAuthorizedPerResource("cms.staging", "ManageDocumentsTasks"))
                {
                    RedirectToAccessDenied("cms.staging", "ManageDocumentsTasks");
                }

                // Register the dialog script
                ScriptHelper.RegisterDialogScript(this);

                ltlScript.Text +=
                    ScriptHelper.GetScript("function ConfirmDeleteTask(taskId) { return confirm(" +
                                           ScriptHelper.GetString(GetString("Tasks.ConfirmDelete")) + "); }");
                ltlScript.Text +=
                    ScriptHelper.GetScript("function CompleteSync(){" +
                                           Page.ClientScript.GetPostBackEventReference(btnSyncComplete, "") + "}");

                // Initialize grid
                tasksUniGrid.OnDataReload += tasksUniGrid_OnDataReload;
                tasksUniGrid.ShowActionsMenu = true;
                tasksUniGrid.Columns = "TaskID, TaskSiteID, TaskDocumentID, TaskNodeAliasPath, TaskTitle, TaskTime, TaskType, TaskObjectType, TaskObjectID, TaskRunning, (SELECT COUNT(*) FROM Staging_Synchronization WHERE SynchronizationTaskID = TaskID AND SynchronizationErrorMessage IS NOT NULL AND (SynchronizationServerID = @ServerID OR (@ServerID = 0 AND (@TaskSiteID = 0 OR SynchronizationServerID IN (SELECT ServerID FROM Staging_Server WHERE ServerSiteID = @TaskSiteID AND ServerEnabled=1))))) AS FailedCount";
                StagingTaskInfo ti = new StagingTaskInfo();
                tasksUniGrid.AllColumns = SqlHelper.MergeColumns(ti.ColumnNames);

                plcContent.Visible = true;

                // Initialize buttons
                btnSyncSelected.OnClientClick = "return !" + tasksUniGrid.GetCheckSelectionScript();
                btnDeleteAll.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Tasks.ConfirmDeleteAll")) + ");";
                btnDeleteSelected.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";

                pnlLog.Visible = false;
                TaskTypeCategories = TaskHelper.TASK_TYPE_CATEGORY_CONTENT + ";" + TaskHelper.TASK_TYPE_CATEGORY_GENERAL;
            }
        }

        var script = @"var currentNodeId = 0,
selectDocuments = false;

function ChangeServer(value) {
    currentServerId = value;
}

function SelectNode(serverId, nodeId) {
    currentServerId = serverId;
    currentNodeId = nodeId;
    document.location = 'Tasks.aspx?serverId=' + currentServerId + '&stagingnodeid=' + nodeId;
}

function SelectDocNode(serverId, nodeId) {
    currentNodeId = nodeId;
    document.location = 'DocumentsList.aspx?serverId=' + currentServerId + '&stagingnodeid=' + nodeId;
}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "HandlingTasks", ScriptHelper.GetScript(script));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlFooter.Visible = !tasksUniGrid.IsEmpty;
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


    #region "Grid events"

    /// <summary>
    /// Handles the grid external data bound
    /// </summary>
    protected override object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "tasktitle":
                DataRowView dr = (DataRowView)parameter;
                return HTMLHelper.HTMLEncode(TextHelper.LimitLength(dr["TaskTitle"].ToString(), 100));
        }

        return base.OnExternalDataBound(sender, sourceName, parameter);
    }


    protected DataSet tasksUniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Get the tasks
        var tasksQuery = StagingTaskInfoProvider.SelectDocumentTaskList(CurrentSiteID, SelectedServerID, aliasPath, completeWhere, currentOrder, currentTopN, columns, currentOffset, currentPageSize);
        var result = tasksQuery.Result;
        totalRecords = tasksQuery.TotalRecords;

        return result;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "changeServer", ScriptHelper.GetScript("ChangeServer(" + SelectedServerID + ");"));
    }

    #endregion


    #region "Button handling"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case SYNCHRONIZE_CURRENT:
                ctlAsyncLog.TitleText = GetString("Synchronization.Title");
                RunAsync(SynchronizeCurrent);
                break;

            case SYNCHRONIZE_SUBTREE:
                ctlAsyncLog.TitleText = GetString("Synchronization.Title");
                RunAsync(SynchronizeSubtree);
                break;

            case SYNCHRONIZE_COMPLETE:
                ctlAsyncLog.TitleText = GetString("Synchronization.Title");
                RunAsync(SynchronizeComplete);
                break;
        }
    }


    protected void btnSyncAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.Title");

        // Run asynchronous action
        RunAsync(SynchronizeAll);
    }


    protected void btnSyncSelected_Click(object sender, EventArgs e)
    {
        var list = tasksUniGrid.SelectedItems;
        if (list.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.Title");

            // Run asynchronous action
            RunAsync(p => SynchronizeSelected(list));
        }
    }


    protected void btnDeleteAll_Click(object sender, EventArgs e)
    {
        ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");

        // Run asynchronous action
        RunAsync(DeleteAll);
    }


    protected void btnDeleteSelected_Click(object sender, EventArgs e)
    {
        var list = tasksUniGrid.SelectedItems;
        if (list.Count > 0)
        {
            ctlAsyncLog.TitleText = GetString("Synchronization.DeletingTasksTitle");

            // Run asynchronous action
            RunAsync(p => DeleteSelected(list));
        }
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Prepares staging update tasks for documents by given event code and run all tasks
    /// update, delete, create etc. on given subtree.
    /// </summary>
    /// <param name="action">Which action has been chosen and what tasks should be created.</param>
    private void CreateUpdateTasksAndRunTasks(string action)
    {
        RunAction("Synchronization", "SYNCHRONIZE", () => CreateUpdateTasksAndRunTasksInternal(action));
    }


    private string CreateUpdateTasksAndRunTasksInternal(string action)
    {
        string result;
        int sid = SelectedServerID;
        if (sid <= 0)
        {
            sid = SynchronizationInfoProvider.ENABLED_SERVERS;
        }

        EventCode = "SYNCHRONIZE";

        AddLog(String.Format(GetString("Synchronization.LoggingTasks"), PredefinedObjectType.DOCUMENT));

        // Prepare settings for current node
        var settings = new LogMultipleDocumentChangeSettings
        {
            EnsurePublishTask = true,
            NodeAliasPath = aliasPath,
            TaskType = TaskTypeEnum.UpdateDocument,
            ServerID = sid,
            KeepTaskData = false,
            RunAsynchronously = false,
            SiteName = CurrentSiteName
        };

        // Create update task for current node 
        var currentNodeUpdateTask = DocumentSynchronizationHelper.LogDocumentChange(settings);

        // Create update tasks for subtree or for the whole tree, depends on sync action
        if (action != SYNCHRONIZE_CURRENT)
        {
            settings.NodeAliasPath = action == SYNCHRONIZE_COMPLETE ? "/%" : aliasPath.TrimEnd('/') + "/%";
            DocumentSynchronizationHelper.LogDocumentChange(settings);
        }

        AddLog(GetString("Synchronization.RunningTasks"));

        if (action == SYNCHRONIZE_CURRENT)
        {
            // Run sync for the current node only
            result = StagingTaskRunner.RunSynchronization(currentNodeUpdateTask.Select(t => t.TaskID));
        }
        else
        {
            // Get all tasks for given path, depends on the sync action and run them
            string path = action == SYNCHRONIZE_COMPLETE ? "/" : aliasPath;
            DataSet ds = StagingTaskInfoProvider.SelectDocumentTaskList(CurrentSiteID, SelectedServerID, path, null, "TaskID", -1, "TaskID");
            result = StagingTaskRunner.RunSynchronization(ds);
        }
        return result;
    }


    /// <summary>
    /// Prepares staging update tasks.
    /// </summary>
    public void SynchronizeComplete(object parameter)
    {
        EventCode = SYNCHRONIZE_COMPLETE;
        CreateUpdateTasksAndRunTasks(EventCode);
    }


    /// <summary>
    /// All items synchronization.
    /// </summary>
    public void SynchronizeAll(object parameter)
    {
        RunAction("Synchronization", "SYNCHRONIZE", SynchronizeAllInternal);
    }


    private string SynchronizeAllInternal()
    {
        EventCode = "SYNCALLDOCS";

        AddLog(GetString("Synchronization.RunningTasks"));

        // Process all records
        DataSet ds = StagingTaskInfoProvider.SelectDocumentTaskList(CurrentSiteID, SelectedServerID, aliasPath, tasksUniGrid.CustomFilter.WhereCondition, "TaskID", -1, "TaskID");

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(ds);
    }


    /// <summary>
    /// Synchronizes document subtree.
    /// </summary>
    /// <param name="parameter">Leave empty</param>
    protected void SynchronizeSubtree(object parameter)
    {
        EventCode = SYNCHRONIZE_SUBTREE;
        CreateUpdateTasksAndRunTasks(EventCode);
    }


    /// <summary>
    /// Synchronizes selected documents.
    /// </summary>
    /// <param name="parameter">List of document identifiers.</param>
    public void SynchronizeSelected(object parameter)
    {
        var list = parameter as List<String>;
        if (list == null)
        {
            return;
        }

        RunAction("Synchronization", "SYNCSELECTEDDOCS", () => SynchronizeSelectedInternal(list));
    }


    private string SynchronizeSelectedInternal(IEnumerable<string> list)
    {
        AddLog(GetString("Synchronization.RunningTasks"));

        // Run the synchronization
        return StagingTaskRunner.RunSynchronization(list);
    }


    /// <summary>
    /// Synchronizes the current document.
    /// </summary>
    private void SynchronizeCurrent(object parameter)
    {
        EventCode = SYNCHRONIZE_CURRENT;
        CreateUpdateTasksAndRunTasks(EventCode);
    }


    /// <summary>
    /// Deletes selected tasks.
    /// </summary>
    protected void DeleteSelected(object parameter)
    {
        List<String> list = parameter as List<String>;
        if (list == null)
        {
            return;
        }

        RunAction("Deletion", "DELETESELECTEDDOCS", () => DeleteTasks(list));
    }


    /// <summary>
    /// Deletes all tasks.
    /// </summary>
    protected void DeleteAll(object parameter)
    {
        RunAction("Deletion", "DELETEALLDOCS", DeleteAllInternal);
    }


    private string DeleteAllInternal()
    {
        AddLog(GetString("Synchronization.DeletingTasks"));

        // Get the tasks
        DataSet ds = StagingTaskInfoProvider.SelectDocumentTaskList(CurrentSiteID, SelectedServerID, aliasPath, tasksUniGrid.CustomFilter.WhereCondition, null, -1, "TaskID, TaskTitle");

        DeleteTasks(ds);

        return null;
    }

    #endregion
}
