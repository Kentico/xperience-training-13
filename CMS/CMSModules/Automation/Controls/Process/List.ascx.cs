using System;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Automation_Controls_Process_List : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Type of object of states in list.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// ID of object of states in list.
    /// </summary>
    public int ObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// Item edit URL
    /// </summary>
    public string EditActionUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Uni grid
    /// </summary>
    public UniGrid UniGrid
    {
        get
        {
            return gridState;
        }
    }


    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridState.OnExternalDataBound += gridState_OnExternalDataBound;
        gridState.OnAction += gridState_OnAction;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        gridState.WhereCondition = new WhereCondition()
            .WhereEquals("StateObjectID", ObjectID)
            .WhereEquals("StateObjectType", ObjectType)
            .ToString(true);
        gridState.EditActionUrl = EditActionUrl;
    }

    #endregion


    #region "Grid events"

    private void gridState_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int stateId = ValidationHelper.GetInteger(actionArgument, 0);

                var obj = ProviderHelper.GetInfoById(ObjectType, ObjectID);
                var state = AutomationStateInfo.Provider.Get(stateId);

                if (!CurrentUser.IsAuthorizedPerResource(ModuleName.ONLINEMARKETING, "RemoveProcess", SiteContext.CurrentSiteName))
                {
                    RedirectToAccessDenied(ModuleName.ONLINEMARKETING, "RemoveProcess");
                }

                AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
                manager.RemoveProcess(obj, state);

                break;
        }
    }


    /// <summary>
    /// External history binding.
    /// </summary>
    protected object gridState_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "stepname":
                return HTMLHelper.HTMLEncode(GetStep(parameter).StepDisplayName);

            case "processname":
                return HTMLHelper.HTMLEncode(GetProcess(parameter).WorkflowDisplayName);

            case "status":
                return AutomationHelper.GetProcessStatus((ProcessStatusEnum)ValidationHelper.GetInteger(parameter, 0));

            case "delete":
                CMSGridActionButton btn = (CMSGridActionButton)sender;           
                var confirmationMessage = String.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(PredefinedObjectType.CONTACT).ToLowerCSafe()));
                var confirmationScript = "if(!confirm(" + ScriptHelper.GetString(confirmationMessage) + ")) { return false; } ";
                
                // In OnClientClick is JS code to call Unigrid's OnAction, we need to add this code to the end of new JS code to preserve it    
                btn.OnClientClick = confirmationScript + btn.OnClientClick;
               
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteContext.CurrentSiteName))
                {
                    btn.Enabled = false;
                }
                break;
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns step info.
    /// </summary>
    /// <param name="id">Step ID</param>
    /// <returns>Step info object</returns>
    private WorkflowStepInfo GetStep(object id)
    {
        int stepId = ValidationHelper.GetInteger(id, 0);
        return WorkflowStepInfo.Provider.Get(stepId);
    }


    /// <summary>
    /// Returns process info.
    /// </summary>
    /// <param name="id">Process ID</param>
    /// <returns>Workflow info object</returns>
    private WorkflowInfo GetProcess(object id)
    {
        int workflowID = ValidationHelper.GetInteger(id, 0);
        return WorkflowInfo.Provider.Get(workflowID);
    }

    #endregion
}
