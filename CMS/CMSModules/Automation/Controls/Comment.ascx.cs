using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;

public partial class CMSModules_Automation_Controls_Comment : CMSUserControl
{
    #region "Variables"

    private CMSAutomationManager mAutomationManager = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    public string ActionName
    {
        get;
        set;
    }


    /// <summary>
    /// Menu ID
    /// </summary>
    public string MenuID
    {
        get;
        set;
    }


    /// <summary>
    /// Automation manager control
    /// </summary>
    public CMSAutomationManager AutomationManager
    {
        get
        {
            if (mAutomationManager == null)
            {
                mAutomationManager = ControlsHelper.GetChildControl(Page, typeof(CMSAutomationManager)) as CMSAutomationManager;
                if (mAutomationManager == null)
                {
                    throw new Exception("[AutomationMenu.AutomationManager]: Missing automation manager.");
                }
            }

            return mAutomationManager;
        }
    }


    private AutomationManager Manager
    {
        get
        {
            return AutomationManager.Manager;
        }
    }


    private BaseInfo InfoObject
    {
        get
        {
            return AutomationManager.InfoObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        if (AutomationManager.IsActionAllowed(ActionName))
        {
            InitControls();
        }
        else
        {
            Visible = false;
        }
    }


    private void InitControls()
    {
        // Init list of steps
        switch (ActionName)
        {
            case ComponentEvents.AUTOMATION_MOVE_NEXT:
            case ComponentEvents.AUTOMATION_MOVE_SPEC:
            case ComponentEvents.AUTOMATION_MOVE_PREVIOUS:
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
        string menuId = ValidationHelper.GetIdentifier(MenuID);

        // Get js functions
        string nextStr = "MoveNext_" + menuId;
        string specStr = "MoveSpecific_" + menuId;
        string previousStr = "MovePrevious_" + menuId;
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

    switch(action) {
        case '", ComponentEvents.AUTOMATION_MOVE_NEXT, @"':
            if(wopener.", nextStr, @") { wopener.", nextStr, @"(param, comment); } else { wopener.", consStr, @"(); }
        break;

        case '", ComponentEvents.AUTOMATION_MOVE_SPEC, @"':
            if(wopener.", specStr, @") { wopener.", specStr, @"(param, comment); } else { wopener.", consStr, @"(); }
        break;

        case '", ComponentEvents.AUTOMATION_MOVE_PREVIOUS, @"':
            if(wopener.", previousStr, @") { wopener.", previousStr, @"(param, comment); } else { wopener.", consStr, @"(); }
        break;
    }
}"
);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "action", sb.ToString(), true);
    }


    private bool InitSteps()
    {
        if (AutomationManager.InfoObject == null)
        {
            return false;
        }

        bool displayDDL = false;
        List<WorkflowStepInfo> steps = null;
        int stepsCount = 0;

        switch (ActionName)
        {
            case ComponentEvents.AUTOMATION_MOVE_NEXT:
                steps = Manager.GetNextSteps(InfoObject, AutomationManager.StateObject);
                stepsCount = steps.Count;

                if (stepsCount == 0)
                {
                    return false;
                }

                if (stepsCount > 1)
                {
                    // Add all next steps
                    foreach (var step in steps)
                    {
                        drpSteps.Items.Add(new ListItem(GetActionText(AutomationManager.Step, step), step.StepID.ToString()));
                    }
                }

                displayDDL = (drpSteps.Items.Count > 0);
                break;

            case ComponentEvents.AUTOMATION_MOVE_SPEC:
                var allSteps = WorkflowStepInfo.Provider.Get()
                    .Where("StepWorkflowID=" + AutomationManager.StateObject.StateWorkflowID + " AND StepType NOT IN (" + (int)WorkflowStepTypeEnum.Start + "," + (int)WorkflowStepTypeEnum.Note + ")")
                    .OrderBy("StepDisplayName");

                // Add all steps
                foreach (var step in allSteps)
                {
                    drpSteps.Items.Add(new ListItem(GetActionText(AutomationManager.Step, step), step.StepID.ToString()));
                }

                displayDDL = true;
                break;

            case ComponentEvents.AUTOMATION_MOVE_PREVIOUS:
                if (WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, InfoObject.Generalized.ObjectSiteName))
                {
                    steps = Manager.GetPreviousSteps(InfoObject, AutomationManager.StateObject);
                    foreach (var step in steps)
                    {
                        drpSteps.Items.Add(new ListItem(GetActionText(AutomationManager.Step, step), step.RelatedHistoryID.ToString()));
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

        if (!String.IsNullOrEmpty(def?.Text))
        {
            text = String.Format(ResHelper.LocalizeString(def.Text), text);
        }

        return text;
    }

    #endregion
}