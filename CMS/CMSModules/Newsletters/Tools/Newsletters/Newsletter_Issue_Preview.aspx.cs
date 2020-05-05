using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Newsletters.Internal;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;

using MessageTypeEnum = CMS.Base.Web.UI.MessageTypeEnum;

// Set edited object
[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Preview")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Preview : CMSNewsletterPage
{
    private const int MAX_PREVIEW_SUBSCRIBERS = 20;


    private readonly List<string> subjects = new List<string>();
    private readonly List<string> preheaders = new List<string>();
    private readonly List<string> emails = new List<string>();


    protected void Page_Init(object sender, EventArgs e)
    {
        EnsurePermissions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var issue = (IssueInfo)UIContext.EditedObject;
        if (issue == null || issue.IssueSiteID != SiteContext.CurrentSiteID)
        {
            ShowErrorMessage();
            return;
        }

        var newsletter = NewsletterInfo.Provider.Get(issue.IssueNewsletterID);

        InitializeSubscribers(issue, newsletter);

        SetupPreview(issue, newsletter);

        RegisterJavascript(issue);
    }


    private void EnsurePermissions()
    {
        var newsletterIssue = EditedObject as IssueInfo;
        if (newsletterIssue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!newsletterIssue.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(newsletterIssue.TypeInfo.ModuleName, "Read");
        }
    }


    private void ShowErrorMessage()
    {
        plcEmailPreview.Visible = false;
        plcEmailSource.Visible = false;
        CurrentMaster.HeaderActionsPlaceHolder.Visible = false;
        AddMessage(MessageTypeEnum.Error, GetString("newsletter.issue.preview.noobject"));
    }


    private void SetupPreview(IssueInfo issue, NewsletterInfo newsletter)
    {
        SetLabelValues(issue, newsletter);

        SetContentPanelCss();

        SetUpTitle();

        SetUpCodeMirror();

        CurrentMaster.DisplaySiteSelectorPanel = true;

        InitPreheaderTooltip(issue);
    }


    private void InitPreheaderTooltip(IssueInfo issue)
    {
        var template = EmailTemplateInfo.Provider.Get(issue.IssueTemplateID);
        if (!template.ContainsPreheaderMacro() && !string.IsNullOrEmpty(issue.IssuePreheader))
        {
            lblPreheaderAlert.Visible = iconPreheaderAlert.Visible = true;
            lblPreheaderAlert.Text = iconPreheaderAlert.ToolTip = GetString("newsletterissue.preheader.tooltip");
        }
    }


    private void SetLabelValues(IssueInfo issue, NewsletterInfo newsletter)
    {
        var senderRetriever = new SenderRetriever(issue, newsletter);

        lblFromValue.Text = HTMLHelper.HTMLEncode(senderRetriever.GetSenderName());
        lblFromEmailValue.Text = senderRetriever.GetSenderEmail();
    }


    private void SetContentPanelCss()
    {
        CurrentMaster.PanelContent.RemoveCssClass("PageContent");
        CurrentMaster.PanelContent.AddCssClass("preview-content");
    }


    private void SetUpTitle()
    {
        string caption = GetString("newsletter.issue.preview");

        ScriptHelper.RegisterRequireJs(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "BreadcrumbsOverwriting", ScriptHelper.GetScript($@"
        cmsrequire(['CMS/EventHub', 'jQuery'], function(hub, $) {{
              hub.publish('OverwriteBreadcrumbs', {GetBreadcrumbsData(caption)});
              window.top.document.title += $('<div/>').html(' / {caption}').text();
        }});"));
    }


    private void SetUpCodeMirror()
    {
        txtSource.Editor.Language = LanguageEnum.HTMLMixed;
        txtSource.Editor.AutoSize = true;
    }


    private string GetBreadcrumbsData(string caption)
    {
        var breadcrumbsList = new List<dynamic>
        {
            new
            {
                text = caption,
                isRoot = true
            }
        };

        return JsonConvert.SerializeObject(new { data = breadcrumbsList }, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
    }


    private void InitializeSubscribers(IssueInfo issue, NewsletterInfo newsletter)
    {
        if (newsletter.NewsletterType == EmailCommunicationTypeEnum.Newsletter)
        {
            InitializeSubscribersForNewsletter(issue, newsletter);
        }
        else
        {
            InitializeSubscribersForCampaign(issue, newsletter);
        }
    }


    private void RegisterJavascript(IssueInfo issue)
    {
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        RegisterEmailPreviewModule(issue);
    }


    private void InitializeSubscribersForCampaign(IssueInfo issue, NewsletterInfo newsletter)
    {
        var recipients = issue.GetRecipientsProvider()
                              .GetMarketableRecipients()
                              .TopN(MAX_PREVIEW_SUBSCRIBERS)
                              .ToList();

        if (recipients.Count > 0)
        {
            for (int index = 0; index < recipients.Count; index++)
            {
                var recipient = recipients[index];
                var subscriber = recipient.ToContactSubscriber();
                var emailViewer = new EmailViewer(issue, newsletter, subscriber, true);

                emails.Add(HttpUtility.UrlEncode(recipient.ContactEmail));
                subjects.Add(GetEncodedSubscriberSubject(emailViewer));
                preheaders.Add(GetEncodedSubscriberPreheader(emailViewer));
                drpSubscribers.Items.Add(new ListItem(recipient.ContactEmail, index.ToString()));
            }
        }
        else
        {
            DisableSubscribersDropDownList();

            var emailViewer = new EmailViewer(issue, newsletter, null, true);

            InitializeZeroSubscribers(emailViewer.GetSubject(), emailViewer.GetPreheader());
        }
    }

    private void DisableSubscribersDropDownList()
    {
        drpSubscribers.Items.Add(new ListItem(GetString("newsletter.issue.preview.nosubscribers")));
        drpSubscribers.Enabled = false;
    }


    private void InitializeSubscribersForNewsletter(IssueInfo issue, NewsletterInfo newsletter)
    {
        var subscribers = SubscriberInfo.Provider
            .Get()
            .TopN(MAX_PREVIEW_SUBSCRIBERS)
            .WhereIn("SubscriberID", new IDQuery(SubscriberNewsletterInfo.OBJECT_TYPE, "SubscriberID")
                .WhereEquals("NewsletterID", issue.IssueNewsletterID)
                .Where(w => w.WhereTrue("SubscriptionApproved")
                             .Or()
                             .WhereNull("SubscriptionApproved")));

        int subscriberIndex = 0;
        foreach (var subscriber in subscribers)
        {
            var member = SubscriberInfoProvider.GetSubscribers(subscriber, 1).FirstOrDefault();
            if (member == null)
            {
                continue;
            }

            var emailViewer = new EmailViewer(issue, newsletter, member, true);

            var identifier = GetSubscriberIdentifier(member, subscriber.SubscriberType);
            var subject = GetEncodedSubscriberSubject(emailViewer);
            var preheader = GetEncodedSubscriberPreheader(emailViewer);

            subjects.Add(subject);
            preheaders.Add(preheader);
            emails.Add(HttpUtility.UrlEncode(member.SubscriberEmail));

            drpSubscribers.Items.Add(new ListItem(identifier, subscriberIndex.ToString()));
            subscriberIndex++;
        }

        if (!SubscriberExists())
        {
            DisableSubscribersDropDownList();

            var emailViewer = new EmailViewer(issue, newsletter, null, true);

            InitializeZeroSubscribers(emailViewer.GetSubject(), emailViewer.GetPreheader());
        }
    }


    private bool SubscriberExists()
    {
        return drpSubscribers.Items.Count > 0;
    }


    private string GetEncodedSubscriberSubject(EmailViewer emailViewer)
    {
        var subject = emailViewer.GetSubject();
        return HTMLHelper.HTMLEncode(subject);
    }


    private string GetEncodedSubscriberPreheader(EmailViewer emailViewer)
    {
        var preheader = emailViewer.GetPreheader();
        return HTMLHelper.HTMLEncode(preheader);
    }


    private string GetSubscriberIdentifier(SubscriberInfo subscriber, string originalType)
    {
        var suffix = GetString(originalType == PredefinedObjectType.CONTACT ? "objecttype.om_contact" : "objecttype.om_contactgroup").ToLowerInvariant();
        return $"{subscriber.SubscriberEmail} ({suffix})";
    }


    private void InitializeZeroSubscribers(string subject, string preheader)
    {
        subjects.Add(HTMLHelper.HTMLEncode(subject));
        preheaders.Add(HTMLHelper.HTMLEncode(preheader));
        emails.Add("0");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetApproximationAlert();
    }


    private void SetApproximationAlert()
    {
        lblApproximationAlert.Text = iconApproximationAlert.ToolTip = GetString("newsletter.issue.preview.approximationalert");
    }


    private void RegisterEmailPreviewModule(IssueInfo issue)
    {
        ScriptHelper.RegisterModule(this, "CMS.Newsletter/EmailPreview", new
        {
            Text = GetString("general.attachments"),
            lblSubjectId = lblSubjectValue.ClientID,
            lblPreheaderId = lblPreheaderValue.ClientID,
            dropDownListId = drpSubscribers.ClientID,
            subjects = this.subjects,
            preheaders = this.preheaders,
            emails = this.emails,
            url = URLHelper.ResolveUrl($"~/CMSPages/Newsletters/GetEmailPreviewContent.ashx?issueId={issue.IssueID}")
        });
    }
}