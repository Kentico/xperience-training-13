using System;

using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "CustomTables", false, true)]
public partial class CMSModules_CustomTables_Tools_CustomTable_Data_ViewItem : CMSCustomTablesModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("customtable.data.viewitemtitle");
        Page.Title = PageTitle.TitleText;
        // Get custom table id from url
        int customTableId = QueryHelper.GetInteger("customtableid", 0);
        // Get custom table item id
        int itemId = QueryHelper.GetInteger("itemid", 0);

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(customTableId);
        // Set edited object
        EditedObject = dci;

        if (dci == null)
        {
            return;
        }

        // Check 'Read' permission
        if (!dci.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied("cms.customtables", "Read");
        }

        CustomTableItem item = CustomTableItemProvider.GetItem(itemId, dci.ClassName);
        customTableViewItem.CustomTableItem = item;
    }
}