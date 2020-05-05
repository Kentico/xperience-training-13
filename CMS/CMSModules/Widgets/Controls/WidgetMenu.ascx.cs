using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_Widgets_Controls_WidgetMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Properties
        lblProperties.Text = ResHelper.GetString("WebPartMenu.IconProperties", culture);
        pnlProperties.Attributes.Add("onclick", "ContextConfigureWidget();");

        // Up menu - Bottom
        iTop.Text = ResHelper.GetString("UpMenu.IconTop", culture);
        iTop.Attributes.Add("onclick", "ContextMoveWidgetTop();");

        // Up
        iUp.Text = ResHelper.GetString("WebPartMenu.IconUp", culture);
        iUp.Attributes.Add("onclick", "ContextMoveWidgetUp();");

        // Down
        iDown.Text = ResHelper.GetString("WebPartMenu.IconDown", culture);
        iDown.Attributes.Add("onclick", "ContextMoveWidgetDown();");

        // Down menu - Bottom
        iBottom.Text = ResHelper.GetString("DownMenu.IconBottom", culture);
        iBottom.Attributes.Add("onclick", "ContextMoveWidgetBottom();");

        // Move to
        iMoveTo.Text = ResHelper.GetString("WebPartMenu.IconMoveTo", culture);

        iCopy.Text = ResHelper.GetString("WidgetMenu.copy", culture);
        iCopy.Attributes.Add("onclick", "ContextCopyWidget(this);");

        iPaste.Text = ResHelper.GetString("WidgetMenu.paste", culture);
        iPaste.Attributes.Add("onclick", "ContextPasteWidget(this);");
        iPaste.ToolTip = ResHelper.GetString("WidgetMenu.pasteTooltip", culture);

        // Delete
        lblDelete.Text = ResHelper.GetString("general.remove", culture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveWidget();");

        // Top
        lblTop.Text = ResHelper.GetString("UpMenu.IconTop", culture);
        pnlTop.Attributes.Add("onclick", "ContextMoveWidgetTop();");

        // Bottom
        lblBottom.Text = ResHelper.GetString("DownMenu.IconBottom", culture);
        pnlBottom.Attributes.Add("onclick", "ContextMoveWidgetBottom();");
    }
}