using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Newsletters;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_NewsletterSubscription : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if (ai == null)
        {
            return false;
        }

        switch (ai.ActivityType)
        {
            case PredefinedActivityType.NEWSLETTER_SUBSCRIBING:
            case PredefinedActivityType.NEWSLETTER_UNSUBSCRIBING:
                break;
            default:
                return false;
        }

        // Get newsletter name
        NewsletterInfo newsletterInfo = NewsletterInfo.Provider.Get(ai.ActivityItemID);
        if (newsletterInfo != null)
        {
            ucDetails.AddRow("om.activitydetails.newsletter", newsletterInfo.NewsletterDisplayName);
        }

        // Get issue subject only for unsubscribing activity. Subscribing activity has reference to the subscriber in ItemDetailID.
        if (ai.ActivityType == PredefinedActivityType.NEWSLETTER_UNSUBSCRIBING)
        {
            IssueInfo issueInfo = IssueInfo.Provider.Get(ai.ActivityItemDetailID);
            if (issueInfo != null)
            {
                ucDetails.AddRow("om.activitydetails.newsletterissue", issueInfo.IssueDisplayName);
            }
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}