using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Automation_Controls_Process_Edit : CMSUserControl
{
    private CMSAutomationManager mAutomationManager;


    #region "Properties"

    /// <summary>
    /// Object instance
    /// </summary>
    public BaseInfo InfoObject
    {
        get
        {
            return AutomationManager.InfoObject;
        }
    }


    /// <summary>
    /// State object
    /// </summary>
    public AutomationStateInfo StateObject
    {
        get
        {
            return AutomationManager.StateObject;
        }
    }


    /// <summary>
    /// Automation manager
    /// </summary>
    public AutomationManager Manager
    {
        get
        {
            return AutomationManager.Manager;
        }
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

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// External history binding.
    /// </summary>
    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "action":
                drv = (DataRowView)parameter;
                bool wasrejected = ValidationHelper.GetBoolean(drv["HistoryWasRejected"], false);

                // Get type of the steps
                WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryStepType"), 0);
                WorkflowStepTypeEnum targetStepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTargetStepType"), 0);
                WorkflowTransitionTypeEnum transitionType = (WorkflowTransitionTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTransitionType"), 0);

                if (!wasrejected)
                {
                    bool isAutomatic = (transitionType == WorkflowTransitionTypeEnum.Automatic);
                    string actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") + " ({0})" : "{0}";
                    // Return correct step title
                    switch (targetStepType)
                    {
                        case WorkflowStepTypeEnum.Finished:
                            actionString = string.Format(actionString, GetString("ma.finished"));
                            break;

                        default:
                            if (stepType == WorkflowStepTypeEnum.Start)
                            {
                                actionString = string.Format(actionString, GetString("ma.started"));
                            }
                            else
                            {
                                actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") : GetString("ma.movedtonextstep");
                            }
                            break;
                    }

                    return actionString;
                }
                else
                {
                    return GetString("ma.movedtopreviousstep");
                }

            case "stepname":
                drv = (DataRowView)parameter;
                string step = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "HistoryStepDisplayName"), String.Empty);
                string targetStep = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "HistoryTargetStepDisplayName"), String.Empty);
                if (!string.IsNullOrEmpty(targetStep))
                {
                    step += " -> " + targetStep;
                }
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(step));
        }
        return parameter;
    }


    /// <summary>
    /// Reloads the page data.
    /// </summary>
    protected void ReloadData()
    {
        if (InfoObject != null)
        {
            var workflow = AutomationManager.Process;
            if (workflow != null)
            {
                ucDesigner.WorkflowID = workflow.WorkflowID;
                ucDesigner.SelectedStepID = StateObject.StateStepID;
                ucDesigner.ReadOnly = true;
                ucDesigner.Height = new Unit(350);

                // Initialize grids
                gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
                gridHistory.ZeroRowsText = string.Format(GetString("ma.nohistoryyet"), TypeHelper.GetNiceObjectTypeName(InfoObject.TypeInfo.ObjectType).ToLowerInvariant());
            }
        }
        else
        {
            pnlWorkflow.Visible = false;
        }

        gridHistory.WhereCondition = "HistoryStateID=" + AutomationManager.StateObjectID;
        gridHistory.ReloadData();
    }

    #endregion
}