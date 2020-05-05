using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Members_Member_Invite : CMSGroupPage
{
    #region "Private variables"

    protected int groupId = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get group ID
        groupId = QueryHelper.GetInteger("groupid", 0);

        // Initialize editing control
        groupInviteElem.GroupID = groupId;
        groupInviteElem.DisplayUserSelector = true;
        groupInviteElem.DisplayGroupSelector = false;
        groupInviteElem.AllowInviteNewUser = true;
        groupInviteElem.DisplayAdvancedOptions = true;

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("group.members"),
            RedirectUrl = "~/CMSModules/Groups/Tools/Members/Member_List.aspx?groupId=" + groupId,
            Target = "_self"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("group.member.invitemember")
        });

        // Remove breadcrumbs suffix because it makes no sense here
        UIHelper.SetBreadcrumbsSuffix("");

        groupInviteElem.IsLiveSite = false;
    }
}
