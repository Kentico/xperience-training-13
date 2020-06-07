using System;
using System.Data;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_OnLineForm_List : CMSAdminListControl, IPostBackEventHandler
{
    #region "Variables"

    private int indexId = 0;
    private SearchIndexInfo sii = null;
    private SearchIndexSettings sis = null;
    private bool smartSearchEnabled = SettingsKeyInfoProvider.GetBoolValue("CMSSearchIndexingEnabled");

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Show panel with message how to enable indexing
        ucDisabledModule.TestSettingKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.OnDataReload += UniGrid_OnDataReload;
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
        UniGrid.ShowActionsMenu = true;
        UniGrid.AllColumns = "id, Type, DisplayName, ClassNames, WhereCondition";

        indexId = QueryHelper.GetInteger("indexid", 0);

        sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexId);
    }


    #region "Events"

    /// <summary>
    /// Reloads datagrid.
    /// </summary>
    private DataSet UniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        DataSet sorted = null;

        if (sii != null)
        {
            DataSet result = sii.IndexSettings.GetAll();

            if (!DataHelper.DataSourceIsEmpty(result))
            {
                // Set 'id' column to first position
                if (result.Tables[0].Columns["id"] != null)
                {
                    result.Tables[0].Columns["id"].SetOrdinal(0);
                }

                // Check if 'type' column exists
                if (result.Tables[0].Columns["type"] == null)
                {
                    result.Tables[0].Columns.Add(new DataColumn("type"));
                }

                // Sort result
                sorted = new DataSet();
                sorted.Tables.Add(result.Tables[0].Clone());

                if (result.Tables[0].Columns.Contains("classnames"))
                {
                    foreach (DataRow dr in result.Tables[0].Select(null, "classnames"))
                    {
                        sorted.Tables[0].ImportRow(dr);
                    }
                }
            }
        }
        return sorted;
    }


    /// <summary>
    /// Unigrid databound handler.
    /// </summary>
    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "sitename":
                var site = SiteInfo.Provider.Get(ValidationHelper.GetString(parameter, ""));
                if (site != null)
                {
                    return site.DisplayName;
                }
                break;
        }
        return parameter;
    }


    /// <summary>
    /// Unigrid on action handler.
    /// </summary>
    private void UniGrid_OnAction(string actionName, object actionArgument)
    {
        Guid guid;
        switch (actionName)
        {
            case "edit":
                guid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
                RaiseOnAction("edit", guid);
                break;

            case "delete":
                // Delete search index info object from database with it's dependences
                guid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);

                sis = sii.IndexSettings;
                sis.DeleteSearchIndexSettingsInfo(guid);
                sii.IndexSettings = sis;
                SearchIndexInfoProvider.SetSearchIndexInfo(sii);

                // Show message about rebuilding index
                if (smartSearchEnabled)
                {
                    ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                }
                break;
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            if (SearchHelper.CreateRebuildTask(indexId))
            {
                ShowInformation(GetString("srch.index.rebuildstarted"));
            }
            else
            {
                ShowError(GetString("index.nocontent"));
            }
        }
    }

    #endregion
}