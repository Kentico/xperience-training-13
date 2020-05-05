using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Workflows_Controls_UI_Comment : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    protected string Action
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }


    /// <summary>
    /// Menu ID
    /// </summary>
    protected string MenuID
    {
        get
        {
            return QueryHelper.GetString("menuid", null);
        }
    }


    /// <summary>
    /// Tree node
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        DocumentManager.OnCheckPermissions += DocumentManager_OnCheckPermissions;

        // Check permissions
        if (DocumentManager.IsActionAllowed(Action))
        {
            InitControls();
        }
        else
        {
            Visible = false;
        }
    }


    protected void DocumentManager_OnCheckPermissions(object sender, SimpleDocumentManagerEventArgs e)
    {
        e.CheckDefault = false;
        e.IsValid = (CurrentUser.IsAuthorizedPerDocument(e.Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed) || DocumentManager.WorkflowManager.CanUserManageWorkflow(CurrentUser, Node.NodeSiteName);
    }


    private void InitControls()
    {
        // Init list of steps
        switch (Action)
        {
            case DocumentComponentEvents.APPROVE:
            case DocumentComponentEvents.PUBLISH:
            case DocumentComponentEvents.ARCHIVE:
            case DocumentComponentEvents.REJECT:
                if (!InitSteps())
                {
                    ShowError("doc.nosteps");
                    pnlContainer.Visible = false;
                    return;
                }
                break;
        }

        RegisterActionScript();
    }


    private void RegisterActionScript()
    {
        // Get js functions
        string menuId = ValidationHelper.GetIdentifier(MenuID);

        string approveStr = "Approve_" + menuId;
        string publishStr = "Publish_" + menuId;
        string rejectStr = "Reject_" + menuId;
        string checkinStr = "CheckIn_" + menuId;
        string archiveStr = "Archive_" + menuId;
        string consStr = "CheckConsistency_" + menuId;


        StringBuilder sb = new StringBuilder();
        sb.Append(@"
function ProcessAction(action) { 
    var comment = document.getElementById('", txtComment.ClientID, @"').value;
    var param = 0;
    var drpEl = document.getElementById('", drpSteps.ClientID, @"');
    if(drpEl != null) { 
        param = drpEl.value; 
    }
    else {
        drpEl = document.getElementById('", hdnArg.ClientID, @"');
        if(drpEl != null) {
            param = drpEl.value; 
        }
    }

    switch(action) {
        case '", DocumentComponentEvents.PUBLISH, @"':
        case '", DocumentComponentEvents.APPROVE, @"':
            if(param == -1) {
                if(wopener.", publishStr, @") { wopener.", publishStr, @"(comment); } else { wopener.", consStr, @"(); }
            }
            else {
                if(wopener.", approveStr, @") { wopener.", approveStr, @"(param, comment); } else { wopener.", consStr, @"(); }
            }
        break;

        case '", DocumentComponentEvents.REJECT, @"':
            if(wopener.", rejectStr, @") { wopener.", rejectStr, @"(param, comment); } else { wopener.", consStr, @"(); }
        break;

        case '", DocumentComponentEvents.ARCHIVE, @"':
            if(wopener.", archiveStr, @") { wopener.", archiveStr, @"(param, comment); } else { wopener.", consStr, @"(); }
        break;

        case '", DocumentComponentEvents.CHECKIN, @"':
            if(wopener.", checkinStr, @") { wopener.", checkinStr, @"(comment); } else { wopener.", consStr, @"(); }
        break;
    }
}"
);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "action", sb.ToString(), true);
    }


    private bool InitSteps()
    {
        if (Node == null)
        {
            return false;
        }

        bool displayDDL = false;
        List<WorkflowStepInfo> steps = null;
        int stepsCount = 0;

        switch (Action)
        {
            case DocumentComponentEvents.APPROVE:
            case DocumentComponentEvents.PUBLISH:
                bool publishStepVisible = false;
                steps = DocumentManager.WorkflowManager.GetNextStepInfo(Node);
                stepsCount = steps.Count;

                var appSteps = steps.FindAll(s => !s.StepIsArchived);
                int appStepsCount = appSteps.Count;
                if (appStepsCount == 0)
                {
                    return false;
                }

                if (appStepsCount > 1)
                {
                    // Add all next steps
                    foreach (var step in appSteps)
                    {
                        publishStepVisible |= step.StepIsPublished;
                        drpSteps.Items.Add(new ListItem(GetActionText(Node.WorkflowStep, step), step.StepID.ToString()));
                    }
                }
                else if (appStepsCount == 1)
                {
                    // There are also archived steps
                    if (stepsCount > 1)
                    {
                        // Set command argument
                        hdnArg.Value = appSteps[0].StepID.ToString();
                    }
                }

                // Display direct publish button
                WorkflowStepInfo pub = appSteps[0];
                if (!publishStepVisible && !pub.StepIsPublished && (DocumentManager.Step != null) && DocumentManager.Step.StepAllowPublish)
                {
                    drpSteps.Items.Add(new ListItem(ResHelper.LocalizeString(pub.StepDisplayName), pub.StepID.ToString()));
                    drpSteps.Items.Add(new ListItem(GetString("EditMenu.IconPublished"), "-1"));
                }

                displayDDL = (drpSteps.Items.Count > 0);
                break;

            case DocumentComponentEvents.REJECT:
                if (DocumentManager.WorkflowManager.CanUserManageWorkflow(CurrentUser, DocumentManager.SiteName))
                {
                    steps = DocumentManager.WorkflowManager.GetPreviousSteps(Node);
                    foreach (var step in steps)
                    {
                        drpSteps.Items.Add(new ListItem(GetActionText(Node.WorkflowStep, step), step.RelatedHistoryID.ToString()));
                    }
                }

                displayDDL = (drpSteps.Items.Count > 1);
                break;

            case DocumentComponentEvents.ARCHIVE:
                steps = DocumentManager.WorkflowManager.GetNextStepInfo(Node);
                stepsCount = steps.Count;

                var archSteps = steps.FindAll(s => s.StepIsArchived);
                int archStepsCount = archSteps.Count;
                if (archStepsCount > 1)
                {
                    // Add all archived steps
                    foreach (var step in archSteps)
                    {
                        drpSteps.Items.Add(new ListItem(GetActionText(Node.WorkflowStep, step), step.StepID.ToString()));
                    }
                }
                else if (archStepsCount == 1)
                {
                    // There are also approve steps
                    if (stepsCount > 1)
                    {
                        // Set command argument
                        hdnArg.Value = archSteps[0].StepID.ToString();
                    }
                }

                displayDDL = (drpSteps.Items.Count > 1);
                break;
        }

        plcSteps.Visible = displayDDL;

        return true;
    }


    private string GetActionText(WorkflowStepInfo currentStep, WorkflowStepInfo nextStep)
    {
        string text = ResHelper.LocalizeString(nextStep.StepDisplayName);
        WorkflowTransitionInfo transition = nextStep.RelatedTransition;
        SourcePoint def = (transition != null) ? currentStep.GetSourcePoint(transition.TransitionSourcePointGUID) : null;
        if (def != null)
        {
            if (!String.IsNullOrEmpty(def.Text))
            {
                text = String.Format(ResHelper.LocalizeString(def.Text), text);
            }
        }

        return text;
    }

    #endregion
}