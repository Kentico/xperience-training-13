using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_IssueLinks : CMSUserControl
{
    #region "Variables"

    // Default page size 15
    private const int PAGESIZE = 15;
    private int sentOrDeliveredEmails;
    private int issueID;
    private bool isMainABTestIssue;
    private bool isABTest;

    #endregion


    #region "Properties"

    /// <summary>
    /// Newsletter issue ID.
    /// </summary>
    public int IssueID { get; set; }


    /// <summary>
    /// Indicates whether click statistics from winner issue (variant + main issue) are included.
    /// </summary>
    public bool IncludeWinnerStatistics { get; set; }


    /// <summary>
    /// Indicates whether click statistics from all variants are displayed for the main issue.
    /// </summary>
    public bool IncludeAllVariants { get; set; }


    /// <summary>
    /// Indicates whether the filter is displayed above the listing.
    /// </summary>
    public bool ShowFilter { get; set; }


    /// <summary>
    /// Selects only N most clicked links.
    /// </summary>
    public int TopN { get; set; }


    /// <summary>
    /// Indicates whether the grid displays only clicked links (having ClickLink records).
    /// </summary>
    public bool DisplayOnlyClickedLinks { get; set; }


    /// <summary>
    /// Indicates whether the grid displays the Click rate column for AB tested issue.
    /// </summary>
    public bool AllowHidingClickRateColumnForAbTest { get; set; }


    /// <summary>
    /// Inner grid control.
    /// </summary>
    public UniGrid UniGrid => ugLinks;


    /// <summary>
    /// Text displayed when UniGrid data source is empty.
    /// </summary>
    public string NoDataText { get; set; }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        IssueInfo issue = IssueInfo.Provider.Get(IssueID);

        if (issue == null)
        {
            return;
        }

        issueID = IssueID;
        isABTest = issue.IssueIsABTest;

        // Get number of sent or delivered emails according to the "Monitor bounced email" setting
        sentOrDeliveredEmails = GetSentOrDelivered(issue);

        ScriptHelper.RegisterDialogScript(Page);

        var subscriberClicksUrl = ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_SubscribersClicks.aspx");
        string scriptBlock = $@"
            function OpenTarget(url) {{ window.open(url, 'LinkTarget'); return false; }}
            function ViewClicks(id) {{ modalDialog('{subscriberClicksUrl}?linkid=' + id, 'NewsletterIssueSubscriberClicks', '900px', '700px');  return false; }}";

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);

        
        // Issue is the main A/B test issue
        isMainABTestIssue = issue.IssueIsABTest && !issue.IssueIsVariant;
        if (isMainABTestIssue)
        {
            // Initialize variant selector in the filter
            fltLinks.IssueId = issue.IssueID;

            // Get current issue ID from variant selector
            issueID = fltLinks.IssueId;

            // Get number of sent emails for selected variant
            var issueVariant = IssueInfo.Provider.Get(issueID);
            sentOrDeliveredEmails = CalculateVariantSentEmails(issue, issueVariant);

            // Set the visibility of Click rate column
            ugLinks.OnBeforeDataReload += SetClickRateColumnVisibility;
            ugLinks.OnAfterDataReload += SetClickRateColumnVisibility;

        }

        ugLinks.Pager.DefaultPageSize = PAGESIZE;
        ugLinks.Pager.ShowPageSize = false;
        ugLinks.FilterLimit = 1;
        ugLinks.OnExternalDataBound += UniGrid_OnExternalDataBound;
        ugLinks.Columns = "LinkID, LinkIssueID, LinkTarget, LinkDescription";

        ugLinks.OnDataReload += GetLinks;
        
        fltLinks.Visible = ShowFilter;
        ugLinks.ZeroRowsText = NoDataText;
    }


    protected DataSet GetLinks(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        // Default where condition
        WhereCondition whereIssues = new WhereCondition().WhereEquals("LinkIssueID", issueID);
        isMainABTestIssue = isMainABTestIssue && (issueID == IssueID);        
        int winnerIssueId = 0;

        if (IncludeAllVariants && isABTest)
        {
            // Get all variants
            var issueIDs = IssueInfo.Provider.Get()
                                            .Column("IssueID")
                                            .WhereEquals("IssueID", IssueID)
                                            .Or()
                                            .WhereEquals("IssueVariantOfIssueID", IssueID).GetListResult<Int32>();

            // Include all links for all variants
            whereIssues = new WhereCondition().WhereIn("LinkIssueID", issueIDs);
        }
        else if (IncludeWinnerStatistics && isMainABTestIssue)
        {
            var test = ABTestInfoProvider.GetABTestInfoForIssue(issueID);

            if (test != null)
            {
                // Get winner variant issue ID
                winnerIssueId = test.TestWinnerIssueID;
            }

            if (winnerIssueId > 0)
            {
                // Include winner issue
                whereIssues = whereIssues.Or().WhereEquals("LinkIssueID", winnerIssueId);
            }
        }

        // Get links with clicks statistics for issue(s)
        var query = LinkInfo.Provider.Get()
                        .Columns(
                            new QueryColumn("LinkID"),
                            new QueryColumn("LinkIssueID"),
                            new QueryColumn("LinkTarget"),
                            new QueryColumn("LinkDescription"),

                            // Get total and unique clicks
                            new AggregatedColumn(AggregationType.Count, "DISTINCT(ClickedLinkEmail)").As("UniqueClicks"),
                            new AggregatedColumn(AggregationType.Count, "ClickedLinkEmail").As("TotalClicks")
                        )
                        .Where(whereIssues)
                        .And()
                        .Where(fltLinks.WhereCondition)
                        .Source(s => s.LeftJoin<ClickedLinkInfo>("LinkID", "ClickedLinkNewsletterLinkID"))
                        .GroupBy("LinkID", "LinkIssueID", "LinkTarget", "LinkDescription");


        if ((winnerIssueId > 0) || IncludeAllVariants)
        {
            // Aggregate same links (LinkTarget, LinkDescription) from various variants (variant + winner OR all variants)
            query = query.AsNested()
                 .Columns(
                     new QueryColumn("LinkTarget"),
                     new QueryColumn("LinkDescription"),

                     // Get ID of main issue link (grater than variant link ID)
                     new AggregatedColumn(AggregationType.Max, "LinkID").As("LinkID"),
                     // Get ID of the original issue (the original is the parent of variants so it has the lowest ID)
                     new AggregatedColumn(AggregationType.Min, "LinkIssueID").As("LinkIssueID"),

                     // Get total and unique clicks (sum variants)
                     new AggregatedColumn(AggregationType.Sum, "UniqueClicks").As("UniqueClicks"),
                     new AggregatedColumn(AggregationType.Sum, "TotalClicks").As("TotalClicks")

                 )
                 .GroupBy("LinkTarget", "LinkDescription");
                 
            if (DisplayOnlyClickedLinks)
            {
                query = query.Having(x => x.WhereGreaterThan(new AggregatedColumn(AggregationType.Sum, "UniqueClicks"), 0));
            }
        }
        else if (DisplayOnlyClickedLinks)
        {
            query = query.Having(x => x.WhereGreaterThan(new AggregatedColumn(AggregationType.Count, "DISTINCT(ClickedLinkEmail)").As("UniqueClicks"), 0));
        }

        if (TopN > 0)
        {
            query = query.TopN(TopN);
        }

        query = query.OrderByDescending("UniqueClicks");

        return query.Result;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int linkID = 0;
        DataRowView dr = parameter as DataRowView;

        if (dr != null)
        {
            linkID = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(dr, "LinkID"), 0);
        }

        switch (sourceName)
        {
            case "clickrate":
                {
                    var uniqueClicks = ValidationHelper.GetDouble(GetStatisticsValueForLink((DataRowView)parameter, "UniqueClicks"), 0);

                    if(uniqueClicks == 0)
                    {
                        return "0%";
                    }

                    var rate = (sentOrDeliveredEmails == 0) ? 0 : ((uniqueClicks / sentOrDeliveredEmails) * 100);
                    return GetClickDialogLink(linkID, $"{rate:F2}%");
                }
            case "uniqueclicks":
                {
                    var uniqueClicks = GetStatisticsValueForLink((DataRowView)parameter, "UniqueClicks");
                    if (uniqueClicks > 0)
                    {
                        return GetClickDialogLink(linkID, uniqueClicks.ToString());
                    }

                    return uniqueClicks;
                }
            case "totalclicks":
                {
                    var totalClicks = GetStatisticsValueForLink((DataRowView)parameter, "TotalClicks");
                    if (totalClicks > 0)
                    {
                        return GetClickDialogLink(linkID, totalClicks.ToString());
                    }

                    return totalClicks;
                }

            case "linktarget":
                return $@"<a href=""#"" onclick=""OpenTarget('{ScriptHelper.GetString(parameter.ToString(), false)}')"">
                        {HTMLHelper.HTMLEncode(TextHelper.LimitLength(parameter.ToString(), 50))}</a>";
            default:
                return parameter;
        }
    }

    /// <summary>
    /// Returns HTML link to link click dialog.
    /// </summary>
    /// <param name="linkID">LinkInfo ID</param>
    /// <param name="text">Link text</param>
    private string GetClickDialogLink(int linkID, string text)
    {
        // Do not show links in Most Click table for ABTestsed issue
        if (isABTest && IncludeAllVariants)
        {
            return text;
        }

        return $@"<a href=""#"" onclick=""ViewClicks({linkID})"">{text}</a>";
    }


    /// <summary>
    /// Returns statistics value for the given link.
    /// </summary>
    /// <param name="link">Newsletter link DataRowView</param>
    /// <param name="columnName">Column name</param>
    private int GetStatisticsValueForLink(DataRowView link, string columnName)
    {
        var value = DataHelper.GetIntValue(link.Row, columnName);
        return value;
    }


    /// <summary>
    /// In the parent issue is stored "SentEmail" amount of all variants and the remainder,
    /// so the remainder amount have to be computed by subtraction of all sent amounts
    /// from particular variants.
    /// </summary>
    /// <param name="parentIssue">Parent issue</param>
    /// <param name="variantIssue">Current variant</param>
    /// <returns></returns>
    private int CalculateVariantSentEmails(IssueInfo parentIssue, IssueInfo variantIssue)
    {
        if ((parentIssue == null) || (variantIssue == null))
        {
            return 0;
        }

        if (IncludeAllVariants)
        {
            return GetSentOrDelivered(parentIssue);
        }
        
        // If the winner has not been selected yet, or the selected variant is not the winner
        // return SentEmails of the current variant
        if (parentIssue.IssueID != variantIssue.IssueID)
        {
            return variantIssue.IssueSentEmails;
        }

        var abTest = ABTestInfoProvider.GetABTestInfoForIssue(parentIssue.IssueID);
        if (abTest == null)
        {
            return 0;
        }

        // If variantIssue equals parentIssue it means that winner variant was selected, but the
        // filter has returned the parent variant, and we need winner variant object
        // For the winner variant is returned the sum of variant SentEmail amount and the remainder SentEmail amount.
        // See the "Summary" section for the domain specification.
        var sentSumFromRemainingVariants = IssueInfo.Provider.Get()
                                           .Column(new AggregatedColumn(AggregationType.Sum, "IssueSentEmails"))
                                           .WhereEquals("IssueVariantOfIssueID", parentIssue.IssueID)
                                           .And()
                                           .WhereNotEquals("IssueID", abTest.TestWinnerIssueID)
                                           .GetScalarResult(0);

        return parentIssue.IssueSentEmails - sentSumFromRemainingVariants;
    }


    /// <summary>
    /// Returns issue's sent emails amount or delivered emails amount based on 
    /// monitor bounced email setting.
    /// </summary>
    /// <param name="issue">Issue</param>
    private int GetSentOrDelivered(IssueInfo issue)
    {
        if (NewsletterHelper.MonitorBouncedEmails(CurrentSite.SiteName))
        {
            return issue.IssueSentEmails - issue.IssueBounces;
        }

        return issue.IssueSentEmails;
    }


    /// <summary>
    /// Sets the visibility of "Click rate" column according 
    /// to control setting.
    /// </summary>
    private void SetClickRateColumnVisibility()
    {
        ugLinks.NamedColumns["clickrate"].Visible = !AllowHidingClickRateColumnForAbTest;
    }

    #endregion
}
