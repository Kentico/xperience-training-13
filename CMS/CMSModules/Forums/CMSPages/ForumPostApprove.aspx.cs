using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_CMSPages_ForumPostApprove : CMSDeskPage
{
    private string mode;
    private int postId;
    private int userId;


    #region "Page events"

    /// <summary>
    /// Initializes necessary properties.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        postId = QueryHelper.GetInteger("postid", 0);
        mode = QueryHelper.GetString("mode", "approval").ToLowerInvariant();
        userId = QueryHelper.GetInteger("userid", 0);
    }


    /// <summary>
    /// OnLoad override.
    /// Verifies permisisons and module availability
    /// </summary>
    /// <param name="e">Event agrs</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckDocPermissions = false;

        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Forums);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Forums", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Forums");
        }

        CurrentUserInfo user = MembershipContext.AuthenticatedUser;

        if ((mode != "subscription") || (userId != user.UserID))
        {

            // Check permissions for CMS Desk -> Tools -> Forums
            if (!user.IsAuthorizedPerUIElement("CMS.Forums", "Forums"))
            {
                RedirectToUIElementAccessDenied("CMS.Forums", "Forums");
            }

            // Check 'Read' permission
            if (!user.IsAuthorizedPerResource("CMS.Forums", "Read"))
            {
                RedirectToAccessDenied("CMS.Forums", "Read");
            }
        }

        SetUpControl();
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog scripts
        RegisterModalPageScripts();
        RegisterEscScript();
    }

    #endregion


    private void SetUpControl()
    {
        if (mode == "subscription")
        {
            PostApproveFooter.Mode = "subscription";
            PostApprove.Mode = "subscription";
        }

        // Set the post ID
        PostApprove.PostID = PostApproveFooter.PostID = postId;
        PostApprove.UserID = PostApproveFooter.UserID = userId;

        // Page title
        PageTitle.TitleText = GetString("forums_forumnewpost_header.preview");
    }
}