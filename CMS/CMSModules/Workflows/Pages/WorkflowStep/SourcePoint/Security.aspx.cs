using System;

using CMS.Helpers;
using CMS.WorkflowEngine.Web.UI;


public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Security : CMSWorkflowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        securityElem.WorkflowStepID = QueryHelper.GetInteger("workflowstepid", 0);
        securityElem.SourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);
    }
}