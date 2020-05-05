using System;

using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_Export_Site_community_group : ImportExportControl
{
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