using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[SaveAction(0)]
public partial class CMSModules_Workflows_Workflow_Step_Emails : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int stepId = QueryHelper.GetInteger("workflowStepId", 0);
        if (stepId > 0)
        {
            // Set edited object
            EditedObject = WorkflowStepInfoProvider.GetWorkflowStepInfo(stepId);
            ucEmails.WorkflowStepID = stepId;
        }
    }
}
