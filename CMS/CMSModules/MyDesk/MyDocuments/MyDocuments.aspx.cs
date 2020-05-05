using System;

using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CONTENT, "MyDocuments")]
public partial class CMSModules_MyDesk_MyDocuments_MyDocuments : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((MembershipContext.AuthenticatedUser == null) || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "MyDocuments")))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "MyDocuments");
        }

        // Setup page title text and image
        PageTitle.TitleText = GetString("MyDesk.MyDocumentsTitle");
        ucMyDocuments.SiteName = SiteContext.CurrentSiteName;
    }
}