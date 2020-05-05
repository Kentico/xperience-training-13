using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

// Set edited object
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[Title("newsletterissue_content.senddraft")]
[UIElement(ModuleName.NEWSLETTER, "Newsletter")]
[Security(Resource = ModuleName.NEWSLETTER, UIElements = "Newsletters;Newsletter;EditNewsletterProperties;Newsletter.Issues;EditIssueProperties;Newsletter.Issue.Content")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SendDraft : CMSDeskPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        var issue = EditedObject as IssueInfo;

        if (issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!issue.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(issue.TypeInfo.ModuleName, "AuthorIssues");
        }

        // Check the license
        if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty)))
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Newsletters);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", SiteContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get newsletter issue and check its existence
        IssueInfo issue = (IssueInfo)EditedObject;

        if (issue != null)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Fill draft emails box
                NewsletterInfo newsletter = NewsletterInfo.Provider.Get(issue.IssueNewsletterID);
                EditedObject = newsletter;
                txtSendDraft.Text = newsletter.NewsletterDraftEmails;
            }
        }
        else
        {
            btnSend.Enabled = false;
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Check field for emptyness
        if (string.IsNullOrEmpty(txtSendDraft.Text) || (txtSendDraft.Text.Trim().Length == 0))
        {
            ShowError(GetString("newsletter.recipientsmissing"));
        }
        // Check e-mail address validity
        else if (!ValidationHelper.AreEmails(txtSendDraft.Text.Trim()))
        {
            ShowError(GetString("newsletter.wrongemailformat"));
        }
        else
        {
            Service.Resolve<IDraftSender>().SendAsync((IssueInfo)EditedObject, txtSendDraft.Text.Trim());

            // Close the dialog
            ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "if (CloseDialog) { CloseDialog(); }", true);
        }
    }
}
