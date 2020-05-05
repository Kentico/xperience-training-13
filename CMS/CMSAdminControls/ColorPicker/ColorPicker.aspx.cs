using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


[Title("dialog.colorpicker.title")]
public partial class CMSAdminControls_ColorPicker_ColorPicker : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetSaveJavascript("CP_SetColor(); return false;");
        // Registers a colorpicker script file
        ScriptHelper.RegisterScriptFile(this, "~/CMSAdminControls/ColorPicker/colorpicker.js");
    }
}
