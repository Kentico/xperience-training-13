using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Cultures_Controls_UI_ResourceStringEdit : CMSAdminEditControl
{
    #region "Private variables"

    private Dictionary<string, FormEngineUserControl> mTranslations = new Dictionary<string, FormEngineUserControl>();
    private ResourceStringInfo mResourceStringInfo;
    private string selectedCultureCode = QueryHelper.GetString("culturecode", CultureHelper.PreferredUICultureCode);
    private bool mEnableTranslations = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Redirects url after save. May contain one parameter for string key.
    /// (e.g., Edit.aspx?stringkey={0})
    /// </summary>
    public string RedirectUrlAfterSave
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets save button visiblity.
    /// </summary>
    public bool ShowSaveButton
    {
        get
        {
            return btnOk.Visible;
        }
        set
        {
            btnOk.Visible = value;
        }
    }


    /// <summary>
    /// Edited resource string key.
    /// </summary>
    public string ResourceStringKey
    {
        get
        {
            return txtStringKey.Text.Trim();
        }
    }


    /// <summary>
    /// Indicates if default translation is required.
    /// </summary>
    public bool DefaultTranslationRequired
    {
        get;
        set;
    }


    /// <summary>
    /// Enable/disable translation boxes.
    /// </summary>
    public bool EnableTranslations
    {
        get
        {
            return mEnableTranslations;
        }
        set
        {
            mEnableTranslations = value;
        }
    }


    /// <summary>
    /// Resource string key which was already edited.
    /// </summary>
    private String EditedResourceStringKey
    {
        get
        {
            return ValidationHelper.GetString(ViewState["EditedResourceStringKey"], String.Empty);
        }
        set
        {
            ViewState["EditedResourceStringKey"] = value;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings"))
        {
            txtStringKey.Enabled = false;
            chkIsCustom.Enabled = false;
            EnableTranslations = false;
        }

        mResourceStringInfo = ResourceStringInfo.Provider.Get(EditedResourceStringKey);
        if (mResourceStringInfo == null)
        {
            mResourceStringInfo = ResourceStringInfo.Provider.Get(QueryHelper.GetInteger("stringid", 0));
            if (mResourceStringInfo == null)
            {
                // Try to load resource string info by string key
                string stringKey = QueryHelper.GetString("stringkey", String.Empty);
                mResourceStringInfo = ResourceStringInfo.Provider.Get(stringKey);

                if (mResourceStringInfo == null)
                {
                    mResourceStringInfo = new ResourceStringInfo();
                    mResourceStringInfo.StringIsCustom = !SystemContext.DevelopmentMode;
                    mResourceStringInfo.StringKey = stringKey;
                }
            }
        }

        // Set edited object
        EditedObject = mResourceStringInfo;

        if (!RequestHelper.IsPostBack())
        {
            txtStringKey.Text = mResourceStringInfo.StringKey;
            chkIsCustom.Checked = mResourceStringInfo.StringIsCustom;
            plcIsCustom.Visible = SystemContext.DevelopmentMode;

            // Automatically display changes saved text
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }

            LoadLastSelectedItem();
        }

        tblHeaderCellLabel.Text = GetString("culture.translation");

        ReloadData();
    }


    /// <summary>
    /// Handles filter button click.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        txtFilter.Focus();
    }


    /// <summary>
    /// Handles submit button click.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        Save();
    }


    /// <summary>
    /// Handles radio button list index change.
    /// </summary>
    protected void rbCultures_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Save last selected item
        SessionHelper.SetValue("SelectedValue", rbCultures.SelectedValue);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        DataSet cultures = GetCultures();
        if (DataHelper.DataSourceIsEmpty(cultures))
        {
            return;
        }

        foreach (DataRow dr in cultures.Tables[0].Rows)
        {
            string cultureName = ValidationHelper.GetString(dr["CultureName"], String.Empty);
            string cultureCode = ValidationHelper.GetString(dr["CultureCode"], String.Empty);

            TableRow row = GetRow(cultureName, cultureCode);

            tblGrid.Rows.Add(row);
        }

        SetTranslationElementId();
    }


    /// <summary>
    /// Saves resource string and its translations. Returns true if the string is successfully saved.
    /// </summary>
    public bool Save()
    {
        if (!EnableTranslations)
        {
            return false;
        }

        string resKey = ResourceStringKey;
        if (!ValidationHelper.IsCodeName(resKey))
        {
            ShowError(GetString("culture.invalidresstringkey"));
            return false;
        }

        // Check if key is free for use (must be free or id must be same)
        if (!KeyIsFreeToUse(mResourceStringInfo.StringID, resKey))
        {
            base.ShowError(ResHelper.GetStringFormat("localizable.keyexists", resKey));
            return false;
        }

        // Check if default translation is set if required
        if (DefaultTranslationRequired)
        {
            string defaultTranslation = mTranslations[CultureHelper.DefaultUICultureCode.ToLowerInvariant()].Text.Trim();
            string defaultCultureName = CultureInfo.Provider.Get(CultureHelper.DefaultUICultureCode).CultureName;
            if (String.IsNullOrEmpty(defaultTranslation))
            {
                base.ShowError(ResHelper.GetStringFormat("localizable.deletedefault", defaultCultureName));
                return false;
            }
        }

        // Log staging tasks synchronously
        using (new CMSActionContext { AllowAsyncActions = false })
        {
            SaveResourceStringInfo(resKey);
            EditedResourceStringKey = resKey;
            SaveTranslations();
        }

        ShowChangesSaved();

        if (!String.IsNullOrEmpty(RedirectUrlAfterSave))
        {
            RedirectToUrlAfterSave();
        }

        return true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads last selected item from session.
    /// </summary>
    private void LoadLastSelectedItem()
    {
        string lastSelected = (string)SessionHelper.GetValue("SelectedValue");
        if (!String.IsNullOrEmpty(lastSelected))
        {
            rbCultures.SelectedValue = lastSelected;
        }
    }


    private void SaveResourceStringInfo(string resKey)
    {
        mResourceStringInfo.StringIsCustom = chkIsCustom.Checked;
        mResourceStringInfo.StringKey = resKey;

        ResourceStringInfo.Provider.Set(mResourceStringInfo);

        // We need reload mResourceStringInfo, because StringId was changed after first save
        mResourceStringInfo = ResourceStringInfo.Provider.Get(resKey);
    }


    /// <summary>
    /// Redirects to url after save.
    /// </summary>
    private void RedirectToUrlAfterSave()
    {
        string redirectUrl = String.Format(RedirectUrlAfterSave, mResourceStringInfo.StringKey);
        redirectUrl = URLHelper.AddParameterToUrl(redirectUrl, "saved", "1");

        URLHelper.Redirect(UrlResolver.ResolveUrl(redirectUrl));
    }


    private void SaveTranslations()
    {
        foreach (string cultureCode in mTranslations.Keys)
        {
            string translation = mTranslations[cultureCode.ToLowerInvariant()].Text.Trim();
            int cultureId = CultureInfo.Provider.Get(cultureCode).CultureID;
            var resTranslation = ResourceTranslationInfo.Provider.Get(mResourceStringInfo.StringID, cultureId);

            // Save translation only if not empty and if the same translation does not exist in resource file 
            if (String.IsNullOrEmpty(translation) || translation.Equals(ResHelper.GetFileString(mResourceStringInfo.StringKey, cultureCode, string.Empty, false), StringComparison.InvariantCultureIgnoreCase))
            {
                resTranslation?.Delete();
            }
            else
            {
                if (resTranslation == null)
                {
                    resTranslation = new ResourceTranslationInfo();
                }

                resTranslation.TranslationStringID = mResourceStringInfo.StringID;
                resTranslation.TranslationCultureID = cultureId;
                resTranslation.TranslationText = translation;

                ResourceTranslationInfo.Provider.Set(resTranslation);
            }
        }
    }


    /// <summary>
    /// Key is free to use if is not used, or is used with current resKey.
    /// </summary>
    /// <param name="stringId"></param>
    /// <param name="resKey"></param>
    private bool KeyIsFreeToUse(int stringId, string resKey)
    {
        ResourceStringInfo rsi = ResourceStringInfo.Provider.Get(resKey);
        if (rsi == null)
        {
            return true;
        }

        return rsi.StringID == stringId;
    }


    /// <summary>
    /// Set translation element id to each text area to enable translation services.
    /// </summary>
    private void SetTranslationElementId()
    {
        string defaultCultureElementId = mTranslations[CultureHelper.DefaultUICultureCode.ToLowerInvariant()].ValueElementID;

        foreach (FormEngineUserControl textArea in mTranslations.Values)
        {
            textArea.SetValue("TranslationElementClientID", defaultCultureElementId);
        }
    }


    /// <summary>
    /// Adds new row into result table.
    /// </summary>
    private TableRow GetRow(string cultureName, string cultureCode)
    {
        bool isDefaultCulture = cultureCode.Equals(CultureHelper.DefaultUICultureCode, StringComparison.InvariantCultureIgnoreCase);
        bool isCurrentCulture = cultureCode.Equals(selectedCultureCode, StringComparison.InvariantCultureIgnoreCase);

        if (isDefaultCulture)
        {
            cultureName = String.Format("{0} ({1})", cultureName, GetString("general.default").ToLowerInvariant());
        }
        else if (isCurrentCulture)
        {
            cultureName = String.Format("{0} ({1})", cultureName, GetString("general.current").ToLowerInvariant());
        }

        Image flag = new Image();
        flag.ImageUrl = GetFlagIconUrl(cultureCode, "16x16");
        flag.CssClass = "cms-icon-80";
        flag.Style.Add("vertical-align", "middle");
        flag.ToolTip = String.Format("{0} ({1})", cultureName, cultureCode);
        flag.AlternateText = flag.ToolTip;

        var label = new Label();

        label.Text = " " + cultureName;

        TableCell cultureCell = new TableCell();
        cultureCell.Width = new Unit("250px");
        cultureCell.Controls.Add(flag);
        cultureCell.Controls.Add(label);

        var textArea = (FormEngineUserControl)LoadControl("~/CMSFormControls/Inputs/LargeTextArea.ascx");
        textArea.ID = cultureCode;
        textArea.Enabled = EnableTranslations;

        if (!String.IsNullOrEmpty(mResourceStringInfo.StringKey))
        {
            textArea.Text = ResourceStringInfoProvider.GetStringFromDB(mResourceStringInfo.StringKey, cultureCode);
            if (string.IsNullOrEmpty(textArea.Text))
            {
                // If translation not found in database try to load it from resource file
                textArea.Text = ResHelper.GetFileString(mResourceStringInfo.StringKey, cultureCode, string.Empty, false);
            }
        }

        if (!isDefaultCulture && EnableTranslations)
        {
            textArea.SetValue("AllowTranslationServices", true);
            textArea.SetValue("TranslationSourceLanguage", CultureHelper.DefaultUICultureCode);
            textArea.SetValue("TranslationTargetLanguage", cultureCode);
        }

        TableCell resStringCell = new TableCell();
        resStringCell.Controls.Add(textArea);

        TableRow row = new TableRow();
        row.Cells.Add(cultureCell);
        row.Cells.Add(resStringCell);

        mTranslations.Add(cultureCode.ToLowerInvariant(), textArea);

        return row;
    }


    /// <summary>
    /// Returns DataSet according to filter settings.
    /// </summary>
    private DataSet GetCultures()
    {
        string filterText = SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(txtFilter.Text.Trim()));
        string sqlSafeCultureCode = SqlHelper.EscapeQuotes(selectedCultureCode);
        string columns = "CultureName, CultureCode";
        string where = null;
        string orderBy = String.Format(@"CASE WHEN CultureCode = '{0}' THEN 1 WHEN CultureCode = '{1}' THEN 2 ELSE 3 END, CultureName",
            CultureHelper.DefaultUICultureCode, sqlSafeCultureCode);

        switch (rbCultures.SelectedValue)
        {
            case "allcultures":
                where = String.Format("CultureCode = '{0}' OR CultureCode = '{1}' OR CultureName LIKE '%{2}%'",
                    CultureHelper.DefaultUICultureCode, sqlSafeCultureCode, filterText);
                break;

            case "uicultures":
                where = String.Format("CultureCode = '{0}' OR CultureCode = '{1}' OR (CultureIsUICulture = 1 AND CultureName LIKE '%{2}%')",
                    CultureHelper.DefaultUICultureCode, sqlSafeCultureCode, filterText);
                break;

            case "sitecultures":
                where = String.Format("(CultureName LIKE '%{0}%' AND CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = {1})) OR CultureCode = '{2}'",
                    filterText, SiteContext.CurrentSiteID, CultureHelper.DefaultUICultureCode);
                break;
        }

        return CultureInfoProvider.GetCultures(where, orderBy, columns: columns);
    }

    #endregion
}