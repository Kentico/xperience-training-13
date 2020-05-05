using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine.Web.UI;


[ParentObject("cms.workflow", "workflowid")]
[EditedObject("cms.workflowstep", "workflowstepid")]

[Help("workflow_step", "helpTopic")]
public partial class CMSModules_Workflows_Workflow_Step_New : CMSWorkflowPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.EditForm.RedirectUrlAfterCreate = URLHelper.AppendQuery(
            UIContextHelper.GetElementUrl(ModuleName.CMS, "Edit.Workflows.EditStep", false),
            "objectid={%EditedObject.ID%}&saved=1&workflowid=" + WorkflowId + "&parentObjectId=" + WorkflowId);

        // Initializes page title
        CreateBreadcrumbs();
    }


    /// <summary>
    /// Crates breadcrumbs on master page title.
    /// </summary>
    private void CreateBreadcrumbs()
    {
        string workflowSteps = GetString("Development-Workflow_Step_New.Steps");
        string currentWorkflow = GetString("Development-Workflow_Steps.NewStep");
        string workflowStepsUrl = URLHelper.AppendQuery(
            UIContextHelper.GetElementUrl(ModuleName.CMS, "Workflows.Steps", false),
            "parentObjectId=" + WorkflowId + "&workflowId=" + WorkflowId);

        // Set the breadcrumbs element
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = workflowSteps,
            RedirectUrl = workflowStepsUrl,
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = currentWorkflow
        });
    }

    #endregion
}