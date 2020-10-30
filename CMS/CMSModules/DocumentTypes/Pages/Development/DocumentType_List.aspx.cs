using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Modules;
using CMS.UIControls;

// New document type action
[Action(0, "DocumentType_List.NewDoctype", "DocumentType_New.aspx")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterExportScript();

        // Unigrid initialization
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(GetEditUrl(actionArgument.ToString()));
        }
        else if (actionName == "delete")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);
            var dci = DataClassInfoProvider.GetDataClassInfo(classId);
            if (dci == null)
            {
                return;
            }

            // Delete dataclass and its dependencies
            try
            {
                string className = dci.ClassName;
                DataClassInfoProvider.DeleteDataClassInfo(dci);

                // Delete icons
                string iconFile = UIHelper.GetDocumentTypeIconPath(this, className, "", false);
                string iconLargeFile = UIHelper.GetDocumentTypeIconPath(this, className, "48x48", false);
                iconFile = Server.MapPath(iconFile);
                iconLargeFile = Server.MapPath(iconLargeFile);

                if (File.Exists(iconFile))
                {
                    File.Delete(iconFile);
                }
                // Ensure that ".gif" file will be deleted
                iconFile = iconFile.Replace(".png", ".gif");

                if (File.Exists(iconFile))
                {
                    File.Delete(iconFile);
                }

                if (File.Exists(iconLargeFile))
                {
                    File.Delete(iconLargeFile);
                }
                // Ensure that ".gif" file will be deleted
                iconLargeFile = iconLargeFile.Replace(".png", ".gif");
                if (File.Exists(iconLargeFile))
                {
                    File.Delete(iconLargeFile);
                }
            }
            catch (CheckDependenciesException)
            {
                var description = uniGrid.GetCheckDependenciesDescription(dci);
                ShowError(GetString("unigrid.deletedisabledwithoutenable"), description);
            }
            catch (Exception ex)
            {
                LogAndShowError("Development", "DeleteDocType", ex);
            }
        }
    }

    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="documentTypeId">Document type identifier</param>
    private String GetEditUrl(string documentTypeId)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditDocumentType");
        if (uiChild != null)
        {
            var url = UIContextHelper.GetElementUrl(uiChild, UIContext);
            url = URLHelper.AddParameterToUrl(url, "objectid", documentTypeId.ToString());
            url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");

            return url;
        }

        return String.Empty;
    }

    #endregion
}