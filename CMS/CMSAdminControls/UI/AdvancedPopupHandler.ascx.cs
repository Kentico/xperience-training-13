using System;

using CMS.Base.Web.UI;
using CMS.DeviceProfiles;
using CMS.UIControls;


public partial class CMSAdminControls_UI_AdvancedPopupHandler : CMSUserControl
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Use classic dialogs for devices with touch screen
        bool useClassicDialogs = UIHelper.ClassicDialogs || DeviceContext.CurrentDevice.IsMobile();

        // When this control is not visible, classic dialogs will be used
        Visible &= !useClassicDialogs;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterJQueryUI(Page);

        ScriptHelper.RegisterModule(Page, "CMS/AdvancedPopupHandler");
        ScriptHelper.RegisterScriptFile(Page, "DragAndDrop/dragiframe.js");
    }

    #endregion
}
