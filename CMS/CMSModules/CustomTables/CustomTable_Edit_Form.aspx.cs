using System;

using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "CustomTable.Form")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Form : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";

        layoutElem.FormLayoutType = FormLayoutTypeEnum.CustomTable;
        layoutElem.ObjectID = QueryHelper.GetInteger("objectid", 0);
    }
}