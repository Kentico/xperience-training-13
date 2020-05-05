using System;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[Breadcrumbs()]
[Breadcrumb(0, TargetUrl = "~/CMSModules/DocumentTypes/Pages/AlternativeForms/AlternativeForms_List.aspx?parentobjectid={?parentobjectid?}", ResourceString = "altforms.listlink")]
[Breadcrumb(1, ResourceString = "altform.newbread")]
[Help("alternative_forms_general", "helpTopic")]
public partial class CMSModules_DocumentTypes_Pages_AlternativeForms_AlternativeForms_New : GlobalAdminPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("parentobjectid", 0);
        altFormEdit.OnBeforeSave += (o, ea) => ((AlternativeFormInfo)altFormEdit.EditedObject).FormClassID = classId;

        // Set redirect url - created form ID is needed => need late bound redirect.
        altFormEdit.RedirectUrlAfterCreate = String.Empty;
        altFormEdit.OnAfterSave += (o, ea) => URLHelper.Redirect(GetEditUrl(classId,((AlternativeFormInfo)EditedObject).FormID));
    }

    
    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="classId">Class info identifier</param>
    /// <param name="formId">Alternative form identifier</param>
    private String GetEditUrl(int classId, int formId)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditAlternativeForm");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false&objectid=" + formId + "&parentobjectid=" + classId);
        }

        return String.Empty;
    }

    #endregion
}