using System;
using System.Data;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Helpers.Markup;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Scoring_Controls_UI_Score_List : CMSAdminListControl
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.scoring", "Read"))
        {
            return;
        }

        var whereCondition = new WhereCondition(gridElem.WhereCondition);

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = whereCondition.ToString(true);

        gridElem.ZeroRowsText = GetString("om.score.notfound");
        gridElem.EditActionUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS.Scoring", "ScoringProperties"), "displayTitle=0&objectid={0}");
    }


    object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "scorestatus":
                var rowView = parameter as DataRowView;
                var info = new ScoreInfo(rowView.Row);
                return GetFormattedStatus(info);
        }
        return sender;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets formatted score status. Score can be disabled, it can be scheduled to rebuild in the future or its status is one of <see cref="ScoreStatusEnum"/>.
    /// </summary>
    private FormattedText GetFormattedStatus(ScoreInfo info)
    {
        var formatter = new ScoreStatusFormatter(info);
        formatter.RecalculationURL = GetRecalculationURL(info.ScoreID);
        formatter.DisplayTooltips = true;
        return formatter.GetFormattedStatus();
    }


    /// <summary>
    /// Gets URL for modal dialog for recalculation. Adds needed query parameters.
    /// </summary>
    private string GetRecalculationURL(int scoreID)
    {
        var recalculationQuery = QueryHelper.BuildQueryWithHash(new[] { "scoreID", scoreID.ToString() });
        return ResolveUrl("~/CMSModules/Scoring/Pages/ScheduleRecalculationDialog.aspx") + recalculationQuery;
    }

    #endregion
}