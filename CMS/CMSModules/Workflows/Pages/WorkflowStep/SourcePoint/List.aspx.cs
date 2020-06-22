using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Helpers.UniGraphConfig;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.GraphConfig;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_List : CMSWorkflowPage
{
    private WorkflowStepInfo mCurrentWorkflowStep;
    private WorkflowStepSourcePointListControl mListControl;


    private WorkflowStepInfo CurrentWorkflowStep => mCurrentWorkflowStep ?? (mCurrentWorkflowStep = WorkflowStepInfo.Provider.Get(QueryHelper.GetInteger("workflowStepId", 0)));


    private WorkflowStepSourcePointListControl ListControl
    {
        get
        {
            if (mListControl == null)
            {
                if (CurrentWorkflowStep.StepWorkflowType == WorkflowTypeEnum.Automation)
                {
                    return mListControl = LoadControl("~/CMSModules/OnlineMarketing/Controls/UI/WorkflowStep/SourcePoint/List.ascx") as WorkflowStepSourcePointListControl;
                }

                return mListControl = LoadControl("~/CMSModules/Workflows/Controls/UI/WorkflowStep/SourcePoint/List.ascx") as WorkflowStepSourcePointListControl;
            }

            return mListControl;
        }
    }


    private bool CanAddSourcePoint
    {
        get
        {
            if (CurrentWorkflowStep != null)
            {
                var node = WorkflowNode.GetInstance(CurrentWorkflowStep);
                return NodeSourcePointsLimits.Max[node.Type] > CurrentWorkflowStep.StepDefinition.SourcePoints.Count;
            }

            return false;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ListControl.WorkflowStepID = CurrentWorkflowStep.StepID;
        ListControl.IsLiveSite = false;
        pnlPanel.Controls.Add(ListControl);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        EditedObject = CurrentWorkflowStep;

        if (CurrentWorkflowStep != null)
        {
            if (CurrentWorkflowStep.StepAllowBranch)
            {
                // Condition step type can't have additional switch cases
                if (CurrentWorkflowStep.StepType != WorkflowStepTypeEnum.Condition)
                {
                    string graphName = QueryHelper.GetString("graph", string.Empty);

                    // Set actions
                    HeaderAction action = new HeaderAction
                    {
                        Text = GetString("Development-Workflow_Step_SourcePoints.New"),
                        RedirectUrl = $"~/CMSModules/Workflows/Pages/WorkflowStep/SourcePoint/General.aspx?workflowStepId={CurrentWorkflowStep.StepID}&graph={graphName}"
                    };
                    if (!CanAddSourcePoint)
                    {
                        action.Enabled = false;
                        action.Tooltip = GetString("workflowstep.toomanysourcepoints");
                    }

                    CurrentMaster.HeaderActions.AddAction(action);
                }
            }
            else
            {
                ShowInformation(GetString("workflowstep.cannothavecustomsourcepoints"));
                ListControl.StopProcessing = true;
            }
        }
    }
}