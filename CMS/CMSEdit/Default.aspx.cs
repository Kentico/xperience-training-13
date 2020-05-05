using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSEdit_default : AbstractCMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the web site root by default
        string returnUrl = URLHelper.ResolveUrl("~/");

        // Check whether on-site editing is enabled
        if (PortalHelper.IsOnSiteEditingEnabled(SiteContext.CurrentSiteName))
        {
            var cui = MembershipContext.AuthenticatedUser;
            // Check the permissions
            if (cui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor,SiteContext.CurrentSiteName)  && cui.IsAuthorizedPerResource("cms.content", "ExploreTree") && cui.IsAuthorizedPerResource("cms.content", "Read"))
            {
                // Set edit-live view mode
                PortalContext.SetViewMode(ViewModeEnum.EditLive);
            }
            else
            {
                // Redirect to access denied page when the current user does not have permissions for the OnSite editing
                CMSPage.RedirectToUINotAvailable();
            }

            // Try get return URL
            string queryUrl = QueryHelper.GetString("editurl", String.Empty);
            if (!String.IsNullOrEmpty(queryUrl) && (queryUrl.StartsWithCSafe("~/") || queryUrl.StartsWithCSafe("/")))
            {
                returnUrl = URLHelper.ResolveUrl(queryUrl);
            }

            // Remove view mode value from query string
            returnUrl = URLHelper.RemoveParameterFromUrl(returnUrl, "viewmode");
        }

        // Redirect to the requested page
        URLHelper.Redirect(returnUrl);
    }
}
