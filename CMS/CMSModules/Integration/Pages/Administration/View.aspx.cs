using System;
using System.Data;
using System.Text;
using System.Xml;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_Integration_Pages_Administration_View : CMSIntegrationPage
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    public int TaskID
    {
        get
        {
            return QueryHelper.GetInteger("taskid", 0);
        }
    }

    #endregion"


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("Task.ViewHeader");
        RegisterModalPageScripts();

        IntegrationTaskInfo ti = IntegrationTaskInfo.Provider.Get(TaskID);
        // Set edited object
        EditedObject = ti;

        if (ti != null)
        {
            PageTitle.TitleText += " (" + HTMLHelper.HTMLEncode(ti.TaskTitle) + ")";

            string direction = GetString(ti.TaskIsInbound ? "integration.inbound" : "integration.outbound");

            // Prepare task description
            StringBuilder sbTaskInfo = new StringBuilder();
            sbTaskInfo.Append("<table class='table table-hover'>");
            sbTaskInfo.Append("<tr><td style='width: 20%;'><strong>" + GetString("integration.taskdirection") + ":</strong></td><td>" + direction + "</td></tr>");
            sbTaskInfo.Append("<tr><td><strong>" + GetString("integration.tasktype") + ":</strong></td><td>" + ti.TaskType + "</td></tr>");
            sbTaskInfo.Append("<tr><td><strong>" + GetString("integration.tasktime") + ":</strong></td><td>" + ti.TaskTime + "</td></tr>");
            sbTaskInfo.Append("</table>");

            string objectType = ti.TaskObjectType;
            if (ti.TaskNodeID > 0)
            {
                objectType = TreeNode.OBJECT_TYPE;
            }
            viewDataSet.ObjectType = objectType;
            viewDataSet.DataSet = GetDataSet(ti.TaskData, ti.TaskType, ti.TaskObjectType);
            viewDataSet.AdditionalContent = sbTaskInfo.ToString();
        }
    }

    #endregion


    #region "Methods"

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

    #endregion
}