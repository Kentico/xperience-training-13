using System;

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

        lnkSelectAll.Text = GetString("ExportObjects.SelectAll");
        lnkSelectNone.Text = GetString("ExportObjects.SelectNone");
        lnkSelectDefault.Text = GetString("ExportObjects.SelectDefault");
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
