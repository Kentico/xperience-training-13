using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_buttons : CMSWebPartPropertiesPage
{
    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " Buttons";
    }


    /// <summary>
    /// Load event handler
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        chkRefresh.Text = GetString("WebpartProperties.Refresh");

        ltlScript.Text += ScriptHelper.GetScript("function GetRefreshStatus() { return document.getElementById('" + chkRefresh.ClientID + "').checked; }");

        // Set button texts
        btnOk.Text = GetString("general.saveandclose");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");

        // Set button click events
        btnCancel.OnClientClick = FramesManager.GetCancelScript();
        btnApply.OnClientClick = FramesManager.GetApplyScript();
        btnOk.OnClientClick = FramesManager.GetOKScript();

        string action = QueryHelper.GetString("tab", "properties");

        switch (action)
        {
            case "properties":
                break;

            case "code":
                break;

            case "binding":
                chkRefresh.Visible = false;
                btnApply.Visible = false;
                btnOk.Visible = false;
                btnCancel.Text = GetString("WebpartProperties.Close");
                break;
        }
    }
}
