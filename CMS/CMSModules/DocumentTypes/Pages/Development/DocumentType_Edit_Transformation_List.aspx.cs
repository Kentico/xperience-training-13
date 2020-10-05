using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[UIElement("CMS.DocumentEngine", "Transformations")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int documentTypeId = QueryHelper.GetInteger("parentobjectid", 0);

        // New item links
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("DocumentType_Edit_Transformation_List.btnNew"),
            RedirectUrl = ResolveUrl("DocumentType_Edit_Transformation_New.aspx?parentobjectid=" + documentTypeId + "&hash=" + QueryHelper.GetHash("?objectid=" + documentTypeId))
        });

        // Set the query editor control
        classTransformations.ClassID = documentTypeId;
        classTransformations.EditPageUrl = GetEditUrl();
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditTransformation");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}