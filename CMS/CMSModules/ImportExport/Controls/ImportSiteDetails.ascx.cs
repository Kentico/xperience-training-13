using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SiteProvider.Internal;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ImportSiteDetails : CMSUserControl
{
    public CMSModules_ImportExport_Controls_ImportSiteDetails()
    {
        Settings = null;
    }


    #region "Properties"

    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings Settings
    {
        get;
        set;
    }


    /// <summary>
    /// Default site display name.
    /// </summary>
    public string SiteDisplayName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteDisplayName"], null);
        }
        set
        {
            ViewState["SiteDisplayName"] = value;
        }
    }


    /// <summary>
    /// Site presentation URL.
    /// </summary>
    public string SitePresentationURL
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SitePresentationURL"], null);
        }
        set
        {
            ViewState["SitePresentationURL"] = value;
        }
    }


    /// <summary>
    /// Default site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], null);
        }
        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Default domain name.
    /// </summary>
    public string DomainName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["DomainName"], null);
        }
        set
        {
            ViewState["DomainName"] = value;
        }
    }


    /// <summary>
    /// Site default culture.
    /// </summary>
    public string Culture
    {
        get
        {
            return DataHelper.GetNotEmpty(cultureElem.Value, DataHelper.GetNotEmpty(SettingsKeyInfoProvider.GetValue("CMSDefaultCultureCode"), CultureHelper.DefaultUICultureCode));
        }
        set
        {
            cultureElem.Value = value;
        }
    }


    /// <summary>
    /// Display culture selector.
    /// </summary>
    public bool DisplayCulture
    {
        get
        {
            return plcCulture.Visible;
        }
        set
        {
            plcCulture.Visible = value;
        }
    }


    /// <summary>
    /// If true, existing site selestion is allowed.
    /// </summary>
    public bool AllowExisting
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AllowExisting"], true);
        }
        set
        {
            ViewState["AllowExisting"] = value;
        }
    }


    /// <summary>
    /// If true, single object is imported.
    /// </summary>
    public bool SingleObject
    {
        get
        {
            if (Settings == null)
            {
                return false;
            }
            return ValidationHelper.GetBoolean(Settings.GetInfo(ImportExportHelper.INFO_SINGLE_OBJECT), false);
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            if (!RequestHelper.IsCallback())
            {
                radNewSite.CheckedChanged += rad_CheckedChanged;
                radExisting.CheckedChanged += rad_CheckedChanged;

                rfvSiteDisplayName.ErrorMessage = GetString("ImportSite.StepSiteDetails.SiteDisplayNameError");
                rfvSitePresentationUrl.ErrorMessage = GetString("ImportSite.StepSiteDetails.SitePresentationUrlError");
                rfvDomain.ErrorMessage = GetString("ImportSite.StepSiteDetails.ErrorDomain");
                rfvSiteDisplayName.AddCssClass("label-full-width");
                rfvSitePresentationUrl.AddCssClass("label-full-width");
                rfvDomain.AddCssClass("label-full-width");

                lblSiteDisplayName.Text = GetString("ImportSite.StepSiteDetails.SiteDisplayName");
                lblSiteName.Text = GetString("ImportSite.StepSiteDetails.SiteName");
                lblSitePresentationUrl.Text = GetString("ImportSite.StepSiteDetails.SitePresentationUrl");
                lblDomain.Text = GetString("ImportSite.StepSiteDetails.DomainName");
                lblCulture.Text = GetString("ImportSite.StepSiteDetails.CultureName");
                lblSite.Text = GetString("ImportSite.StepSiteDetails.Site");

                radExisting.Text = GetString("ImportSite.StepSiteDetails.ExistingSite");
                radNewSite.Text = GetString("ImportSite.StepSiteDetails.NewSite");

                if (AllowExisting)
                {
                    siteSelector.AllowAll = false;
                    siteSelector.AllowEmpty = false;
                    if (siteSelector.DropDownSingleSelect.Items.Count == 0)
                    {
                        if (SiteInfoProvider.GetSitesCount() > 0)
                        {
                            plcNewSelection.Visible = true;
                            plcExisting.Visible = true;

                            siteSelector.Value = SiteContext.CurrentSiteID;

                            // Force to load now
                            siteSelector.Reload(true);
                        }
                        else
                        {
                            HideSelection();
                        }
                    }

                    // Single site object is imported
                    if (!SingleObject || (Settings.TemporaryFilesCreated && !Settings.SiteIsIncluded))
                    {
                        ltlScript.Text += ScriptHelper.GetScript(
                            "function SelectionChanged() { \n" +
                            "   var newSite = document.getElementById('" + radNewSite.ClientID + "').checked; \n" +
                            "   document.getElementById('" + txtDomain.ClientID + "').disabled = !newSite; \n" +
                            "   document.getElementById('" + txtSitePresentationUrl.ClientID + "').disabled = !newSite; \n" +
                            "   document.getElementById('" + txtSiteDisplayName.ValueElementID + "').disabled = !newSite; \n" +
                            "   document.getElementById('" + txtSiteName.ValueElementID + "').disabled = !newSite; \n" +
                            "   document.getElementById('" + siteSelector.ValueElementID + "').disabled = newSite; \n" +
                            "} \n"
                            );

                        radNewSite.Attributes.Add("onclick", "SelectionChanged()");
                        radExisting.Attributes.Add("onclick", "SelectionChanged()");
                    }
                }
                else
                {
                    HideSelection();
                }
            }
        }
    }


    private void rad_CheckedChanged(object sender, EventArgs e)
    {
        rfvDomain.Enabled = radNewSite.Checked;
        rfvSiteDisplayName.Enabled = radNewSite.Checked;

        ReloadData(false);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public void ReloadData()
    {
        ReloadData(true);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    /// <param name="refreshSelection">Refresh selection</param>
    public void ReloadData(bool refreshSelection)
    {
        DataTable table = ImportProvider.GetObjectsData(Settings, SiteInfo.OBJECT_TYPE, true);

        bool singleSiteObject = Settings.SiteIsIncluded && (SingleObject || !SiteIsSupported(table));

        // Refresh selection
        if (refreshSelection)
        {
            radExisting.Checked = singleSiteObject;
            radNewSite.Checked = !singleSiteObject;
            plcExistingSelection.Visible = !singleSiteObject;
            plcNewSelection.Visible = !singleSiteObject && AllowExisting && (siteSelector.DropDownSingleSelect.Items.Count != 0);
            plcNewSite.Visible = !singleSiteObject;
            siteSelector.Enabled = singleSiteObject;
        }

        txtDomain.Enabled = radNewSite.Checked;
        txtSitePresentationUrl.Enabled = radNewSite.Checked;
        txtSiteDisplayName.Enabled = radNewSite.Checked;
        txtSiteName.Enabled = radNewSite.Checked;
        cultureElem.Enabled = radNewSite.Checked;
        siteSelector.Enabled = !radNewSite.Checked;

        if (!DataHelper.DataSourceIsEmpty(table))
        {
            // Get datarow
            DataRow dr = table.Rows[0];

            txtSiteDisplayName.Text = SiteDisplayName ?? dr["SiteDisplayName"].ToString();

            txtSitePresentationUrl.Text = SitePresentationURL ?? dr["SitePresentationURL"].ToString();

            txtDomain.Text = DomainName ?? dr["SiteDomainName"].ToString();

            txtSiteName.Text = SiteName ?? dr["SiteName"].ToString();
        }
    }


    /// <summary>
    /// Apply control settings.
    /// </summary>
    public bool ApplySettings()
    {
        // Set additional settings
        Settings.ExistingSite = !radNewSite.Checked;
        Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_DASHBOARDS, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_SITE_DASHBOARDS, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_CUSTOMTABLE_DATA, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_WORKFLOW_SCOPES, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES, radNewSite.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES_PHYSICAL, radNewSite.Checked);

        // Existing site settings
        if (Settings.ExistingSite)
        {
            Settings.SiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);

            var si = SiteInfo.Provider.Get(Settings.SiteId);
            if (si != null)
            {
                Settings.SiteDisplayName = si.DisplayName;
                Settings.SiteDomain = si.DomainName;
                Settings.SiteName = si.SiteName;
                Settings.SiteDescription = si.Description;
                Settings.SitePresentationUrl = si.SitePresentationURL;
            }

            return true;
        }

        // New site settings
        Settings.SiteId = 0;

        if (ValidateNewSite(out var siteName))
        {
            Settings.SiteDisplayName = txtSiteDisplayName.Text;
            Settings.SitePresentationUrl = txtSitePresentationUrl.Text;
            Settings.SiteDomain = txtDomain.Text = URLHelper.RemoveProtocol(txtDomain.Text);
            Settings.SiteName = siteName;

            return true;
        }

        return false;
    }


    private bool ValidateNewSite(out string siteName)
    {
        var isValid = rfvSiteDisplayName.IsValid && rfvDomain.IsValid && rfvSitePresentationUrl.IsValid;
        isValid &= ValidateSiteName(out siteName);

        if (!Uri.IsWellFormedUriString(txtSitePresentationUrl.Text, UriKind.Absolute))
        {
            isValid = rfvSitePresentationUrl.IsValid = false;
        }

        var presentationUrl = PresentationUrlNormalizer.Normalize(txtSitePresentationUrl.Text);
        if (String.Equals(presentationUrl.Url, GetDomainInPresentationUrlFormat(txtDomain.Text).Url, StringComparison.InvariantCultureIgnoreCase))
        {
            rfvDomain.ErrorMessage = GetString("importsite.stepsitedetailscollisiondomain");
            isValid = rfvDomain.IsValid = false;
        }

        return isValid;
    }


    private bool ValidateSiteName(out string siteName)
    {
        string codeNameError = String.Empty;

        siteName = txtSiteName.Text.Trim();

        if (siteName == InfoHelper.CODENAME_AUTOMATIC)
        {
            siteName = txtSiteName.Text = ValidationHelper.GetCodeName(txtSiteDisplayName.Text);
        }
        else
        {
            var validator = new Validator()
                .NotEmpty(siteName, GetString("ImportSite.StepSiteDetails.SiteNameError"))
                .IsCodeName(siteName, GetString("ImportSite.StepSiteDetails.SiteNameNotValid"));

            codeNameError = validator.Result;
        }

        // Check if there is site with specified code name
        if (String.IsNullOrEmpty(codeNameError) && SiteInfo.Provider.Get(siteName) != null)
        {
            codeNameError = GetString("SiteImport.StepSiteDetails.ErrorSiteExists");
        }

        if (!String.IsNullOrEmpty(codeNameError))
        {
            lblErrorSiteName.Text = codeNameError;
            lblErrorSiteName.Visible = true;

            return false;
        }

        return true;
    }


    private NormalizedPresentationUrl GetDomainInPresentationUrlFormat(string domain)
    {
        // Since the presentation URL is schema agnostic the HTTP schema can be used directly to create a fake presentation URL.
        return PresentationUrlNormalizer.Normalize($"http://{domain}");
    }


    private void HideSelection()
    {
        plcNewSelection.Visible = false;
        plcExisting.Visible = false;
    }


    private bool SiteIsSupported(DataTable table)
    {
        if ((table != null) && Settings.IsLowerVersion("13.0"))
        {
            return DataHelper.GetBoolValue(table.Rows[0], "SiteIsContentOnly", false);
        }

        return true;
    }
}
