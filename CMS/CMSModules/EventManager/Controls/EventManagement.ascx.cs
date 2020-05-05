using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.EventManager;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_EventManager_Controls_EventManagement : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the order by condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return eventList.OrderBy;
        }
        set
        {
            eventList.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return eventList.ItemsPerPage;
        }
        set
        {
            eventList.ItemsPerPage = value;
        }
    }


    /// <summary>
    /// Stop processing.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            eventList.StopProcessing = value;
            emailSender.StopProcessing = value;
            attendeesList.StopProcessing = value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Header actions control
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return plcHeaderActions;
        }
    }

    #endregion


    #region "Methods" 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(hdnEventID.Value) && (eventList.SelectedEventID == 0))
        {
            eventList.SelectedEventID = ValidationHelper.GetInteger(hdnEventID.Value, 0);
            attendeesList.EventID = eventList.SelectedEventID;
            emailSender.EventID = eventList.SelectedEventID;
        }

        lnkBackHidden.Click += lnkBackHidden_Click;
        eventList.UsePostBack = true;

        // Tabs creation
        tabControlElem.AddTab(new UITabItem
        {
            Text = GetString("Events_Attendee_List.General"),
            OnClientClick = "",
        });
        tabControlElem.AddTab(new UITabItem
        {
            Text = GetString("Events_Edit.SendEmail"),
            OnClientClick = "",
        });
            
        tabControlElem.UsePostback = true;
        attendeesList.OnCheckPermissions += attendeesList_OnCheckPermissions;
        emailSender.OnCheckPermissions += attendeesList_OnCheckPermissions;
    }


    /// <summary>
    /// Breadcrumbs back clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        eventList.SelectedEventID = 0;
        hdnEventID.Value = String.Empty;
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permission</param>
    /// <param name="sender">Sender</param>
    private void attendeesList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (eventList.SelectedEventID != 0)
        {
            eventList.Visible = false;
            eventList.StopProcessing = true;
            pnlAttendees.Visible = true;
            hdnEventID.Value = eventList.SelectedEventID.ToString();
            attendeesList.EventID = eventList.SelectedEventID;
            emailSender.EventID = eventList.SelectedEventID;
            SetBreadcrumbs();
        }
        else
        {
            eventList.Visible = true;
            pnlAttendees.Visible = false;
            attendeesList.StopProcessing = true;
            emailSender.StopProcessing = true;
            eventList.ReloadData();
            attendeesList.Reset();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Sets breadcrumbs.
    /// </summary>
    private void SetBreadcrumbs()
    {
        string eventCapacity = "0";
        string eventTitle = "";
        string registeredAttendees = null;
        string eventBreadcrumbsText;

        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("Events_Edit.itemlistlink"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        DataSet ds = EventProvider.GetEvent(eventList.SelectedEventID, null, "EventCapacity, EventName, AttendeesCount");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            eventCapacity = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["EventCapacity"], 0).ToString();
            eventTitle = ValidationHelper.GetString(ds.Tables[0].Rows[0]["EventName"], "");
            registeredAttendees = ValidationHelper.GetString(ds.Tables[0].Rows[0]["AttendeesCount"], "");
        }

        if (ValidationHelper.GetInteger(eventCapacity, 0) > 0)
        {
            eventBreadcrumbsText = String.Format(GetString("Events_Edit.RegisteredAttendeesOfCapacity"), eventTitle, registeredAttendees, eventCapacity);
        }
        else
        {
            eventBreadcrumbsText = String.Format(GetString("Events_Edit.RegisteredAttendeesNoLimit"), eventTitle, registeredAttendees);
        }

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = eventBreadcrumbsText
        });
    }


    protected void tabControlElem_clicked(object sender, EventArgs e)
    {
        int selectedTab = tabControlElem.SelectedTab;
        if (selectedTab == 1)
        {
            attendeesList.Visible = false;
            pnlSendEmail.Visible = true;
            emailSender.ReloadData(true);
        }
        else
        {
            attendeesList.Visible = true;
            pnlSendEmail.Visible = false;
            attendeesList.Reset();
        }
    }

    #endregion
}