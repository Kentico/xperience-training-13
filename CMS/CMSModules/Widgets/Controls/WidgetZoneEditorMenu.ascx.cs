using System;
using System.Linq;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_Widgets_Controls_WidgetZoneEditorMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Main menu
        lblNewWebPart.Text = ResHelper.GetString("ZoneMenu.IconNewWidget", culture);
        pnlNewWebPart.Attributes.Add("onclick", "ContextNewWidgetEditor();");


        // Delete all widgets
        lblDelete.Text = ResHelper.GetString("ZoneMenu.RemoveAllWidgets", culture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveAllWidgetsEditor();");

        // Copy all widgets
        lblCopy.Text = ResHelper.GetString("WidgetZoneMenu.CopyAll", culture);
        pnlCopyAllItem.Attributes.Add("onclick", "ContextCopyAllWidgetsEditor();");

        // Paste widget(s)
        lblPaste.Text = ResHelper.GetString("WidgetZoneEditorMenu.paste", culture);
        pnlPaste.Attributes.Add("onclick", "ContextPasteWidgetZoneEditor();");
        pnlPaste.ToolTip = ResHelper.GetString("WidgetZoneEditorMenu.pasteTooltip", culture);
    }

}
