using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "AlternativeForms")]
public partial class CMSModules_CustomTables_AlternativeForms_AlternativeForms_List : CMSCustomTablesPage
{
    private int classId;


    protected void Page_Load(object sender, EventArgs e)
    {
        classId = QueryHelper.GetInteger("parentobjectid", 0);

        // Init alternative forms listing
        listElem.FormClassID = classId;
        listElem.OnEdit += listElem_OnEdit;
        listElem.OnDelete += listElem_OnDelete;


        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("altforms.newformlink"),
            RedirectUrl = ResolveUrl("AlternativeForms_New.aspx?parentobjectid=" + classId)
        });
    }


    protected void listElem_OnEdit(object sender, object actionArgument)
    {
        URLHelper.Redirect(GetEditUrl(classId,ValidationHelper.GetInteger(actionArgument, 0)));
    }


    protected void listElem_OnDelete(object sender, object actionArgument)
    {
        AlternativeFormInfoProvider.DeleteAlternativeFormInfo(ValidationHelper.GetInteger(actionArgument, 0));
    }


    #region "Private methods"

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
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false&objectid=" + formId + "&parentobjectid=" + classId);
        }

        return String.Empty;
    }

    #endregion
}