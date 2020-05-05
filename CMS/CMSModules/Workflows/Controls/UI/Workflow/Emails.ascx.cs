using System;
using System.Linq;

using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Workflows_Controls_UI_Workflow_Emails : CMSUserControl
{
    #region "Variables"

    private WorkflowInfo mWorkflow;
    protected string currentUsers = string.Empty;

    #endregion
    
    
    #region "Properties"

    /// <summary>
    /// Workflow ID
    /// </summary>
    public int WorkflowID 
    { 
        get; 
        set; 
    }


    /// <summary>
    /// Workflow
    /// </summary>
    public WorkflowInfo Workflow => mWorkflow ?? (mWorkflow = WorkflowInfoProvider.GetWorkflowInfo(WorkflowID));


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
            usUsers.StopProcessing = value;
            ucApprove.StopProcessing = value;
            ucReadyApproval.StopProcessing = value;
            ucReject.StopProcessing = value;
            ucPublish.StopProcessing = value;
            ucArchive.StopProcessing = value;
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
            usUsers.IsLiveSite = value;
            ucApprove.IsLiveSite = value;
            ucReadyApproval.IsLiveSite = value;
            ucReject.IsLiveSite = value;
            ucPublish.IsLiveSite = value;
            ucArchive.IsLiveSite = value;
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
        if (WorkflowID <= 0)
        {
            StopProcessing = true;
            return;
        }

        usUsers.ObjectType = UserInfo.OBJECT_TYPE;
        usUsers.WhereCondition = "(UserIsHidden = 0 OR UserIsHidden IS NULL)";

        ucApprove.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucReadyApproval.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucReject.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucPublish.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucArchive.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;
        ucNotif.TemplateType = WorkflowModule.WORKFLOW_EMAIL_TEMPLATE_TYPE_NAME;

        // Get the active users for this site
        var users = WorkflowUserInfoProvider.GetWorkflowUsers()
            .WhereEquals("WorkflowID", WorkflowID)
            .Column("UserID")
            .GetListResult<int>();
        currentUsers = string.Join(";", users.ToArray());

        chkEmails.NotSetChoice.Text = GetString("general.usesitesettings") + " (##DEFAULT##)";
        chkEmails.SetDefaultValue(WorkflowHelper.SendWorkflowEmails(SiteContext.CurrentSiteName));
        chkEmails.AutoPostBack = true;
        chkEmails.CheckedChanged += SendNotification_Changed;

        ucApprove.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.Approved);
        ucReadyApproval.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.ReadyForApproval);
        ucReject.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.Rejected);
        ucArchive.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.Archived);
        ucPublish.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.Published);
        ucNotif.CurrentSelector.TextBoxSelect.WatermarkText = WorkflowHelper.GetDefaultEmailTemplateName(WorkflowEmailTypeEnum.Notification);

        chkApprove.CheckedChanged += chkApprove_CheckedChanged;
        chkReadyApproval.CheckedChanged += chkReadyApproval_CheckedChanged;
        chkReject.CheckedChanged += chkReject_CheckedChanged;
        chkArchive.CheckedChanged += chkArchive_CheckedChanged;
        chkPublish.CheckedChanged += chkPublish_CheckedChanged;
        
        if (!RequestHelper.IsPostBack())
        {
            chkEmails.InitFromThreeStateValue(Workflow, "WorkflowSendEmails");
            usUsers.Value = currentUsers;
            ucApprove.Value = Workflow.WorkflowApprovedTemplateName;
            ucReadyApproval.Value = Workflow.WorkflowReadyForApprovalTemplateName;
            ucReject.Value = Workflow.WorkflowRejectedTemplateName;
            ucArchive.Value = Workflow.WorkflowArchivedTemplateName;
            ucPublish.Value = Workflow.WorkflowPublishedTemplateName;
            ucNotif.Value = Workflow.WorkflowNotificationTemplateName;

            ucApprove.Enabled = chkApprove.Checked = Workflow.WorkflowSendApproveEmails;
            ucReadyApproval.Enabled = chkReadyApproval.Checked = Workflow.WorkflowSendReadyForApprovalEmails;
            ucReject.Enabled = chkReject.Checked = Workflow.WorkflowSendRejectEmails;
            ucArchive.Enabled = chkArchive.Checked = Workflow.WorkflowSendArchiveEmails;
            ucPublish.Enabled = chkPublish.Checked = Workflow.WorkflowSendPublishEmails;

            pnlTemplates.Enabled = pnlUsers.Enabled = usUsers.Enabled = ValidationHelper.GetBoolean(chkEmails.GetActualValue(), true);
        }

        bool documents = Workflow.IsDocumentWorkflow;
        plcApprove.Visible = documents;
        plcRest.Visible = documents;
    }


    void chkReadyApproval_CheckedChanged(object sender, EventArgs e)
    {
        ucReadyApproval.Enabled = chkReadyApproval.Checked;
    }


    void chkApprove_CheckedChanged(object sender, EventArgs e)
    {
        ucApprove.Enabled = chkApprove.Checked;
    }


    void chkReject_CheckedChanged(object sender, EventArgs e)
    {
        ucReject.Enabled = chkReject.Checked;
    }


    void chkArchive_CheckedChanged(object sender, EventArgs e)
    {
        ucArchive.Enabled = chkArchive.Checked;
    }


    void chkPublish_CheckedChanged(object sender, EventArgs e)
    {
        ucPublish.Enabled = chkPublish.Checked;
    }


    void SendNotification_Changed(object sender, EventArgs e)
    {
        pnlTemplates.Enabled = pnlUsers.Enabled = usUsers.Enabled = ValidationHelper.GetBoolean(chkEmails.GetActualValue(), true);
    }


    /// <summary>
    /// Saves data
    /// </summary>
    public void SaveData()
    {
        if (Workflow != null)
        {
            chkEmails.SetThreeStateValue(Workflow, "WorkflowSendEmails");

            Workflow.WorkflowApprovedTemplateName = ValidationHelper.GetString(ucApprove.Value, null);
            Workflow.WorkflowReadyForApprovalTemplateName = ValidationHelper.GetString(ucReadyApproval.Value, null);
            Workflow.WorkflowRejectedTemplateName = ValidationHelper.GetString(ucReject.Value, null);
            Workflow.WorkflowPublishedTemplateName = ValidationHelper.GetString(ucPublish.Value, null);
            Workflow.WorkflowArchivedTemplateName = ValidationHelper.GetString(ucArchive.Value, null);
            Workflow.WorkflowNotificationTemplateName = ValidationHelper.GetString(ucNotif.Value, null);

            Workflow.WorkflowSendApproveEmails = chkApprove.Checked;
            Workflow.WorkflowSendReadyForApprovalEmails = chkReadyApproval.Checked;
            Workflow.WorkflowSendRejectEmails = chkReject.Checked;
            Workflow.WorkflowSendArchiveEmails = chkArchive.Checked;
            Workflow.WorkflowSendPublishEmails = chkPublish.Checked;

            // Save workflow info
            WorkflowInfoProvider.SetWorkflowInfo(Workflow);

            // Save selected users
            SaveUsersData();

            ShowChangesSaved();
        }
    }

    #endregion


    #region "Control handling"

    /// <summary>
    /// Saves users data
    /// </summary>
    private void SaveUsersData()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentUsers);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add all new items to site
            foreach (string item in newItems)
            {
                int userId = ValidationHelper.GetInteger(item, 0);
                // If user is authorized, remove it
                WorkflowUserInfo wsu = WorkflowUserInfoProvider.GetWorkflowUserInfo(WorkflowID, userId);
                if (wsu != null)
                {
                    WorkflowUserInfoProvider.DeleteWorkflowUserInfo(wsu);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentUsers, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add all new items to site
            foreach (string item in newItems)
            {
                int userId = ValidationHelper.GetInteger(item, 0);

                // If user is not authorized, authorize it
                if (WorkflowUserInfoProvider.GetWorkflowUserInfo(WorkflowID, userId) == null)
                {
                    WorkflowUserInfoProvider.AddUserToWorkflow(WorkflowID, userId);
                }
            }
        }
    }

    #endregion
}

