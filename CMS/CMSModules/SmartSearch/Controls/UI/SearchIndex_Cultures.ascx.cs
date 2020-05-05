using System;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Cultures : CMSAdminListControl, IPostBackEventHandler
{
    #region "Variables"

    private int indexId = 0;
    private string currentValues = "";

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


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        indexId = QueryHelper.GetInteger("indexid", 0);

        // Add sites filter
        uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
        uniSelector.SetValue("DefaultFilterValue", SiteContext.CurrentSiteID);
        uniSelector.SetValue("FilterMode", "cultures");

        // Get the active sites
        DataSet ds = SearchIndexCultureInfoProvider.GetSearchIndexCultures("IndexID = " + indexId, null, 0, "IndexID, IndexCultureID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "IndexCultureID"));
        }

        if (!RequestHelper.IsPostBack())
        {
            uniSelector.Value = currentValues;
        }
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Remove all new items
                foreach (string item in newItems)
                {
                    int cultureId = ValidationHelper.GetInteger(item, 0);
                    SearchIndexCultureInfo sici = SearchIndexCultureInfoProvider.GetSearchIndexCultureInfo(indexId, cultureId);
                    SearchIndexCultureInfoProvider.DeleteSearchIndexCultureInfo(sici);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items
                foreach (string item in newItems)
                {
                    int cultureId = ValidationHelper.GetInteger(item, 0);
                    SearchIndexCultureInfoProvider.AddSearchIndexCulture(indexId, cultureId);
                }
            }
        }

        // Show saved message with rebuild link
        ShowChangesSaved();
        ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            // Check if document index has at least one culture specified
            if (!SearchIndexCultureInfoProvider.SearchIndexHasAnyCulture(indexId))
            {
                ShowError(GetString("index.noculture"));
                return;
            }

            // Check if there is at least one site assigned
            if (!SearchIndexSiteInfoProvider.SearchIndexHasAnySite(indexId))
            {
                ShowError(GetString("index.nosite"));
                return;
            }

            if (!SearchHelper.CreateRebuildTask(indexId))
            {
                ShowError(GetString("index.nocontent"));
                return;
            }
            
            ShowInformation(GetString("srch.index.rebuildstarted"));
        }
    }

    #endregion
}