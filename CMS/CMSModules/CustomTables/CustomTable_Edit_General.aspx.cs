using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.UIControls;


[EditedObject("cms.class", "objectid")]
[UIElement(ModuleName.CUSTOMTABLES, "CustomTable.General")]
public partial class CMSModules_CustomTables_CustomTable_Edit_General : CMSCustomTablesPage
{   
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.OnAfterSave += editElem_OnAfterSave;
    }


    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        // Refresh parent frame header due to possible display name change (Custom tables has no ItemChanged support)
        ScriptHelper.RefreshTabHeader(Page, ((BaseInfo)EditedObject).Generalized.ObjectDisplayName);
    }

    #endregion
}
