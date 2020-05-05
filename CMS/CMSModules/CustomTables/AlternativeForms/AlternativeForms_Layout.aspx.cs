using System;

using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "AlternativeForm.Layout")]
public partial class CMSModules_CustomTables_AlternativeForms_AlternativeForms_Layout : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        layoutElem.FormLayoutType = FormLayoutTypeEnum.CustomTable;
        layoutElem.ObjectID = QueryHelper.GetInteger("objectid", 0);
        layoutElem.IsAlternative = true;
    }
}