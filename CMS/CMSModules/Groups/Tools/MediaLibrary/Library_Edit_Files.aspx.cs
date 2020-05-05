using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_MediaLibrary_Library_Edit_Files : CMSGroupMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureScriptManager();

        libraryElem.LibraryID = QueryHelper.GetInteger("objectid", 0);

        ScriptHelper.HideVerticalTabs(this);

        // Ensure breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("ObjectType.media_grouplibrary"));
    }
}
