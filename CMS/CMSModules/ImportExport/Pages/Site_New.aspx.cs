using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;
using CMS.DataEngine;

[Title("Site_Edit.NewSite")]
[UIElement(ModuleName.CMS, "NewSite")]
public partial class CMSModules_ImportExport_Pages_Site_New : CMSImportExportPage
{
    private const string CI_SETTINGS_KEY = "CMSEnableCI";

    protected void Page_Load(object sender, EventArgs e)
    {
        SetBreadcrumb(0, GetString("general.sites"), UIContextHelper.GetElementUrl(ModuleName.CMS, "sites", false), null, null);
        SetBreadcrumb(1, GetString("Site_Edit.NewSite"), string.Empty, null, null);

        var isCIEnabled = SettingsKeyInfoProvider.GetBoolValue(CI_SETTINGS_KEY);
        if (isCIEnabled)
        {
            ShowWarning(GetString("newsite.cienabled.warning"));
        }
    }
}