using System;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Core;

[UIElement(ModuleName.CMS, "SendE-mail")]
public partial class CMSModules_EmailQueue_SendEmail : EmailQueuePage
{
    private string siteName = null;
    private const string SEND_CMND_NAME = "send";


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register Send header action
        HeaderActions.AddAction(new HeaderAction
        {
            ButtonStyle = ButtonStyle.Primary,
            Text = GetString("general.send"),
            CommandName = SEND_CMND_NAME
        });
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Enable client side validation for emails
        txtFrom
            .EnableClientSideEmailFormatValidation(errorMessageResourceString: "System_Email.ErrorEmail")
            .RegisterCustomValidator(rfvFrom);

        txtTo
            .RegisterCustomValidator(rfvTo);

        // Get selected site name
        siteName = SiteInfoProvider.GetSiteName(QueryHelper.GetInteger("siteid", -1));

        // Initialize validators        
        rfvTo.ErrorMessage = rfvFrom.ErrorMessage = GetString("System_Email.EmptyEmail");

        // Initialize uploader
        uploader.AddButtonIconClass = "icon-paperclip";

        // Initialize the text editor
        htmlText.AutoDetectLanguage = false;
        htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlText.ToolbarSet = "SimpleEdit";
        htmlText.MediaDialogConfig.UseFullURL = true;
        htmlText.LinkDialogConfig.UseFullURL = true;
        htmlText.QuickInsertConfig.UseFullURL = true;

        // Get e-mail format from settings
        EmailFormatEnum emailFormat = EmailHelper.GetEmailFormat(siteName);
        switch (emailFormat)
        {
            case EmailFormatEnum.Html:
                // Hide plain text field
                plcPlainText.Visible = false;
                break;

            case EmailFormatEnum.PlainText:
                // Hide html text field
                plcText.Visible = false;
                break;
        }
    }


    /// <summary>
    /// Handles header actions' events.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals(SEND_CMND_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            Send();
        }
    }


    /// <summary>
    /// Handles email sending.
    /// </summary>
    protected void Send()
    {
        txtFrom.Text = txtFrom.Text.Trim();
        txtTo.Text = txtTo.Text.Trim();
        txtCc.Text = txtCc.Text.Trim();
        txtBcc.Text = txtBcc.Text.Trim();

        // Validate no required emails fields are empty
        string result = new Validator()
            .NotEmpty(txtFrom.Text, GetString("System_Email.EmptyEmail"))
            .NotEmpty(txtTo.Text, GetString("System_Email.EmptyEmail"))
            .Result;

        if (result != string.Empty)
        {
            ShowError(result);
            return;
        }

        // Validate e-mail addresses
        if (!AreEmailsValid())
        {
            ShowError(GetString("System_Email.ErrorEmail"));
            return;
        }

        // Send the e-mail
        try
        {
            SendEmail();
            ShowConfirmation(GetString("System_Email.EmailSent"));
        }
        catch (Exception ex)
        {
            string message = EventLogProvider.GetExceptionLogMessage(ex);
            ShowError(ex.Message, message, null);
        }
    }


    /// <summary>
    /// Validates emails
    /// </summary>
    /// <returns><c>True</c> if all email inputs are valid</returns>
    private bool AreEmailsValid()
    {
        return txtFrom.IsValid() && txtTo.IsValid() &&
                 txtCc.IsValid() && txtBcc.IsValid();
    }


    /// <summary>
    /// Sends the e-mail.
    /// </summary>
    protected void SendEmail()
    {
        EmailMessage msg = new EmailMessage
        {
            From = txtFrom.Text,
            Recipients = txtTo.Text,
            CcRecipients = txtCc.Text,
            BccRecipients = txtBcc.Text,
            Subject = txtSubject.Text
        };

        if (plcText.Visible)
        {
            // Resolve relative URLs (leading with ~) which may eventually occur in ResolvedValue because its getter unresolves the links contained.
            msg.Body = URLHelper.MakeLinksAbsolute(htmlText.ResolvedValue);
        }
        if (plcPlainText.Visible)
        {
            msg.PlainTextBody = txtPlainText.Text;
        }

        // Add the attachments
        HttpPostedFile[] attachments = uploader.PostedFiles;
        foreach (HttpPostedFile att in attachments)
        {
            msg.Attachments.Add(new Attachment(att.InputStream, att.FileName));
        }

        EmailSender.SendEmail(siteName, msg);
    }
}