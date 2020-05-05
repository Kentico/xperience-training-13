using System;

using CMS.Membership;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Macros_Dialogs_Default : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Checks user permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertMacro"))
        {
            RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", "InsertMacro");
        }
    }
}