using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "Transformations")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Transformation_List : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int customTableId = QueryHelper.GetInteger("parentobjectid", 0);

        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("customtable.edit.newtransformation"),
            RedirectUrl = ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_New.aspx?parentobjectid=" + customTableId + "&iscustomtable=1")
        });

        // Set the query editor control
        classTransformations.ClassID = customTableId;
        classTransformations.EditPageUrl = GetEditUrl();
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditTransformation");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}