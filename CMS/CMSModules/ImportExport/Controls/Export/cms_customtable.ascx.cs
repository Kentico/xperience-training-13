using System;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_cms_customtable : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkObject.Text = GetString("CMSExport_CustomTables.ExportCustomTable");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_CUSTOMTABLE_DATA, chkObject.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_CUSTOMTABLE_DATA), false);
    }
}