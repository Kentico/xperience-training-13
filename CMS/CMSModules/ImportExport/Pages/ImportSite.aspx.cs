using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;
using CMS.DataEngine;

[Title("ImportSite.Title")]
[UIElement(ModuleName.CMS, "ImportSiteOrObjects")]
public partial class CMSModules_ImportExport_Pages_ImportSite : CMSImportExportPage
{
    private const string CI_SETTINGS_KEY = "CMSEnableCI";


    protected void Page_Load(object sender, EventArgs e)
    {
        SetBreadcrumb(0, GetString("general.sites"), UIContextHelper.GetElementUrl(ModuleName.CMS, "sites", false), null, null);
        SetBreadcrumb(1, GetString("ImportSite.ImportSite"), string.Empty, null, null);

        var isCIEnabled = SettingsKeyInfoProvider.GetBoolValue(CI_SETTINGS_KEY);
        if (isCIEnabled)
        {
            ShowWarning(GetString("importsite.cienabled.warning"));
        }
    }
}
