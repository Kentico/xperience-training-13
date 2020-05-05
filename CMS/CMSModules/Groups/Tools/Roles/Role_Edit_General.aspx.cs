using System;

using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Roles_Role_Edit_General : CMSGroupRolesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the editing control
        roleEditElem.ItemID = QueryHelper.GetInteger("roleid", 0);

        // Edit/Create only roles from current site
        if (SiteContext.CurrentSite != null)
        {
            roleEditElem.SiteID = SiteContext.CurrentSite.SiteID;
        }

        roleEditElem.OnCheckPermissions += roleEditElem_OnCheckPermissions;

        roleEditElem.GroupID = QueryHelper.GetInteger("groupid", 0);
    }


    private void roleEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckGroupPermissions(roleEditElem.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}