using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;
using CMS.UIControls;


[UIElement("CMS.MediaLibrary", "Files")]
public partial class CMSModules_MediaLibrary_Tools_Library_Edit_Files : CMSMediaLibraryPage
{
    #region "Variables"

    private int libraryId = QueryHelper.GetInteger("objectid", 0);

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureScriptManager();

        CurrentMaster.PanelContent.CssClass = "";

        libraryElem.LibraryID = libraryId;
        libraryElem.OnCheckPermissions += libraryElem_OnCheckPermissions;

        ScriptHelper.HideVerticalTabs(this);

        // Ensure breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.media_library"));
    }


    private void libraryElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo MediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(libraryId);
        if (permissionType.ToLowerCSafe() == "read")
        {
            // Check 'Read' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, permissionType))
            {
                RedirectToAccessDenied("cms.medialibrary", "Read");
            }
        }
    }

    #endregion
}
