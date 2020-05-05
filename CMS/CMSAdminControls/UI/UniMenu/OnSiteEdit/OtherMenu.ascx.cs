using System;
using System.Web.UI.WebControls;

using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSAdminControls_UI_UniMenu_OnSiteEdit_OtherMenu : CMSUserControl
{
    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var cui = CurrentUser;

        if (cui != null)
        {
            if (cui.IsAuthorizedPerUIElement("CMS.OnSiteEdit", "OnSiteHighlight"))
            {
                // Highlight button
                MenuItem highlightItem = new MenuItem();
                highlightItem.CssClass = "BigButton";
                highlightItem.ImageAlign = ImageAlign.Top;
                highlightItem.IconClass = "icon-square-dashed-line";
                highlightItem.OnClientClick = "OEHighlightToggle(event, this);";
                highlightItem.Text = PortalHelper.LocalizeStringForUI("onsiteedit.highlight");
                highlightItem.Tooltip = PortalHelper.LocalizeStringForUI("onsiteedit.highlighttooltip");
                highlightItem.ImageAltText = PortalHelper.LocalizeStringForUI("onsiteedit.highlight");

                otherMenu.Buttons.Add(highlightItem);
            }

            if (cui.IsAuthorizedPerUIElement("CMS.OnSiteEdit", "OnSiteHidden"))
            {
                // Hidden button
                MenuItem hiddenItem = new MenuItem();
                hiddenItem.CssClass = "BigButton OnSiteHiddenButton";
                hiddenItem.ImageAlign = ImageAlign.Top;
                hiddenItem.IconClass = "icon-eye-slash";
                hiddenItem.Text = PortalHelper.LocalizeStringForUI("general.hidden");
                hiddenItem.Tooltip = PortalHelper.LocalizeStringForUI("onsiteedit.hiddentooltip");
                hiddenItem.ImageAltText = PortalHelper.LocalizeStringForUI("general.hidden");

                // Add temporary empty sub menu item to ensure generating of the sub menu functions
                SubMenuItem epmtyItem = new SubMenuItem();
                hiddenItem.SubItems.Add(epmtyItem);

                otherMenu.Buttons.Add(hiddenItem);
            }
        }
    }

    #endregion
}
