using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[Title("ExportSettings.ExportSiteSetings")]
[UIElement(ModuleName.CMS, "Export")]
public partial class CMSModules_ImportExport_Pages_ExportObjects : CMSImportExportPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteID", 0);
        if (siteID > 0)
        {
            wzdExport.SiteId = siteID;
        }

        SetBreadcrumb(0, GetString("general.sites"), UIContextHelper.GetElementUrl(ModuleName.CMS, "sites", false), null, null);
        SetBreadcrumb(1, GetString("ExportSettings.ExportSiteSetings"), string.Empty, null, null);
    }
}
