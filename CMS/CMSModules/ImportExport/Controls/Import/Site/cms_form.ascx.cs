using System;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_Site_cms_form : ImportExportControl
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
        chkObject.Visible = true;
        chkObject.Text = GetString("CMSImport_BizForms.ImportBizForm");
        chkPhysicalFiles.Text = GetString("CMSImport.ImportPhysicalFiles");
        lblInfo.Text = GetString("CMSImport_BizForms.InfoLabel");

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
        Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA, Import);
        Settings.SetSettings(ImportExportHelper.SETTINGS_BIZFORM_FILES_PHYSICAL, (chkPhysicalFiles.Checked && chkPhysicalFiles.Enabled));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BIZFORM_DATA), !ExistingSite);
        chkPhysicalFiles.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BIZFORM_FILES_PHYSICAL), !ExistingSite);
    }
}
