using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EventManager;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_EventManager_Controls_EventAttendees : CMSAdminControl
{
    #region "Variables"

    private int mEventID = 0;
    protected int attendeeId = 0;
    protected EventAttendeeInfo eai = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Attendees' EventID.
    /// </summary>
    public int EventID
    {
        get
        {
            return mEventID;
        }
        set
        {
            mEventID = value;
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
            attendeesList.StopProcessing = value;
            attendeeEdit.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            actionsElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lnkBackHidden.Click += lnkBackHidden_Click;

        HeaderAction action = new HeaderAction();
        action.Text = GetString("events_attendee_edit.newitemcaption");
        action.CommandName = "new_attendee";
        action.RedirectUrl = null;
        actionsElem.AddAction(action);

        // Load edit attendee id from hidden field
        if (!String.IsNullOrEmpty(hdnState.Value))
        {
            attendeeEdit.EventID = EventID;
            attendeeEdit.AttendeeID = ValidationHelper.GetInteger(hdnState.Value, 0);
        }

        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        attendeesList.UsePostback = true;
        attendeesList.EventID = EventID;
        attendeeEdit.OnCheckPermissions += attendeeEdit_OnCheckPermissions;
        attendeesList.OnCheckPermissions += attendeeEdit_OnCheckPermissions;
    }


    /// <summary>
    /// Breadcrumbs clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        hdnState.Value = String.Empty;
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void attendeeEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    /// <summary>
    /// Creates breadcrumbs.
    /// </summary>
    public void CreateBreadCrumbs()
    {
        // Initialize breadcrumbs
        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { 
            Text = GetString("events_attendee_list.general"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        EventAttendeeInfo eai = EventAttendeeInfoProvider.GetEventAttendeeInfo(ValidationHelper.GetInteger(hdnState.Value, 0));
        
        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = (eai == null) ? GetString("events_attendee_edit.newitemcaption") : eai.AttendeeEmail,
        });
    }


    /// <summary>
    /// New attendee click handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "new_attendee":
                hdnState.Value = "0";
                break;
        }
    }


    /// <summary>
    /// Rest info about attendee selection.
    /// </summary>
    public void Reset()
    {
        hdnState.Value = String.Empty;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (attendeeEdit.NewItemID != 0)
        {
            hdnState.Value = attendeeEdit.NewItemID.ToString();
        }

        if (attendeesList.SelectedAttendeeID != 0)
        {
            hdnState.Value = attendeesList.SelectedAttendeeID.ToString();
        }

        if (String.IsNullOrEmpty(hdnState.Value))
        {
            pnlEdit.Visible = false;
            pnlList.Visible = true;
            attendeesList.EventID = EventID;
            attendeesList.ReloadData();
        }
        else
        {
            attendeesList.StopProcessing = true;
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            attendeeEdit.AttendeeID = ValidationHelper.GetInteger(hdnState.Value, 0);
            attendeeEdit.EventID = EventID;
            attendeeEdit.LoadEditData();
            CreateBreadCrumbs();
        }
    }

    #endregion
}