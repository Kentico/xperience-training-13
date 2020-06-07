using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Send")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[Title("Newsletter_Issue_Header.Send")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Send : CMSNewsletterSendPage
{
    private const decimal PERCENTAGE_THRESHOLD = 0.9m;
    private const string SCHEDULE_ACTION_IDENTIFIER = "schedule";


    private bool IsIssueTemplateBased => Issue.IssueTemplateID > 0;

    private Lazy<int> MarketableRecipientsCount => new Lazy<int>(GetMarketableRecipientsCount);
    
    private Lazy<int> LicenseMaxNumberOfRecipients => new Lazy<int>(GetLicenseMaxNumberOfRecipients);


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ctrSmartTip.Visible = IssueEditingEnabled && !Issue.IssueForAutomation;

        InitSendControls();

        InitHeaderActions();

        AddBrokenEmailUrlNotifier(Newsletter, lblUrlWarning);

        DisplayMessage(GetInfoMessage(Issue.IssueStatus));
    }


    /// <summary>
    /// Sends an issue.
    /// </summary>
    protected void SendIssue()
    {
        HandleMissingPermissionsForIssueEditing();

        var errMessage = String.Empty;

        if (ctrSendIssue.Visible)
        {
            errMessage = SendDynamicIssue();
        }

        if (ctrSendTemplateBasedIssue.Visible)
        {
            errMessage = SendTemplateBasedIssue();
        }

        HandleSendActionResult(errMessage);
    }


    protected void hdrActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SUBMIT:
                SendIssue();
                break;
            case SCHEDULE_ACTION_IDENTIFIER:
                ScheduleTemplateBasedIssue();
                break;
        }
    }


    private void InitSendControls()
    {
        if (IsIssueTemplateBased)
        {
            ctrSendTemplateBasedIssue.IssueID = Issue.IssueID;
            ctrSendTemplateBasedIssue.Visible = true;
        }
        else
        {
            ctrSendIssue.IssueID = Issue.IssueID;
            ctrSendIssue.NewsletterID = Issue.IssueNewsletterID;
            ctrSendIssue.Visible = true;
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        var hdrActions = CurrentMaster.HeaderActions;
        hdrActions.ActionsList.Clear();

        var sendingIssueAllowed = Issue.IssueStatus == IssueStatusEnum.Idle || Issue.IssueStatus == IssueStatusEnum.ReadyForSending;
        if (!sendingIssueAllowed)
        {
            return;
        }

        var recipientsCountAllowed = LicenseMaxNumberOfRecipients.Value == 0 || MarketableRecipientsCount.Value <= LicenseMaxNumberOfRecipients.Value;

        if (!recipientsCountAllowed)
        {
            MessagesPlaceHolder.AddError(string.Format(GetString("newsletter.issue.send.subcriberlimiterror"), MarketableRecipientsCount.Value, LicenseMaxNumberOfRecipients.Value));
        }

        var hasWidgetWithUnfilledRequiredProperty = Issue.HasWidgetWithUnfilledRequiredProperty();
        var hasWidgetWithMissingDefinition = Issue.HasWidgetWithMissingDefinition();
        var isValidWidgetDefinition = !hasWidgetWithUnfilledRequiredProperty && !hasWidgetWithMissingDefinition;

        if (!isValidWidgetDefinition)
        {
            MessagesPlaceHolder.AddError(GetString("newsletter.issue.send.widgeterror"));
        }

        if (IsIssueTemplateBased)
        {
            AddTemplateBasedHeaderActions(hdrActions, isValidWidgetDefinition && recipientsCountAllowed);
        }
        else
        {
            AddSendHeaderAction(hdrActions, isValidWidgetDefinition && recipientsCountAllowed, ButtonStyle.Primary);
        }

        hdrActions.ActionPerformed += hdrActions_ActionPerformed;
        hdrActions.ReloadData();

        CurrentMaster.DisplayActionsPanel = true;
    }


    /// <summary>
    /// Adds template-based header actions to <paramref name="hdrActions"/>.
    /// </summary>
    private static void AddTemplateBasedHeaderActions(HeaderActions hdrActions, bool enabled)
    {
        hdrActions.ActionsList.Add(new HeaderAction
        {
            CommandName = SCHEDULE_ACTION_IDENTIFIER,
            Text = GetString("newsletterissue_send.saveschedule"),
            Tooltip = GetString("newsletterissue_send.saveschedule"),
            Enabled = enabled
        });

        hdrActions.ActionsList.Add(new HeaderAction
        {
            CommandName = ComponentEvents.SUBMIT,
            Text = GetString("newsletterissue_send.sendnowbutton"),
            Tooltip = GetString("newsletterissue_send.sendnowbutton"),
            Enabled = enabled,
            OnClientClick = "return confirm('" + GetString("newsletterissue_send.confirmationdialog") + "');",
            ButtonStyle = ButtonStyle.Default
        });
    }


    /// <summary>
    /// Schedules a template-based issue.
    /// </summary>
    /// <remarks>It also checks permissions and handles possible error messages.</remarks>
    /// <returns>Error message on failure.</returns>
    private void ScheduleTemplateBasedIssue()
    {
        HandleMissingPermissionsForIssueEditing();

        string errorMessage = String.Empty;

        if (!ctrSendTemplateBasedIssue.SendScheduled())
        {
            errorMessage = ctrSendTemplateBasedIssue.ErrorMessage;
        }

        HandleSendActionResult(errorMessage);
    }


    /// <summary>
    /// Sends a template-based issue.
    /// </summary>
    /// <returns>Error message on failure.</returns>
    private string SendTemplateBasedIssue()
    {
        if (!ctrSendTemplateBasedIssue.SendNow())
        {
            return ctrSendTemplateBasedIssue.ErrorMessage;
        }

        return String.Empty;
    }


    /// <summary>
    /// Sends a dynamic issue.
    /// </summary>
    /// <returns>Error message on failure.</returns>
    private string SendDynamicIssue()
    {
        if (!ctrSendIssue.SendIssue())
        {
            return ctrSendIssue.ErrorMessage;
        }

        if (Newsletter != null)
        {
            // Redirect to the issue list page
            var issueListPageUrl = ResolveUrl($"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_List.aspx?newsletterid={Newsletter.NewsletterID}");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "Newsletter_Issue_Send", $"parent.location='{issueListPageUrl}';", true);
        }

        return String.Empty;
    }


    /// <summary>
    /// Gets info message depending on issue status.
    /// </summary>
    /// <param name="issueStatus">Issue status.</param>
    /// <returns>Info message.</returns>
    private string GetInfoMessage(IssueStatusEnum issueStatus)
    {
        switch (issueStatus)
        {
            case IssueStatusEnum.Finished:
                return GetString("Newsletter_Issue_Header.AlreadySent");

            case IssueStatusEnum.ReadyForSending:
                return AppendReachingLimitMessage(GetString("Newsletter_Issue_Header.AlreadyScheduled"));

            case IssueStatusEnum.Idle:
                return AppendReachingLimitMessage(GetString("Newsletter_Issue_Header.NotSentYet"));
        }

        return GetString("Newsletter_Issue_Header.NotSentYet");
    }


    private string AppendReachingLimitMessage(string message)
    {
        if (IsMarketableRecipientsCountNearLicenseLimitation(MarketableRecipientsCount.Value, LicenseMaxNumberOfRecipients.Value))
        {
            return $"{message} {string.Format(GetString("Newsletter_Issue_Header.LicenseLimitationWarning"), MarketableRecipientsCount.Value, LicenseMaxNumberOfRecipients.Value)}";
        }

        return message;
    }


    private static bool IsMarketableRecipientsCountNearLicenseLimitation(int marketableRecipients, int maxNumberOfRecipients)
    {
        return maxNumberOfRecipients != 0  && marketableRecipients >= maxNumberOfRecipients * PERCENTAGE_THRESHOLD && marketableRecipients <= maxNumberOfRecipients;
    }


    private int GetLicenseMaxNumberOfRecipients()
    {
        var site = SiteInfo.Provider.Get(Issue.IssueSiteID);
        
        return LicenseKeyInfoProvider.VersionLimitations(site.DomainName, FeatureEnum.SimpleContactManagement, false);
    }


    private int GetMarketableRecipientsCount()
    {
        return Issue
            .GetRecipientsProvider()
            .GetMarketableRecipients()
            .Count;
    }
}
