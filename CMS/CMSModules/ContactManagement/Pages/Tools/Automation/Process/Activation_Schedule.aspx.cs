using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[EditedObject(WorkflowInfo.OBJECT_TYPE_AUTOMATION, "processid")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Activation_Schedule : CMSModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetTitle(GetString("automationdesigner.activationscheduler.title"));

        editForm.SubmitButton.Visible = false;
        editForm.ShowValidationErrorMessage = false;

        var master = (ICMSModalMasterPage)CurrentMaster;
        master.ShowSaveAndCloseButton();
        master.SetSaveResourceString("general.save");
        master.Save += OnSave;
        
        plcInfoMessage.ShowInformation(GetString("automationdesigner.activationscheduler.infomessage"));
    }


    private void OnSave(object sender, EventArgs e)
    {
        if (IsSchedulerSettingsValid() && editForm.SaveData(null, false))
        {
            ToggleWorkflowState();
            ScriptHelper.RegisterStartupScript(Page, GetType(), "MA_ScheduleActivation_CloseDialog", ScriptHelper.GetScript("CloseAndRefresh()"));
        }
    }


    private bool IsSchedulerSettingsValid()
    {
        var enabledFrom = ValidationHelper.GetDateTime(editForm.GetFieldValue("WorkflowEnabledFrom"), DateTimeHelper.ZERO_TIME);
        var enabledTo = ValidationHelper.GetDateTime(editForm.GetFieldValue("WorkflowEnabledTo"), DateTimeHelper.ZERO_TIME);

        if (enabledFrom < enabledTo || enabledTo == DateTimeHelper.ZERO_TIME)
        {
            return true;
        }

        editForm.ShowError(GetString("automationdesigner.activationscheduler.validationerror"));
        return false;
    }


    private void ToggleWorkflowState()
    {
        var workflow = (WorkflowInfo)editForm.EditedObject;
        if (workflow.IsScheduled())
        {
            workflow.WorkflowEnabled = IsActive(workflow);
            WorkflowInfo.Provider.Set(workflow);
        }
    }


    private bool IsActive(WorkflowInfo workflow)
    {
        var now = DateTime.Now;
        return workflow.WorkflowEnabledFrom <= now && (now <= workflow.WorkflowEnabledTo || workflow.WorkflowEnabledTo == DateTimeHelper.ZERO_TIME);
    }
}
