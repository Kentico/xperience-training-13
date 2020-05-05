using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_CustomTable_Edit : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private string mItemType = null;

    private bool smartSearchEnabled = SettingsKeyInfoProvider.GetBoolValue("CMSSearchIndexingEnabled");

    #endregion


    #region "Public properties"

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
    /// Gets or sets item type.
    /// </summary>
    public string ItemType
    {
        get
        {
            return mItemType;
        }
        set
        {
            mItemType = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            customTableSelector.StopProcessing = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // Init controls
        if (!StopProcessing && !RequestHelper.IsPostBack())
        {
            LoadControls();
        }
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public void LoadControls()
    {
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);

        // If we are editing existing search index
        if (sii != null)
        {
            SearchIndexSettings sis = new SearchIndexSettings();
            sis.LoadData(sii.IndexSettings.GetData());
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
            if (sisi != null)
            {
                customTableSelector.Value = sisi.ClassNames;
                txtWhere.TextArea.Text = sisi.WhereCondition;
            }
        }
    }


    /// <summary>
    /// Stores data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
        if (sii != null)
        {
            SearchIndexSettings sis = sii.IndexSettings;
            SearchIndexSettingsInfo sisi;

            var isInsert = false;

            if (ItemGUID != Guid.Empty)
            {
                // If we are updating existing Search Index Settings Info
                sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
            }
            else
            {
                // If we are creating new Search Index Settings Info
                isInsert = true;

                sisi = new SearchIndexSettingsInfo();
                sisi.ID = Guid.NewGuid();
                sisi.Type = ItemType;
            }

            // Save values
            if (sisi != null)
            {
                sisi.ClassNames = Convert.ToString(customTableSelector.Value);
                sisi.WhereCondition = txtWhere.TextArea.Text.Trim();

                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(sisi.ClassNames);
                if (dci != null)
                {
                    sisi.SetValue("DisplayName", dci.ClassDisplayName);
                }

                // Update settings item
                sis.SetSearchIndexSettingsInfo(sisi);
                
                // Update xml value
                sii.IndexSettings = sis;
                SearchIndexInfoProvider.SetSearchIndexInfo(sii);
                ItemGUID = sisi.ID;

                if (isInsert)
                {
                    // Redirect to edit mode
                    var editUrl = "SearchIndex_Content_Edit.aspx";
                    editUrl = URLHelper.AddParameterToUrl(editUrl, "indexId", sii.IndexID.ToString());
                    editUrl = URLHelper.AddParameterToUrl(editUrl, "guid", sisi.ID.ToString());
                    editUrl = URLHelper.AddParameterToUrl(editUrl, "saved", "1");
                    if (smartSearchEnabled)
                    {
                        editUrl = URLHelper.AddParameterToUrl(editUrl, "rebuild", "1");
                    }
                    URLHelper.Redirect(UrlResolver.ResolveUrl(editUrl));
                }

                ShowChangesSaved();

                if (smartSearchEnabled)
                {
                    // Show rebuild message
                    ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                }
            }
            // Error loading SearchIndexSettingsInfo
            else
            {
                ShowError(GetString("srch.err.loadingsisi"));
            }
        }
        // Error loading SearchIndexInfo
        else
        {
            ShowError(GetString("srch.err.loadingsii"));
        }
    }


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
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