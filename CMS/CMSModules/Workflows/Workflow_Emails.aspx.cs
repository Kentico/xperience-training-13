using System;

using CMS.UIControls;
using CMS.WorkflowEngine.Web.UI;


[SaveAction(0)]
public partial class CMSModules_Workflows_Workflow_Emails : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowId > 0)
        {
            // Set edited object
            EditedObject = CurrentWorkflow;
            ucEmails.WorkflowID = WorkflowId;
        }
    }
}
