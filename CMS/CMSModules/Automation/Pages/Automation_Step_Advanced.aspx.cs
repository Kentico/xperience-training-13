using System;

using CMS.UIControls;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflowstep", "workflowStepId")]
public partial class CMSModules_Automation_Pages_Automation_Step_Advanced : CMSWorkflowPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        CurrentMaster.PanelContent.CssClass += " automation-sidepanel-content";

        if (CurrentWorkflow?.IsAutomation == true)
        {
            editElem.EditForm.AlternativeFormName = "AutomationStep_Advanced";
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (CurrentWorkflow?.IsAutomation != true)
        {
            editElem.Visible = false;
            ShowError(GetString("ma.error.notautomationstep"));
        }
    }
}