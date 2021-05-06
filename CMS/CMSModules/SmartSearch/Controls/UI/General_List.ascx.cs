using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.UIControls;

using Treenode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_SmartSearch_Controls_UI_General_List : CMSAdminControl, IPostBackEventHandler
{
    #region "Variables"

    private bool smartSearchEnabled = SettingsKeyInfoProvider.GetBoolValue("CMSSearchIndexingEnabled");
    private int mItemId = QueryHelper.GetInteger("indexid", 0);

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


    /// <summary>
    /// Item ID.
    /// </summary>
    public int ItemID
    {
        get
        {
            return mItemId;
        }
        set
        {
            mItemId = value;
        }
    }


    /// <summary>
    /// Search index info.
    /// </summary>
    public SearchIndexInfo SearchIndexInfo
    {
        get
        {
            return SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {

        if (!RequestHelper.IsPostBack())
        {
            if (SearchIndexInfo != null)
            {
                SearchIndexSettings sis = SearchIndexInfo.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.SIMPLE_ITEM_ID);

                if (sisi != null)
                {
                    drpObjType.Value = sisi.ClassNames;
                    txtWhere.TextArea.Text = sisi.WhereCondition;
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (SearchIndexInfo != null && smartSearchEnabled)
        {
            var excludedGeneralObjects = new List<string>
            {
                    PredefinedObjectType.CUSTOMTABLECLASS,
                    PredefinedObjectType.BIZFORM_ITEM_PREFIX,
                    PredefinedObjectType.DOCUMENT,
                    PredefinedObjectType.NODE,
                    SearchHelper.ONLINEFORMINDEX,
                }
                .Select(x => x.ToLowerInvariant())
                .ToList();

            // Add Users into excluded object if index has not type Azure
            if (!SearchIndexInfo.IndexProvider.Equals(SearchIndexInfo.AZURE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase))
            {
                excludedGeneralObjects.Add(UserInfo.OBJECT_TYPE);
            }

            var items = drpObjType.DropDownList.Items;

            var itemsToDelete = items
                .Cast<ListItem>()
                .Where(item => excludedGeneralObjects.Any(item.Value.ToLowerInvariant().StartsWith))
                .ToList();

            foreach (var item in itemsToDelete)
            {
                items.Remove(item);
            }
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate - check for selected object name
        if (string.IsNullOrEmpty(drpObjType.Value.ToString()))
        {
            ShowError(GetString("srch.index.objectname.required"));
            return;
        }

        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
        if (sii != null)
        {
            SearchIndexSettings sis = sii.IndexSettings;
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.SIMPLE_ITEM_ID);

            // Create new
            if (sisi == null)
            {
                sisi = new SearchIndexSettingsInfo();
                sisi.ID = SearchHelper.SIMPLE_ITEM_ID;
            }

            sisi.ClassNames = ValidationHelper.GetString(drpObjType.Value, String.Empty);
            sisi.WhereCondition = txtWhere.TextArea.Text.Trim();

            // Update settings item
            sis.SetSearchIndexSettingsInfo(sisi);
            // Update xml value
            sii.IndexSettings = sis;

            SearchIndexInfoProvider.SetSearchIndexInfo(sii);

            ShowChangesSaved();

            // Display a message
            if (smartSearchEnabled)
            {
                ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
            }
        }
    }


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii.IndexType.Equals(Treenode.OBJECT_TYPE, StringComparison.OrdinalIgnoreCase))
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

            if (SearchHelper.CreateRebuildTask(ItemID))
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