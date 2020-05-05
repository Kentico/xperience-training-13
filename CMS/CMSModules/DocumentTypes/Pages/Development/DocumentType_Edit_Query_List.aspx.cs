using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_List : GlobalAdminPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int documentTypeId = QueryHelper.GetInteger("parentobjectid", 0);

        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("DocumentType_Edit_Query_List.btnNew"),
            RedirectUrl = ResolveUrl(string.Format("DocumentType_Edit_Query_Edit.aspx?parentobjectid={0}", documentTypeId))
        });

        // Set the query editor control
        classEditQuery.ClassID = documentTypeId;
        classEditQuery.EditPageUrl = GetEditUrl();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditQuery");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}