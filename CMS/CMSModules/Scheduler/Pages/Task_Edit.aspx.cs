using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Scheduler;
using CMS.UIControls;
using CMS.WebFarmSync;


public partial class CMSModules_Scheduler_Pages_Task_Edit : CMSScheduledTasksPage
{
    #region "Variables"

    protected int? mTaskId = null;
    protected TaskInfo mTaskObj = null;
    protected bool developmentMode = SystemContext.DevelopmentMode;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets task ID passed as parameter to the page, or 0.
    /// </summary>
    public int TaskID
    {
        get
        {
            if (mTaskId == null)
            {
                mTaskId = QueryHelper.GetInteger("taskname", 0);
            }
            return mTaskId.Value;
        }
    }


    /// <summary>
    /// Gets or sets task info object.
    /// </summary>
    public TaskInfo TaskInfo
    {
        get
        {
            return mTaskObj ?? (mTaskObj = TaskInfoProvider.GetTaskInfo(TaskID));
        }
        set
        {
            mTaskObj = value;
        }
    }

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        // Page is used for creating and editing global and site tasks 
        string elementName = null;

        if (TaskID == 0)
        {
            elementName = "NewTask";
        }
        else if (TaskInfo == null)
        {
            RedirectToInformation("editedobject.notexists");
        }
        else if (TaskInfo.TaskType == ScheduledTaskTypeEnum.Standard)
        {
            elementName = "EditTask";
        }
        else
        {
            elementName = "EditSystemTask";
        }

        var uiElement = new UIElementAttribute("CMS.ScheduledTasks", elementName);
        uiElement.Check(this);

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Control initializations
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvName.ErrorMessage = GetString("Task_Edit.EmptyName");
        lblFrom.Text = GetString("scheduler.from");
        btnReset.OnClientClick = "if (!confirm(" + ScriptHelper.GetLocalizedString("tasks.reset") + ")) return false;";

        plcDevelopment.Visible = developmentMode;

        // Show 'Allow run in external service' check-box in development mode
        plcAllowExternalService.Visible = developmentMode;

        string currentTask = GetString("Task_Edit.NewItemCaption");

        if (TaskID > 0)
        {
            // Set edited object
            EditedObject = TaskInfo;

            if (TaskInfo != null)
            {
                // Global task and user is not global administrator and task's site id is different than current site id
                if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && ((TaskInfo.TaskSiteID == 0) || (TaskInfo.TaskSiteID != SiteID)))
                {
                    RedirectToAccessDenied(GetString("general.nopermission"));
                }

                currentTask = TaskInfo.TaskDisplayName;

                if (!RequestHelper.IsPostBack())
                {
                    ReloadData();

                    // Show that the task was created or updated successfully
                    if (QueryHelper.GetBoolean("saved", false))
                    {
                        ShowChangesSaved();
                    }
                }
            }
        }
        else
        {
            // Check "modify" permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
            {
                RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
            }

            if (WebFarmContext.WebFarmEnabled)
            {
                if (!RequestHelper.IsPostBack())
                {
                    chkAllServers.Visible = true;
                }
                chkAllServers.Attributes.Add("onclick", "document.getElementById('" + txtServerName.ClientID + "').disabled = document.getElementById('" + chkAllServers.ClientID + "').checked;");
            }
        }

        plcRunIndividually.Visible = (SiteID <= 0);

        // Initializes page title control		
        BreadcrumbItem tasksLink = new BreadcrumbItem();
        tasksLink.Text = GetString("Task_Edit.ItemListLink");

        bool notSystem = (TaskInfo == null) || (TaskInfo.TaskType != ScheduledTaskTypeEnum.System);
        string listUrl = UIContextHelper.GetElementUrl("CMS.ScheduledTasks", GetElementName(notSystem ? "Tasks" : "SystemTasks"), true);
        listUrl = URLHelper.AddParameterToUrl(listUrl, "siteid", SiteID.ToString());
        tasksLink.RedirectUrl = listUrl;

        PageBreadcrumbs.Items.Add(tasksLink);
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = currentTask
        });
    }


    /// <summary>
    /// Load data of editing task.
    /// </summary>
    protected void ReloadData()
    {
        if (TaskInfo != null)
        {
            assemblyElem.AssemblyName = TaskInfo.TaskAssemblyName;
            assemblyElem.ClassName = TaskInfo.TaskClass;

            txtTaskData.Text = TaskInfo.TaskData;
            txtTaskName.Text = TaskInfo.TaskName;
            lblExecutions.Text = (TaskInfo.TaskExecutions == 0) ? "0" : TaskInfo.TaskExecutions.ToString();
            chkTaskEnabled.Checked = TaskInfo.TaskEnabled;
            chkTaskDeleteAfterLastRun.Checked = TaskInfo.TaskDeleteAfterLastRun;
            schedInterval.ScheduleInterval = TaskInfo.TaskInterval;
            txtTaskDisplayName.Text = TaskInfo.TaskDisplayName;
            txtServerName.Text = TaskInfo.TaskServerName;
            chkRunTaskInSeparateThread.Checked = TaskInfo.TaskRunInSeparateThread;
            ucMacroEditor.Text = TaskInfo.TaskCondition;
            chkRunIndividually.Checked = ValidationHelper.GetBoolean(TaskInfo.TaskRunIndividuallyForEachSite, false);

            lblFrom.Text += " " + ((TaskInfo.TaskLastExecutionReset != DateTimeHelper.ZERO_TIME) ? TaskInfo.TaskLastExecutionReset.ToString("d") : " - ");

            bool allowExternalService = TaskInfo.TaskAllowExternalService;
            chkTaskAllowExternalService.Checked = allowExternalService;
            chkTaskUseExternalService.Checked = TaskInfo.TaskUseExternalService;

            // Show 'Use external service' if task can run in external service
            plcUseExternalService.Visible = developmentMode || allowExternalService;

            drpModule.Value = TaskInfo.TaskResourceID;
            ucUser.Value = TaskInfo.TaskUserID;

            plcResetFrom.Visible = TaskInfo.TaskLastExecutionReset != DateTimeHelper.ZERO_TIME;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.ScheduledTasks", "Modify"))
        {
            RedirectToAccessDenied("CMS.ScheduledTasks", "Modify");
        }

        // Check required fields format
        string errorMessage = new Validator()
            .NotEmpty(txtTaskDisplayName.Text, rfvDisplayName.ErrorMessage)
            .NotEmpty(txtTaskName.Text, rfvName.ErrorMessage)
            .IsCodeName(txtTaskName.Text, GetString("Task_Edit.InvalidTaskName"))
            .MatchesCondition(schedInterval.StartTime.SelectedDateTime, DataTypeManager.IsValidDate, String.Format("{0} {1}.", GetString("BasicForm.ErrorInvalidDateTime"), DateTime.Now))
            .Result;

        if ((errorMessage == String.Empty) && !schedInterval.CheckIntervalPreceedings())
        {
            errorMessage = GetString("Task_Edit.BetweenIntervalPreceedingsError");
        }

        if ((errorMessage == String.Empty) && !schedInterval.CheckOneDayMinimum())
        {
            errorMessage = GetString("Task_Edit.AtLeastOneDayError");
        }

        // Validate assembly, but only if task is enabled (so tasks for not-installed modules can be disabled)
        if ((errorMessage == String.Empty) && chkTaskEnabled.Checked && !assemblyElem.IsValid())
        {
            errorMessage = assemblyElem.ErrorMessage;
        }

        // Checking date/time limit (SQL limit)
        if (errorMessage == String.Empty)
        {
            TaskInterval ti = SchedulingHelper.DecodeInterval(schedInterval.ScheduleInterval);
            if ((ti != null) && ((ti.StartTime < DataTypeManager.MIN_DATETIME) || (ti.StartTime > DataTypeManager.MAX_DATETIME)))
            {
                ti.StartTime = DateTime.Now;
                schedInterval.ScheduleInterval = SchedulingHelper.EncodeInterval(ti);
            }
        }

        // Check macro condition length
        if ((errorMessage == String.Empty) && (ucMacroEditor.Text.Length > 400))
        {
            errorMessage = String.Format(GetString("task_edit.invalidlength"), 400);
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
        else
        {
            // Check existing task name
            TaskInfo existingTask = TaskInfoProvider.GetTaskInfo(txtTaskName.Text.Trim(), SiteInfo != null ? SiteInfo.SiteName : null);

            if ((existingTask != null) && ((TaskInfo == null) || (existingTask.TaskID != TaskInfo.TaskID)))
            {
                ShowError(GetString("Task_Edit.TaskNameExists").Replace("%%name%%", existingTask.TaskName));
                return;
            }

            if (TaskInfo == null)
            {
                // create new item -> insert
                TaskInfo = new TaskInfo { TaskSiteID = SiteID };
                if (!developmentMode)
                {
                    TaskInfo.TaskAllowExternalService = true;
                }
            }

            TaskInfo.TaskAssemblyName = assemblyElem.AssemblyName.Trim();
            TaskInfo.TaskClass = assemblyElem.ClassName.Trim();
            TaskInfo.TaskData = txtTaskData.Text.Trim();
            TaskInfo.TaskName = txtTaskName.Text.Trim();
            TaskInfo.TaskEnabled = chkTaskEnabled.Checked;
            TaskInfo.TaskDeleteAfterLastRun = chkTaskDeleteAfterLastRun.Checked;
            TaskInfo.TaskInterval = schedInterval.ScheduleInterval;
            TaskInfo.TaskDisplayName = txtTaskDisplayName.Text.Trim();
            TaskInfo.TaskServerName = txtServerName.Text.Trim();
            TaskInfo.TaskRunInSeparateThread = chkRunTaskInSeparateThread.Checked;
            TaskInfo.TaskUseExternalService = chkTaskUseExternalService.Checked;
            TaskInfo.TaskCondition = ucMacroEditor.Text;
            TaskInfo.TaskRunIndividuallyForEachSite = chkRunIndividually.Checked;

            if (plcAllowExternalService.Visible)
            {
                TaskInfo.TaskAllowExternalService = chkTaskAllowExternalService.Checked;
            }
            if (plcUseExternalService.Visible)
            {
                TaskInfo.TaskUseExternalService = chkTaskUseExternalService.Checked;
            }

            TaskInfo.TaskNextRunTime = SchedulingHelper.GetFirstRunTime(SchedulingHelper.DecodeInterval(TaskInfo.TaskInterval));

            if (drpModule.Visible)
            {
                TaskInfo.TaskResourceID = ValidationHelper.GetInteger(drpModule.Value, 0);
            }

            TaskInfo.TaskUserID = ValidationHelper.GetInteger(ucUser.Value, 0);

            // Set synchronization to true (default is false for Scheduled task)
            TaskInfo.Generalized.StoreSettings();
            TaskInfo.Generalized.LogSynchronization = SynchronizationTypeEnum.LogSynchronization;
            TaskInfo.Generalized.LogIntegration = true;
            TaskInfo.Generalized.LogEvents = true;

            // If web farm support, create the tasks for all servers
            if (chkAllServers.Checked)
            {
                TaskInfoProvider.CreateWebFarmTasks(TaskInfo);
            }
            else
            {
                TaskInfoProvider.SetTaskInfo(TaskInfo);
            }

            // Restore original settings
            TaskInfo.Generalized.RestoreSettings();

            bool notSystem = (TaskInfo == null) || (TaskInfo.TaskType != ScheduledTaskTypeEnum.System);
            string url = UIContextHelper.GetElementUrl("CMS.ScheduledTasks", GetElementName(notSystem ? "EditTask" : "EditSystemTask"), true);
            
            // Add task ID and saved="1" query parameters
            url = URLHelper.AddParameterToUrl(String.Format(url, TaskInfo.TaskID), "saved", "1");

            // Add site ID query parameter and redirect to the finished URL
            URLHelper.Redirect(URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString()));
        }
    }


    /// <summary>
    /// Performs reset of execution counter.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        if (TaskInfo != null)
        {
            TaskInfo.TaskExecutions = 0;
            TaskInfo.TaskLastExecutionReset = DateTime.Now;
            TaskInfoProvider.SetTaskInfo(TaskInfo);

            lblFrom.Text += " " + DateTime.Now.ToString("d");
            plcResetFrom.Visible = true;

            ShowConfirmation(GetString("task.executions.reseted"));
        }
    }
}