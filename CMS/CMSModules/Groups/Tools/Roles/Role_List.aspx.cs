using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Roles_Role_List : CMSGroupRolesPage
{
    private int groupId;


    protected void Page_Load(object sender, EventArgs e)
    {
        groupId = QueryHelper.GetInteger("groupid", 0);

        // New item link
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        { 
            Text = GetString("Administration-Role_List.NewRole"),
            RedirectUrl = ResolveUrl("Role_New.aspx?groupid=" + groupId),
        });

        // Show only roles from current site
        if (SiteContext.CurrentSite != null)
        {
            roleListElem.SiteID = SiteContext.CurrentSite.SiteID;
        }
        roleListElem.GroupID = groupId;
        roleListElem.IsGroupList = true;
        roleListElem.OnAction += roleListElem_OnAction;
        roleListElem.OnCheckPermissions += roleListElem_OnCheckPermissions;
    }


    private void roleListElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (permissionType == CMSAdminControl.PERMISSION_READ)
        {
            // Check permissions
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_READ))
            {
                RedirectToAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_READ);
            }
        }
        else
        {
            // Check permissions
            CheckGroupPermissions(roleListElem.GroupID, CMSAdminControl.PERMISSION_MANAGE);
        }
    }


    protected void roleListElem_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_READ))
                {
                    RedirectToAccessDenied("CMS.Groups", CMSAdminControl.PERMISSION_READ);
                }
                string editUrl = UIContextHelper.GetElementUrl("CMS.Roles", "EditRole.Groups");
                editUrl = URLHelper.AddParameterToUrl(editUrl, "groupid", groupId.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "roleid", e.CommandArgument.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "objectid", e.CommandArgument.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "displaytitle", "false");
                URLHelper.Redirect(editUrl);
                break;
        }
    }
}