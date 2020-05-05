using System;

using CMS.Community.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_MediaLibrary_Library_Edit_Security : CMSGroupMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        librarySecurity.MediaLibraryID = QueryHelper.GetInteger("objectid", 0);
    }
}