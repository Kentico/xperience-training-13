using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using MessageTypeEnum = CMS.Base.Web.UI.MessageTypeEnum;


[Security(ModuleName.NEWSLETTER, "AuthorIssues", "Newsletters;Newsletter;EditNewsletterProperties;Newsletter.Issues;NewIssue")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New : CMSNewsletterPage
{
    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMessages;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (IsDialog)
        {
            PageTitle.TitleText = GetString("newsletter_issue_list.newitemcaption");
        }

        InitializeHeaderActions();
        InitializeMarketingAutomationUsage();
        InitializeTemplateSelector();
        InitializeTooltips();
    }


    private void InitializeTooltips()
    {
        pnlIssueDisplayName.ToolTip = lblDisplayName.ToolTip = GetString("newsletterissue.displayname.description");
        pnlIssueForAutomation.ToolTip = lblIssueForAutomation.ToolTip = GetString("newsletterissue.issueforautomation.description");
    }


    private void InitializeHeaderActions()
    {
        CurrentMaster.HeaderActions.AddAction(
            new SaveAction
            {
                Text = GetString("general.create")
            });

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    private void InitializeMarketingAutomationUsage()
    {
        var license = LicenseKeyInfoProvider.GetLicenseKeyInfo(RequestContext.CurrentDomain);
        if (license.Edition != ProductEditionEnum.EnterpriseMarketingSolution)
        {
            pnlIssueForAutomation.Visible = false;
            chbIssueForAutomation.Checked = false;

            return;
        }

        if (QueryHelper.GetBoolean("isinautomation", false))
        {
            pnlIssueForAutomation.Visible = false;
            chbIssueForAutomation.Checked = true;
        }
    }


    private void InitializeTemplateSelector()
    {
        InitializeTemplateSelectorDataSource();
    }


    private void InitializeTemplateSelectorDataSource()
    {
        var templates = LoadAvailableEmailTemplates();

        ucTemplateSelector.DataSource = templates;

        EnsureFirstTemplateSelection(templates.FirstOrDefault());

        ucTemplateSelector.DataBind();
    }


    private void EnsureFirstTemplateSelection(EmailTemplateInfo template)
    {
        if (ucTemplateSelector.SelectedId.Equals(0))
        {
            ucTemplateSelector.SelectedId = template?.TemplateID ?? 0;
        }
    }


    private static ObjectQuery<EmailTemplateInfo> LoadAvailableEmailTemplates()
    {
        return EmailTemplateInfo.Provider.Get()
                                        .Columns("TemplateID", "TemplateDisplayName", "TemplateDescription", "TemplateThumbnailGUID", "TemplateIconClass")
                                        .WhereEquals("TemplateType", EmailTemplateTypeEnum.Issue.ToStringRepresentation())
                                        .Where(GetAssignedTemplatesWhere());
    }


    private static WhereCondition GetAssignedTemplatesWhere()
    {
        var newsletterId = GetNewsletterId();
        var newsletterTemplates = EmailTemplateNewsletterInfo.Provider.Get()
            .Columns("TemplateID")
            .WhereEquals("NewsletterID", newsletterId);

        return new WhereCondition().WhereIn("TemplateID", newsletterTemplates);
    }


    private static int GetNewsletterId()
    {
        var newsletterId = QueryHelper.GetInteger("parentobjectid", 0);

        if (newsletterId > 0)
        {
            return newsletterId;
        }

        var newsletterGuid = QueryHelper.GetGuid("parentobjectguid", Guid.Empty);

        return NewsletterInfo.Provider.Get(newsletterGuid, SiteContext.CurrentSiteID)?.NewsletterID ?? 0;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals(ComponentEvents.SAVE, StringComparison.OrdinalIgnoreCase))
        {
            ValidateInputAndCreateNewIssue();
        }
    }


    private void ValidateInputAndCreateNewIssue()
    {
        if (!InputIsValid())
        {
            return;
        }

        var issueInfo = CreateIssueInfo();

        if (SetIssueInfo(issueInfo))
        {
            RedirectToEmailBuilder(issueInfo);
        }
    }


    private bool InputIsValid()
    {
        return IssueNameIsSpecified() && EmailTemplateIsSelected();
    }


    private bool IssueNameIsSpecified()
    {
        var isValid = !string.IsNullOrEmpty(txtDisplayName.Text);

        if (!isValid)
        {
            AddMessage(MessageTypeEnum.Error, GetString("newslettertemplateselect.error.emptyname"));
        }

        return isValid;
    }


    private bool EmailTemplateIsSelected()
    {
        var isValid = ucTemplateSelector.SelectedId > 0;

        if (!isValid)
        {
            AddMessage(MessageTypeEnum.Error, GetString("newslettertemplateselect.error.emptytemplate"));
        }

        return isValid;
    }


    private IssueInfo CreateIssueInfo()
    {
        var newsletterId = GetNewsletterId();
        var newsletter = NewsletterInfo.Provider.Get(newsletterId);

        return new IssueInfo
        {
            IssueDisplayName = txtDisplayName.Text,
            IssueSubject = txtDisplayName.Text,
            IssueNewsletterID = newsletter.NewsletterID,
            IssueSenderName = newsletter.NewsletterSenderName,
            IssueSenderEmail = newsletter.NewsletterSenderEmail,
            IssueTemplateID = ucTemplateSelector.SelectedId,
            IssueSiteID = newsletter.NewsletterSiteID,
            IssueText = string.Empty,
            IssueUseUTM = false,
            IssueForAutomation = chbIssueForAutomation.Checked
        };
    }


    private bool SetIssueInfo(IssueInfo issueInfo)
    {
        try
        {
            IssueInfo.Provider.Set(issueInfo);
            return true;
        }
        catch (Exception ex)
        {
            LogAndShowError("NEWSLETTER", "Save", ex);
            return false;
        }
    }


    private void RedirectToEmailBuilder(IssueInfo issue)
    {
        var url = GetRedirectUrl(issue);
        URLHelper.Redirect(url);
    }


    private string GetRedirectUrl(IssueInfo issue)
    {
        var url = UIContextHelper.GetElementUrl(ModuleName.NEWSLETTER, "EditIssueProperties");
        url = URLHelper.AddParameterToUrl(url, "tabname", EmailBuilderHelper.EMAIL_BUILDER_UI_ELEMENT);

        if (IsDialog)
        {
            url = URLHelper.PropagateUrlParameters(url, "dialog", "returnhandler", "returntype");
        }

        url = URLHelper.PropagateUrlParameters(url, "isinautomation");
        url = URLHelper.AddParameterToUrl(url, "objectid", issue.IssueGUID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentobjectid", issue.IssueNewsletterID.ToString());
        url = ApplicationUrlHelper.AppendDialogHash(url);

        return url;
    }
}
