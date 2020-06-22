using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Staging_Tools_Controls_StagingTasksFilter : StagingTasksFilterBase
{
    #region "Variables"

    private bool mTaskGroupSelectorEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Generates filter where condition for staging tasks.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition().ToString(true);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Decides what kind of task types will be in filter.
    /// Depends on EnumCategoryAttribute in TaskTypeEnum.
    /// If empty all task types are selected.
    /// </summary>
    public override string TaskTypeCategories
    {
        get
        {
            return taskTypeSelector.SelectedCategories;
        }
        set
        {
            taskTypeSelector.SelectedCategories = value;
        }
    }


    /// <summary>
    /// Decides whether to show task group selector, if it makes sense for given UI.
    /// By default returns true.
    /// </summary>
    public override bool TaskGroupSelectorEnabled
    {
        get 
        {
            return mTaskGroupSelectorEnabled;
        }
        set
        {
            mTaskGroupSelectorEnabled = value;
        }
    }

    #endregion


    #region "Page events

    protected void Page_Load(object sender, EventArgs e)
    {
        btnFilter.Click += btnFilter_Click;
        btnReset.Click += btnReset_Click;
        stagingTaskGroupSelector.ObjectType = TaskGroupInfo.OBJECT_TYPE;

        var currentUserField = new SpecialField();
        currentUserField.Text = GetString("staging.currentUser");
        currentUserField.Value = CMSActionContext.CurrentUser.UserID.ToString();
        userSelector.UniSelector.SpecialFields.Add(currentUserField);

        if (!RequestHelper.IsPostBack())
        {
            if (!TaskGroupSelectorEnabled)
            {
                stagingTaskGroupPanel.Visible = false;
                stagingTaskGroupSelector.StopProcessing = true;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    private WhereCondition GenerateWhereCondition()
    {
        var where = new WhereCondition();

        where.And().Where(fltTaskTitle.GetCondition());

        TaskTypeEnum taskType = (TaskTypeEnum)taskTypeSelector.Value.ToInteger(0);
        if (taskType != TaskTypeEnum.All)
        {
            where.WhereStartsWith("TaskType", TaskHelper.GetTaskTypeString(taskType));
        }

        fltTimeBetween.Column = "TaskTime";
        where.Where(fltTimeBetween.GetCondition());
        var selected = ValidationHelper.GetInteger(userSelector.Value, -1);
        GetStagingTasksByUser(where, selected);

        var taskGroupSelected = ValidationHelper.GetInteger(stagingTaskGroupSelector.Value, -1);
        GetStagingTasksByTaskGroup(where, taskGroupSelected);
            
        return where;
    }


    /// <summary>
    /// Applies filter to UniGrid control.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (Grid != null)
        {
            Grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        if (Grid != null)
        {
            Grid.Reset();
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        taskTypeSelector.Value = (int)TaskTypeEnum.All;
        fltTaskTitle.ResetFilter();
        fltTimeBetween.Clear();
        userSelector.Value = UniSelector.US_ALL_RECORDS;
        stagingTaskGroupSelector.Value = UniSelector.US_ALL_RECORDS;
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("From", fltTimeBetween.ValueFromTime);
        state.AddValue("To", fltTimeBetween.ValueToTime);
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        fltTimeBetween.ValueFromTime = state.GetDateTime("From");
        fltTimeBetween.ValueToTime = state.GetDateTime("To");
    }

    #endregion


    #region "Where Condition Generators"

    private void GetStagingTasksByTaskGroup(WhereCondition where, int taskGroupSelected)
    {
        if (taskGroupSelected > 0)
        {
            // Get tasks for given task group
            where.WhereIn("TaskID", TaskGroupTaskInfo.Provider.Get().WhereEquals("TaskGroupID", taskGroupSelected).Column("TaskID"));
        }
        else if (taskGroupSelected == UniSelector.US_NONE_RECORD)
        {
            where.WhereNotIn("TaskID", TaskGroupTaskInfo.Provider.Get().Column("TaskID"));
        }
    }


    private void GetStagingTasksByUser(WhereCondition where, int selected)
    {
        if (selected > 0)
        {
            // Get tasks for current user
            where.WhereIn("TaskID", StagingTaskUserInfo.Provider.Get().WhereEquals("UserID", selected).Column("TaskID"));
        }
        else if (selected == UniSelector.US_NONE_RECORD)
        {
            // Get all tasks without any assigned user
            where.WhereNotIn("TaskID", StagingTaskUserInfo.Provider.Get().Column("TaskID"));
        }
    }

    #endregion
}