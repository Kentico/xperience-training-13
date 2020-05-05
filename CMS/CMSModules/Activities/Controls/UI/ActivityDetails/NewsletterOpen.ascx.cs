using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Newsletters;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_NewsletterOpen : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.NEWSLETTER_OPEN))
        {
            return false;
        }

        // Get newsletter name
        int newsletterId = ai.ActivityItemID;
        NewsletterInfo newsletterInfo = NewsletterInfo.Provider.Get(newsletterId);
        if (newsletterInfo != null)
        {
            ucDetails.AddRow("om.activitydetails.newsletter", newsletterInfo.NewsletterDisplayName);
        }

        // Get issue subject
        int issueId = ai.ActivityItemDetailID;
        IssueInfo issueInfo = IssueInfo.Provider.Get(issueId);
        if (issueInfo != null)
        {
            ucDetails.AddRow("om.activitydetails.newsletterissue", issueInfo.IssueDisplayName);
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}