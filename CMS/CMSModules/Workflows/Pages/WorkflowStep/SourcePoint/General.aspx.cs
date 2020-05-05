using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[EditedObject("cms.workflowstep", "workflowstepid")]

[SaveAction(0)]
public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_General : CMSWorkflowPage
{
    private WorkflowStepInfo CurrentStepInfo
    {
        get
        {
            return (WorkflowStepInfo)UIContext.EditedObject;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register save event
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => editElem.Save(false));
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        string graphName = QueryHelper.GetString("graph", String.Empty);

        if (CurrentStepInfo != null)
        {
            var sourcePointGuid = QueryHelper.GetGuid("sourcepointguid", Guid.Empty);

            editElem.SourcePointGuid = sourcePointGuid;
            editElem.RuleCategoryNames = CurrentWorkflow.IsAutomation ? ModuleName.ONLINEMARKETING : WorkflowInfo.OBJECT_TYPE;

            if (sourcePointGuid == Guid.Empty)
            {
                if (CurrentWorkflow.IsAutomation)
                {
                    editElem.AfterCreateRedirectURL = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditStepProperties.Cases", false);
                }
                else
                {
                    editElem.AfterCreateRedirectURL = UIContextHelper.GetElementUrl(ModuleName.CMS, "Workflows.EditCase", false);
                }
            }
            else if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
                WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
            }
        }
        else
        {
            editElem.StopProcessing = true;
        }
    }
}