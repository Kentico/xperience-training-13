using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Forums_Subscriptions_ForumSubscription_List : CMSGroupForumPage
{
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = QueryHelper.GetInteger("forumid", 0);
        subscriptionList.ForumID = forumId;
        subscriptionList.OnAction += new CommandEventHandler(subscriptionList_OnAction);
        subscriptionList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(subscriptionList_OnCheckPermissions);
        subscriptionList.IsLiveSite = false;

        InitializeMasterPage(forumId);
    }


    private void subscriptionList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(subscriptionList.ForumID);
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


    private void subscriptionList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect(UrlResolver.ResolveUrl("ForumSubscription_Edit.aspx?subscriptionId=" + Convert.ToString(e.CommandArgument) + "&forumid=" + forumId));

                break;

            case "delete":
            default:
                subscriptionList.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(int forumId)
    {
        Title = "Forums - Subscriptions List";

        // Set actions       
        HeaderAction action = new HeaderAction();
        action.Text = GetString("ForumSubscription_List.NewItemCaption");

        if (forumId > 0)
        {
            action.RedirectUrl = ResolveUrl("ForumSubscription_Edit.aspx?forumid=" + forumId.ToString());
        }

        CurrentMaster.HeaderActions.AddAction(action);
    }
}