using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Base.Web.UI.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.Newsletters;
using CMS.Newsletters.Filters;
using CMS.Newsletters.Internal;
using CMS.Newsletters.Issues.Widgets.Configuration;
using CMS.Newsletters.Web.UI;
using CMS.PortalEngine;
using CMS.PortalEngine.Internal;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, EmailBuilderHelper.EMAIL_BUILDER_UI_ELEMENT)]
public partial class CMSModules_Newsletters_EmailBuilder_Email_Builder : CMSNewsletterPage
{
    private const string REFRESH_PARENT_SCRIPT = @"
var currentParentUrl = window.parent.location.href.replace(/&saved=1/g, '');
window.parent.location.href = currentParentUrl + '&saved=1'";

    private const string ATTACHMENTS_ACTION_CLASS = "attachments-header-action";
    private bool? mEditEnabled;
    private bool mIsFullRefresh;
    private IssueInfo mIssueInfo;


    /// <summary>
    /// Indicates whether editing is allowed.
    /// </summary>
    private bool EditEnabled
    {
        get
        {
            if (!mEditEnabled.HasValue)
            {
                mEditEnabled = EmailBuilderHelper.IsIssueEditableByUser(Issue, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);
            }

            return mEditEnabled.Value;
        }
    }


    private IssueInfo Issue => mIssueInfo ?? (mIssueInfo = EditedObject as IssueInfo);


    private bool IsInAutomation => QueryHelper.GetBoolean("isinautomation", false);


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!Issue.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(Issue.TypeInfo.ModuleName, "Read");
        }

        if (IsDialog && PageTitle != null)
        {
            PageTitle.TitleText = HTMLHelper.HTMLEncode(Issue.IssueDisplayName);
        }

        // Init right panel tabs
        InitWidgetsTab();
        InitEmailPropertiesTab();
        InitAbTestingTab();

        if (!RequestHelper.IsAsyncPostback())
        {
            string moduleId = "CMS.Newsletter/emailbuilder";
            var zonesConfiguration = Service.Resolve<IZonesConfigurationServiceFactory>().Create(Issue);
            var localizationProvider = Service.Resolve<IClientLocalizationProvider>();
            var newsletter = NewsletterInfo.Provider.Get(Issue.IssueNewsletterID);
            var filter = new EmailBuilderContentFilter(Issue, newsletter);

            ScriptHelper.RegisterModule(this, "CMS/RegisterClientLocalization", localizationProvider.GetClientLocalization(moduleId));
            ScriptHelper.RegisterModule(this, "CMS.Newsletter/EmailBuilder", new
            {
                issueId = Issue.IssueID,
                enableWidgetManipulation = EditEnabled,
                emailContent = zonesConfiguration.GetEmailContent(filter)
            });

            ScriptHelper.HideVerticalTabs(this);
            ScriptHelper.RefreshTabHeader(Page, Issue.Generalized.ObjectDisplayName);

            InitHeaderActions();
        }

        SetClientApplicationData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!mIsFullRefresh)
        {
            // Set builder iframe source
            builderIframe.Src = URLHelper.ResolveUrl($"~/CMSPages/Newsletters/GetEmailBuilderContent.ashx?issueid={Issue.IssueID}");

            InitVariantDropDownList();
        }
    }


    /// <summary>
    /// Handler for email properties save action.
    /// </summary>
    private void RightPanelSave(object sender, EmailBuilderEventArgs eventArgs)
    {
        if (eventArgs.FullRefresh)
        {
            mIsFullRefresh = true;

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "RefreshParent", ScriptHelper.GetScript(REFRESH_PARENT_SCRIPT));
            return;
        }

        var issueNavigationUrl = EmailBuilderHelper.GetNavigationUrl(eventArgs.NewsletterID, eventArgs.IssueID, IsInAutomation, eventArgs.SelectedTabIndex, eventArgs.SelectedTabIndex == 1);

        if (IsDialog)
        {
            issueNavigationUrl = URLHelper.PropagateUrlParameters(issueNavigationUrl, "dialog", "returnhandler");
            issueNavigationUrl = ApplicationUrlHelper.AppendDialogHash(issueNavigationUrl);
        }

        URLHelper.Redirect(issueNavigationUrl);
    }


    private void InitWidgetsTab()
    {
        rptEmailWidgets.DataSource = GetAvailableWidgets(Issue.IssueTemplateID);
        rptEmailWidgets.DataBind();

        alNoRecords.Visible = !rptEmailWidgets.HasData();
    }


    private void InitEmailPropertiesTab()
    {
        // Initialize email properties control
        emailProperties.Enabled = EditEnabled;
        emailProperties.IssueID = Issue.IssueID;
        emailProperties.NewsletterID = Issue.IssueNewsletterID;
        emailProperties.Save += RightPanelSave;
    }


    private void InitAbTestingTab()
    {
        if (Issue.IssueIsABTest && (Issue.IssueVariantOfIssueID <= 0))
        {
            // Parent issue of A/B tests cannot be modified in Email builder => redirect to the original variant
            string originalVariantUrl = EmailBuilderHelper.GetOriginalVariantIssueUrl(Issue, IsInAutomation);
            URLHelper.Redirect(originalVariantUrl);
        }

        if (!NewsletterHelper.IsABTestingAvailable())
        {
            lnkAbTesting.Visible = false;
            plcAbTestingTab.Visible = false;
        }

        // Initialize A/B testing control
        abVariants.Issue = Issue;
        abVariants.Enabled = EditEnabled;
        abVariants.Save += RightPanelSave;
    }


    /// <summary>
    /// Initializes the variants drop-down list or hides it if the issue has no variants.
    /// </summary>
    private void InitVariantDropDownList()
    {
        if (!NewsletterHelper.IsABTestingAvailable())
        {
            return;
        }

        var variants = IssueHelper.GetIssueVariants(Issue)
                                  .Select(v => new ListItem(v.IssueVariantName, v.IssueID.ToString())).ToList();
        if (variants.Any())
        {
            foreach (var variant in variants)
            {
                drpVariantsSelector.Items.Add(variant);
            }

            drpVariantsSelector.SelectedValue = Issue.IssueID.ToString();
            pnlTabs.AddCssClass("with-variants");
            plcVariantSelection.Visible = true;
        }
    }


    private void InitHeaderActions()
    {
        ScriptHelper.RegisterDialogScript(Page);
        RegisterAttachmentsCountUpdateModule();

        InitPreviewButton();

        InitSendDraftButton();

        InitAttachmentsButton();

        InitPlainTextButton();
    }


    private void InitPlainTextButton()
    {
        headerActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("newsletterissue.plaintext"),
            Tooltip = GetString("newsletterissue.plaintext"),
            OnClientClick = $"if (modalDialog) {{modalDialog('{ApplicationUrlHelper.GetElementDialogUrl(ModuleName.NEWSLETTER, "Newsletter.Issue.PlainText", Issue.IssueID)}', 'PlainText', '95%', '760');}} return false;",
            ButtonStyle = ButtonStyle.Default
        });
    }


    private void InitAttachmentsButton()
    {
        headerActions.ActionsList.Add(new HeaderAction
        {
            Text = GetAttachmentButtonText(),
            Tooltip = GetString("general.attachments"),
            OnClientClick = CreateAttachmentDialogScript(),
            Enabled = true,
            CssClass = ATTACHMENTS_ACTION_CLASS,
            ButtonStyle = ButtonStyle.Default
        });
    }


    private void InitPreviewButton()
    {
        headerActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.preview"),
            Tooltip = GetString("general.preview"),
            Target = "_blank",
            RedirectUrl = GetPreviewLink(),
            ButtonStyle = ButtonStyle.Primary
        });
    }


    private string GetPreviewLink()
    {
        var linkProvider = Service.Resolve<IUILinkProvider>();
        return linkProvider.GetSingleObjectLink(ModuleName.NEWSLETTER, "Newsletter.Issue.Preview", new ObjectDetailLinkParameters
        {
            ObjectIdentifier = Issue.IssueID,
            Persistent = true
        });
    }


    private void InitSendDraftButton()
    {
        headerActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("newsletterissue_content.senddraft"),
            Tooltip = GetString("newsletterissue_content.senddraft"),
            OnClientClick = $"if (modalDialog) {{modalDialog('{ResolveUrl("~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_SendDraft.aspx")}?objectid={Issue.IssueID}', 'SendDraft', '700', '300');}}" + " return false;",
            Enabled = true,
            ButtonStyle = ButtonStyle.Default
        });
    }


    private void SetClientApplicationData()
    {
        var emailBuilderData = new Dictionary<string, object>();
        emailBuilderData.Add("widgetPropertiesIframeUrl", URLHelper.ResolveUrl($"~/CMSModules/Newsletters/EmailBuilder/EmailWidgetProperties.aspx?issueid={Issue.IssueID}"));
        emailBuilderData.Add("builderIframeId", $"{builderIframe.ClientID}");
        emailBuilderData.Add("variantSelectorId", $"{drpVariantsSelector.ClientID}");

        RequestContext.ClientApplication.Add("emailBuilder", emailBuilderData);
    }


    private static IEnumerable<EmailWidgetInfo> GetAvailableWidgets(int emailTemplateId)
    {
        return CacheHelper.Cache(cs =>
        {
            var result = EmailWidgetInfo.Provider.Get()
                                                .OnSite(SiteContext.CurrentSiteID)
                                                .WhereIn("EmailWidgetID", EmailWidgetTemplateInfo.Provider.Get()
                                                                                                         .Column("EmailWidgetID")
                                                                                                         .WhereEquals("TemplateID", emailTemplateId))
                                                .Columns("EmailWidgetDisplayName", "EmailWidgetDescription", "EmailWidgetGuid", "EmailWidgetThumbnailGUID", "EmailWidgetIconCssClass")
                                                .OrderBy("EmailWidgetDisplayName")
                                                .TypedResult;

            cs.CacheDependency = CacheHelper.GetCacheDependency(EmailWidgetInfo.OBJECT_TYPE + "|all");

            return result;
        }, new CacheSettings(10, "EmailBuilder", "WidgetList", emailTemplateId));
    }


    protected string GetWidgetTooltip(string displayName, string description)
    {
        string tooltip = ResHelper.LocalizeString(displayName);

        if (!String.IsNullOrEmpty(description))
        {
            tooltip += "\n  -----\n" + description;
        }

        return tooltip;
    }


    private void RegisterAttachmentsCountUpdateModule()
    {
        ScriptHelper.RegisterModule(this, "CMS/AttachmentsCountUpdater", new
        {
            Selector = "." + ATTACHMENTS_ACTION_CLASS,
            Text = GetString("general.attachments")
        });
    }


    private string GetAttachmentButtonText()
    {
        var attachmentsCount = GetAttachmentsCount();

        return GetString("general.attachments") + (attachmentsCount > 0 ? " (" + attachmentsCount + ")" : string.Empty);
    }


    private string CreateAttachmentDialogScript()
    {
        var metaFileDialogUrl = GetMetaFileDialogUrl();

        return $"if (modalDialog) {{modalDialog('{metaFileDialogUrl}', 'Attachments', '700', '500');}} return false;";
    }


    private string GetMetaFileDialogUrl()
    {
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string objectType = (Issue.IssueIsVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE);
        string query = $"?objectid={Issue.IssueID}&objecttype={objectType}&allowpaste=false&allowedit={EditEnabled}&hideobjectmenu=true";
        metaFileDialogUrl += $"{query}&category={ObjectAttachmentsCategories.ISSUE}&hash={QueryHelper.GetHash(query)}";
        return metaFileDialogUrl;
    }


    private int GetAttachmentsCount()
    {
        var metafiles = MetaFileInfoProvider.GetMetaFiles(
            Issue.IssueID,
            Issue.IssueIsVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE,
            ObjectAttachmentsCategories.ISSUE,
            null, null,
            "MetafileID",
            -1);

        return metafiles.Items.Count;
    }
}
