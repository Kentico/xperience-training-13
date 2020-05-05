using System;

using CMS.Community.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_New : CMSGroupPollsPage
{
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get GroupID
        groupId = QueryHelper.GetInteger("groupid", 0);

        CheckGroupPermissions(groupId, "Read");

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("group.polls.title"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Polls/Polls_List.aspx?groupId=" + groupId),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("polls_new.newitemcaption"),
        });

        // PollNew control initialization
        PollNew.OnSaved += PollNew_OnSaved;
        PollNew.OnCheckPermissions += PollNew_OnCheckPermissions;
        PollNew.GroupID = groupId;
        PollNew.SiteID = SiteContext.CurrentSiteID;
    }


    private void PollNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Saved event handler. 
    /// </summary>
    private void PollNew_OnSaved(object sender, EventArgs e)
    {
        string error = null;
        // Show possible license limitation error
        if (!String.IsNullOrEmpty(PollNew.LicenseError))
        {
            error = "&error=" + PollNew.LicenseError;
        }

        string editActionUrl = URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(
            URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.Polls", "Groups.EditGroup.EditPoll"), "objectid", PollNew.ItemID.ToString()), "groupid", groupId.ToString()), "displaytitle", "false"), "saved", "1");
        URLHelper.Redirect(editActionUrl + error);
    }
}