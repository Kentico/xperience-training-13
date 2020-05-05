using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_ColorPicker_ColorPicker_Control : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string previewId = QueryHelper.GetText("previewid", string.Empty);
        string controlId = QueryHelper.GetText("controlid", string.Empty);
        bool postback = QueryHelper.GetBoolean("postback", false);
        string color = QueryHelper.GetText("color", "#FFFF00");

        if (!RequestHelper.IsPostBack())
        {
            txtColor.Text = color;
        }
        string script = "function CP_SetColor(){SetColor(" + ScriptHelper.GetString(txtColor.ClientID) + ", " + ScriptHelper.GetString(controlId) + ", " + ScriptHelper.GetString(previewId) + ", " + postback.ToString().ToLowerCSafe() + ");}";
        ltlScript.Text = ScriptHelper.GetScript(script);

        // Register wopener script
        ScriptHelper.RegisterWOpenerScript(Page);
    }
}
