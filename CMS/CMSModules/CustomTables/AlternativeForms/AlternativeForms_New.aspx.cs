using System;

using CMS.Core;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[Breadcrumbs()]
[Breadcrumb(0, TargetUrl = "~/CMSModules/Customtables/AlternativeForms/AlternativeForms_List.aspx?parentobjectid={?parentobjectid?}", ResourceString = "altforms.listlink")]
[Breadcrumb(1, ResourceString = "altform.newbread")]
// Set help
[Help("alternative_forms_general", "helpTopic")]
[UIElement(ModuleName.CUSTOMTABLES, "AlternativeForm.General")]
public partial class CMSModules_CustomTables_AlternativeForms_AlternativeForms_New : CMSCustomTablesPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("parentobjectid", 0);
        altFormEdit.EditedObject.FormClassID = classId;

        // Set redirect url - created form ID is needed => need late bound redirect.
        altFormEdit.RedirectUrlAfterCreate = String.Empty;
        altFormEdit.OnAfterSave += (o, ea) => URLHelper.Redirect(GetEditUrl(classId, ((AlternativeFormInfo)EditedObject).FormID));
    }

    #endregion


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="classId">Class info identifier</param>
    /// <param name="formId">Alternative form identifier</param>
    private String GetEditUrl(int classId, int formId)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditAlternativeForm");
        if (uiChild != null)
        {
            var url = UIContextHelper.GetElementUrl(uiChild, UIContext);
            url = URLHelper.AddParameterToUrl(url, "objectid", formId.ToString());
            url = URLHelper.AddParameterToUrl(url, "parentobjectid", classId.ToString());
            url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");

            return url;
        }

        return String.Empty;
    }
}