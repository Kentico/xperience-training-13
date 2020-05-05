using System;

using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;
using CMS.UIControls;


[EditedObject("media.library","objectid")]
[UIElement("CMS.MediaLibrary", "General")]
public partial class CMSModules_MediaLibrary_Tools_Library_Edit_General : CMSMediaLibraryPage
{
    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        MediaLibraryInfo mli = (MediaLibraryInfo)EditedObject;
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToAccessDenied("cms.medialibrary", "Read");
        }

        elemEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(elemEdit_OnCheckPermissions);
        elemEdit.MediaLibraryID = mli.LibraryID;
    }


    /// <summary>
    /// OnCheckPermissions event handler.
    /// </summary>
    private void elemEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo mli = (MediaLibraryInfo)EditedObject;
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToAccessDenied("cms.medialibrary", "Read");
        }
    }

    #endregion
}