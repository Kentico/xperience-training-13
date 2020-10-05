using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Displays a list of issues for a specified newsletter.
/// </summary>
[ParentObject(NewsletterInfo.OBJECT_TYPE, "parentobjectid")]
[UIElement("CMS.Newsletter", "Newsletter.Issues")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_List : CMSNewsletterPage, ICallbackEventHandler
{
    private const string ZERO_PERCENT = "0%";
    private const string ZERO = "0";

    private bool mBounceMonitoringEnabled;
    private bool mTrackingEnabled;
    private NewsletterInfo mNewsletter;
    private DataSet mClickedLinksSummary;
    private DataSet mVariantIssueSummaries;
    private DataSet mVariantIssues;
    private string confirmMessage;
    private int issueIdToConfirm;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        mNewsletter = EditedObjectParent as NewsletterInfo;

        if (mNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!mNewsletter.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mNewsletter.TypeInfo.ModuleName, "Read");
        }

        mBounceMonitoringEnabled = NewsletterHelper.MonitorBouncedEmails(CurrentSiteName);
        mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();

        if (mTrackingEnabled && !mBounceMonitoringEnabled)
        {
            ShowInformation(GetString("newsletter.viewadditionalstatsmessage"));
        }

        RegisterScripts();

        InitializeUnigrid();

        InitHeaderActions();

        // Prepare data for listing
        // Group clicked link records by IssueID with Columns LinkIssueID, UniqueUserClicks, VariantParent (to calculate clicked links through all variant)
        mClickedLinksSummary = ClickedLinkInfo.Provider.Get()
            .Columns(new QueryColumn("LinkIssueID"),
                     new AggregatedColumn(AggregationType.Count, "DISTINCT(ClickedLinkEmail)").As("UniqueUserClicks"))
            .Source(s => s.Join<LinkInfo>("ClickedLinkNewsletterLinkID", "LinkID"))
            .GroupBy("LinkIssueID");

        // Prepare variant summaries
        mVariantIssueSummaries = IssueInfo.Provider.Get()
            .Columns(new QueryColumn("IssueVariantOfIssueID"),
                     new AggregatedColumn(AggregationType.Sum, "IssueOpenedEmails").As("OpenedEmailsSum"))
            .WhereEquals("IssueNewsletterID", mNewsletter.NewsletterID)
            .GroupBy("IssueVariantOfIssueID")
            .Having("IssueVariantOfIssueID IS NOT NULL");

        // AB Variant issues for current newsletter
        mVariantIssues = IssueInfo.Provider.Get()
            .Columns("IssueID", "IssueVariantOfIssueID")
            .WhereEquals("IssueNewsletterID", mNewsletter.NewsletterID)
            .WhereNotNull("IssueVariantOfIssueID");

        AddBrokenEmailUrlNotifier(mNewsletter, lblUrlWarning);
    }


    private void RegisterScripts()
    {
        var script = $@"
showConfirmMessage = function(callbackResult) {{
    var result = JSON.parse(callbackResult);

    if (confirm(result.message)) {{
        window.CMS.UG_{UniGrid.ClientID}.command('delete', result.issueId);
    }}
}}";

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "EM_ConfirmIssueDeletion", ScriptHelper.GetScript(script));
        ScriptHelper.RegisterTooltip(this);
    }


    private void InitializeUnigrid()
    {
        InitDeleteActions();

        UniGrid.WhereCondition = $"IssueNewsletterID={mNewsletter.NewsletterID} AND IssueVariantOfIssueID IS NULL";
        UniGrid.ZeroRowsText = GetString("Newsletter_Issue_List.NoIssuesFound");
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    private void InitDeleteActions()
    {
        UniGrid.GridActions.Actions
            .Where(a => a.Name.Equals("delete", StringComparison.OrdinalIgnoreCase)).ToList()
            .ForEach(a => a.OnClick = $"{Page.ClientScript.GetCallbackEventReference(this, "'{0}'", "showConfirmMessage", null)}; return false;");
    }


    private void InitHeaderActions()
    {
        if (!mNewsletter.NewsletterSource.Equals(NewsletterSource.Dynamic, StringComparison.OrdinalIgnoreCase))
        {
            CurrentMaster.HeaderActions.AddAction(new HeaderAction
            {
                RedirectUrl = "Newsletter_Issue_New.aspx?parentobjectid=" + mNewsletter.NewsletterID,
                Text = GetString("Newsletter_Issue_List.NewItemCaption"),
                Tooltip = GetString("Newsletter_Issue_List.NewItemCaption"),
                Permission = "AuthorIssues",
                ResourceName = mNewsletter.TypeInfo.ModuleName
            });
        }
    }


    private void UniGrid_OnBeforeDataReload()
    {
        // Hide opened/clicked emails if tracking is not available
        UniGrid.NamedColumns["openedemails"].Visible = mTrackingEnabled;
        UniGrid.NamedColumns["openedemailsrate"].Visible = mTrackingEnabled;
        UniGrid.NamedColumns["issueclickedlinks"].Visible = mTrackingEnabled;
        UniGrid.NamedColumns["issueclickedlinksrate"].Visible = mTrackingEnabled;

        // Hide bounced emails info if monitoring disabled or tracking is not available
        UniGrid.NamedColumns["delivered"].Visible = mBounceMonitoringEnabled;
        UniGrid.NamedColumns["deliveryrate"].Visible = mBounceMonitoringEnabled;
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="sourceName">Name of the source</param>
    /// <param name="parameter">The data row</param>
    /// <returns>Formatted value to be used in the UniGrid</returns>
    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        var webControl = sender as WebControl;

        // Prepare a tooltip for the column
        string tooltipText = GetTooltipText(sourceName);

        // If the sender is from a column with a tooltip, append tooltip to the control
        if ((webControl != null) && !String.IsNullOrEmpty(tooltipText))
        {
            ScriptHelper.AppendTooltip(webControl, tooltipText, null);
        }

        switch (sourceName.ToLowerInvariant())
        {
            case "issuedisplayname":
                return GetIssueDisplayName(parameter as DataRowView);

            case "issuestatus":
                IssueStatusEnum status = EnumHelper.GetDefaultValue<IssueStatusEnum>();
                var statusID = ValidationHelper.GetInteger(parameter, -1);

                if (Enum.IsDefined(typeof(IssueStatusEnum), statusID))
                {
                    status = (IssueStatusEnum)statusID;
                }

                return IssueHelper.GetStatusFriendlyName(status, null);

            case "issuesentemails":
                var num = ValidationHelper.GetInteger(parameter, 0);
                return (num > 0) ? num.ToString() : String.Empty;

            case "issueopenedemailsrate":
                return GetOpenedEmailsRate(parameter as DataRowView);

            case "issueopenedemails":
                return GetOpenedEmailsCount(parameter as DataRowView);

            case "issueclickedlinksrate":
                return GetClickRate(parameter as DataRowView);

            case "issueclickedlinks":
                return GetClickCount(parameter as DataRowView);

            case "deliveryrate":
                return GetDeliveryRate(parameter as DataRowView);

            case "delivered":
                return GetDeliveryCount(parameter as DataRowView);

            case "unsubscriberate":
                return GetUnsubscriptionRate(parameter as DataRowView);

            case "unsubscribtions":
                return GetUnsubscriptionCount(parameter as DataRowView);

            default:
                return parameter;
        }
    }


    private string GetTooltipText(string sourceName)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "issueopenedemailsrate":
                return GetString(mBounceMonitoringEnabled ? "newsletter.openratetooltip.delivered" : "newsletter.openratetooltip.sent");

            case "issueopenedemails":
                return GetString("newsletter.openstooltip");

            case "issueclickedlinksrate":
                return GetString(mBounceMonitoringEnabled ? "newsletter.clickratetooltip.delivered" : "newsletter.clickratetooltip.sent");

            case "issueclickedlinks":
                return GetString("newsletter.clickstooltip");

            case "deliveryrate":
                return GetString("newsletter.deliveryratetooltip");

            case "unsubscriberate":
                return GetString(mBounceMonitoringEnabled ? "newsletter.unsubscriptionratetooltip.delivered" : "newsletter.unsubscriptionratetooltip.sent");

            default:
                return null;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    private void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                EditIssue(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "delete":
                DeleteIssue(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }


    /// <summary>
    /// Returns a display name of an issue. A/B test icon is added to the subject if the issue is an A/B test.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    private string GetIssueDisplayName(DataRowView rowView)
    {
        var isABTest = DataHelper.GetBoolValue(rowView.Row, "IssueIsABTest");
        var isUsedForAutomation = DataHelper.GetBoolValue(rowView.Row, "IssueForAutomation");
        var rawDisplayName = DataHelper.GetStringValue(rowView.Row, "IssueDisplayName");
        var displayName = HTMLHelper.HTMLEncode(rawDisplayName);

        // Add the icon for A/B tests
        if (isABTest)
        {
            displayName += UIHelper.GetAccessibleIconTag("NodeLink icon-two-squares-line tn-13", GetString("unigrid.newsletter_issue.abtesticontooltip"));
        }

        // Add the icon for emails used in Marketing automation
        if (isUsedForAutomation)
        {
            displayName += UIHelper.GetAccessibleIconTag("NodeLink icon-process-scheme tn-13", GetString("unigrid.newsletter_issue.usedforautomationicontooltip"), FontIconSizeEnum.Standard);
        }

        return displayName;
    }


    private string GetOpenedEmailsRate(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        var openedEmails = GetOpenedEmails(rowView.Row);
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(openedEmails, delivered);
    }


    private string GetOpenedEmailsCount(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        return GetOpenedEmails(rowView.Row).ToString();
    }


    private string GetClickRate(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        if (DataHelper.DataSourceIsEmpty(mClickedLinksSummary))
        {
            return ZERO_PERCENT;
        }

        var uniqueClicks = GetClickedLinks(rowView.Row);
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(uniqueClicks, delivered);
    }


    private string GetClickCount(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        if (DataHelper.DataSourceIsEmpty(mClickedLinksSummary))
        {
            return ZERO;
        }

        return GetClickedLinks(rowView.Row).ToString();
    }


    private string GetDeliveryRate(DataRowView rowView)
    {
        var sent = GetSentEmails(rowView.Row);
        if (sent == 0)
        {
            return String.Empty;
        }
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(delivered, sent);
    }


    private string GetDeliveryCount(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        return GetDelivered(rowView.Row).ToString();
    }


    private string GetUnsubscriptionRate(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        var unsubscribed = DataHelper.GetIntValue(rowView.Row, "IssueUnsubscribed");
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(unsubscribed, delivered);
    }


    private static string FormatRate(int currentValue, int maxValue)
    {
        if ((currentValue > 0) && (maxValue > 0))
        {
            return $"{((double)currentValue / maxValue) * 100:F2}%";
        }

        return ZERO_PERCENT;
    }


    private string GetUnsubscriptionCount(DataRowView rowView)
    {
        if (GetSentEmails(rowView.Row) == 0)
        {
            return String.Empty;
        }

        return DataHelper.GetIntValue(rowView.Row, "IssueUnsubscribed").ToString();
    }


    private static int GetSentEmails(DataRow dataRow)
    {
        return DataHelper.GetIntValue(dataRow, "IssueSentEmails");
    }


    private int GetOpenedEmails(DataRow issueRow)
    {
        int issueId = DataHelper.GetIntValue(issueRow, "IssueID");
        int openedEmails = DataHelper.GetIntValue(issueRow, "IssueOpenedEmails");

        // Add winner variant data if it is an A/B test and a winner has been selected
        if (DataHelper.GetBoolValue(issueRow, "IssueIsABTest"))
        {
            var row = mVariantIssueSummaries.Tables[0].Select($"IssueVariantOfIssueID={issueId}").FirstOrDefault();
            if (row != null)
            {
                int variantOpensSum = DataHelper.GetIntValue(row, "OpenedEmailsSum");
                openedEmails += variantOpensSum;
            }
        }

        return openedEmails;
    }


    private int GetDelivered(DataRow dataRow)
    {
        var sent = GetSentEmails(dataRow);
        if (!mBounceMonitoringEnabled)
        {
            return sent;
        }
        var bounces = DataHelper.GetIntValue(dataRow, "IssueBounces");

        return sent - bounces;
    }


    private int GetClickedLinks(DataRow dataRow)
    {
        var issueId = DataHelper.GetIntValue(dataRow, "IssueID");

        // All issue ids (main issue and AB variants) to sum up click count
        var allIssueIds = new List<int>
        {
            issueId
        };
        // Get variants for current issue and add them to issue id list
        var variantIds = mVariantIssues.Tables[0].Select($"IssueVariantOfIssueID={issueId}");
        allIssueIds.AddRange(variantIds.Select(variantRow => DataHelper.GetIntValue(variantRow, "IssueID")));

        // Get clicked links summary rows for main issue and AB variant issues
        var clickedLinks = mClickedLinksSummary.Tables[0].Select($"LinkIssueID IN ({TextHelper.Join(",", allIssueIds)})");

        // Sum up unique clicks for the base issue and also the AB variants
        return clickedLinks.Sum(row => ValidationHelper.GetInteger(DataHelper.GetDataRowValue(row, "UniqueUserClicks"), 0));
    }


    private void EditIssue(int issueId)
    {
        var issue = GetIssueOrRedirect(issueId);

        var url = UIContextHelper.GetElementUrl("cms.newsletter", "EditIssueProperties", false, issueId);
        url = URLHelper.AddParameterToUrl(url, "parentobjectid", Convert.ToString(mNewsletter.NewsletterID));

        if (IssueIsSent(issue))
        {
            url = URLHelper.AddParameterToUrl(url, "tabname", "Newsletter.Issue.Reports.Overview");
        }

        URLHelper.Redirect(url);
    }


    private static bool IssueIsSent(IssueInfo issue)
    {
        return issue.IssueStatus.Equals(IssueStatusEnum.Finished);
    }


    /// <summary>
    /// Deletes an issue specified by its ID (if authorized).
    /// </summary>
    /// <param name="issueId">Issue's ID</param>
    private static void DeleteIssue(int issueId)
    {
        var issue = GetIssueOrRedirect(issueId);

        // User has to have both destroy and issue privileges to be able to delete the issue.
        if (!issue.CheckPermissions(PermissionsEnum.Delete, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            RedirectToAccessDenied(issue.TypeInfo.ModuleName, "AuthorIssues");
        }

        if (issue.IssueIsABTest)
        {
            var abTest = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
            NewsletterTasksManager.DeleteWinnerSelectionTask(abTest);
        }

        IssueInfo.Provider.Delete(issue);
    }


    private static IssueInfo GetIssueOrRedirect(int issueId)
    {
        var issue = IssueInfo.Provider.Get(issueId);

        if (issue == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        return issue;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        confirmMessage = GetString("General.ConfirmDelete");
        issueIdToConfirm = ValidationHelper.GetInteger(eventArgument, 0);

        var issue = GetIssueOrRedirect(issueIdToConfirm);

        // For issues referenced in a Marketing automation process show different confirmation message.
        if (issue.IssueForAutomation)
        {
            var processNames = NewsletterIssueAutomationHelper.GetProcessNamesWhereIssueIsUsed(issue);
            if (processNames.Any())
            {
                confirmMessage = String.Format(GetString("newsletter.issue.confirm.emailusedinmarketingautomation"), processNames.Join(Environment.NewLine));
            }
        }
    }


    public string GetCallbackResult()
    {
        var result = new
        {
            Message = confirmMessage,
            IssueId = issueIdToConfirm
        };

        return JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}
