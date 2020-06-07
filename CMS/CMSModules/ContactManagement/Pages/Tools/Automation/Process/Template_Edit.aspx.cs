using System;
using System.Web.UI;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title("ma.template.create")]
[Security(Resource = ModuleName.ONLINEMARKETING, Permission = "ManageProcesses")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Template_Edit : CMSModalPage
{
    private const int NEW_RECORD_VALUE = -10;


    private int EditedTemplateId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["EditedTemplateId"], NEW_RECORD_VALUE);
        }
        set
        {
            ViewState["EditedTemplateId"] = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        var master = (ICMSModalMasterPage)CurrentMaster;
        master.ShowSaveAndCloseButton();
        master.SetSaveResourceString("general.submit");
        master.Save += (sender, eventArgs) => { editForm.SaveData(""); };

        editForm.SubmitButton.Visible = false;
    }


    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(((Pair)savedState).First);

        LoadFormData();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        drpSaveAs.DropDownSingleSelect.AutoPostBack = true;
        drpSaveAs.SpecialFields.Insert(0, new SpecialField
        {
            Value = NEW_RECORD_VALUE.ToString(),
            Text = GetString("general.newitem")
        });
    }


    /// <summary>
    /// Customized SaveViewState to ensure LoadViewState call on the next postback.
    /// </summary>
    protected override object SaveViewState()
    {
        return new Pair(base.SaveViewState(), null);
    }


    protected void editForm_OnAfterSave(object sender, EventArgs e)
    {
        var processId = QueryHelper.GetInteger("processid", 0);
        var processInfo = WorkflowInfoProvider.GetWorkflowInfo(processId);
        if (processInfo != null)
        {
            AutomationTemplateManager.UpsertConfigurationFromWorkflow(editForm.EditedObject as AutomationTemplateInfo, processInfo);
        }

        ScriptHelper.RegisterStartupScript(this, typeof(string), "MA_Template_CloseDialog", "setTimeout('CloseDialog(false)', 500);", true);
    }


    protected void drpSaveAs_OnSelectionChanged(object sender, EventArgs e)
    {
        EditedTemplateId = ValidationHelper.GetInteger(drpSaveAs.Value, NEW_RECORD_VALUE);

        LoadFormData();

        editForm.InitCompleted = false;
        editForm.ReloadData();
    }


    private void LoadFormData()
    {
        var template = AutomationTemplateInfo.Provider.Get(EditedTemplateId);

        UIContext["EnableEditedObjectReset"] = true;
        editForm.EditedObject = template;
    }
}