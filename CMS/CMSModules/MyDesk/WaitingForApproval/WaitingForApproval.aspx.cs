using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CONTENT, "Pending")]
public partial class CMSModules_MyDesk_WaitingForApproval_WaitingForApproval : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Pending" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Pending");
        }

        ucWaitingForApproval.SiteName = SiteContext.CurrentSiteName;
    }
}