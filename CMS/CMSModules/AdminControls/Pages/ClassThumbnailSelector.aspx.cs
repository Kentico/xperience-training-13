using System;

using CMS.Base.Web.UI;

using System.Linq;
using System.Text;

using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// Modal dialog invoked in the ClassImageSelectorOpener control.
/// Displays selector of images predefined for the given object type.
/// Sends unique identifier of the selected image to the window opener so that it can be processed further.
/// </summary>
public partial class CMSModules_AdminControls_Pages_ClassThumbnailSelector : CMSModalPage
{
    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterModule(this, "AdminControls/ClassThumbnailSelector", new
        {
            EventId = QueryHelper.GetString("eventid", ""),
            OkButtonId = btnOk.ClientID,
            ItemsCSSSelector = selectElem.ItemsCSSSelector,
        });

        PageTitle.TitleText = GetString("dialogs.header.title.selectimage");
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }

    #endregion
}
