using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_Custom : CMSAdminControl, IPostBackEventHandler
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.CUSTOM_INDEX_DATA);

                // Create new
                if (sisi != null)
                {
                    assemblyElem.AssemblyName = ValidationHelper.GetString(sisi.GetValue("AssemblyName"), String.Empty);
                    assemblyElem.ClassName = ValidationHelper.GetString(sisi.GetValue("ClassName"), String.Empty);
                    txtData.TextArea.Text = ValidationHelper.GetString(sisi.GetValue("CustomData"), String.Empty);
                }
            }
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (assemblyElem.IsValid())
        {
            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(ItemID);
            if (sii != null)
            {
                SearchIndexSettings sis = sii.IndexSettings;
                SearchIndexSettingsInfo sisi = sis.GetSearchIndexSettingsInfo(SearchHelper.CUSTOM_INDEX_DATA);

                // Create new
                if (sisi == null)
                {
                    sisi = new SearchIndexSettingsInfo();
                    sisi.ID = SearchHelper.CUSTOM_INDEX_DATA;
                }

                sisi.SetValue("AssemblyName", assemblyElem.AssemblyName.Trim());
                sisi.SetValue("ClassName", assemblyElem.ClassName.Trim());
                sisi.SetValue("CustomData", txtData.TextArea.Text);

                // Update settings item
                sis.SetSearchIndexSettingsInfo(sisi);
                // Update xml value
                sii.IndexSettings = sis;

                SearchIndexInfoProvider.SetSearchIndexInfo(sii);

                ShowChangesSaved();

                // Redirect to edit mode
                if (smartSearchEnabled)
                {
                    ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                }
            }
        }
        else
        {
            ShowError(assemblyElem.ErrorMessage);
            return;
        }
    }

    #endregion


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