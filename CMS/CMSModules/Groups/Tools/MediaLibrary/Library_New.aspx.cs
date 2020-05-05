using System;

using CMS.Community.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[Breadcrumb(0, "media.new.librarylistlink", "~/CMSModules/Groups/Tools/MediaLibrary/Library_List.aspx?parentobjectid={?parentobjectid?}", "")]
[Breadcrumb(1, "media.new.newlibrary")]
[Title(HelpTopic = "library_new")]
public partial class CMSModules_Groups_Tools_MediaLibrary_Library_New : CMSGroupMediaLibraryPage
{
    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        int mGroupId = QueryHelper.GetInteger("parentobjectid", 0);
        CheckGroupPermissions(mGroupId, CMSAdminControl.PERMISSION_READ);

        if (mGroupId > 0)
        {
            elemEdit.MediaLibraryGroupID = mGroupId;
            elemEdit.EditingForm.RedirectUrlAfterCreate = GetEditUrl();
        }
        else
        {
            elemEdit.Enable = false;
        }
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.MediaLibrary", "Group.EditMediaLibrary");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false&objectid={%EditedObject.ID%}&parentobjectid=" + elemEdit.MediaLibraryGroupID);
        }

        return String.Empty;
    }

    #endregion
}