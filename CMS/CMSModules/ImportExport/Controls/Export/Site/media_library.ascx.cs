using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_Site_media_library : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblInfo.Text = GetString("CMSExport_MediaLibrary.Info");
        chkFiles.Text = GetString("CMSExport_MediaLibrary.ExportFiles");
        chkPhysicalFiles.Text = GetString("CMSExport.ExportPhysicalFiles");

        // Javascript
        string script = "var medChck1 = document.getElementById('" + chkFiles.ClientID + "'); \n" +
                        "var medChck2 = document.getElementById('" + chkPhysicalFiles.ClientID + "'); \n" +
                        "medInitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkFiles.Attributes.Add("onclick", "medCheckChange();");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES, chkFiles.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES_PHYSICAL, (chkPhysicalFiles.Checked && chkPhysicalFiles.Enabled));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkFiles.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES), true);
        chkPhysicalFiles.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_MEDIA_FILES_PHYSICAL), false);
    }
}
