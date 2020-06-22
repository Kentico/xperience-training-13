using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_OnlineMarketing_Controls_UI_WorkflowStep_SourcePoint_List : WorkflowStepSourcePointListControl
{
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMess;


    /// <summary>
    /// Unigrid control
    /// </summary>
    protected override UniGrid ListControl => gridElem;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ListControl.Pager.DisplayPager = false;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            ScriptHelper.RegisterDialogScript(Page);

            // Refresh designer after a new case is created and redirected to this page
            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, QueryHelper.GetString("graph", String.Empty));
            }

            if (CurrentStepInfo?.StepType == WorkflowStepTypeEnum.Multichoice)
            {
                ShowInformation(GetString("workflow.multichoicenotsupported"));
            }
        }
    }


    protected override void OnListControlAction(string actionName, object actionArgument)
    {
        var graphName = QueryHelper.GetString("graph", String.Empty);

        switch (actionName.ToLowerInvariant())
        {
            case "delete":
                var sourcePointGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
                if (sourcePointGuid != Guid.Empty && CreateDeleteAction(sourcePointGuid))
                {
                    WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
                }
                break;

            case "#move":
                if (UniGridFunctions.TryParseMoveActionArguments(actionArgument.ToString(), out _, out var oldIndex, out var newIndex))
                {
                    if (CurrentStepInfo.MoveSourcePoint(oldIndex, newIndex))
                    {
                        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
                    }
                }
                break;
        }
    }


    protected override object OnListControlExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "edit":
                var editButton = (CMSGridActionButton)sender;

                var url = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditStepProperties.Cases.EditCase", true), "workflowStepId", WorkflowStepID.ToString());
                url = URLHelper.AddParameterToUrl(url, "dialog", "True");
                url = AddHashAndGraphParametersToUrl(url, editButton.CommandArgument);

                editButton.OnClientClick = ScriptHelper.GetModalDialogScript(url, "StepSourcePointEdit", "90%", "85%");
                break;

            case "allowaction":
                // Default case can't be moved or deleted. The same goes for condition step type case
                var container = (GridViewRow)parameter;
                var sourcePointType = (SourcePointTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue((DataRowView)container.DataItem, "Type"), 0);
                if ((sourcePointType != SourcePointTypeEnum.SwitchCase) || (CurrentStepInfo.StepType == WorkflowStepTypeEnum.Condition))
                {
                    var button = (CMSGridActionButton)sender;
                    button.Enabled = false;
                    button.RemoveCssClass("js-_move");
                    container.AddCssClass("unsortable");
                }
                break;

            default:
                return base.OnListControlExternalDataBound(sender, sourceName, parameter);
        }

        return parameter;
    }


    protected override bool CanShowDefaultStepInformation()
    {
        return base.CanShowDefaultStepInformation() && CurrentStepInfo?.StepType != WorkflowStepTypeEnum.Multichoice;
    }
}