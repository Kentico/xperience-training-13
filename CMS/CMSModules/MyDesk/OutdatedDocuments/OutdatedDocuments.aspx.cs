using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CONTENT, "OutdatedDocs")]
public partial class CMSModules_MyDesk_OutdatedDocuments_OutdatedDocuments : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((MembershipContext.AuthenticatedUser == null) || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "OutdatedDocs")))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "OutdatedDocs");
        }

        // Setup page title text and image
        PageTitle.TitleText = GetString("MyDesk.OutdatedDocumentsTitle");
        ucOutdatedDocuments.SiteName = SiteContext.CurrentSiteName;
    }
}