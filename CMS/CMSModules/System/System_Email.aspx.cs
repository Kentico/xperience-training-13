using System;
using System.Net.Mail;

using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_System_System_Email : GlobalAdminPage
{
    private EmailServerAuthenticationType SelectedAuthenticationType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<EmailServerAuthenticationType>(ValidationHelper.GetString(rbAuthenticationType.Value, ""));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Enable client side validation for emails
        txtFrom
            .EnableClientSideEmailFormatValidation(errorMessageResourceString: "System_Email.ErrorEmail")
            .RegisterCustomValidator(rfvFrom);

        txtTo
            .EnableClientSideEmailFormatValidation(errorMessageResourceString: "System_Email.ErrorEmail")
            .RegisterCustomValidator(rfvTo);

        // Initialize required field validators
        rfvServer.ErrorMessage = GetString("System_Email.ErrorServer");
        rfvFrom.ErrorMessage = rfvTo.ErrorMessage = GetString("System_Email.EmptyEmail");


        rbAuthenticationType.CurrentSelector.AutoPostBack = true;
        rbAuthenticationType.CurrentSelector.SelectedIndexChanged += rbAuthenticationType_SelectedIndexChanged;

        if (!RequestHelper.IsPostBack())
        {
            // Fill SMTP fields with the default server data
            if (SiteContext.CurrentSite != null)
            {
                string siteName = SiteContext.CurrentSiteName;
                txtServer.Text = EmailHelper.Settings.ServerName(siteName);
                txtUserName.Text = EmailHelper.Settings.ServerUserName(siteName);
                txtPassword.Text = EmailHelper.Settings.ServerPassword(siteName);
                txtPassword.Attributes.Add("value", txtPassword.Text);

                rbAuthenticationType.Value = EmailHelper.Settings.ServerAuthenticationType(siteName);
                selOAuthCredentials.Value = EmailHelper.Settings.ServerOAuthCredentials(siteName);
            }
            else
            {
                rbAuthenticationType.Value = EmailServerAuthenticationType.Basic;
            }

            LoadVisibleCredentialsForm();
        }
    }


    /// <summary>
    /// Handles the Click event of the btnSend control.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
    protected void btnSend_Click(object sender, EventArgs e)
    {
        txtFrom.Text = txtFrom.Text.Trim();
        txtTo.Text = txtTo.Text.Trim();
        txtServer.Text = txtServer.Text.Trim();

        string result = null;
        
        if (SelectedAuthenticationType == EmailServerAuthenticationType.Basic)
        {
            result = new Validator()
            .NotEmpty(txtServer.Text, GetString("System_Email.ErrorServer"))
            .NotEmpty(txtFrom.Text, GetString("System_Email.EmptyEmail"))
            .NotEmpty(txtTo.Text, GetString("System_Email.EmptyEmail"))
            .Result;
        }
        else if (SelectedAuthenticationType == EmailServerAuthenticationType.OAuth)
        {
            result = new Validator()
                .NotEmpty(selOAuthCredentials.Value, GetString("System_Email.EmptyCredentials"))
                .Result;
        }

        if (!string.IsNullOrEmpty(result))
        {
            ShowError(result);
            return;
        }

        // Validate e-mail addresses
        if (!(txtFrom.IsValid() && txtTo.IsValid()))
        {
            ShowError(GetString("System_Email.ErrorEmail"));
            return;
        }

        // Send the testing e-mail
        try
        {
            SendEmail();
            ShowInformation(GetString("System_Email.EmailSent"));
        }
        catch (Exception ex)
        {
            string message = EventLogProvider.GetExceptionLogMessage(ex);
            ShowError(ex.Message, message, null);
        }
    }


    /// <summary>
    /// Sends a test e-mail message.
    /// </summary>
    protected void SendEmail()
    {
        // Initialize message
        EmailMessage email = new EmailMessage
                                 {
                                     From = txtFrom.Text,
                                     Recipients = txtTo.Text,
                                     Subject = TextHelper.LimitLength(txtSubject.Text.Trim(), 450),
                                     EmailFormat = EmailFormatEnum.Html,
                                     Body = txtText.Text
                                 };

        // Attach file if something was uploaded
        if ((FileUploader.PostedFile != null) && (FileUploader.PostedFile.InputStream != null))
        {
            email.Attachments.Add(new Attachment(FileUploader.PostedFile.InputStream,
                                                 Path.GetFileName(FileUploader.PostedFile.FileName)));
        }

        // Initialize SMTP server object
        SMTPServerInfo smtpServer = new SMTPServerInfo
                                        {
                                            ServerName = txtServer.Text,
                                            ServerUserName = txtUserName.Text,
#pragma warning disable 618
                                            ServerPassword = EncryptionHelper.EncryptData(txtPassword.Text),
#pragma warning restore 618
                                            ServerUseSSL = false,
                                            ServerAuthenticationType = SelectedAuthenticationType,
                                            ServerOAuthCredentials = ValidationHelper.GetGuid(selOAuthCredentials.Value, Guid.Empty)
        };

        string siteName = SiteContext.CurrentSiteName ?? string.Empty;

        EmailSender.SendTestEmail(siteName, email, smtpServer);
    }


    protected void rbAuthenticationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadVisibleCredentialsForm();
    }


    private void LoadVisibleCredentialsForm()
    {
        var selectedValue = SelectedAuthenticationType;

        plcOAuth.Visible = selectedValue == EmailServerAuthenticationType.OAuth;
        plcBasic.Visible = selectedValue == EmailServerAuthenticationType.Basic;
    }
}