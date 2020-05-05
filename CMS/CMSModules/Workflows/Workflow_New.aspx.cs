using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[Help("workflow_new", "helpTopic")]

[EditedObject("cms.workflow", "workflowid")]
public partial class CMSModules_Workflows_Workflow_New : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Workflows - New Workflow";

        // Redirect after successful action
        editElem.Form.RedirectUrlAfterCreate = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "EditWorkflow", false), "&objectID={%EditedObject.ID%}&saved=1");
        editElem.CurrentWorkflow.WorkflowType = (WorkflowTypeEnum)QueryHelper.GetInteger("type", (int)WorkflowTypeEnum.Basic);

        // Initializes labels
        string workflowList = GetString("Development-Workflow_Edit.Workflows");

        string newResString = (WorkflowType == WorkflowTypeEnum.Basic) ? "Development-Workflow_List.NewWorkflow" : "Development-Workflow_List.NewAdvancedWorkflow";
        string currentWorkflow = GetString(newResString);

        // Initialize master page elements
        CreateBreadcrumbs(workflowList, currentWorkflow);
    }


    /// <summary>
    /// Creates breadcrumbs on master page title.
    /// </summary>
    private void CreateBreadcrumbs(string workflowList, string currentWorkflow)
    {
        // Initializes page title
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = workflowList,
            RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Workflows", false)
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = currentWorkflow
        });
    }
}