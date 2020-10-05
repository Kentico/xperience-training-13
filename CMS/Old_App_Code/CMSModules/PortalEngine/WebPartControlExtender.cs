using CMS;
using CMS.Base.Web.UI;
using CMS.UIControls;

[assembly: RegisterCustomClass("WebPartHeaderControlExtender", typeof(WebPartHeaderControlExtender))]

/// <summary>
/// Extender class
/// </summary>
public class WebPartHeaderControlExtender : UITabsExtender
{
    public override void OnInit()
    {
        base.OnInit();

        ScriptHelper.RegisterClientScriptBlock(Control, typeof(string), "InfoScript", ScriptHelper.GetScript("function IsCMSDesk() { return true; }"));
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
    }
}
