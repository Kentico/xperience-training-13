using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.EventManager;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_EventManager_Controls_EventAttendees_List : CMSAdminControl
{
    #region "Variables"

    private bool mUsePostback;

    #endregion


    #region "Properties"

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
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Attendees' EventID.
    /// </summary>
    public int EventID
    {
        get;
        set;
    }


    /// <summary>
    /// Use post back instead of redirect.
    /// </summary>
    public bool UsePostback
    {
        get
        {
            return mUsePostback;
        }
        set
        {
            mUsePostback = value;
            UniGrid.DelayedReload = value;
        }
    }


    /// <summary>
    /// ID of edited attendee.
    /// </summary>
    public int SelectedAttendeeID
    {
        get;
        set;
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
            UniGrid.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Script for UniGri's edit action 
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditAttendee",
                                               ScriptHelper.GetScript("function EditAttendee(attendeeId){" +
                                                                      "location.replace('Events_Attendee_Edit.aspx?attendeeid=' + attendeeId + '&eventid=" + EventID + "'); }"));

        // Refresh parent frame header
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshHeader",
                                               ScriptHelper.GetScript("function RefreshHeader() {if (parent.frames['eventsHeader']) { " +
                                                                      "parent.frames['eventsHeader'].location.replace(parent.frames['eventsHeader'].location); }} \n"));

        //Unigrid settings
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.ZeroRowsText = GetString("Events_List.NoAttendees");
        UniGrid.HideControlForZeroRows = false;

        if (UsePostback)
        {
            UniGrid.GridName = "~/CMSModules/EventManager/Tools/Events_Attendee_List_Control.xml";
        }
        else
        {
            UniGrid.GridName = "~/CMSModules/EventManager/Tools/Events_Attendee_List.xml";
        }

        if (EventID > 0)
        {
            UniGrid.WhereCondition = "AttendeeEventNodeId = " + EventID;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ShowEventInfo();
    }


    private void ShowEventInfo()
    {
        string eventCapacity = "0";
        string eventTitle = "";
        string registeredAttendees = null;

        DataSet ds = EventProvider.GetEvent(EventID, SiteContext.CurrentSiteName, "EventCapacity, EventName, AttendeesCount");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            eventCapacity = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["EventCapacity"], 0).ToString();
            eventTitle = ValidationHelper.GetString(ds.Tables[0].Rows[0]["EventName"], "");
            registeredAttendees = ValidationHelper.GetString(ds.Tables[0].Rows[0]["AttendeesCount"], "");
        }

        string message = ValidationHelper.GetInteger(eventCapacity, 0) > 0 ? String.Format(GetString("Events_Edit.RegisteredAttendeesOfCapacity"), HTMLHelper.HTMLEncode(eventTitle), registeredAttendees, eventCapacity) : String.Format(GetString("Events_Edit.RegisteredAttendeesNoLimit"), HTMLHelper.HTMLEncode(eventTitle), registeredAttendees);
        ShowInformation(message);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        // Check 'Modify' permission (because of delete action in unigrid)
        if (!CheckPermissions("cms.eventmanager", "Modify"))
        {
            return;
        }

        switch (actionName)
        {
            case "delete":
                EventAttendeeInfoProvider.DeleteEventAttendeeInfo(ValidationHelper.GetInteger(actionArgument, 0));
                // Refresh parent frame header
                ltlScript.Text = ScriptHelper.GetScript("RefreshHeader();");
                UniGrid.ReloadData();
                ShowEventInfo();
                break;

            case "sendemail":
                // Resend invitation email
                TreeProvider mTree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNode node = mTree.SelectSingleNode(EventID);

                EventAttendeeInfo eai = EventAttendeeInfoProvider.GetEventAttendeeInfo(ValidationHelper.GetInteger(actionArgument, 0));

                if ((node != null) && (node.NodeClassName.EqualsCSafe("cms.bookingevent", true)) && (eai != null))
                {
                    EventProvider.SendInvitation(SiteContext.CurrentSiteName, node, eai, TimeZoneHelper.ServerTimeZone);

                    ShowConfirmation(GetString("eventmanager.invitationresend"));
                }
                break;

            case "edit":
                SelectedAttendeeID = ValidationHelper.GetInteger(actionArgument, 0);
                break;
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        UniGrid.WhereCondition = "AttendeeEventNodeId = " + EventID;
        UniGrid.ReloadData();
    }

    #endregion
}
