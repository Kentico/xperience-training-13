using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflow", "workflowid")]
public partial class CMSModules_Workflows_Workflow_General : CMSWorkflowPage
{
    #region "Constants"

    /// <summary>
    /// Convert event name
    /// </summary>
    private const string CONVERT_ACTION = "convertadvanced";

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize form and controls
        Title = "Workflow Edit - General";

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("converted", false))
            {
                ShowConfirmation(GetString("workflow.converted"));
            }
            else if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
        }
        // Register conversion
        ComponentEvents.RequestEvents.RegisterForEvent(CONVERT_ACTION, (s, args) => ConvertWorkflow());

        bool fullRefreshRequired = editElem.Form.ItemChanged("WorkflowAllowedObjects");

        if (fullRefreshRequired)
        {
            RefreshFrameset();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (CurrentWorkflow != null)
        {
            // Only basic workflow without auto-publish can be converted
            if (WorkflowInfoProvider.IsAdvancedWorkflowAllowed() && CurrentWorkflow.IsBasic)
            {
                HeaderAction convert = new HeaderAction
                    {
                    Text = GetString("workflow.convert"),
                    OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("workflow.confirmconversion"), true) + ");",
                    EventName = CONVERT_ACTION,
                    ButtonStyle = ButtonStyle.Default
                };

                if (editElem.CurrentWorkflow.WorkflowAutoPublishChanges)
                {
                    convert.Tooltip = GetString("workflow.conversionerror.versioningwithoutworkflow");
                    convert.Enabled = false;
                }
                CurrentMaster.HeaderActions.AddAction(convert);
            }
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Refreshed whole frameset.
    /// </summary>
    private void RefreshFrameset()
    {
        string script = @"
var url = parent.location.href;
url = url.replace('converted=1', '');
if(url.indexOf('saved=1') == -1){
    url += '&saved=1';
}
parent.location.href = url;
";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "changeAllowedObjects", script, true);
    }


    /// <summary>
    /// Converts current workflow to advanced workflow.
    /// </summary>
    protected void ConvertWorkflow()
    {
        try
        {
            WorkflowInfoProvider.ConvertToAdvancedWorkflow(WorkflowId);
            ScriptHelper.RegisterStartupScript(this, typeof(string), "convert", "parent.location.href = parent.location.href + '&converted=1';", true);
        }
        catch (Exception ex)
        {
            ShowError(GetString("workflow.converterror"));
            Service.Resolve<IEventLogService>().LogException("Workflow", "Convert", ex);
        }
    }

    #endregion
}
