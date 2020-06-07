using System;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.Helpers;
using CMS.Search.Azure;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_SearchTaskAzure_Report : GlobalAdminPage
{
    private SearchTaskAzureInfo mSearchTaskAzureInfo;
    
    
    /// <summary>
    /// Gets the search task identifier.
    /// </summary>
    private int SearchTaskAzureID
    {
        get
        {
            return QueryHelper.GetInteger("taskid", 0);
        }
    }


    /// <summary>
    /// Gets the search task info object.
    /// </summary>
    private SearchTaskAzureInfo SearchTaskAzureInfo
    {
        get
        {
            return mSearchTaskAzureInfo ?? (mSearchTaskAzureInfo = SearchTaskAzureInfo.Provider.Get(SearchTaskAzureID));
        }
    }


    /// <summary>
    /// OnLoad event handler
    /// </summary>
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
    protected override void OnPreRender(EventArgs e)
    {
        if (SearchTaskAzureInfo != null)
        {
            StringBuilder report = new StringBuilder();
            report.Append("<div class='form-horizontal'>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.tasktype"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(GetString("smartsearch.tasktype." + SearchTaskAzureInfo.SearchTaskAzureType.ToStringRepresentation())), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskobjecttype"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(SearchTaskAzureInfo.SearchTaskAzureObjectType)), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.additionaldata"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskAzureInfo.SearchTaskAzureAdditionalData), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskcreated"), ":</span></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskAzureInfo.SearchTaskAzureCreated.ToString()), "</span></div></div>");
            report.Append("<div class='form-group'><div class='editing-form-label-cell'><span class='control-label'>", GetString("smartsearch.task.taskerrormessage"), ":</strong></div><div class='editing-form-value-cell'><span class='form-control-text'>", HTMLHelper.HTMLEncode(SearchTaskAzureInfo.SearchTaskAzureErrorMessage), "</span></div></div>");
            report.Append("</div>");

            lblReport.Text = report.ToString();
        }
        else
        {
            lblReport.Text = GetString("srch.task.tasknotexist");
        }
    }
}
