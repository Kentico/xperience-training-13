using System;
using System.Web.UI;

using CMS.Activities.Loggers;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Controls_RegistrationApproval : CMSUserControl
{
    #region "Variables"

    private bool mNotifyAdministrator;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets registered user.
    /// </summary>
    private UserInfo RegisteredUser
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets default url.
    /// </summary>
    private string DefaultUrl
    {
        get;
        set;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether administrator should be informed about new user.
    /// </summary>
    public bool NotifyAdministrator
    {
        get
        {
            return mNotifyAdministrator || QueryHelper.GetBoolean("notifyadmin", false);
        }
        set
        {
            mNotifyAdministrator = value;
        }
    }


    /// <summary>
    /// Gets or sets the administrator e-mail address.
    /// </summary>
    public string AdministratorEmail
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets waiting for approval text.
    /// </summary>
    public string WaitingForApprovalText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets email address of sender.
    /// </summary>
    public string FromAddress
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Successful Approval Text.
    /// </summary>
    public string SuccessfulApprovalText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Unsuccessful Approval text.
    /// </summary>
    public string UnsuccessfulApprovalText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ui deleted text.
    /// </summary>
    public string UserDeletedText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button text.
    /// </summary>
    public string ConfirmationButtonText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the confirmation button CSS class.
    /// </summary>
    public string ConfirmationButtonCssClass
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

    #endregion


    #region "Control events"

    /// <summary>
    /// Page Load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        Guid userGuid = QueryHelper.GetGuid("userguid", Guid.Empty);

        // If StopProcessing flag is set or userguid is empty, do nothing
        if (StopProcessing || (userGuid == Guid.Empty))
        {
            Visible = false;
            return;
        }

        // Validate hash
        if (!QueryHelper.ValidateHash("hash", "aliaspath", new HashSettings("")))
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
        }

        // Get registered user
        RegisteredUser = UserInfo.Provider.Get(userGuid);

        // Set default url
        DefaultUrl = ResolveUrl("~/");

        bool controlPb = false;

        if (RequestHelper.IsPostBack())
        {
            Control pbCtrl = ControlsHelper.GetPostBackControl(Page);
            if (pbCtrl == btnConfirm)
            {
                controlPb = true;
            }
        }

        SetupControls(!controlPb);

        if (!controlPb)
        {
            CheckUserStatus();
        }
    }


    /// <summary>
    /// Click event of btnConfirm.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        CheckUserStatus();
        ConfirmUserRegistration();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initialize controls properties.
    /// <param name="forceReload">Force reload</param>
    /// </summary>
    private void SetupControls(bool forceReload)
    {
        btnConfirm.CssClass = ConfirmationButtonCssClass;

        if (forceReload)
        {
            btnConfirm.Text = DataHelper.GetNotEmpty(ConfirmationButtonText, GetString("general.registration_confirmbutton"));
            lblInfo.Text = GetString("mem.reg.registration_confirmtext");
        }
    }


    /// <summary>
    /// Checks if user exists or if user has already been activated.
    /// </summary>
    private void CheckUserStatus()
    {
        // User was not found, probably late activation try
        if (RegisteredUser == null)
        {
            DisplayErrorMessage(UserDeletedText, GetString("mem.reg.UserDeletedText"));

            return;
        }

        // User has already been activated or is waiting for administrator approval.
        if (RegisteredUser.Enabled || 
            (RegisteredUser.UserSettings.UserActivationDate > DateTime.MinValue) ||
            RegisteredUser.UserSettings.UserWaitingForApproval)
        {
            DisplayErrorMessage(UnsuccessfulApprovalText, GetString("mem.reg.UnsuccessfulApprovalText"));
        }
    }


    /// <summary>
    /// Confirms user registration.
    /// </summary>
    private void ConfirmUserRegistration()
    {
        // Approve user registration
        ApproveRegistration();

        // Log user activity
        LogActivity();
    }


    /// <summary>
    /// Approve user registration.
    /// </summary>
    private void ApproveRegistration()
    {
        string currentSiteName = SiteContext.CurrentSiteName;
        bool administrationApproval = SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSRegistrationAdministratorApproval");

        // Administrator approve is not required, enable user
        if (!administrationApproval)
        {
            ShowInformation(DataHelper.GetNotEmpty(SuccessfulApprovalText, GetString("mem.reg.succesfullapprovaltext")));

            // Get logon link if confirmation was successful
            lblInfo.Text = String.Format(GetString("memberhsip.logonlink"), ResolveUrl(AuthenticationHelper.DEFAULT_LOGON_PAGE));
            btnConfirm.Visible = false;

            // Enable user
            RegisteredUser.UserSettings.UserActivationDate = DateTime.Now;
            RegisteredUser.Enabled = true;
        }
        // User must wait for administration approval
        else
        {
            ShowInformation(DataHelper.GetNotEmpty(WaitingForApprovalText, GetString("mem.reg.SuccessfulApprovalWaitingForAdministratorApproval")));

            // Mark for admin approval
            RegisteredUser.UserSettings.UserWaitingForApproval = true;

            // Display link to home page
            lblInfo.Text = String.Format(GetString("general.gotohomepage"), DefaultUrl);
            btnConfirm.Visible = false;
        }

        // Save changes
        UserInfo.Provider.Set(RegisteredUser);

        // Notify administrator if enabled and email confirmation is not required
        if ((!String.IsNullOrEmpty(AdministratorEmail)) && (administrationApproval || NotifyAdministrator))
        {
            SendEmailToAdministrator(administrationApproval);
        }
    }


    /// <summary>
    /// Log user activity.
    /// </summary>
    private void LogActivity()
    {
        // Create new activity for registered user
        Service.Resolve<IMembershipActivityLogger>().LogRegistration(RegisteredUser.UserName, DocumentContext.CurrentDocument, false);
    }


    /// <summary>
    /// Send e-mail to administrator about new registration.
    /// </summary>
    /// <param name="administrationApproval">Indicates if administration approval is required</param>
    private void SendEmailToAdministrator(bool administrationApproval)
    {
        MacroResolver resolver = MembershipResolvers.GetRegistrationResolver(RegisteredUser);
        string currentSiteName = SiteContext.CurrentSiteName;
        var template = EmailTemplateInfo.Provider.Get(administrationApproval ? "Registration.Approve" : "Registration.New", SiteContext.CurrentSiteID);

        if (template == null)
        {
            var logData = new EventLogData(EventTypeEnum.Error, "RegistrationForm", "GetEmailTemplate")
            {
                EventUrl = RequestContext.RawURL,
                UserName = CurrentUser.UserName,
                SiteID = SiteContext.CurrentSiteID
            };
            
            Service.Resolve<IEventLogService>().LogEvent(logData);
        }
        else
        {
            // E-mail template ok
            string from = EmailHelper.GetSender(template, (!String.IsNullOrEmpty(FromAddress)) ? FromAddress : SettingsKeyInfoProvider.GetValue(currentSiteName + ".CMSNoreplyEmailAddress"));
            if (!String.IsNullOrEmpty(from))
            {
                // Email message
                EmailMessage email = new EmailMessage();
                email.EmailFormat = EmailFormatEnum.Default;
                email.Recipients = AdministratorEmail;
                email.From = from;
                email.Subject = GetString("RegistrationForm.EmailSubject");

                try
                {
                    EmailSender.SendEmailWithTemplateText(currentSiteName, email, template, resolver, true);
                }
                catch
                {
                    Service.Resolve<IEventLogService>().LogError("Membership", "RegistrationApprovalEmail");
                }
            }
            else
            {
                Service.Resolve<IEventLogService>().LogError("RegistrationApproval", "EmailSenderNotSpecified");
            }
        }
    }


    /// <summary>
    /// Display error message, home page link and hide confirmation button.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="defaultErrorMessage">Default error message, it is displayed if error message is empty</param>
    private void DisplayErrorMessage(object errorMessage, string defaultErrorMessage)
    {
        ShowError(DataHelper.GetNotEmpty(errorMessage, defaultErrorMessage));
        lblInfo.Text = String.Format(GetString("general.gotohomepage"), DefaultUrl);
        btnConfirm.Visible = false;
    }

    #endregion
}