using System;
using System.Web.UI;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Security(Resource = ModuleName.ONLINEMARKETING, Permission = "ManageAutomationTemplates")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Template_Edit : CMSModalPage
{
    private const int NEW_RECORD_VALUE = -10;
    private bool? mEditingExistingTemplate;


    public override MessagesPlaceHolder MessagesPlaceHolder => pnlMessagePlaceholder;


    private int EditedTemplateId
    {
        get => ValidationHelper.GetInteger(ViewState["EditedTemplateId"], NEW_RECORD_VALUE);
        set => ViewState["EditedTemplateId"] = value;
    }


    private bool EditingExistingTemplate
    {
        get => mEditingExistingTemplate ?? (mEditingExistingTemplate = QueryHelper.Contains("templateId")).Value;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        var master = (ICMSModalMasterPage)CurrentMaster;
        master.ShowSaveAndCloseButton();
        master.SetSaveResourceString("general.submit");
        master.Save += (sender, eventArgs) => editForm.SaveData("");

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

        if (!EditingExistingTemplate)
        {
            drpSaveAs.DropDownSingleSelect.AutoPostBack = true;
            drpSaveAs.SpecialFields.Insert(0, new SpecialField
            {
                Value = NEW_RECORD_VALUE.ToString(),
                Text = GetString("general.newitem")
            });
        }
        else if (!RequestHelper.IsPostBack())
        {
            EditedTemplateId = QueryHelper.GetInteger("templateId", NEW_RECORD_VALUE);

            drpSaveAs.Value = EditedTemplateId;
            ReloadFormData();
            pnlSaveAs.Visible = false;
        }

        DisableValidationForLocalizationFields();
        SetTitle(EditingExistingTemplate ? GetString("ma.template.edit") : GetString("ma.template.create"));
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
        if (!EditingExistingTemplate)
        {
            var processId = QueryHelper.GetInteger("processid", 0);
            if (processId > 0)
            {
                var processInfo = WorkflowInfo.Provider.Get(processId);
                if (processInfo != null)
                {
                    AutomationTemplateManager.UpsertConfigurationFromWorkflow(editForm.EditedObject as AutomationTemplateInfo, processInfo);
                }
            }
        }

        CloseAndRefreshParent();
    }


    protected void drpSaveAs_OnSelectionChanged(object sender, EventArgs e)
    {
        EditedTemplateId = ValidationHelper.GetInteger(drpSaveAs.Value, NEW_RECORD_VALUE);
        ReloadFormData();
    }


    private void CloseAndRefreshParent()
    {
        var refreshParentScript = @"
function refreshParent() {
    if (window.wopener) {
        window.wopener.location = window.wopener.location;
    }
}";

        var closeAndRefreshParentScript = $@"
{(EditingExistingTemplate ? refreshParentScript : String.Empty)}
setTimeout('CloseDialog(); {(EditingExistingTemplate ? "refreshParent();" : String.Empty)}', 500);";

        ScriptHelper.RegisterStartupScript(this, GetType(), "MA_Template_CloseDialog", closeAndRefreshParentScript, true);
    }


    private void DisableValidationForLocalizationFields()
    {
        var displayNameField = (CMSFormControls_System_LocalizableTextBox)editForm.FieldControls["TemplateDisplayName"];
        var descriptionField = (CMSFormControls_System_LocalizableTextBox)editForm.FieldControls["TemplateDescription"];

        displayNameField.HideValidationErrors = descriptionField.HideValidationErrors = true;
    }


    private void ReloadFormData()
    {
        LoadFormData();

        editForm.InitCompleted = false;
        editForm.ReloadData();
    }


    private void LoadFormData()
    {
        var template = AutomationTemplateInfo.Provider.Get(EditedTemplateId);

        if (EditedTemplateId != NEW_RECORD_VALUE && !EditingExistingTemplate)
        {
            ShowInformation(ResHelper.GetStringFormat("ma.template.updateexisting.information", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(template.TemplateDisplayName))));
        }
        else
        {
            MessagesPlaceHolder.ClearLabels();
        }

        UIContext["EnableEditedObjectReset"] = true;
        editForm.EditedObject = template;
    }
}
