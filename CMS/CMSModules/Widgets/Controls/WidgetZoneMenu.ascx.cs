using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_Widgets_Controls_WidgetZoneMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Main menu
        lblNewWebPart.Text = ResHelper.GetString("ZoneMenu.IconNewWidget", culture);
        pnlNewWebPart.Attributes.Add("onclick", "ContextNewWidget();");

        // Properties
        lblConfigureZone.Text = ResHelper.GetString("ZoneMenu.IconConfigureWebpartZone", culture);
        pnlConfigureZone.Attributes.Add("onclick", "ContextConfigureWidgetZone();");

        // Delete all widgets
        lblDelete.Text = ResHelper.GetString("ZoneMenu.RemoveAllWidgets", culture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveAllWidgets();");

        // Copy all widgets
        lblCopy.Text = ResHelper.GetString("WidgetZoneMenu.CopyAll", culture);
        pnlCopyAllItem.Attributes.Add("onclick", "ContextCopyAllWidgets();");

        // Paste widget(s)
        lblPaste.Text = ResHelper.GetString("WidgetZoneMenu.paste", culture);
        pnlPaste.Attributes.Add("onclick", "ContextPasteWidgetZone();");
        pnlPaste.ToolTip = ResHelper.GetString("WidgetZoneMenu.pasteTooltip", culture);
    }
}