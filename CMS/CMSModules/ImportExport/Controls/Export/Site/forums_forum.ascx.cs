using System;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_Site_forums_forum : ImportExportControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        chkObject.Text = GetString("CMSExport_Forum.ExportForum");
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_FORUM_POSTS, chkObject.Checked);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_FORUM_POSTS), false);
    }
}