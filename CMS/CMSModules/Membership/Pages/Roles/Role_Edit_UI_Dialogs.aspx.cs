using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Roles_Role_Edit_UI_Dialogs : CMSRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "read" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", "Read"))
        {
            RedirectToAccessDenied("CMS.UIPersonalization", "Read");
        }

        int siteID = 0;

        if (SelectedSiteID != 0)
        {
            siteID = SelectedSiteID;
        }
        else if (SiteID != 0)
        {
            siteID = SiteID;
        }

        editElem.SiteID = siteID;
        editElem.RoleID = QueryHelper.GetInteger("roleid", 0);
        editElem.IsLiveSite = false;
        editElem.HideSiteSelector = true;
    }
}