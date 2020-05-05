using System;
using System.Linq;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_OnLineForm_Edit : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

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

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        // Set events and default values for site selector
        selSite.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
        selSite.PostbackOnDropDownChange = true;
        selSite.UseCodeNameForSelection = true;

        // Init controls
        if (!RequestHelper.IsPostBack())
        {
            LoadControls();
        }
    }


    private void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetControlsStatus(true);
    }


    /// <summary>
    /// Enable or disable controls with dependence on current settings.
    /// </summary>
    /// <param name="clear">Indicates whether selector value should be cleared</param>
    protected void SetControlsStatus(bool clear)
    {
        if (clear)
        {
            selectForm.Value = null;
        }

        selectForm.Enabled = true;

        string siteName = ValidationHelper.GetString(selSite.Value, String.Empty);
        if (String.IsNullOrEmpty(siteName) || (siteName == "-1"))
        {
            selectForm.Enabled = false;
            btnOk.Enabled = false;
        }
        else
        {
            selectForm.WhereCondition = new WhereCondition()
                .WhereEquals("FormSiteID", SiteInfoProvider.GetSiteID(siteName))
                .ToString(true);
        }
        selectForm.Reload(true);
    }



    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public void LoadControls()
    {
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);

        // Load only those site to which the index is assigned
        var siteIDs = SearchIndexSiteInfoProvider.GetIndexSiteBindings(ItemID).Select(x => x.IndexSiteID).ToList();
        if (siteIDs.Any())
        {
            // Preselect current site if it is assigned to index
            if (!RequestHelper.IsPostBack() && siteIDs.Contains(SiteContext.CurrentSiteID))
            {
                selSite.Value = SiteContext.CurrentSiteName;
            }

            selSite.UniSelector.WhereCondition = SqlHelper.GetWhereInCondition("SiteID", siteIDs, false, false);
            selSite.Reload(false);
        }
        else
        {
            pnlForm.Visible = false;

            ShowError(GetString("srch.index.nositeselected"));
        }

        // If we are editing existing search index
        if (sii != null)
        {
            SearchIndexSettings sis = new SearchIndexSettings();
            sis.LoadData(sii.IndexSettings.GetData());
            SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(ItemGUID);
            if (sisi != null)
            {
                selectForm.Value = ValidationHelper.GetString(sisi.GetValue("FormName"), "");
                txtWhere.TextArea.Text = sisi.WhereCondition;
                selSite.Value = sisi.SiteName;
            }
        }

        // Init controls
        SetControlsStatus(false);
    }


    /// <summary>
    /// Stores data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!selSite.UniSelector.HasData || !selectForm.HasData)
        {
            ShowError(GetString("srch.err.selectform"));
            return;
        }

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
            }

            // Save values
            if (sisi != null)
            {
                string formName = ValidationHelper.GetString(selectForm.Value, "");

                sisi.SetValue("FormName", formName);
                sisi.WhereCondition = txtWhere.TextArea.Text.Trim();
                sisi.SiteName = selSite.Value.ToString();

                BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formName, sisi.SiteName);
                if (bfi != null)
                {
                    sisi.SetValue("DisplayName", bfi.FormDisplayName);
                    sisi.ClassNames = BizFormItemProvider.BIZFORM_ITEM_PREFIX + DataClassInfoProvider.GetClassName(bfi.FormClassID);
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