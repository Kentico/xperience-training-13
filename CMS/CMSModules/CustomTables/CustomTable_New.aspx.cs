using System;

using CMS.Core;
using CMS.UIControls;

// Master page title
[Title("customtable.list.NewCustomTable")]

// Breadcrumbs
[Breadcrumb(0, "customtable.list.Title", "~/CMSModules/CustomTables/CustomTable_List.aspx", null)]
[Breadcrumb(1, "customtable.list.NewCustomTable")]
[UIElement(ModuleName.CUSTOMTABLES, "CustomTable.General")]
public partial class CMSModules_CustomTables_CustomTable_New : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the inner control properties
        newTableWizard.Theme = Theme;

        // Set new custom table wizard
        newTableWizard.Mode = NewClassWizardModeEnum.CustomTable;
        newTableWizard.SystemDevelopmentMode = false;
        newTableWizard.Step6Description = "customtable_new.description";
    }
}