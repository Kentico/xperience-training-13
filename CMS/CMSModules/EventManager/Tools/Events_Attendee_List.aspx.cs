using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[Action(0, "Events_Attendee_List.NewItemCaption", "Events_Attendee_Edit.aspx?eventid={?eventId?}")]
public partial class CMSModules_EventManager_Tools_Events_Attendee_List : CMSEventManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int eventNodeId = QueryHelper.GetInteger("eventId", 0);
        attendeesList.EventID = eventNodeId;
        attendeesList.OnCheckPermissions += attendeesList_OnCheckPermissions;
    }


    /// <summary>
    /// 'Check permission' event handler.
    /// </summary>
    private void attendeesList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check READ permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.EventManager", permissionType))
        {
            RedirectToAccessDenied("CMS.EventManager", permissionType);
        }
    }
}