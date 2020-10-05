using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
    private const char SEPARATOR = '#';


    /// <summary>
    /// Available tile actions.
    /// </summary>
    private enum TileAction
    {
        None,
        Create,
        Delete
    }


    private WorkflowInfo mWorkflow;


    private TileAction CurrentAction { get; set; }


    private string CreationMessageError => GetString("ma.process.creationerror");


    private bool CanManageTemplates => LicenseIsSufficient && AuthorizedToManageTemplates;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        repeaterElement.ItemDataBound += RepeaterElement_ItemDataBound;

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
        RegisterCreateWorkflowScripts();
        RegisterManageTemplateScripts();
    }


    private void RegisterCreateWorkflowScripts()
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

function createWorkflow(templateId) {{
    var argument = '{TileAction.Create}{SEPARATOR}' + templateId;
    {Page.ClientScript.GetCallbackEventReference(this, "argument", "processResult", null)}
    if (window.Loader) {{
        window.Loader.show();
    }}
}}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MA_CreateWorkflowAndRedirect", ScriptHelper.GetScript(script));
        ScriptHelper.RegisterRequireJs(this);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "MA_CreateWorkflowAndRedirectStartup", ScriptHelper.GetScript(@"
            cmsrequire(['CMS/MessageService'], function (msgService) {{
                    window.msgService = msgService;
            }});
        "));
    }


    private void RegisterManageTemplateScripts()
    {
        if (!CanManageTemplates)
        {
            return;
        }

        var editTemplateScript = $@"
function editTemplate(templateId) {{
    modalDialog('{HttpUtility.JavaScriptStringEncode(UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Template_Edit.aspx"))}?templateId=' + templateId, 'MA_Edit_Template', 800, 650);
}}";

        var deleteTemplateScript = $@"
function deleteTemplate(templateId) {{
    if (confirm({ScriptHelper.GetLocalizedString("ma.template.delete.confirmation")})) {{
        var argument = '{TileAction.Delete}{SEPARATOR}' + templateId;
        {Page.ClientScript.GetCallbackEventReference(this, "argument", "refreshPage", null)};
    }}
}}";

        var refreshPageScript = @"
function refreshPage() {
    window.location = window.location.href;
}";

        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MA_RefreshPage", ScriptHelper.GetScript(refreshPageScript));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MA_SaveAsTemplate", ScriptHelper.GetScript(editTemplateScript));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MA_DeleteTemplate", ScriptHelper.GetScript(deleteTemplateScript));
    }


    private void RepeaterElement_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (!CanManageTemplates)
        {
            return;
        }

        var item = e.Item;

        if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var templateId = ((AutomationTemplateViewModel)item.DataItem).Id;

        if (templateId == CREATE_FROM_SCRATCH_ID)
        {
            return;
        }

        var tileOptions = ControlsHelper.GetChildControl<CMSMoreOptionsButton>(item);

        tileOptions.Visible = true;
        tileOptions.ToolTip = GetString("EditMenu.MoreActions");
        tileOptions.Actions = new List<CMSButtonAction>()
        {
            new CMSButtonAction
            {
                Text = GetString("general.edit"),
                ToolTip = GetString("general.edit"),
                OnClientClick = $"editTemplate({templateId}); return false;"
            },
            new CMSButtonAction
            {
                Text = GetString("general.delete"),
                ToolTip = GetString("general.delete"),
                OnClientClick = $"deleteTemplate({templateId}); return false;"
            }
        };
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
            var args = ParseEventArguments(eventArgument);
            var templateId = args.Value;
            CurrentAction = args.Key;

            switch (CurrentAction)
            {
                case TileAction.Create:
                    ExecuteCreateAction(templateId);
                    break;
                case TileAction.Delete:
                    ExecuteDeleteAction(templateId);
                    break;
            }
        }
        catch (Exception e)
        {
            Service.Resolve<IEventLogService>().LogException("Automation process", "MANAGEOBJ", e);
        }
    }


    private void ExecuteCreateAction(int templateId)
    {
        if (templateId == CREATE_FROM_SCRATCH_ID)
        {
            mWorkflow = AutomationHelper.CreateEmptyWorkflow();
        }
        else
        {
            var template = AutomationTemplateInfo.Provider.Get(templateId);
            if (template != null)
            {
                mWorkflow = AutomationTemplateManager.CreateAutomationProcessFromTemplate(template, MacroIdentityOption.FromUserInfo(CurrentUser));
            }
        }
    }


    private void ExecuteDeleteAction(int templateId)
    {
        if (templateId <= 0)
        {
            return;
        }

        var template = AutomationTemplateInfo.Provider.Get(templateId);
        if (template != null)
        {
            AutomationTemplateInfo.Provider.Delete(template);
        }
    }


    private KeyValuePair<TileAction, int> ParseEventArguments(string eventArgument)
    {
        var args = eventArgument.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

        return new KeyValuePair<TileAction, int>(EnumStringRepresentationExtensions.ToEnum<TileAction>(args[0]), ValidationHelper.GetInteger(args[1], CREATE_FROM_SCRATCH_ID));
    }


    /// <summary>
    /// Returns the result with redirect url or error message to display.
    /// </summary>
    public string GetCallbackResult()
    {
        if (CurrentAction != TileAction.Create)
        {
            return String.Empty;
        }

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
