using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_CMSPages_Unsubscribe : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get object display name
        ForumInfo fi = ForumInfoProvider.GetForumInfo((unsubscription.SubscriptionObject != null) ? unsubscription.SubscriptionObject.SubscriptionForumID : 0);
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo((unsubscription.SubscriptionObject != null) ? unsubscription.SubscriptionObject.SubscriptionPostID : 0);
        string name = (fpi != null) ? TextHelper.LimitLength(fpi.PostSubject, 50) : (fi != null) ? fi.ForumDisplayName : null;

        // Set text to display according to subscription object
        string objectText = "forum.unsubscription";
        if (fpi != null)
        {
            objectText = "forumpost.unsubscription";
        }

        PageTitle title = PageTitle;
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString(objectText), (name != null) ? ScriptHelper.GetString(name) : null));
    }
}