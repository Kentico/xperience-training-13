using System;

using CMS.Community.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Roles_Role_New : CMSGroupRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set group id
        int groupId = QueryHelper.GetInteger("groupid", 0);

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("general.roles"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Roles/Role_List.aspx?groupid=" + groupId),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Administration-Role_New.NewRole"),
        });

        // Edit/Create only roles from current site
        if (SiteContext.CurrentSite != null)
        {
            roleEditElem.SiteID = SiteContext.CurrentSite.SiteID;
        }
        roleEditElem.GroupID = groupId;

        roleEditElem.OnSaved += roleEditElem_OnSaved;
        roleEditElem.OnCheckPermissions += roleEditElem_OnCheckPermissions;
    }


    private void roleEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckGroupPermissions(roleEditElem.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void roleEditElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(String.Format("{0}&roleId={1}&groupid={2}&objectid={1}&displaytitle=false",UIContextHelper.GetElementUrl("CMS.Roles", "EditRole.Groups"), roleEditElem.RoleID, roleEditElem.GroupID));
    }
}