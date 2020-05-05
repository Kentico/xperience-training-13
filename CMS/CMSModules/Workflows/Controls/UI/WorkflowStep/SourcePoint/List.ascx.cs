using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine.Web.UI;

public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_List : WorkflowStepSourcePointListControl
{
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMess;


    /// <summary>
    /// Unigrid control
    /// </summary>
    protected override UniGrid ListControl => gridElem;


    protected override void OnListControlAction(string actionName, object actionArgument)
    {
        var sourcePointGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
        var oldIndex = CurrentStepInfo.StepDefinition.SourcePoints.FindIndex(i => i.Guid == sourcePointGuid);
        var graphName = QueryHelper.GetString("graph", String.Empty);

        if (sourcePointGuid != Guid.Empty)
        {
            switch (actionName.ToLowerInvariant())
            {
                case "edit":
                    var url = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl(ModuleName.CMS, "Workflows.EditCase", false), "workflowStepId", WorkflowStepID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "isindialog", QueryHelper.GetBoolean("isindialog", false).ToString());
                    url = AddHashAndGraphParametersToUrl(url, sourcePointGuid.ToString());

                    URLHelper.Redirect(url);
                    break;

                case "delete":
                    if (CreateDeleteAction(sourcePointGuid))
                    {
                        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
                    }
                    break;

                case "moveup":
                    if (CurrentStepInfo.MoveSourcePoint(oldIndex, oldIndex - 1))
                    {
                        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
                    }
                    break;

                case "movedown":
                    if (CurrentStepInfo.MoveSourcePoint(oldIndex, oldIndex + 1))
                    {
                        WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, graphName);
                    }
                    break;
            }
        }
    }


    protected override object OnListControlExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "allowaction":
                // Default case can't be moved or deleted. The same goes for condition step type case
                var container = (GridViewRow)parameter;
                var sourcePointType = (SourcePointTypeEnum)ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue((DataRowView)container.DataItem, "Type"), 0);
                if ((sourcePointType != SourcePointTypeEnum.SwitchCase) || (CurrentStepInfo.StepType == WorkflowStepTypeEnum.Condition))
                {
                    ((Control)sender).Visible = false;
                }
                break;

            default:
                return base.OnListControlExternalDataBound(sender, sourceName, parameter);
        }

        return parameter;
    }
}