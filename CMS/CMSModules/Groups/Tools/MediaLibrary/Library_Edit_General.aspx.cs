using System;

using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject("media.grouplibrary", "objectid")]
public partial class CMSModules_Groups_Tools_MediaLibrary_Library_Edit_General : CMSGroupMediaLibraryPage
{
    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("parentobjectid", 0);

        if (groupId <= 0)
        {
            elemEdit.Enable = false;
        }
        else
        {
            elemEdit.MediaLibraryID = QueryHelper.GetInteger("objectid", 0);
            elemEdit.MediaLibraryGroupID = groupId;
        }
    }
}