using System;
using System.Linq;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflowstep", "workflowStepId")]
public partial class CMSModules_Automation_Pages_Automation_Step_General : CMSWorkflowPage
{
    private WorkflowStepInfo WorkflowStep => (WorkflowStepInfo)EditedObject;


    /// <summary>
    /// Load event handler
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CurrentMaster.BodyClass += " automation-sidepanel-content";

        if (CurrentWorkflow?.IsAutomation != true)
        {
            editElem.Visible = false;
            ShowError(GetString("ma.error.notautomationstep"));

            return;
        }

        // This helps to display validation messages below each field editor.
        editElem.ParametersForm.OnBeforeValidate += (sender, args) =>
        {
            editElem.ParametersForm.IsLiveSite = true;
        };
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CurrentWorkflow?.IsAutomation == true)
        {
            ShowParameterWarning();
        }
    }


    private void ShowParameterWarning()
    {
        var names = Enumerable.Empty<string>();
        if (!RequestHelper.IsPostBack() && WorkflowStep?.ValidateParameters(out names) == false)
        {
            var error = ResHelper.GetStringFormat("ma.step.emptyrequiredparameters", String.Join(", ", names.Select(i => $"<b>{HTMLHelper.HTMLEncode(i)}</b>")));
            ShowWarning(error);
        }
    }
}