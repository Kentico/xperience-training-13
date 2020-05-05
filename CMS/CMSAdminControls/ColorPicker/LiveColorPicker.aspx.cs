using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[Title("dialog.colorpicker.title")]
public partial class CMSAdminControls_ColorPicker_LiveColorPicker : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Registers a colorpicker script file
        ScriptHelper.RegisterScriptFile(this, "~/CMSAdminControls/ColorPicker/colorpicker.js");
    }
}
