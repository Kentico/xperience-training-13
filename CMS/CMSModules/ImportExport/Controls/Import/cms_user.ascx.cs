using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_cms_user : ImportExportControl
{
    /// <summary>
    /// True if import into existing site.
    /// </summary>
    protected bool ExistingSite
    {
        get
        {
            if (Settings != null)
            {
                return ((SiteImportSettings)Settings).ExistingSite;
            }
            return true;
        }
    }


    /// <summary>
    /// True if the data should be imported.
    /// </summary>
    protected bool Import
    {
        get
        {
            return chkObject.Checked;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkObject.Text = GetString("CMSImport_Users.ImportUserDashboards");
        chkSiteObjects.Text = GetString("CMSImport_Users.ImportUserSiteDashboards");
        // Javascript
        string script = "var dashboardChck1 = document.getElementById('" + chkObject.ClientID + "'); \n" +
                        "var dashboardChck2 = document.getElementById('" + chkSiteObjects.ClientID + "'); \n" +
                        "InitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkObject.Attributes.Add("onclick", "CheckChange();");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        chkSiteObjects.Visible = ((SiteImportSettings)Settings).SiteIsIncluded;
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_DASHBOARDS, chkObject.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_USER_SITE_DASHBOARDS, (chkSiteObjects.Checked && chkSiteObjects.Enabled && chkSiteObjects.Visible));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_USER_DASHBOARDS), true);
        chkSiteObjects.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_USER_SITE_DASHBOARDS), !ExistingSite && chkSiteObjects.Visible);
    }
}
