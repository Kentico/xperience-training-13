using System;

using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "Sites")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Sites : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get classID from querystring
        int classId = QueryHelper.GetInteger("objectid", 0);
        if (classId > 0)
        {
            ClassSites.TitleString = GetString("customtable.edit.selectsite");
            ClassSites.ClassId = classId;
        }
    }
}