using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Tools_Subscriptions_ForumSubscription_Edit : CMSForumsPage
{
    protected int subscriptionId = 0;
    protected int forumId = 0;
    protected bool isNewItem = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forums - Edit subscription";

        subscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        subscriptionEdit.SubscriptionID = subscriptionId;
        subscriptionEdit.OnSaved += new EventHandler(subscriptionEdit_OnSaved);
        subscriptionEdit.IsLiveSite = false;

        string currentForumSubscription = "";
        ForumSubscriptionInfo forumSubscriptionObj = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);

        // get forumSubscription id from querystring
        if (forumSubscriptionObj != null)
        {
            currentForumSubscription = HTMLHelper.HTMLEncode(forumSubscriptionObj.SubscriptionEmail);

            // Initialize breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("ForumSubscription_Edit.ItemListLink"),
                RedirectUrl = ResolveUrl("~/CMSModules/Forums/Tools/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumSubscriptionObj.SubscriptionForumID),
            });
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = currentForumSubscription,
            });
        }
        else
        {
            forumId = QueryHelper.GetInteger("forumid", 0);
            if (forumId == 0)
            {
                return;
            }
            subscriptionEdit.ForumID = forumId;
            isNewItem = true;

            // Initialize breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("ForumSubscription_Edit.ItemListLink"),
                RedirectUrl = ResolveUrl("~/CMSModules/Forums/Tools/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumId),
            });
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("ForumSubscription_Edit.NewItemCaption"),
            });
        }

        ForumContext.CheckSite(0, forumId, 0);
    }


    protected void subscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        if (isNewItem)
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("ForumSubscription_Edit.aspx?subscriptionid=" + subscriptionEdit.SubscriptionID + "&saved=1&checked=" + subscriptionEdit.SendEmailConfirmation.ToString()));
        }
    }
}