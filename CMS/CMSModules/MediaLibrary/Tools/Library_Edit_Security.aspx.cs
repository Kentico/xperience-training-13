using System;

using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;
using CMS.UIControls;


[UIElement("CMS.MediaLibrary", "Security")]
public partial class CMSModules_MediaLibrary_Tools_Library_Edit_Security : CMSMediaLibraryPage
{
    #region "Variables"

    private int libraryId = QueryHelper.GetInteger("objectid", 0);

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(libraryId);
        EditedObject = mli;

        // Check 'Read' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToAccessDenied("cms.medialibrary", "Read");
        }

        // Check 'Manage' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Manage"))
        {
            librarySecurity.Enable = false;
        }

        librarySecurity.MediaLibraryID = libraryId;
        librarySecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(librarySecurity_OnCheckPermissions);
    }


    private void librarySecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(libraryId);
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToAccessDenied("cms.medialibrary", "Read");
        }
    }

    #endregion
}