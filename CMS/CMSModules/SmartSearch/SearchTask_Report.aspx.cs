using System;

using CMS.Helpers;

using System.Linq;
using System.Text;

using CMS.Base;
using CMS.DataEngine;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_SearchTask_Report : GlobalAdminPage
{
    #region "Variables"

    private SearchTaskInfo mSearchTaskInfo = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the search task identifier.
    /// </summary>
    private int SearchTaskID
    {
        get
        {
            return QueryHelper.GetInteger("taskid", 0);
        }
    }


    /// <summary>
    /// Gets the search task info object.
    /// </summary>
    private SearchTaskInfo SearchTaskInfo
    {
        get
        {
            return mSearchTaskInfo ?? (mSearchTaskInfo = SearchTaskInfoProvider.GetSearchTaskInfo(SearchTaskID));
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnLoad event handler
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register modal dialog scripts
        RegisterModalPageScripts();

        PageTitle.TitleText = GetString("smartsearch.taskreport");
    }


    /// <summary>
    /// OnPreRender event handler
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        if (SearchTaskInfo != null)
        {
            GeneralizedInfo relatedObjectInfo = ProviderHelper.GetInfoById(SearchTaskInfo.SearchTaskRelatedObjectType, SearchTaskInfo.SearchTaskRelatedObjectID);
            string relatedObjectStr = String.Empty;

            if (relatedObjectInfo == null)
            {
                relatedObjectStr = ResHelper.GetStringFormat(
                    "smartsearch.searchtaskrelatedobjectnotexist",
                    TypeHelper.GetNiceObjectTypeName(SearchTaskInfo.SearchTaskRelatedObjectType),
                    SearchTaskInfo.SearchTaskRelatedObjectID
                );
            }
            else
            {
                relatedObjectStr = relatedObjectInfo.GetFullObjectName(false, true, false);
            }

            StringBuilder report = new StringBuilder();
            report.Append("<div class='form-horizontal'>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.tasktype"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(GetString("smartsearch.tasktype." + SearchTaskInfo.SearchTaskType.ToStringRepresentation())), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskobjecttype"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(SearchTaskInfo.SearchTaskObjectType)), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskfield"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskInfo.SearchTaskField), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskvalue"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskInfo.SearchTaskValue), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskrelatedobject"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(relatedObjectStr), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskservername"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskInfo.SearchTaskServerName), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskcreated"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskInfo.SearchTaskCreated.ToString()), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskstatus"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", UniGridFunctions.ColoredSpanMsg(HTMLHelper.HTMLEncode(GetString("smartsearch.searchtaskstatusenum." + SearchTaskInfo.SearchTaskStatus.ToStringRepresentation())), SearchTaskInfo.SearchTaskStatus != SearchTaskStatusEnum.Error), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskerrormessage"), ":</strong></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskInfo.SearchTaskErrorMessage), "</span></div></div>");
            report.Append("</div>");

            lblReport.Text = report.ToString();
        }
        else
        {
            lblReport.Text = GetString("srch.task.tasknotexist");
        }
    }

    #endregion
}
