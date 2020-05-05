using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_Site_cms_form : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkObject.Text = GetString("CMSExport_BizForms.ExportBizForm");
        chkPhysicalFiles.Text = GetString("CMSExport.ExportPhysicalFiles");

        // Javascript
        string script = "var bizChck1 = document.getElementById('" + chkObject.ClientID + "'); \n" +
                        "var bizChck2 = document.getElementById('" + chkPhysicalFiles.ClientID + "'); \n" +
                        "bizInitCheckboxes(); \n";

        ltlScript.Text = ScriptHelper.GetScript(script);

        chkObject.Attributes.Add("onclick", "bizCheckChange();");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA, chkObject.Checked);
        Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_FILES_PHYSICAL, (chkPhysicalFiles.Checked && chkPhysicalFiles.Enabled));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA), false);
        chkPhysicalFiles.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BIZFORM_FILES_PHYSICAL), true);
    }
}
