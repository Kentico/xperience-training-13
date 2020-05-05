using System;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Forums_Tools_Forums_Forum_Security : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int forumID = QueryHelper.GetInteger("forumid", 0);
        ForumContext.CheckSite(0, forumID, 0);

        forumSecurity.ForumID = forumID;
        forumSecurity.IsGroupForum = false;
        forumSecurity.IsLiveSite = false;

        forumSecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumSecurity_OnCheckPermissions);

        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes master page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forums - Forum security";
    }


    /// <summary>
    /// OnCheckPermissions handler.
    /// </summary>
    /// <param name="permissionType">Type of the permission</param>
    /// <param name="sender">The sender</param>
    private void forumSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.forums", permissionType))
        {
            forumSecurity.StopProcessing = true;

            // Redirect only if permission READ is check
            if (permissionType == CMSAdminControl.PERMISSION_READ)
            {
                RedirectToAccessDenied("CMS.Forums", "Read");
            }
        }
    }
}