using System;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Community.Web.UI;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Group_Delete : CMSGroupPage
{
    #region "Private variables"

    private string mGroupListUrl = "~/CMSModules/Groups/Tools/Group_List.aspx";
    private GroupInfo gi = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Only community manager can delete group
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        int groupId = QueryHelper.GetInteger("groupid", 0);
        gi = GroupInfoProvider.GetGroupInfo(groupId);

        if (gi != null)
        {
            lblMsg.Style.Add("font-weight", "bold");
            chkDeleteAll.Text = MacroResolver.Resolve(GetString("group.deleterelated"));
            mGroupListUrl = ResolveUrl(mGroupListUrl);

            // Pagetitle
            PageTitle.TitleText = GetString("group.deletegroup") + " \"" + HTMLHelper.HTMLEncode(gi.GroupDisplayName) + "\"";
            // Initialize breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("deletegroup.listlink"),
                RedirectUrl = mGroupListUrl,
            });
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = HTMLHelper.HTMLEncode(gi.GroupDisplayName),
            });

            btnDelete.Click += new EventHandler(btnDelete_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }
    }


    private void btnDelete_Click(object sender, EventArgs e)
    {
        // Only community manager can delete group
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            RedirectToAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE);
        }

        if (gi != null)
        {
            try
            {
                GroupInfoProvider.DeleteGroupInfo(gi, chkDeleteAll.Checked);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, EventLogProvider.GetExceptionLogMessage(ex), null);
                return;
            }
        }
        URLHelper.Redirect(UrlResolver.ResolveUrl(mGroupListUrl));
    }


    private void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl(mGroupListUrl));
    }

    #endregion
}