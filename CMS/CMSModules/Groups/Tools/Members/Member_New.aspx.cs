using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Members_Member_New : CMSGroupPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        memberEditElem.GroupID = QueryHelper.GetInteger("groupid", 0);
        memberEditElem.OnSaved += memberEditElem_OnSaved;
        memberEditElem.IsLiveSite = false;

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("group.members"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Members/Member_List.aspx?groupId=" + memberEditElem.GroupID),
            Target = "_self",
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("group.member.newmember"),
        });
    }


    protected void memberEditElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("Member_Edit.aspx?memberId=" + memberEditElem.MemberID));
    }
}