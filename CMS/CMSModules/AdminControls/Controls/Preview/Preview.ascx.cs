using System;
using System.Data;

using CMS.Helpers;

using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Preview_Preview : CMSPreviewControl
{
    #region "Properties"

    /// <summary>
    /// If true, controls values are loaded from cache (even when postback)
    /// </summary>
    public bool SetControls
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        imgRefresh.ToolTip = GetString("general.refresh");

        TrimPreviewValues();

        if (RequestHelper.IsPostBack())
        {
            ProcessPostback();
        }

        if (PreviewObjectName != String.Empty)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
function ChangeLanguage(culture) {
    ", ControlsHelper.GetPostBackEventReference(btnLanguage, "#").Replace("'#'", "culture"), @"
}
");

            ScriptHelper.RegisterStartupScript(this, typeof(String), "PostbackScript", sb.ToString(), true);
        }
    }


    /// <summary>
    /// Get values from session and set it to controls
    /// </summary>
    private void TrimPreviewValues()
    {
        if (String.IsNullOrEmpty(PreviewObjectName))
        {
            return;
        }

        string[] parameters = null;

        // For dialog mode first time load or when preview is initialized - set actual settings, not stored 
        if (LoadSessionValues)
        {
            // Get values from session
            parameters = SessionHelper.GetValue(PreviewObjectName) as string[];
        }

        if ((parameters != null) && (parameters.Length == 4))
        {
            // Store SiteID for path selector
            ucPath.SiteID = String.IsNullOrEmpty(parameters[1]) ? SiteContext.CurrentSiteID : ValidationHelper.GetInteger(parameters[1], 0);
            if (!RequestHelper.IsPostBack() || SetControls)
            {
                ucPath.Value = parameters[0];
            }

            ucSelectCulture.SelectedCulture = String.IsNullOrEmpty(parameters[2]) ? LocalizationContext.PreferredCultureCode : parameters[2];
        }
        else
        {
            if ((parameters == null) || (parameters.Length != 4))
            {
                // First time load
                parameters = new String[4];
                parameters[1] = SiteContext.CurrentSiteID.ToString();
                parameters[3] = "";

                if (!RequestHelper.IsPostBack() || SetControls)
                {
                    parameters[2] = LocalizationContext.PreferredCultureCode;
                    parameters[0] = DefaultPreviewPath;
                }
            }

            // First try get alias path from property
            String aliasPath = DefaultAliasPath;
            if (String.IsNullOrEmpty(aliasPath))
            {
                // Then get path settings from query string (used in CMS Desk)
                aliasPath = QueryHelper.GetString("aliaspath", String.Empty);
            }

            if (!String.IsNullOrEmpty(aliasPath))
            {
                parameters[0] = aliasPath;
            }

            // Set selectors by parameters value
            ucPath.Value = parameters[0];
            ucPath.SiteID = ValidationHelper.GetInteger(parameters[1], SiteContext.CurrentSiteID);
            ucSelectCulture.SelectedCulture = parameters[2];
        }

        // Store new values for dialog mode
        if (!LoadSessionValues)
        {
            SessionHelper.SetValue(PreviewObjectName, parameters);
        }

        ucPath.Config.ContentSites = AvailableSitesEnum.All;
        ucSelectCulture.SiteID = ucPath.SiteID;
        ucPath.PathTextBox.WatermarkText = GetString("general.pleaseselectdots");
        ucPath.PathTextBox.WatermarkCssClass = "WatermarkText";
    }


    /// <summary>
    /// Registers script for refresh preview
    /// </summary>
    public void RegisterRefreshPreviewScript()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(String), "RefreshScript", GetRefreshPreviewScript(), true);
    }


    /// <summary>
    /// Set new values to session for device preview
    /// </summary>
    /// <param name="updateDocumentSettings">Indicates whether the preview language or preview device has changed</param>
    private void UpdatePreview(bool updateDocumentSettings)
    {
        if (updateDocumentSettings)
        {
            string[] parameters = SessionHelper.GetValue(PreviewObjectName) as string[] ?? new String[4];

            // Store new values based on settings in controls
            int siteID = ucPath.SiteID;
            parameters[0] = ValidationHelper.GetString(ucPath.Value, String.Empty);
            parameters[1] = (siteID != 0) ? siteID.ToString() : parameters[1];
            parameters[2] = ValidationHelper.GetString(ucSelectCulture.SelectedCulture, LocalizationContext.PreferredCultureCode);

            SessionHelper.SetValue(PreviewObjectName, parameters);
        }

        RegisterRefreshPreviewScript();
    }


    /// <summary>
    /// Handles the PathChanged event of the ucPath control.
    /// </summary>
    protected void ucPath_PathChanged(object sender, EventArgs ea)
    {
        string[] parameters = SessionHelper.GetValue(PreviewObjectName) as string[];

        // Get old site ID
        int SiteID = SiteContext.CurrentSiteID;
        if ((parameters != null) && (parameters.Length == 4))
        {
            SiteID = ValidationHelper.GetInteger(parameters[1], 0);
        }

        // If site ID changed - register postback for reload update panel with culture selector
        if (SiteID != ucPath.SiteID)
        {
            String script = Page.ClientScript.GetPostBackEventReference(imgRefresh, "reloadculture");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "UpdateImageScript", script, true);

            // If cultures from other site is shown
            DataSet siteCulturesDS = CultureSiteInfoProvider.GetSiteCultures(SiteInfoProvider.GetSiteName(ucPath.SiteID));
            if (!DataHelper.DataSourceIsEmpty(siteCulturesDS))
            {
                DataTable siteCultures = siteCulturesDS.Tables[0];
                // SelectedCulture may not be in site culture list
                DataRow[] dr = siteCulturesDS.Tables[0].Select("CultureCode= '" + ucSelectCulture.SelectedCulture + "'");
                if (dr.Length == 0)
                {
                    // In such case, select first site's culture 
                    ucSelectCulture.SelectedCulture = ValidationHelper.GetString(siteCultures.Rows[0]["CultureCode"], LocalizationContext.PreferredCultureCode);
                }
            }

        }

        UpdatePreview(true);
    }


    /// <summary>
    /// Handles the clicked event of the imgRefresh control.
    /// </summary>
    protected void imgRefresh_clicked(object sender, EventArgs ea)
    {
        // If path was changed reload culture selector in case site was changed also
        string arg = Request[Page.postEventArgumentID];
        if (arg != "reloadculture")
        {
            UpdatePreview(true);
        }
    }


    /// <summary>
    /// Handles the OnSelectionChanged event of the CurrentSelector control.
    /// </summary>
    protected void CurrentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        UpdatePreview(true);
    }


    /// <summary>
    /// Updates the previewed content according to the currently selected language and device.
    /// </summary>
    private void ProcessPostback()
    {
        string arg = Request[Page.postEventArgumentID];
        string target = Request[Page.postEventSourceID];
        
        // Language change
        if (target == btnLanguage.UniqueID)
        {
            ucSelectCulture.SelectedCulture = arg;
            UpdatePreview(true);
        }
    }


    /// <summary>
    /// Gets the script which refreshes the previewed content according to the currently selected properties.
    /// </summary>
    private string GetRefreshPreviewScript()
    {
        return "refreshPreviewParam('" + GetPreviewURL() + "');";
    }

    #endregion
}
