using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventManager;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_EventManager_Controls_EventAttendees_Edit : CMSAdminControl
{
    #region "Variables"

    protected int mAttendeeID = 0;
    private bool error;
    EventAttendeeInfo eai;
    private bool redirectAfterInsert = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Attendees' EventID.
    /// </summary>
    public int EventID
    {
        get;
        set;
    }


    /// <summary>
    /// Saved.
    /// </summary>
    public bool Saved
    {
        get;
        set;
    }


    /// <summary>
    /// Attendee ID.
    /// </summary>
    public int AttendeeID
    {
        get
        {
            return mAttendeeID;
        }
        set
        {
            mAttendeeID = value;
        }
    }


    /// <summary>
    /// After new item is created this stores its ID.
    /// </summary>
    public int NewItemID
    {
        get;
        set;
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
    /// Indicates whether the control should redirect after creating a new attendee
    /// or just refresh breadcrumbs.
    /// </summary>
    public bool RedirectAfterInsert
    {
        get
        {
            return redirectAfterInsert;
        }
        set
        {
            redirectAfterInsert = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFirstName.Text = GetString("Event_Attendee_Edit.lblFirstName");
        lblLastName.Text = GetString("Event_Attendee_Edit.lblLastName");
        lblPhone.Text = GetString("Event_Attendee_Edit.lblPhone");

        rfvEmail.Text = GetString("Event_Attendee_Edit.rfvEmail");

        txtEmail.RegisterCustomValidator(rfvEmail);

        // Load current attendee
        eai = AttendeeID > 0 ? EventAttendeeInfoProvider.GetEventAttendeeInfo(AttendeeID) : new EventAttendeeInfo();
        EditedObject = eai;

        if (Saved)
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Checks if current event (EventID) exist and hides control content if not.
    /// Returns true if event exist, false otherwise.
    /// </summary>
    private bool CheckIfEventExists()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode nd = tree.SelectSingleNode(EventID, LocalizationContext.PreferredCultureCode, tree.CombineWithDefaultCulture, false);
        if (nd == null)
        {
            ShowError(GetString("editedobject.notexists"));
            pnlContent.Visible = false;
            return false;
        }
        return true;
    }


    /// <summary>
    /// Loads data for edit form.
    /// </summary>
    public void LoadEditData()
    {
        // Check if current event exist
        if (!CheckIfEventExists())
        {
            return;
        }

        if (error)
        {
            return;
        }

        if ((eai != null) && (eai.AttendeeID != AttendeeID))
        {
            // Reload current attendee
            eai = AttendeeID > 0 ? EventAttendeeInfoProvider.GetEventAttendeeInfo(AttendeeID) : new EventAttendeeInfo();
            EditedObject = eai;
        }

        if ((eai != null) && (eai.AttendeeID > 0))
        {
            txtFirstName.Text = eai.AttendeeFirstName;
            txtLastName.Text = eai.AttendeeLastName;
            txtEmail.Text = eai.AttendeeEmail;
            txtPhone.Text = eai.AttendeePhone;

            // Show warning if duplicity email was used
            bool isDuplicit = EventAttendeeInfoProvider.GetEventAttendees(eai.AttendeeEventNodeID)
                                                       .And().Where("AttendeeEmail", QueryOperator.Equals, eai.AttendeeEmail)
                                                       .And().Where("AttendeeID", QueryOperator.NotEquals, eai.AttendeeID).TopN(1).Any();
            if (isDuplicit)
            {
                ShowWarning(GetString("eventmanager.attendeeregisteredwarning"));
            }

            if (!RequestHelper.IsPostBack())
            {
                if (Saved)
                {
                    ShowChangesSaved();
                }
            }
        }
        else
        {
            txtFirstName.Text = String.Empty;
            txtLastName.Text = String.Empty;
            txtEmail.Text = String.Empty;
            txtPhone.Text = String.Empty;
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check if current event exist
        if (!CheckIfEventExists())
        {
            return;
        }

        // Check 'Modify' permission        
        if (!CheckPermissions("cms.eventmanager", "Modify"))
        {
            return;
        }

        txtEmail.Text = txtEmail.Text.Trim();

        // Validate fields
        string errorMessage = new Validator()
            .NotEmpty(txtEmail.Text, rfvEmail.Text)
            .MatchesCondition(txtEmail, input => input.IsValid(), GetString("Event_Attendee_Edit.IncorectEmailFormat"))
            .Result;

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        // Indicates new attendee
        bool isNew = false;

        if (AttendeeID <= 0)
        {
            eai.AttendeeEventNodeID = EventID;
            isNew = true;
        }
        else
        {
            eai = EventAttendeeInfoProvider.GetEventAttendeeInfo(AttendeeID);
        }

        if (eai != null)
        {
            eai.AttendeeFirstName = txtFirstName.Text;
            eai.AttendeeLastName = txtLastName.Text;
            eai.AttendeeEmail = txtEmail.Text;
            eai.AttendeePhone = txtPhone.Text;
            EventAttendeeInfoProvider.SetEventAttendeeInfo(eai);

            // If new item store new attendeeID .. used in post back situations
            if (isNew)
            {
                NewItemID = eai.AttendeeID;
                if (RedirectAfterInsert)
                {
                    string redirectTo = "~/CMSModules/EventManager/Tools/Events_Attendee_Edit.aspx";
                    redirectTo = URLHelper.AddParameterToUrl(redirectTo, "eventId", EventID.ToString());
                    redirectTo = URLHelper.AddParameterToUrl(redirectTo, "attendeeId", NewItemID.ToString());
                    redirectTo = URLHelper.AddParameterToUrl(redirectTo, "saved", "1");
                    URLHelper.Redirect(redirectTo);
                }
                else
                {
                    RefreshBreadCrumbs();
                    ShowChangesSaved();
                }
            }
            else
            {
                RefreshBreadCrumbs();
                ShowChangesSaved();
            }
        }
        else
        {
            ShowError(GetString("general.invalidid"));
        }
    }



    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }


    private void RefreshBreadCrumbs()
    {
        CMSPage cmsPage = Page as CMSPage;
        if ((cmsPage != null) && (cmsPage.CurrentMaster != null) && (cmsPage.PageTitle != null))
        {
            var breadcrumbs = cmsPage.PageBreadcrumbs.Items;
            if ((breadcrumbs != null) && breadcrumbs.Any())
            {
                breadcrumbs[breadcrumbs.Count - 1].Text = txtEmail.Text.Trim();
            }
        }
    }

    #endregion
}