using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSPages_DevicePreview : CMSAdministrationPage
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";

        // Setup the device rotation
        ucView.RotateDevice = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.CurrentDeviceProfileRotate), false);       
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        CurrentMaster.HeaderActions.IsLiveSite = false;

        base.OnPreRender(e);
    }
}
