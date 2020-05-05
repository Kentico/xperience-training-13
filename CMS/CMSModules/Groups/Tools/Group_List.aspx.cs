using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Community.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[Action(0, "Group.NewItemCaption", "~/CMSModules/Groups/Tools/Group_New.aspx")]
[UIElement(ModuleName.GROUPS, "Groups")]
public partial class CMSModules_Groups_Tools_Group_List : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        PageTitle.TitleText = GetString("group.HeaderCaption");
        // Only current site groups can be listed
        if (SiteContext.CurrentSite != null)
        {
            groupListElem.SiteID = SiteContext.CurrentSite.SiteID;
        }

        groupListElem.OnAction += new CommandEventHandler(groupListElem_OnAction);
    }


    private void groupListElem_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.Trim().ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Groups", "EditGroup", false, e.CommandArgument.ToInteger(0)));
                break;

            case "delete":
                CheckGroupPermissions(ValidationHelper.GetInteger(e.CommandArgument, 0), CMSAdminControl.PERMISSION_MANAGE);
                URLHelper.Redirect("~/CMSModules/Groups/Tools/Group_Delete.aspx?groupid=" + e.CommandArgument.ToString());
                break;
        }
    }
}