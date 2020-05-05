using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_CMSPages_SubscriptionApproval : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get object display name
        ForumInfo fi = ForumInfoProvider.GetForumInfo((subscriptionApproval.SubscriptionObject != null) ? subscriptionApproval.SubscriptionObject.SubscriptionForumID : 0);
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo((subscriptionApproval.SubscriptionObject != null) ? subscriptionApproval.SubscriptionObject.SubscriptionPostID : 0);
        string name = (fpi != null) ? TextHelper.LimitLength(fpi.PostSubject, 50) : (fi != null) ? fi.ForumDisplayName : null;

        // Set text to display according to subscription object
        string objectText = "forum.subscriptionconfirmation";
        if (fpi != null)
        {
            objectText = "forumpost.subscriptionconfirmation";
        }

        PageTitle title = PageTitle;
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString(objectText), (name != null) ? ScriptHelper.GetString(name) : null));
    }
}