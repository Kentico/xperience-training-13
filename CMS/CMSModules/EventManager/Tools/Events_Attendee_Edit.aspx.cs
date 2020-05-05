using System;

using CMS.EventManager;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_EventManager_Tools_Events_Attendee_Edit : CMSEventManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int eventNodeId = QueryHelper.GetInteger("eventId", 0);
        int attendeeId = QueryHelper.GetInteger("attendeeId", 0);
        attendeeEdit.EventID = eventNodeId;
        attendeeEdit.AttendeeID = attendeeId;
        attendeeEdit.Saved = QueryHelper.GetBoolean("saved", false);

        string attEmail = GetString("Events_Attendee_Edit.NewItemCaption");
        EventAttendeeInfo eai = null;

        if (attendeeId > 0)
        {
            eai = EventAttendeeInfoProvider.GetEventAttendeeInfo(attendeeId);
        }
        
        if (eai != null)
        {
            attEmail = eai.AttendeeEmail;
        }

        // Initializes breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Events_Attendee_Edit.itemlistlink"),
            RedirectUrl = ResolveUrl("~/CMSModules/EventManager/Tools/Events_Attendee_List.aspx?eventid=" + attendeeEdit.EventID),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = attEmail,
        });

        attendeeEdit.OnCheckPermissions += attendeeEdit_OnCheckPermissions;
    }


    private void attendeeEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.EventManager", permissionType))
        {
            RedirectToAccessDenied("CMS.EventManager", permissionType);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        attendeeEdit.LoadEditData();
        base.OnPreRender(e);
    }
}