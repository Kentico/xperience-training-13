using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "objectid")]
[Security(Resource = "CMS.EmailTemplates", Permission = "Read")]
[Title("emailtemplates.senddraft")]
public partial class CMSModules_EmailTemplates_Pages_SendDraft : CMSModalPage
{
    private const string NO_REPLY_KEY_NAME = "CMSNoreplyEmailAddress";
    private const string EMPTY_EMAIL_ERROR_KEY = "emailinput.validationerror";

    private string mNoReplyAddress;


    private EmailTemplateInfo Template 
    {
        get
        {
            return (EmailTemplateInfo)EditedObject;
        }
    }


    private string NoReplyAddress
    {
        get
        {
            return mNoReplyAddress ?? (mNoReplyAddress = SettingsKeyInfoProvider.GetValue(NO_REPLY_KEY_NAME, SiteContext.CurrentSiteName));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        plcFrom.Visible = !IsFromAddressValid();
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        if (!IsFromAddressValid())
        {
            // Form field editor is shown, needs to be validated
            if (string.IsNullOrEmpty(emlFrom.Text))
            {
                ShowError(GetString(EMPTY_EMAIL_ERROR_KEY));
                return;
            }

            if (!emlFrom.IsValid())
            {
                ShowError(emlFrom.ValidationError);
                return;
            }
        }

        if (string.IsNullOrEmpty(emlRecipients.Text))
        {
            ShowError(GetString(EMPTY_EMAIL_ERROR_KEY));
            return;
        }

        if (!emlRecipients.IsValid())
        {
            ShowError(emlRecipients.ValidationError);
            return;
        }

        var from = !string.IsNullOrEmpty(NoReplyAddress) ? NoReplyAddress : emlFrom.Text;
        var recipients = emlRecipients.Text;

        SendEmailWithTemplate(from, recipients);
        CloseDialog();
    }


    private void SendEmailWithTemplate(string from, string recipients)
    {
        var resolver = MacroResolver.GetInstance();
        resolver.Settings.KeepUnresolvedMacros = true;

        var message = new EmailMessage()
        {
            From = from,
            Recipients = recipients,
            EmailFormat = EmailFormatEnum.Both
        };

        EmailSender.SendEmailWithTemplateText(CurrentSiteName, message, Template, resolver, true);
    }


    private void CloseDialog()
    {
        ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "if (CloseDialog) { CloseDialog(); }", true);
    }


    /// <summary>
    /// Returns true when form email address can be retrieved from email template or from settings
    /// </summary>
    private bool IsFromAddressValid()
    {
        return (Template != null && ValidationHelper.IsEmail(Template.TemplateFrom)) || ValidationHelper.IsEmail(NoReplyAddress);
    }
}