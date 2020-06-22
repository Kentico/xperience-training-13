using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Properties_ProcessProperties : CMSAutomationPage
{
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMessages;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var workflowStep = WorkflowStepInfo.Provider.Get(QueryHelper.GetInteger("objectId", -1));
        if (workflowStep != null)
        {
            if (workflowStep.StepWorkflowType != WorkflowTypeEnum.Automation)
            {
                lblStepName.Text = HTMLHelper.HTMLEncode(GetString("general.error"));
                ShowError(GetString("ma.error.notautomationstep"));

                return;
            }

            var localizedDisplayName = workflowStep.StepType == WorkflowStepTypeEnum.Start ? ResHelper.GetString("ma.startstep.node.displayname") : ResHelper.LocalizeString(workflowStep.StepDisplayName);

            lblStepName.Text = HTMLHelper.HTMLEncode(localizedDisplayName);
            lblStepName.ToolTip = localizedDisplayName;

            var graphName = QueryHelper.GetString("graph", String.Empty);
            var additionalQuery = $"processId={workflowStep.StepWorkflowID}";
            additionalQuery += String.IsNullOrEmpty(graphName) ? String.Empty : $"&graph={graphName}";

            frmStepEdit.Src = ApplicationUrlHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditStepProperties", false, workflowStep.StepID, additionalQuery);
        }
    }
}
