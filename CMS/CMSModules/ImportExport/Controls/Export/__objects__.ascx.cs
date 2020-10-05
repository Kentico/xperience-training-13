using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export___objects__ : ImportExportControl
{
    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings ExportSettings
    {
        get
        {
            if (Settings != null)
            {
                return (SiteExportSettings)Settings;
            }
            return null;
        }
        set
        {
            Settings = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblInfo.Text = GetString("ExportObjects.Info");

        lnkSelectAll.Text = GetString("ImportObjects.SelectAll");
        lnkSelectNone.Text = GetString("ImportObjects.SelectNone");
        lnkSelectDefault.Text = GetString("ImportObjects.SelectDefault");

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
    public override void SaveSettings()
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


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
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


    protected void lnkSelectAll_Click(object sender, EventArgs e)
    {
        ExportTypeEnum exportType = ExportSettings.ExportType;
        DateTime timeStamp = ExportSettings.TimeStamp;

        ExportSettings.ExportType = ExportTypeEnum.All;
        ExportSettings.TimeStamp = DateTimeHelper.ZERO_TIME;
        ExportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ExportSettings.ExportType = exportType;
        ExportSettings.TimeStamp = timeStamp;

        lblInfo.Text = GetString("ImportObjects.AllSelected");
    }


    protected void lnkSelectNone_Click(object sender, EventArgs e)
    {
        ExportTypeEnum exportType = ExportSettings.ExportType;
        DateTime timeStamp = ExportSettings.TimeStamp;

        ExportSettings.ExportType = ExportTypeEnum.None;
        ExportSettings.TimeStamp = DateTimeHelper.ZERO_TIME;
        ExportSettings.LoadDefaultSelection(false);

        SaveSettings();

        ExportSettings.ExportType = exportType;
        ExportSettings.TimeStamp = timeStamp;

        lblInfo.Text = GetString("ImportObjects.NoneSelected");
    }


    protected void lnkSelectDefault_Click(object sender, EventArgs e)
    {
        ExportSettings.LoadDefaultSelection(false);

        SaveSettings();

        lblInfo.Text = GetString("ImportObjects.DefaultSelected");
    }
}
