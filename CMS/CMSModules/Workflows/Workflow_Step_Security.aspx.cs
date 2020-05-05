using System;

using CMS.Helpers;
using CMS.WorkflowEngine.Web.UI;


public partial class CMSModules_Workflows_Workflow_Step_Security : CMSWorkflowPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucSecurity.WorkflowStepID = QueryHelper.GetInteger("workflowStepId", 0);
    }

    #endregion
}