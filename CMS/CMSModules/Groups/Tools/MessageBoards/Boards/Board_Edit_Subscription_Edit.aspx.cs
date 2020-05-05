using System;

using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Subscription_Edit : CMSGroupMessageBoardsPage
{
    #region "Variables"

    private int mSubscriptionId = 0;
    private int boardId = 0;
    private int groupId = 0;
    private BoardSubscriptionInfo mCurrentSubscription = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControl();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
        // Get current subscription ID
        mSubscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        mCurrentSubscription = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(mSubscriptionId);

        // Get current board and group ID
        boardId = QueryHelper.GetInteger("boardid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        BoardInfo boardObj = BoardInfoProvider.GetBoardInfo(boardId);
        if (boardObj != null)
        {
            // Check whether edited board belongs to group
            if ((boardObj.BoardGroupID == 0) || (groupId != boardObj.BoardGroupID))
            {
                EditedObject = null;
            }
        }

        boardSubscription.IsLiveSite = false;
        boardSubscription.BoardID = boardId;
        boardSubscription.GroupID = groupId;
        boardSubscription.SubscriptionID = mSubscriptionId;
        boardSubscription.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(boardSubscription_OnCheckPermissions);
        boardSubscription.OnSaved += new EventHandler(boardSubscription_OnSaved);

        InitializeBreadcrumbs();
    }


    private void boardSubscription_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("~/CMSModules/Groups/Tools/MessageBoards/Boards/Board_Edit_Subscription_Edit.aspx?subscriptionid=" + boardSubscription.SubscriptionID + "&boardid=" + boardId + "&saved=1&groupid=" + groupId);
    }


    private void boardSubscription_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// Initializes the breadcrumbs on the page.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("board.subscription.subscriptions"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/MessageBoards/Boards/Board_Edit_Subscriptions.aspx?boardid=" + boardId + "&groupid=" + groupId),
            Target = "_self",
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = mCurrentSubscription != null ? HTMLHelper.HTMLEncode(mCurrentSubscription.SubscriptionEmail) : GetString("board.subscriptions.newitem"),
        });       
    }

    #endregion
}