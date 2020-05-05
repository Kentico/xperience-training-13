using System;

using CMS.Community;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Members_Member_Edit : CMSGroupPage
{
    #region "Private variables"

    protected int memberId = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get memberId
        memberId = QueryHelper.GetInteger("memberid", 0);

        // Initialize editing control
        memberEditElem.MemberID = memberId;

        GroupMemberInfo gmi = GroupMemberInfoProvider.GetGroupMemberInfo(memberId);
        if (gmi != null)
        {
            memberEditElem.GroupID = gmi.MemberGroupID;
            UserInfo ui = UserInfoProvider.GetUserInfo(gmi.MemberUserID);
            if (ui != null)
            {
                // Initialize breadcrumbs
                PageBreadcrumbs.Items.Add(new BreadcrumbItem()
                {
                    Text = GetString("group.members"),
                    RedirectUrl = "~/CMSModules/Groups/Tools/Members/Member_List.aspx?groupId=" + gmi.MemberGroupID,
                    Target = "_self"
                });

                PageBreadcrumbs.Items.Add(new BreadcrumbItem()
                {
                    Text = HTMLHelper.HTMLEncode(ui.FullName)
                });
            }
        }
    }
}