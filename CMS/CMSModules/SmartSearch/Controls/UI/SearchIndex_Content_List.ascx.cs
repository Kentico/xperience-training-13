using System;
using System.Data;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Content_List : CMSAdminListControl, IPostBackEventHandler
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


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Show panel with message how to enable indexing
        ucDisabledModule.TestSettingKeys = "CMSSearchIndexingEnabled";
        ucDisabledModule.InfoText = GetString("srch.searchdisabledinfo");

        indexId = QueryHelper.GetInteger("indexid", 0);

        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.OnDataReload += UniGrid_OnDataReload;
        UniGrid.ShowActionsMenu = true;
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
        UniGrid.AllColumns = "id, ClassNames, Path, Type";

        sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexId);
    }


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
                if (result.Tables[0].Columns.Contains("type") && result.Tables[0].Columns.Contains("classNames") && result.Tables[0].Columns.Contains("path"))
                {
                    foreach (DataRow dr in result.Tables[0].Select(null, "type, classNames, path"))
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
        DataRowView drv = (parameter as DataRowView);

        switch (sourceName.ToLowerCSafe())
        {
            case "documenttypes":
                string classname = HTMLHelper.HTMLEncode(TextHelper.LimitLength(drv["classNames"].ToString(), 50));
                if (string.IsNullOrEmpty(classname))
                {
                    return GetString("general.selectall");
                }
                else
                {
                    return classname;
                }

            case "location":
                return HTMLHelper.HTMLEncode(TextHelper.LimitLength(drv["path"].ToString(), 50));

            case "type":
                string type = HTMLHelper.HTMLEncode(TextHelper.LimitLength(drv["type"].ToString().ToLowerCSafe(), 50));
                if (type == "allowed")
                {
                    type = "<span class=\"StatusEnabled\">" + GetString("srch.list.allowed") + "</span>";
                }
                else if (type == "excluded")
                {
                    type = "<span class=\"StatusDisabled\">" + GetString("srch.list.excluded") + "</span>";
                }
                return type;
        }
        return null;
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
                    DataSet result = sii.IndexSettings.GetAll();
                    if (!DataHelper.DataSourceIsEmpty(result))
                    {
                        ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                    }
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
            if (sii.IndexType.Equals(TreeNode.OBJECT_TYPE, StringComparison.OrdinalIgnoreCase) || (sii.IndexType == SearchHelper.DOCUMENTS_CRAWLER_INDEX))
            {
                if (!SearchIndexCultureInfoProvider.SearchIndexHasAnyCulture(sii.IndexID))
                {
                    ShowError(GetString("index.noculture"));
                    return;
                }

                if (!SearchIndexSiteInfoProvider.SearchIndexHasAnySite(sii.IndexID))
                {
                    ShowError(GetString("index.nosite"));
                    return;
                }
            }

            if (SearchHelper.CreateRebuildTask(sii.IndexID))
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