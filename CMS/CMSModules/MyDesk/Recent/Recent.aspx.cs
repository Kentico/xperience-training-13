using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CONTENT, "RecentDocs")]
public partial class CMSModules_MyDesk_Recent_Recent : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((MembershipContext.AuthenticatedUser == null) || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "RecentDocs")))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "RecentDocs");
        }

        // Setup page title text and image
        PageTitle.TitleText = GetString("MyDesk.RecentTitle");
        ucRecent.SiteName = SiteContext.CurrentSite.SiteName;
    }
}