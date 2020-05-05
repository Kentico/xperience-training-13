using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_General : CMSGroupPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PollProperties.ItemID = QueryHelper.GetInteger("pollid", 0);
        PollProperties.SiteID = SiteContext.CurrentSiteID;
        PollProperties.GroupID = QueryHelper.GetInteger("groupid", 0);
        PollProperties.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PollProperties_OnCheckPermissions);
        PollProperties.Enabled = CheckGroupPermissions(PollProperties.GroupID, CMSAdminControl.PERMISSION_MANAGE, false);
        PollProperties.OnSaved += PollProperties_OnSaved;
    }

    void PollProperties_OnSaved(object sender, EventArgs e)
    {
        // Refresh header with display name
        ScriptHelper.RefreshTabHeader(Page, ((PollInfo)EditedObject).PollDisplayName);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!RequestHelper.IsPostBack())
        {
            PollProperties.ReloadData();
        }
    }


    private void PollProperties_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permissions
        CheckGroupPermissions(PollProperties.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}
