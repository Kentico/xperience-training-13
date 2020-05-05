using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_CMSPages_LiveForumPostApprove : CMSLiveModalPage
{
    #region "Variables"

    private int postId = 0;
    private int groupId = 0;

    #endregion


    #region "Page events"

    /// <summary>
    /// Page OnInit event.
    /// </summary>
    /// <param name="e">Event args</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check forums availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Forums", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Forums");
        }

        // Check groups availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Groups", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Groups");
        }

        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Groups);
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get postId, groupId
        postId = QueryHelper.GetInteger("postid", 0);
        groupId = 0;
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(postId);
        if (fpi != null)
        {
            ForumInfo fi = ForumInfoProvider.GetForumInfo(fpi.PostForumID);
            if (fi != null)
            {
                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
                if (fgi != null)
                {
                    groupId = fgi.GroupGroupID;
                }
            }
        }

        // Check permissions
        CheckGroupPermissions(groupId, "Read");

        if (groupId > 0)
        {
            // Set the post ID
            PostApprove.PostID = postId;
            PostApproveFooter.PostID = postId;

            // Set methods which check the permissions
            PostApprove.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PostApprove_OnCheckPermissions);
            PostApproveFooter.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PostApprove_OnCheckPermissions);

            // Page title
            PageTitle.TitleText = GetString("forums_forumnewpost_header.preview");
        }
    }


    /// <summary>
    /// Check whether user is group administrator or has manage permission.
    /// </summary>
    /// <param name="groupId">Comunnity group ID</param>
    /// <param name="permissionName">Permission name</param>
    protected void PostApprove_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (permissionType.EqualsCSafe("modify", true))
        {
            permissionType = "Manage";
        }

        // Check permissions
        CheckGroupPermissions(groupId, permissionType);
    }


    /// <summary>
    /// Check whether user is group administrator or has manage permission.
    /// </summary>
    /// <param name="groupId">Comunnity group ID</param>
    /// <param name="permissionName">Permission name</param>
    private static void CheckGroupPermissions(int groupId, string permissionName)
    {
        // Get current group   
        GeneralizedInfo group = ModuleCommands.CommunityGetGroupInfo(groupId);
        if (group != null)
        {
            // Check if group is placed on current site
            int groupSiteID = ValidationHelper.GetInteger(group.GetProperty("GroupSiteID"), 0);
            if (groupSiteID != SiteContext.CurrentSiteID)
            {
                RedirectToAccessDenied(GetString("community.group.notassigned"));
            }
        }

        if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupId))
        {
            // Check permissions
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", permissionName))
            {
                RedirectToAccessDenied("CMS.Groups", permissionName);
            }
        }
    }

    #endregion
}