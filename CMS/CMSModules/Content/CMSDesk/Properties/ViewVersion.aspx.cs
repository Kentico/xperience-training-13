using System;

using CMS.Base.Web.UI;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_ViewVersion : CMSPropertiesPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("Content.ViewVersion");
        PageTitle.IsDialog = false;

        // Register tooltip script
        ScriptHelper.RegisterTooltip(Page);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Redirect to information page when no UI elements displayed
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Versions"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Versions");
        }
    }

    #endregion
}
