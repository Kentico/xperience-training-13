using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Newsletters;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_NewsletterUnsubscriptionFromAll : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.NEWSLETTER_UNSUBSCRIBING_FROM_ALL))
        {
            return false;
        }

        // Get issue subject
        int issueId = ai.ActivityItemID;
        var issueInfo = IssueInfo.Provider.Get(issueId);
        if (issueInfo != null)
        {
            // Get newsletter name
            var newsletterInfo = NewsletterInfo.Provider.Get(issueInfo.IssueNewsletterID);
            if (newsletterInfo != null)
            {
                ucDetails.AddRow("om.activitydetails.newsletter", newsletterInfo.NewsletterDisplayName);
            }
            
            ucDetails.AddRow("om.activitydetails.newsletterissue", issueInfo.IssueDisplayName);
        }

        return ucDetails.IsDataLoaded;
    }

    #endregion
}