using System;

using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MyDesk_RecycleBin_RecycleBin_Documents : CMSContentManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", new[] { "MyRecycleBin", "MyRecycleBin.Documents" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "MyRecycleBin;MyRecycleBin.Documents");
        }

        recycleBin.SiteName = SiteContext.CurrentSiteName;
    }

    #endregion
}