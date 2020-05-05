using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "Queries")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Query_List : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int customTableId = QueryHelper.GetInteger("parentobjectid", 0);

        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("customtable.edit.newquery"),
            RedirectUrl = ResolveUrl("CustomTable_Edit_Query_Edit.aspx?parentobjectid=" + customTableId)
        });

        // Set the query editor control
        classEditQuery.ClassID = customTableId;
        classEditQuery.EditPageUrl = GetEditUrl();
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditQuery");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}