using System;
using System.Data;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventManager;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using DocTreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_EventManager_Controls_EventAttendeesSendEmail : CMSAdminControl
{
    #region "Variables"

    HeaderAction btnSend;

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
    /// Event ID.
    /// </summary>
    public int EventID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        htmlEmail.AutoDetectLanguage = false;
        htmlEmail.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEmail.EditorAreaCSS = String.Empty;
        htmlEmail.ResolverName = "BookingResolver";
    }


    public override void ReloadData(bool forceLoad)
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        DocTreeNode nd = tree.SelectSingleNode(EventID, LocalizationContext.PreferredCultureCode, tree.CombineWithDefaultCulture, false);
        if (nd == null)
        {
            ShowInformation(GetString("editedobject.notexists"));
            plcSend.Visible = false;
            lblTitle.Visible = false;
            return;
        }

        //Enable controls
        txtSenderName.Enabled = true;
        txtSenderEmail.Enabled = true;
        txtSubject.Enabled = true;
        htmlEmail.Enabled = true;
        btnSend.Enabled = true;

        if (forceLoad)
        {
            string siteName = SiteContext.CurrentSiteName;
            txtSenderEmail.Text = SettingsKeyInfoProvider.GetValue(siteName + ".CMSEventManagerInvitationFrom");
            txtSenderName.Text = SettingsKeyInfoProvider.GetValue(siteName + ".CMSEventManagerSenderName");
            txtSubject.Text = SettingsKeyInfoProvider.GetValue(siteName + ".CMSEventManagerInvitationSubject");
        }

        // Disable form if no attendees present or user doesn't have modify permission
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.eventmanager", "Modify"))
        {
            DataSet ds = EventAttendeeInfoProvider.GetEventAttendees(EventID)
                                                    .Column("AttendeeID")
                                                    .TopN(1);

            if (DataHelper.DataSourceIsEmpty(ds))
            {
                DisableForm();
                lblInfo.Text = GetString("Events_List.NoAttendees");
                lblInfo.Visible = true;
            }
        }
        else
        {
            DisableForm();
            ShowWarning(GetString("events_sendemail.modifypermission"), null, null);
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        HeaderActions.ActionsList.Clear();

        HeaderActions.ActionsList.Add(btnSend = new HeaderAction
        {
            CommandName = ComponentEvents.SUBMIT,
            Text = GetString("general.send"),
            Tooltip = GetString("general.send"),
            Enabled = true
        });

        HeaderActions.ActionPerformed += new CommandEventHandler(hdrActions_ActionPerformed);
        HeaderActions.ReloadData();
    }


    protected void hdrActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SUBMIT:
                Send();
                break;
        }
    }


    /// <summary>
    /// Disable form.
    /// </summary>
    private void DisableForm()
    {
        txtSenderName.Enabled = false;
        txtSenderEmail.Enabled = false;
        txtSubject.Enabled = false;
        htmlEmail.Enabled = false;
        btnSend.Enabled = false;
    }


    /// <summary>
    /// Sends e-mail to all attendees.
    /// </summary>
    protected void Send()
    {
        // Check 'Modify' permission
        if (!CheckPermissions("cms.eventmanager", "Modify"))
        {
            return;
        }

        txtSenderName.Text = txtSenderName.Text.Trim();
        txtSenderEmail.Text = txtSenderEmail.Text.Trim();
        txtSubject.Text = txtSubject.Text.Trim();

        // Validate the fields
        string errorMessage = new Validator()
            .NotEmpty(txtSenderName.Text, GetString("Events_SendEmail.EmptySenderName"))
            .NotEmpty(txtSenderEmail.Text, GetString("Events_SendEmail.EmptySenderEmail"))
            .MatchesCondition(txtSenderEmail, input => input.IsValid(), GetString("Events_SendEmail.InvalidEmailFormat"))
            .NotEmpty(txtSubject.Text, GetString("Events_SendEmail.EmptyEmailSubject"))
            .Result;

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        string subject = txtSubject.Text;
        string emailBody = htmlEmail.ResolvedValue;

        // Get event node data
        TreeProvider mTree = new TreeProvider();
        DocTreeNode node = mTree.SelectSingleNode(EventID);

        if (node != null && CMSString.Equals(node.NodeClassName, "cms.bookingevent", true))
        {
            // Initialize macro resolver
            MacroResolver resolver = MacroResolver.GetInstance();
            resolver.Settings.KeepUnresolvedMacros = true;
            resolver.SetAnonymousSourceData(node);
            // Add named source data
            resolver.SetNamedSourceData("Event", node);

            // Event date string macro
            DateTime eventDate = ValidationHelper.GetDateTime(node.GetValue("EventDate"), DateTimeHelper.ZERO_TIME);
            DateTime eventEndDate = ValidationHelper.GetDateTime(node.GetValue("EventEndDate"), DateTimeHelper.ZERO_TIME);
            bool isAllDay = ValidationHelper.GetBoolean(node.GetValue("EventAllDay"), false);

            resolver.SetNamedSourceData("eventdatestring", EventProvider.GetEventDateString(eventDate, eventEndDate, isAllDay, TimeZoneHelper.GetTimeZoneInfo(SiteContext.CurrentSite), SiteContext.CurrentSiteName), false);

            // Resolve e-mail body and subject macros and make links absolute
            emailBody = resolver.ResolveMacros(emailBody);
            emailBody = URLHelper.MakeLinksAbsolute(emailBody);
            subject = TextHelper.LimitLength(resolver.ResolveMacros(subject), 450);

            // EventSendEmail manages sending e-mails to all attendees
            EventSendEmail ese = new EventSendEmail(EventID, SiteContext.CurrentSiteName,
                                                    subject, emailBody, txtSenderName.Text.Trim(), txtSenderEmail.Text.Trim());

            ShowConfirmation(GetString("Events_SendEmail.EmailSent"));
        }
    }

    #endregion
}