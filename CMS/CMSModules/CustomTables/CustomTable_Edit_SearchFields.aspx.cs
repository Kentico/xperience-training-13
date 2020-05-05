using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "SearchFields")]
public partial class CMSModules_CustomTables_CustomTable_Edit_SearchFields : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("objectid", 0);

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);
        // Set edited object
        EditedObject = dci;

        // Class exists
        if ((dci == null) || (!dci.ClassIsCoupledClass))
        {
            ShowWarning(GetString("customtable.ErrorNoFieldsSearch"), null, null);
            SearchFields.Visible = false;
            SearchFields.StopProcessing = true;
        }
        else
        {
            SearchFields.ItemID = classId;
        }
    }
}