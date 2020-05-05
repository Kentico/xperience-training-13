using System;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Displays a table of subscribers who clicked a link in a specified issue.
/// </summary>
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SubscribersClicks : CMSModalPage
{
    #region "Variables"

    private int linkId;

    // Default page size 15
    private const int PAGESIZE = 15;

    #endregion 


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
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

        var user = MembershipContext.AuthenticatedUser;

        // Check permissions for CMS Desk -> Tools -> Newsletter
        if (!user.IsAuthorizedPerUIElement("CMS.Newsletter", "Newsletter"))
        {
            RedirectToUIElementAccessDenied("CMS.Newsletter", "Newsletter");
        }

        // Check 'NewsletterRead' permission
        if (!user.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            RedirectToAccessDenied("CMS.Newsletter", "Read");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("newsletter_issue_subscribersclicks.title");
        linkId = QueryHelper.GetInteger("linkid", 0);
        if (linkId == 0)
        {
            RequestHelper.EndResponse();
        }

        LinkInfo link = LinkInfo.Provider.Get(linkId);
        EditedObject = link;

        IssueInfo issue = IssueInfo.Provider.Get(link.LinkIssueID);
        EditedObject = issue;

        // Prevent accessing issues from sites other than current site
        if (issue.IssueSiteID != SiteContext.CurrentSiteID)
        {
            RedirectToResourceNotAvailableOnSite("Issue with ID " + link.LinkIssueID);
        }

        var listingWhereCondition = new WhereCondition().WhereEquals("ClickedLinkNewsletterLinkID", linkId);

        // Link's issue is the main A/B test issue
        if (issue.IssueIsABTest && !issue.IssueIsVariant)
        {
            // Get A/B test and its winner issue ID
            ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(issue.IssueID);
            if (test != null)
            {
                // Get ID of the same link from winner issue
                var winnerLink = LinkInfo.Provider.Get()
                                                 .WhereEquals("LinkIssueID", test.TestWinnerIssueID)
                                                 .WhereEquals("LinkTarget", link.LinkTarget)
                                                 .WhereEquals("LinkDescription", link.LinkDescription)
                                                 .TopN(1)
                                                 .Column("LinkID")
                                                 .FirstOrDefault();

                if (winnerLink != null)
                {
                    if (winnerLink.LinkID > 0)
                    {
                        // Add link ID of winner issue link
                        listingWhereCondition.Or(new WhereCondition().WhereEquals("ClickedLinkNewsletterLinkID", winnerLink.LinkID));
                    }
                }
            }
        }

        UniGrid.Pager.DefaultPageSize = PAGESIZE;
        UniGrid.Pager.ShowPageSize = false;
        UniGrid.FilterLimit = 1;
        fltOpenedBy.EmailColumn = "ClickedLinkEmail";
        
        // Get click count by email
        UniGrid.DataSource = ClickedLinkInfo.Provider.Get()
            .Columns(
                new QueryColumn("ClickedLinkEmail"),
                new AggregatedColumn(AggregationType.Count, null).As("ClickCount")
            )
            .GroupBy("ClickedLinkEmail")
            .Where(listingWhereCondition)
            .And()
            .Where(fltOpenedBy.WhereCondition)
            .OrderByDescending("ClickCount")
            .Result;
    }

    #endregion
}