using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;
using CMS.UIControls;


[UIElement("CMS.MediaLibrary", "Files")]
public partial class CMSModules_MediaLibrary_Tools_Library_Edit_Files : CMSMediaLibraryPage, ICallbackEventHandler
{
    private const string UI_LAYOUT_KEY = nameof(CMSModules_MediaLibrary_Controls_MediaLibrary_MediaLibrary);


    private readonly int libraryId = QueryHelper.GetInteger("objectid", 0);


    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureScriptManager();

        CurrentMaster.PanelContent.CssClass = "";

        libraryElem.LibraryID = libraryId;
        libraryElem.OnCheckPermissions += libraryElem_OnCheckPermissions;

        ScriptHelper.HideVerticalTabs(this);

        // Ensure breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.media_library"));

        libraryElem.UILayoutKey = UI_LAYOUT_KEY;
    }


    private void libraryElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo MediaLibrary = MediaLibraryInfo.Provider.Get(libraryId);
        if (permissionType.ToLowerCSafe() == "read")
        {
            // Check 'Read' permission
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, permissionType))
            {
                RedirectToAccessDenied("cms.medialibrary", "Read");
            }
        }
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        var parsed = eventArgument.Split(new[] { UILayoutHelper.DELIMITER });
        if (parsed.Length == 2 && String.Equals(UILayoutHelper.WIDTH_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(parsed[1], out var width))
            {
                UILayoutHelper.SetLayoutWidth(UI_LAYOUT_KEY, width);
            }
        }
        else if (parsed.Length == 2 && String.Equals(UILayoutHelper.COLLAPSED_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (bool.TryParse(parsed[1], out var value))
            {
                UILayoutHelper.SetVerticalResizerCollapsed(UI_LAYOUT_KEY, value);
            }
        }
    }


    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }
}
