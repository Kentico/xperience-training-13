using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_UI_PageElements_AnchorDropup : AnchorDropup
{
    #region "Page events"

    /// <summary>
    /// OnLoad override, setup access denied page with dependence on current usage.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Page.PreRenderComplete += Page_PreRenderComplete;
    }
    

    private void Page_PreRenderComplete(object sender, EventArgs e)
    {
        // Hide control if not enough anchors
        var anchorCount = UIContext.AnchorLinks.Count;
        if ((anchorCount < MinimalAnchors) || (anchorCount == 0) || !Visible)
        {
            pnlWrapper.Visible = false;
            return;
        }

        CssRegistration.RegisterCssLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");
        ScriptHelper.RegisterModule(Page, "AdminControls/AnchorDropup", ScrollOffset);

        repNavigationItems.DataSource = UIContext.AnchorLinks;
        repNavigationItems.DataBind();
    }

    #endregion
}
