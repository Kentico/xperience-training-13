using System;
using System.Data;

using CMS.Automation;
using CMS.Base;
using CMS.ContactManagement.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title("ma.history")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Process_History : CMSAutomationPage
{
    private int mStateID;
    private AutomationStateInfo mState;


    private int StateID
    {
        get
        {
            if (mStateID <= 0)
            {
                mStateID = QueryHelper.GetInteger("stateid", 0);
            }

            return mStateID;
        }
    }


    private AutomationStateInfo State
    {
        get
        {
            if (mState == null)
            {
                mState = AutomationStateInfo.Provider.Get(StateID);

                if (mState == null)
                {
                    RedirectToInformation("editedobject.notexists");
                }
            }

            return mState;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckSecurity();
        InitUnigrid();

        CurrentMaster.FooterContainer.Visible = false;
    }


    protected void InitUnigrid()
    {
        gridHistory.ZeroRowsText = String.Format(GetString("ma.nohistoryyet"), TypeHelper.GetNiceObjectTypeName(AutomationStateInfo.OBJECT_TYPE).ToLowerInvariant());
        gridHistory.WhereCondition = "HistoryStateID=" + State.StateID;
    }


    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv;

        switch (sourceName.ToLowerInvariant())
        {
            case "action":
                drv = (DataRowView)parameter;
                var wasRejected = ValidationHelper.GetBoolean(drv["HistoryWasRejected"], false);

                // Get type of the steps
                WorkflowStepTypeEnum stepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryStepType"), 0);
                WorkflowStepTypeEnum targetStepType = (WorkflowStepTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTargetStepType"), 0);
                WorkflowTransitionTypeEnum transitionType = (WorkflowTransitionTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "HistoryTransitionType"), 0);

                if (!wasRejected)
                {
                    var isAutomatic = transitionType == WorkflowTransitionTypeEnum.Automatic;
                    var actionString = isAutomatic ? GetString("WorfklowProperties.Automatic") + " ({0})" : "{0}";

                    // Return correct step title
                    switch (targetStepType)
                    {
                        case WorkflowStepTypeEnum.Finished:
                            return String.Format(actionString, GetString("ma.finished"));

                        default:
                            if (stepType == WorkflowStepTypeEnum.Start)
                            {
                                return String.Format(actionString, GetString("ma.started"));
                            }
                            return isAutomatic ? GetString("WorfklowProperties.Automatic") : GetString("ma.movedtonextstep");
                    }
                }
                else
                {
                    return GetString("ma.movedtopreviousstep");
                }

            case "stepname":
                drv = (DataRowView)parameter;
                var step = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "HistoryStepDisplayName"), String.Empty);
                var targetStep = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "HistoryTargetStepDisplayName"), String.Empty);

                if (!String.IsNullOrEmpty(targetStep))
                {
                    step += " -> " + targetStep;
                }

                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(step));
        }

        return parameter;
    }


    private void CheckSecurity()
    {
        if (!AuthorizedForContacts || !LicenseIsSufficient)
        {
            RedirectToAccessDenied(GetString("general.notauthorizedtopage"));
        }
    }
}
