using System;

using CMS.Community.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.FORUMS, "NewGroupForumGroup")]
public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_New : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get info on the current community group ID
        int groupId = QueryHelper.GetInteger("parentobjectid", 0);
        if (groupId > 0)
        {
            forumGroup.CommunityGroupID = groupId;
        }

        InitializeMasterPage();

        forumGroup.OnSaved += new EventHandler(forumGroup_OnSaved);
        forumGroup.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(forumGroup_OnCheckPermissions);
        forumGroup.IsLiveSite = false;
    }


    private void forumGroup_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check if user is allowed to manage module
        CheckGroupPermissions(forumGroup.CommunityGroupID, CMSAdminControl.PERMISSION_MANAGE);
    }


    protected void forumGroup_OnSaved(object sender, EventArgs e)
    {
        string redirectAddress = UIContextHelper.GetElementUrl(ModuleName.FORUMS, "EditGroupForumGroup", false);
        redirectAddress = URLHelper.AddParameterToUrl(redirectAddress, "objectid", Convert.ToString(forumGroup.GroupID));
        redirectAddress = URLHelper.AddParameterToUrl(redirectAddress, "parentobjectid", Convert.ToString(forumGroup.CommunityGroupID));
        redirectAddress = URLHelper.AddParameterToUrl(redirectAddress, "saved", "1");
        URLHelper.Redirect(redirectAddress);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Set title and help
        Title = GetString("forumgrouplist.newforumgroup");

        int groupId = QueryHelper.GetInteger("groupid", 0);

        // Initialize breadcrumbs
        string bcRedirectUrl = UIContextHelper.GetElementUrl(ModuleName.FORUMS, "Groups.EditGroup.Forums", false);
        if (groupId > 0)
        {
            bcRedirectUrl = URLHelper.AddParameterToUrl(bcRedirectUrl, "groupid", groupId.ToString());
            bcRedirectUrl = URLHelper.AddParameterToUrl(bcRedirectUrl, "parentobjectid", groupId.ToString());
        }

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Group_General.GroupList"),
            RedirectUrl = bcRedirectUrl,
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Group_General.NewGroup"),
        });
    }
}