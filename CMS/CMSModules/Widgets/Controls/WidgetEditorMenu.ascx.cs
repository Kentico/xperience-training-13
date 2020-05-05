using System;
using System.Linq;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_Widgets_Controls_WidgetEditorMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Properties
        lblProperties.Text = ResHelper.GetString("WebPartMenu.IconProperties", culture);
        pnlProperties.Attributes.Add("onclick", "ContextConfigureWidgetEditor();");

        iCopy.Text = ResHelper.GetString("WidgetMenu.copy", culture);
        iCopy.Attributes.Add("onclick", "ContextCopyWidgetEditor(this);");

        iPaste.Text = ResHelper.GetString("WidgetEditorMenu.paste", culture);
        iPaste.Attributes.Add("onclick", "ContextPasteWidgetEditor(this);");
        iPaste.ToolTip = ResHelper.GetString("WidgetEditorMenu.pasteTooltip", culture);

        // Delete
        lblDelete.Text = ResHelper.GetString("general.remove", culture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveWidgetEditor();");

        pnlWidgetMenu.Attributes.Add("onmouseover", "ActivateParentBorder();");
        pnlWidgetMenu.Attributes.Add("onmouseout", "DeactivateParentBorder();");
    }
}
