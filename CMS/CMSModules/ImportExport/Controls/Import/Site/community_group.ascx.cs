using System;

using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Import_Site_community_group : ImportExportControl
{
    /// <summary>
    /// Initialize child controls
    /// </summary>
    /// <param name="sender">Ignored</param>
    /// <param name="e">Ignored</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        mlSettings.Settings = Settings;
    }


    /// <summary>
    /// Saves current settings.
    /// </summary>
    public override void SaveSettings()
    {
        mlSettings.SaveSettings();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        mlSettings.ReloadData();
    }
}