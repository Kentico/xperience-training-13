using System;
using System.Linq;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflowstep", "workflowStepId")]
public partial class CMSModules_Workflows_Workflow_Step_General : CMSWorkflowPage
{
    private WorkflowStepInfo WorkflowStep => (WorkflowStepInfo)EditedObject;


    /// <summary>
    /// Load event handler
    /// </summary>
    /// <seealso cref="BasicForm.DisplayErrorLabel"/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // This helps to display validation messages below each field editor.
        editElem.ParametersForm.OnBeforeValidate += (sender, args) =>
        {
            editElem.ParametersForm.IsLiveSite = true;
        };

        if (CurrentWorkflow?.IsAutomation == true)
        {
            editElem.EditForm.FieldsToHide.Add("StepType");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CurrentWorkflow?.IsAutomation == true)
        {
            ShowParameterWarning();

            var saveButton = HeaderActions.ActionsList.OfType<SaveAction>().FirstOrDefault();
            if (saveButton != null)
            {
                saveButton.Text = ResHelper.GetString("general.apply");
            }
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