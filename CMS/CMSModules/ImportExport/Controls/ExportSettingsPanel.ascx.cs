using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ExportSettingsPanel : CMSUserControl
{
    private SiteExportSettings mSettings;


    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings ExportSettings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkCopyFiles.Text = GetString("ExportObjects.CopyObjectFiles");
        chkCopyGlobalFiles.Text = GetString("ExportObjects.CopyFiles");
        chkCopyAssemblies.Text = GetString("ExportObjects.CopyAssemblies");

        chkCopySiteFiles.Text = GetString("ExportObjects.CopySiteFiles");
        chkExportTasks.Text = GetString("ExportObjects.ExportTasks");

        // Javascript
        string script = "var ex_g_parent = document.getElementById('" + chkCopyFiles.ClientID + "'); \n" +
                        "var ex_g_childIDs = ['" + chkCopyGlobalFiles.ClientID + "', '" + chkCopySiteFiles.ClientID + "','" + chkCopyAssemblies.ClientID + "']; \n" +
                        "InitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkCopyFiles.Attributes.Add("onclick", "CheckChange();");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public void SaveSettings()
    {
        ExportSettings.CopyFiles = chkCopyFiles.Checked;

        // Copy files property is stronger
        bool copyGlobal = chkCopyFiles.Checked && chkCopyGlobalFiles.Checked;
        bool copyAssemblies = chkCopyFiles.Checked && chkCopyAssemblies.Checked;
        bool copySite = chkCopyFiles.Checked && chkCopySiteFiles.Checked;

        ExportSettings.SetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS, copyGlobal);
        ExportSettings.SetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES, copyAssemblies);
        ExportSettings.SetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS, copySite);
        ExportSettings.SetSettings(ImportExportHelper.SETTINGS_TASKS, chkExportTasks.Checked);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }
    
    
    /// <summary>
    /// Reload data.
    /// </summary>
    public void ReloadData()
    {
        // Hide copy files option
        if (ExportSettings.SiteId == 0)
        {
            plcSiteFiles.Visible = false;
        }

        chkCopyFiles.Checked = ExportSettings.CopyFiles;
        chkCopyGlobalFiles.Checked = ValidationHelper.GetBoolean(ExportSettings.GetSettings(ImportExportHelper.SETTINGS_GLOBAL_FOLDERS), true);
        chkCopyAssemblies.Checked = ValidationHelper.GetBoolean(ExportSettings.GetSettings(ImportExportHelper.SETTINGS_ASSEMBLIES), false);
        chkCopySiteFiles.Checked = ValidationHelper.GetBoolean(ExportSettings.GetSettings(ImportExportHelper.SETTINGS_SITE_FOLDERS), true);
        chkExportTasks.Checked = ValidationHelper.GetBoolean(ExportSettings.GetSettings(ImportExportHelper.SETTINGS_TASKS), true);
    }
}
