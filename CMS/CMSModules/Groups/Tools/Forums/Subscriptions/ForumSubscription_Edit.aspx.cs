using System;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Subscriptions_ForumSubscription_Edit : CMSGroupForumPage
{
    protected int subscriptionId = 0;
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forums - Edit subscription";

        string currentForumSubscription = "";
        int subscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        subscriptionEdit.SubscriptionID = subscriptionId;
        subscriptionEdit.OnSaved += subscriptionEdit_OnSaved;
        subscriptionEdit.IsLiveSite = false;

        // get forumSubscription id from querystring
        if (subscriptionId > 0)
        {
            ForumSubscriptionInfo forumSubscriptionObj = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
            currentForumSubscription = HTMLHelper.HTMLEncode(forumSubscriptionObj.SubscriptionEmail);

            // Initialize breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = GetString("ForumSubscription_Edit.ItemListLink"),
                RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Forums/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumSubscriptionObj.SubscriptionForumID),
            });
            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = (string.IsNullOrEmpty(currentForumSubscription)) ? GetString("ForumSubscription_List.NewItemCaption") : currentForumSubscription,
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

            // Initialize breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = GetString("ForumSubscription_Edit.ItemListLink"),
                RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Forums/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumId),
            });
            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = (string.IsNullOrEmpty(currentForumSubscription)) ? GetString("ForumSubscription_List.NewItemCaption") : currentForumSubscription,
            });

            PageTitle.TitleText = GetString("forumsubscription_edit.newitemcaption");
        }

        subscriptionEdit.OnCheckPermissions += subscriptionEdit_OnCheckPermissions;
    }


    private void subscriptionEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(QueryHelper.GetInteger("forumid", 0));
        if (fi != null)
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
            if (fgi != null)
            {
                groupId = fgi.GroupGroupID;
            }
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void subscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        if (subscriptionEdit.SubscriptionID != 0)
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("ForumSubscription_Edit.aspx?subscriptionid=" + Convert.ToString(subscriptionEdit.SubscriptionID) + "&saved=1"));
        }
    }
}