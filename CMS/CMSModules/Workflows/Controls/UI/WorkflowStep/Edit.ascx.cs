using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_Edit : CMSAdminEditControl
{
    #region "Private variables"

    private bool? mShowActionParametersForm;
    private bool? mShowTimeoutForm;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current workflow step info.
    /// </summary>
    private WorkflowStepInfo CurrentStepInfo => (WorkflowStepInfo)editForm.EditedObject;


    /// <summary>
    /// Gets current workflow object.
    /// </summary>
    private WorkflowInfo CurrentWorkflow => (WorkflowInfo)editForm.ParentObject;


    private bool ShowEditForm => ShowAdvancedProperties;


    private bool ShowSourcePointForm => ShowAdvancedProperties;


    private bool ShowActionParametersForm
    {
        get
        {
            if (mShowActionParametersForm == null)
            {
                // All steps except 
                mShowActionParametersForm = ShowGeneralProperties && CurrentStepInfo.StepIsAction;
            }
            return mShowActionParametersForm.Value;
        }
    }


    /// <summary>
    /// Indicates if timeout settings should be visible.
    /// </summary>
    private bool ShowTimeoutForm
    {
        get
        {
            if (mShowTimeoutForm == null)
            {
                // All steps except 
                mShowTimeoutForm = CurrentStepInfo.StepAllowTimeout && (CurrentWorkflow != null) && !CurrentWorkflow.IsBasic &&
                    ((ShowAdvancedProperties && CurrentStepInfo.StepType != WorkflowStepTypeEnum.Wait) || (ShowGeneralProperties && CurrentStepInfo.StepType == WorkflowStepTypeEnum.Wait));
            }
            return mShowTimeoutForm.Value;
        }
    }


    /// <summary>
    /// UIForm used to edit workflow step
    /// </summary>
    public UIForm EditForm => editForm;


    /// <summary>
    /// Gets form for parameter editation.
    /// </summary>
    public BasicForm ParametersForm => ucActionParameters.BasicForm;


    /// <summary>
    /// Indicates if advanced step properties should be displayed.
    /// </summary>
    public bool ShowAdvancedProperties 
    { 
        get; 
        set; 
    }


    /// <summary>
    /// Indicates if general step properties should be displayed.
    /// </summary>
    public bool ShowGeneralProperties 
    { 
        get; 
        set; 
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContainer.CssClass += " automation-sidepanel-content";

        if (StopProcessing)
        {
            // Do nothing!
        }
        else
        {
            btnSubmit.Visible = !ShowEditForm;
            editForm.Visible = ShowEditForm;

            pnlTimeout.Visible = ShowTimeoutForm;
            if (CurrentStepInfo != null)
            {
                LoadData(CurrentStepInfo);
            }

            if (CurrentWorkflow?.IsAutomation == true)
            {
                pnlContainer.CssClass += " automation-step-panel";

                // Remove "form-horizontal" class added by DivLayoutStyle by default
                editForm.FormCssClass = "";

                if (CurrentStepInfo != null)
                {
                    WorkflowScriptHelper.RegisterRefreshDesignerFunction(Page, CurrentStepInfo.StepID, QueryHelper.GetString("graph", String.Empty));

                    lblTimeout.ResourceString = CurrentStepInfo.StepIsWait ? "workflowstep.waitsettings" : "workflowstep.timeoutsettings";
                }

                HeaderActions.ActionControlCreated.Before += (sender, actionEventArgs) =>
                {
                    if (actionEventArgs.Action is SaveAction saveAction)
                    {
                        saveAction.Text = ResHelper.GetString("general.apply");
                    }
                };
            }
            else
            {
                pnlTimeoutForm.CssClass = "form-horizontal";
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CurrentStepInfo != null)
        {
            // Display timeout target source point selector
            plcTimeoutTarget.Visible = ucTimeout.TimeoutEnabled && ucTimeoutTarget.IsVisible();
        }
    }


    /// <summary>
    /// Loads data of edited workflow from DB into TextBoxes.
    /// </summary>
    protected void LoadData(WorkflowStepInfo wsi)
    {
        // Timeout UI is always enabled for wait step type
        ucTimeout.AllowNoTimeout = (wsi.StepType != WorkflowStepTypeEnum.Wait);

        // Display action parameters form only for action step type
        if (ShowActionParametersForm)
        {
            WorkflowActionInfo action = WorkflowActionInfo.Provider.Get(wsi.StepActionID);
            if (action != null)
            {
                if (!RequestHelper.IsPostBack())
                {
                    pnlContainer.CssClass += " " + action.ActionName.ToLowerInvariant();
                }
                ucActionParameters.FormInfo = new FormInfo(action.ActionParameters);
                lblParameters.Text = String.Format(GetString("workflowstep.parameters"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(action.ActionDisplayName)));
            }

            ParametersForm.AllowMacroEditing = true;
            ParametersForm.ShowValidationErrorMessage = false;
            ParametersForm.ResolverName = WorkflowHelper.GetResolverName(CurrentWorkflow);
            ucActionParameters.Parameters = wsi.StepActionParameters;
            ucActionParameters.ReloadData(!RequestHelper.IsPostBack());
            ucActionParameters.Visible = ucActionParameters.CheckVisibility();
        }

        plcParameters.Visible = ucActionParameters.Visible;

        if (plcTimeoutTarget.Visible)
        {
            ucTimeoutTarget.WorkflowStepID = CurrentStepInfo.StepID;
        }

        // Initialize condition edit for certain step types
        ucSourcePointEdit.StopProcessing = true;

        if ((CurrentWorkflow != null) && !CurrentWorkflow.IsBasic)
        {
            bool conditionStep = (wsi.StepType == WorkflowStepTypeEnum.Condition);
            if (conditionStep || (wsi.StepType == WorkflowStepTypeEnum.Wait) || (!wsi.StepIsStart && !wsi.StepIsAction && !wsi.StepIsFinished && (wsi.StepType != WorkflowStepTypeEnum.MultichoiceFirstWin)))
            {
                // Initialize source point edit control
                var sourcePoint = CurrentStepInfo.StepDefinition.DefinitionPoint;
                if (sourcePoint != null)
                {
                    plcCondition.Visible = ShowSourcePointForm;
                    lblCondition.ResourceString = GetHeaderResourceString(wsi.StepType);

                    ucSourcePointEdit.StopProcessing = false;
                    ucSourcePointEdit.SourcePointGuid = sourcePoint.Guid;
                    ucSourcePointEdit.SimpleMode = !conditionStep;
                    ucSourcePointEdit.ShowCondition = GetShowCondition(wsi);
                    ucSourcePointEdit.RuleCategoryNames = CurrentWorkflow.IsAutomation ? ModuleName.ONLINEMARKETING : WorkflowInfo.OBJECT_TYPE;
                }
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            if (ShowTimeoutForm)
            {
                ucTimeout.TimeoutEnabled = wsi.StepDefinition.TimeoutEnabled;
                ucTimeout.ScheduleInterval = wsi.StepDefinition.TimeoutInterval;
            }
        }
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!editForm.Visible && ValidateData())
        {
            ucSourcePointEdit.SaveData(false);
            ucActionParameters.SaveData(false);
            SetFormValues(CurrentStepInfo);

            WorkflowStepInfo.Provider.Set(CurrentStepInfo);

            RefreshDesigner();
        }
    }


    protected void editForm_OnBeforeSave(object sender, EventArgs e)
    {
        if (editForm.Mode == FormModeEnum.Update)
        {
            ucSourcePointEdit.SaveData(false);
            ucActionParameters.SaveData(false);
            SetFormValues(CurrentStepInfo);
        }
        else
        {
            SetFormValues(CurrentStepInfo);
            EnsureStepsOrder();
        }
    }


    protected void editForm_OnAfterValidate(object sender, EventArgs e)
    {
        editForm.StopProcessing = !ValidateData();
    }


    protected void editForm_OnAfterSave(object sender, EventArgs e)
    {
        // Refresh updated node
        RefreshDesigner();
    }


    private void RefreshDesigner()
    {
        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, QueryHelper.GetString("graph", String.Empty));
    }


    private string GetHeaderResourceString(WorkflowStepTypeEnum stepType)
    {
        if (CurrentWorkflow.IsAutomation)
        {
            return "workflowstep.contactmanagement";
        }

        return stepType == WorkflowStepTypeEnum.Condition ? "workflowstep.conditionsettings" : "workflowstep.advancedsettings";
    }


    private bool GetShowCondition(WorkflowStepInfo step)
    {
        if (CurrentWorkflow.IsAutomation && step.StepType == WorkflowStepTypeEnum.Wait)
        {
            return false;
        }

        return step.StepType != WorkflowStepTypeEnum.Userchoice && step.StepType != WorkflowStepTypeEnum.Multichoice && step.StepType != WorkflowStepTypeEnum.MultichoiceFirstWin;
    }


    /// <summary>
    /// Ensures correct steps order
    /// </summary>
    private void EnsureStepsOrder()
    {
        // Ensure correct order for basic workflow
        if ((CurrentWorkflow != null) && CurrentWorkflow.IsBasic)
        {
            // Get published step info for the proper position
            WorkflowStepInfo psi = WorkflowStepInfoProvider.GetPublishedStep(CurrentWorkflow.WorkflowID);
            if (psi != null)
            {
                CurrentStepInfo.StepOrder = psi.StepOrder;
                // Move the published step down
                psi.StepOrder += 1;
                WorkflowStepInfo.Provider.Set(psi);

                // Move the archived step down
                WorkflowStepInfo asi = WorkflowStepInfoProvider.GetArchivedStep(CurrentWorkflow.WorkflowID);
                if (asi != null)
                {
                    asi.StepOrder += 1;
                    WorkflowStepInfo.Provider.Set(asi);
                }
            }
        }
    }


    /// <summary>
    /// Validates the data, returns true if succeeded.
    /// </summary>
    public bool ValidateData()
    {
        // Validate source point control
        if (!ucSourcePointEdit.ValidateData())
        {
            return false;
        }

        // Validate action properties control
        if (CurrentStepInfo.StepIsAction && !ucActionParameters.ValidateData())
        {
            return false;
        }

        return !ucTimeout.Visible || !String.IsNullOrEmpty(ucTimeout.ScheduleInterval) || !ucTimeout.TimeoutEnabled;
    }


    /// <summary>
    /// Sets values from edit form to edited workflows step info
    /// </summary>
    /// <param name="step">Edited workflow step info</param>
    private void SetFormValues(WorkflowStepInfo step)
    {
        if (step == null)
        {
            return;
        }

        if (ShowTimeoutForm)
        {
            Step definition = step.StepDefinition;
            definition.TimeoutEnabled = ucTimeout.TimeoutEnabled;
            definition.TimeoutInterval = ucTimeout.ScheduleInterval;
            if (ucTimeoutTarget.Visible)
            {
                Guid timeoutTarget = ucTimeoutTarget.SourcePointGuid;
                // Add timeout source point
                if (step.StepAllowDefaultTimeoutTarget && !definition.SourcePoints.Exists(s => (s is TimeoutSourcePoint)))
                {
                    TimeoutSourcePoint timeout = new TimeoutSourcePoint();
                    // Timeout source point is selected
                    if (!definition.SourcePoints.Exists(s => (s.Guid == timeoutTarget)))
                    {
                        timeout.Guid = timeoutTarget;
                    }

                    definition.SourcePoints.Add(timeout);
                }
                definition.TimeoutTarget = timeoutTarget;
            }
            else
            {
                // Remove timeout source point
                var timeoutPoints = definition.SourcePoints.FindAll(s => (s is TimeoutSourcePoint));
                foreach (var t in timeoutPoints)
                {
                    string result = step.RemoveSourcePoint(t.Guid);
                    if (result != null)
                    {
                        ShowError(result);
                    }
                }
            }
        }

        if (step.StepIsAction && ucActionParameters.Visible)
        {
            step.StepActionParameters.LoadData(ucActionParameters.Parameters.GetData());
        }

        if (plcCondition.Visible)
        {
            CurrentStepInfo.StepDefinition.DefinitionPoint.Text = ucSourcePointEdit.CurrentSourcePoint.Text;
            CurrentStepInfo.StepDefinition.DefinitionPoint.Tooltip = ucSourcePointEdit.CurrentSourcePoint.Tooltip;
            CurrentStepInfo.StepDefinition.DefinitionPoint.Condition = ucSourcePointEdit.CurrentSourcePoint.Condition;
            CurrentStepInfo.StepDefinition.DefinitionPoint.Label = ucSourcePointEdit.CurrentSourcePoint.Label;
        }
    }

    #endregion
}