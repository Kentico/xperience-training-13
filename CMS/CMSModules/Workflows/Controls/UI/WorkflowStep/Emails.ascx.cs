using System;
using System.Linq;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_Emails : CMSUserControl
{
    #region "Variables"

    private WorkflowStepInfo mWorkflowStep = null;
    private WorkflowInfo mWorkflow = null;

    #endregion
    
    
    #region "Properties"

    /// <summary>
    /// Workflow step ID
    /// </summary>
    public int WorkflowStepID 
    { 
        get; 
        set; 
    }


    /// <summary>
    /// Workflow step
    /// </summary>
    public WorkflowStepInfo WorkflowStep
    {
        get
        {
            if (mWorkflowStep == null)
            {
                mWorkflowStep = WorkflowStepInfo.Provider.Get(WorkflowStepID);
            }

            return mWorkflowStep;
        }
    }


    /// <summary>
    /// Workflow
    /// </summary>
    public WorkflowInfo Workflow
    {
        get
        {
            if (mWorkflow == null)
            {
                mWorkflow = WorkflowInfo.Provider.Get(WorkflowStep.StepWorkflowID);
            }

            return mWorkflow;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            ucApprove.StopProcessing = value;
            ucReadyApproval.StopProcessing = value;
            ucReject.StopProcessing = value;
        }
    }

    
    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get 
        { 
             return base.IsLiveSite;
        }
        set 
        { 
            base.IsLiveSite = value;
            ucApprove.IsLiveSite = value;
            ucReadyApproval.IsLiveSite = value;
            ucReject.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register save event
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => { SaveData(); });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowStepID <= 0)
        {
            StopProcessing = true;
            return;
        }

        ucApprove.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucReadyApproval.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucReject.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;

        chkEmails.NotSetChoice.Text = chkReadyApproval.NotSetChoice.Text = chkApprove.NotSetChoice.Text = chkReject.NotSetChoice.Text = GetString("general.workflowsettings") + " (##DEFAULT##)";
        chkEmails.SetDefaultValue(Workflow.SendEmails(SiteContext.CurrentSiteName, WorkflowEmailTypeEnum.Unknown));
        chkEmails.AutoPostBack = true;
        chkEmails.CheckedChanged += SendNotification_Changed;

        chkApprove.SetDefaultValue(Workflow.SendEmails(SiteContext.CurrentSiteName, WorkflowEmailTypeEnum.Approved));
        chkApprove.AutoPostBack = true;
        chkApprove.CheckedChanged += SendApproveNotification_Changed;

        chkReadyApproval.SetDefaultValue(Workflow.SendEmails(SiteContext.CurrentSiteName, WorkflowEmailTypeEnum.ReadyForApproval));
        chkReadyApproval.AutoPostBack = true;
        chkReadyApproval.CheckedChanged += SendReadyApprovalNotification_Changed;

        chkReject.SetDefaultValue(Workflow.SendEmails(SiteContext.CurrentSiteName, WorkflowEmailTypeEnum.Rejected));
        chkReject.AutoPostBack = true;
        chkReject.CheckedChanged += SendRejectNotification_Changed;

        ucApprove.CurrentSelector.TextBoxSelect.WatermarkText = Workflow.GetEmailTemplateName(WorkflowEmailTypeEnum.Approved);
        ucReadyApproval.CurrentSelector.TextBoxSelect.WatermarkText = Workflow.GetEmailTemplateName(WorkflowEmailTypeEnum.ReadyForApproval);
        ucReject.CurrentSelector.TextBoxSelect.WatermarkText = Workflow.GetEmailTemplateName(WorkflowEmailTypeEnum.Rejected);

        if (!RequestHelper.IsPostBack())
        {
            chkEmails.InitFromThreeStateValue(WorkflowStep, "StepSendEmails");
            chkApprove.InitFromThreeStateValue(WorkflowStep, "StepSendApproveEmails");
            chkReadyApproval.InitFromThreeStateValue(WorkflowStep, "StepSendReadyForApprovalEmails");
            chkReject.InitFromThreeStateValue(WorkflowStep, "StepSendRejectEmails");
            
            ucApprove.Value = WorkflowStep.StepApprovedTemplateName;
            ucReadyApproval.Value = WorkflowStep.StepReadyForApprovalTemplateName;
            ucReject.Value = WorkflowStep.StepRejectedTemplateName;

            pnlTemplates.Enabled = ValidationHelper.GetBoolean(chkEmails.GetActualValue(), true);
            ucReadyApproval.Enabled = ValidationHelper.GetBoolean(chkReadyApproval.GetActualValue(), true);
            ucApprove.Enabled = ValidationHelper.GetBoolean(chkApprove.GetActualValue(), true);
            ucReject.Enabled = ValidationHelper.GetBoolean(chkReject.GetActualValue(), true);
        }

        bool documents = Workflow.IsDocumentWorkflow;
        plcApprove.Visible = documents;
    }


    protected void SendNotification_Changed(object sender, EventArgs e)
    {
        pnlTemplates.Enabled = ValidationHelper.GetBoolean(chkEmails.GetActualValue(), true);
    }


    protected void SendReadyApprovalNotification_Changed(object sender, EventArgs e)
    {
        ucReadyApproval.Enabled = ValidationHelper.GetBoolean(chkReadyApproval.GetActualValue(), true);
    }


    protected void SendApproveNotification_Changed(object sender, EventArgs e)
    {
        ucApprove.Enabled = ValidationHelper.GetBoolean(chkApprove.GetActualValue(), true);
    }


    protected void SendRejectNotification_Changed(object sender, EventArgs e)
    {
        ucReject.Enabled = ValidationHelper.GetBoolean(chkReject.GetActualValue(), true);
    }


    /// <summary>
    /// Saves data
    /// </summary>
    public void SaveData()
    {
        if (WorkflowStep != null)
        {
            chkEmails.SetThreeStateValue(WorkflowStep, "StepSendEmails");
            chkApprove.SetThreeStateValue(WorkflowStep, "StepSendApproveEmails");
            chkReadyApproval.SetThreeStateValue(WorkflowStep, "StepSendReadyForApprovalEmails");
            chkReject.SetThreeStateValue(WorkflowStep, "StepSendRejectEmails");

            WorkflowStep.StepApprovedTemplateName = ValidationHelper.GetString(ucApprove.Value, null);
            WorkflowStep.StepReadyForApprovalTemplateName = ValidationHelper.GetString(ucReadyApproval.Value, null);
            WorkflowStep.StepRejectedTemplateName = ValidationHelper.GetString(ucReject.Value, null);

            // Save workflow info
            WorkflowStepInfo.Provider.Set(WorkflowStep);

            ShowChangesSaved();
        }
    }

    #endregion
}

