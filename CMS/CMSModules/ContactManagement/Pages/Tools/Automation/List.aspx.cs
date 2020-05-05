using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Pages_Tools_Automation_List : CMSAutomationPage
{
    private bool? mCanManageProcesses;


    private bool CanManageProcesses
    {
        get
        {
            if (!mCanManageProcesses.HasValue)
            {
                mCanManageProcesses = WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName);
            }
            return mCanManageProcesses.Value;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set master page elements
        InitializeMasterPage();

        // Check manage permission for object menu
        gridProcesses.ShowObjectMenu = CanManageProcesses;

        // Control initialization
        gridProcesses.RememberStateByParam = "";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterScripts();
    }


    /// <summary>
    ///  Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        if (WorkflowInfoProvider.IsMarketingAutomationAllowed())
        {
            var newProcess = new HeaderAction
            {
                // New process link
                Text = GetString("ma.newprocess"),
                RedirectUrl = "Process/New.aspx",
                Enabled = CanManageProcesses
            };

            AddHeaderAction(newProcess);
        }
    }


    private void RegisterScripts()
    {
        var script = $@"function saveAsTemplate(processId) {{
    modalDialog('{UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Template_Edit.aspx")}?processId=' + processId, 'MA_Edit_Template', 800, 650);
}}";

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "MA_SaveAsTemplate", ScriptHelper.GetScript(script));
    }


    protected object gridProcesses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "recurrencetype":
                var val = (ProcessRecurrenceTypeEnum)ValidationHelper.GetInteger(parameter, (int)ProcessRecurrenceTypeEnum.Recurring);
                return val.ToLocalizedString(null);

            case "delete":
                if (!CanManageProcesses)
                {
                    var btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;

            case "savetemplate":
                ((CMSGridActionButton)sender).Visible = SystemContext.DevelopmentMode && CanManageProcesses;
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridProcesses_OnAction(string actionName, object actionArgument)
    {
        int processId = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "edit":
                var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditProcess");
                url = URLHelper.AddParameterToUrl(url, "displayTitle", "false");
                url = URLHelper.AddParameterToUrl(url, "objectId", processId.ToString());
                URLHelper.Redirect(url);
                break;

            case "delete":
                if (!CanManageProcesses)
                {
                    RedirectToAccessDenied(ModuleName.ONLINEMARKETING, "ManageProcesses");
                }

                // Delete the workflow with all the dependencies
                WorkflowInfoProvider.DeleteWorkflowInfo(processId);

                ShowConfirmation(GetString("ma.process.delete.confirmation"));
                break;
        }
    }
}