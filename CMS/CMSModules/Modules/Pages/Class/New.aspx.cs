using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title(HelpTopic = "new_class")]
[UIElement(ModuleName.CMS, "NewClass")]
public partial class CMSModules_Modules_Pages_Class_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the inner control properties
        newDocWizard.Theme = Theme;
        int moduleID = newDocWizard.ModuleID = QueryHelper.GetInteger("moduleid", 0);

        // Init breadcrumbs
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 0,
            Text = GetString("sysdev.class_header.classes"),
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Module.Classes", false), "parentobjectid=" + moduleID + "&moduleid=" + moduleID)
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 1,
            Text = GetString("sysdev.class_list.newclass")
        });
    }
}