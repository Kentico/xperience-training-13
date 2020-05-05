using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

[ParentObject(DataClassInfo.OBJECT_TYPE, "classid")]
public partial class CMSModules_Modules_Pages_Class_AlternativeForm_New : GlobalAdminPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("classid", 0);
        int moduleId = QueryHelper.GetInteger("moduleid", 0);

        altFormEdit.OnAfterSave += (s, ea) => URLHelper.Redirect(URLHelper.AppendQuery(
            UIContextHelper.GetElementUrl(ModuleName.CMS, "EditForm"),
            "&objectid=" + ((AlternativeFormInfo)EditedObject).FormID + "&parentobjectid=" + classId + "&classid=" + classId + "&moduleid=" + moduleId + "&displaytitle=false&saved=1")
        );
        altFormEdit.RedirectUrlAfterCreate = String.Empty;

        // Check if the 'Combine With User Settings' feature should be available
        if (classId > 0)
        {
            string className = DataClassInfoProvider.GetClassName(classId);
            if (className != null && (className.ToLowerCSafe().Trim() == UserInfo.OBJECT_TYPE.ToLowerCSafe()))
            {
                altFormEdit.ShowCombineUsersSettings = true;
            }
        }

        // Init breadcrumbs
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 0,
            Text = GetString("altforms.listlink"),
            RedirectUrl = URLHelper.AppendQuery(
                UIContextHelper.GetElementUrl(ModuleName.CMS, "AlternativeForms"),
                "parentobjectid=" + classId + "&classid=" + classId + "&moduleid=" + moduleId + "&displaytitle=false")
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 1,
            Text = GetString("altform.newbread")
        });
    }

    #endregion
}