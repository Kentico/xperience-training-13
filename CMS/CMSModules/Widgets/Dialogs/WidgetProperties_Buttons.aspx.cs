using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Widgets_Dialogs_WidgetProperties_Buttons : CMSWidgetPropertiesPage
{
    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " Buttons";

        // set button text
        btnOk.Text = GetString("general.saveandclose");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");

        chkRefresh.Text = GetString("Widget.Properties.Refresh");

        if (inline)
        {
            btnApply.Visible = false;
            chkRefresh.Visible = false;
        }

        ltlScript.Text += ScriptHelper.GetScript(
@"function GetRefreshStatus() {
    var refresh = document.getElementById('" + chkRefresh.ClientID + @"');
    if (refresh != null) {
        return refresh.checked;
    }
    return false;         
}");

        btnCancel.OnClientClick = FramesManager.GetCancelScript();
        btnApply.OnClientClick = FramesManager.GetApplyScript();
        btnOk.OnClientClick = FramesManager.GetOKScript();
    }

    #endregion
}
