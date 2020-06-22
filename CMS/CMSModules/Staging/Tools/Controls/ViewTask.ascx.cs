using System;
using System.Collections.Generic;
using System.Data;

using CMS.Helpers;

using System.Linq;
using System.Text;
using System.Xml;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_Staging_Tools_Controls_ViewTask : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the ID of the task.
    /// </summary>
    public int TaskId
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        StagingTaskInfo ti = StagingTaskInfo.Provider.Get(TaskId);
        // Set edited object
        EditedObject = ti;

        if (ti == null)
        {
            return;
        }

        ((CMSDeskPage)Page).PageTitle.TitleText += " (" + HTMLHelper.HTMLEncode(ti.TaskTitle) + ")";

        // Get users and task groups
        var usersWhoModifiedObject = GetUsersFromStagingTask(ti);
        var taskInGroups = GetTaskGroupsFromStagingTask(ti);

        string objectType = ti.TaskObjectType;
        if (ti.TaskNodeID > 0)
        {
            objectType = TreeNode.OBJECT_TYPE;
        }

        viewDataSet.ObjectType = objectType;
        viewDataSet.DataSet = GetDataSet(ti.TaskData, ti.TaskType, ti.TaskObjectType);
        viewDataSet.AdditionalContent = GetTaskHeader(ti, usersWhoModifiedObject, taskInGroups);
    }


    /// <summary>
    /// Returns task header table in HTML code
    /// </summary>
    private string GetTaskHeader(StagingTaskInfo ti, IEnumerable<string> usersWhoModifiedObject, IEnumerable<string> taskInGroups)
    {
        const string FORMATTED_TABLE_ROW = "<tr><td class=\"Title Grid\">{0}</td><td>{1}</td></tr>";

        var taskHeaderInHtml = "<table class=\"table table-hover\">";
        taskHeaderInHtml += String.Format(FORMATTED_TABLE_ROW, GetString("staging.detailtasktype"), GetEncodedTaskType(ti));
        taskHeaderInHtml += String.Format(FORMATTED_TABLE_ROW, GetString("staging.detailtasktime"), ti.TaskTime);
        taskHeaderInHtml += String.Format(FORMATTED_TABLE_ROW, GetString("staging.taskprocessedby"), GetEncodedTaskProccessedBy(ti));
        taskHeaderInHtml += String.Format(FORMATTED_TABLE_ROW, GetString("taskUser.taskModifiedBy"), GetEncodedEnumeration(usersWhoModifiedObject));
        taskHeaderInHtml += String.Format(FORMATTED_TABLE_ROW, GetString("stagingTaskGroup.taskGroupedIn"), GetEncodedEnumeration(taskInGroups));
        taskHeaderInHtml += "</table>";
        return taskHeaderInHtml;
    }


    private static string GetEncodedTaskType(StagingTaskInfo ti)
    {
        return HTMLHelper.HTMLEncode(ti.TaskType.ToLocalizedString(TaskHelper.TASK_TYPE_RESOURCE_STRING_PREFIX));
    }


    private static string GetEncodedTaskProccessedBy(StagingTaskInfo ti)
    {
        return DataHelper.GetNotEmpty(ti.TaskServers.Trim(';').Replace(";", ", "), "-");
    }


    private static string GetEncodedEnumeration(IEnumerable<string> items)
    {
        return HTMLHelper.HTMLEncode(String.Join(", ", items));
    }


    /// <summary>
    /// Gets the usernames of users, who modified object for which the given staging task was created.
    /// </summary>
    /// <param name="ti">Staging task</param>
    /// <returns>Returns usernames of users, who modified object for which the given staging task was created</returns>
    private static IEnumerable<string> GetUsersFromStagingTask(StagingTaskInfo ti)
    {
        List<string> usersWhoModifiedObject = new List<string>();

        UserInfo.Provider.Get()
            .Columns("UserID", "UserName")
            .WhereIn("UserID", StagingTaskUserInfo.Provider.Get().Column("UserID").WhereEquals("TaskID", ti.TaskID))
            .ForEachObject(u => usersWhoModifiedObject.Add(u.UserName));

        return usersWhoModifiedObject;
    }


    /// <summary>
    /// Gets the task groups, in which the staging task is.
    /// </summary>
    /// <param name="ti">Staging task</param>
    /// <returns>Returns display names of task groups, in which the task exists</returns>
    private static IEnumerable<string> GetTaskGroupsFromStagingTask(StagingTaskInfo ti)
    {
        List<string> taskGroups = new List<string>();

        TaskGroupInfo.Provider.Get()
            .Columns("TaskGroupID", "TaskGroupCodeName")
            .WhereIn("TaskGroupID", TaskGroupTaskInfo.Provider.Get().WhereEquals("TaskID", ti.TaskID).Column("TaskGroupID"))
            .ForEachObject(t => taskGroups.Add(t.TaskGroupCodeName));

        return taskGroups;
    }


    /// <summary>
    /// Returns the dataset loaded from the given document data.
    /// </summary>
    /// <param name="documentData">Document data to make the dataset from</param>
    /// <param name="taskType">Task type</param>
    /// <param name="taskObjectType">Task object type</param>
    protected virtual DataSet GetDataSet(string documentData, TaskTypeEnum taskType, string taskObjectType)
    {
        var man = SyncManager.GetInstance();
        man.OperationType = OperationTypeEnum.Synchronization;
        string className = DocumentHierarchyHelper.GetNodeClassName(documentData, ExportFormatEnum.XML);
        DataSet ds = man.GetSynchronizationTaskDataSet(taskType, className, taskObjectType);

        XmlParserContext xmlContext = new XmlParserContext(null, null, null, XmlSpace.None);
        XmlReader reader = new XmlTextReader(documentData, XmlNodeType.Element, xmlContext);

        return DataHelper.ReadDataSetFromXml(ds, reader, null, null);
    }
}