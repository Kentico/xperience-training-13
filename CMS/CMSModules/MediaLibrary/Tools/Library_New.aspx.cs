using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary.Web.UI;
using CMS.Modules;
using CMS.UIControls;


[Title("media.new.newlibrarytitle")]

[Breadcrumb(0, "media.new.librarylistlink", "~/CMSModules/MediaLibrary/Tools/Library_List.aspx", "")]
[Breadcrumb(1, "media.new.newlibrary")]
[UIElement("CMS.MediaLibrary", "MediaLibrary.AddMediaLibrary")]
public partial class CMSModules_MediaLibrary_Tools_Library_New : CMSMediaLibraryPage
{
    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        elemEdit.EditingForm.RedirectUrlAfterCreate = GetEditUrl();
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.MediaLibrary", "EditMediaLibrary");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, false), "objectid={%EditedObject.ID%}");
        }

        return String.Empty;
    }
}