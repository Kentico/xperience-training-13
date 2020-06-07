using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[Breadcrumbs]
[Breadcrumb(0, "ma.process.list", "~/CMSModules/ContactManagement/Pages/Tools/Automation/List.aspx", null)]
[Breadcrumb(1, "ma.process.new")]
[Help("ma_process_new")]
[Security(Resource = ModuleName.ONLINEMARKETING, Permission = "ManageProcesses")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_New : CMSAutomationPage, ICallbackEventHandler
{
    private const string STEPS_TAB_CODE_NAME = "EditProcessSteps";
    private const string EDIT_ELEMENT_CODE_NAME = "EditProcess";
    private const int CREATE_FROM_SCRATCH_ID = -10;

    private WorkflowInfo mWorkflow;


    private string CreationMessageError => GetString("ma.process.creationerror");


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        MessagesPlaceHolder.ErrorText = CreationMessageError;
        MessagesPlaceHolder.ErrorLabel.CssClass += " alert-error hidden";

        repeaterElement.DataSource = GetTemplates().Select(i => new AutomationTemplateViewModel(i));
        repeaterElement.DataBind();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterScripts();
    }


    private void RegisterScripts()
    {
        var script = $@"
function processResult(callbackResult, context) {{
    var result = JSON.parse(callbackResult);
    if (result.errorMessage != null) {{
        window.msgService.showError(result.errorMessage);

        if (window.Loader) {{
            window.Loader.hide();
        }}
    }}
    else {{
        window.location.href = result.url;
    }}
}}

function createWorkflow(id) {{
    {Page.ClientScript.GetCallbackEventReference(this, "id", "processResult", null)}
    if (window.Loader) {{
        window.Loader.show();
    }}
}}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "createWorkflowAndRedirectScript", ScriptHelper.GetScript(script));
        ScriptHelper.RegisterRequireJs(this);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "createWorkflowAndRedirectStartupScript", ScriptHelper.GetScript(@"
            cmsrequire(['CMS/MessageService'], function (msgService) {{
                    window.msgService = msgService;
            }});
        "));
    }


    private static IEnumerable<AutomationTemplateInfo> GetTemplates()
    {
        var templates = new List<AutomationTemplateInfo>
        {
            new AutomationTemplateInfo
            {
                TemplateDisplayName = "{$ma.template.startfromscratch$}",
                TemplateDescription = "{$ma.template.startfromscratch.description$}",
                TemplateIconClass = "icon-square-dashed",
                TemplateID = CREATE_FROM_SCRATCH_ID
            }
        };

        templates.AddRange(AutomationTemplateInfo.Provider.Get().Columns("TemplateID", "TemplateDisplayName", "TemplateDescription", "TemplateIconClass"));

        return templates;
    }


    /// <summary>
    /// Processes a callback event that targets a control.
    /// </summary>
    /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        try
        {
            var id = ValidationHelper.GetInteger(eventArgument, CREATE_FROM_SCRATCH_ID);
            if (id == CREATE_FROM_SCRATCH_ID)
            {
                mWorkflow = AutomationHelper.CreateEmptyWorkflow();
            }
            else
            {
                var template = AutomationTemplateInfo.Provider.Get(id);
                if (template != null)
                {
                    mWorkflow = AutomationTemplateManager.CreateAutomationProcessFromTemplate(template, MacroIdentityOption.FromUserInfo(CurrentUser));
                }
            }
        }
        catch (Exception e)
        {
            Service.Resolve<IEventLogService>().LogException("Automation process", "CREATEOBJ ERROR", e);
        }
    }


    /// <summary>
    /// Returns the result with redirect url or error message to display.
    /// </summary>
    public string GetCallbackResult()
    {
        object result;

        if (mWorkflow != null)
        {
            var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, EDIT_ELEMENT_CODE_NAME, false, mWorkflow.WorkflowID);
            url = URLHelper.AddParameterToUrl(url, "tabname", STEPS_TAB_CODE_NAME);
            url = URLHelper.AddParameterToUrl(url, "processid", mWorkflow.WorkflowID.ToString());

            result = new
            {
                url
            };
        }
        else
        {
            result = new
            {
                errorMessage = CreationMessageError
            };
        }

        return JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}