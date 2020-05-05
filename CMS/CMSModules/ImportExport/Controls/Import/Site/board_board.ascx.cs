using System;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_Site_board_board : ImportExportControl
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
            if (ExistingSite)
            {
                return false;
            }
            return chkObject.Checked;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = !ExistingSite;

        if (!ExistingSite)
        {
            chkObject.Visible = true;
            chkObject.Text = GetString("CMSImport_Board.ImportBoardMessages");
        }
    }


    /// <summary>
    /// Gets settings.
    /// </summary>
    public override void SaveSettings()
    {
        Settings.SetSettings(ImportExportHelper.SETTINGS_BOARD_MESSAGES, Import);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        if (!ExistingSite)
        {
            chkObject.Checked = ValidationHelper.GetBoolean(Settings.GetSettings(ImportExportHelper.SETTINGS_BOARD_MESSAGES), true);
        }
    }
}