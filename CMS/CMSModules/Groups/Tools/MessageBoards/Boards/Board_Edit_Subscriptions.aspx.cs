using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Subscriptions : CMSGroupMessageBoardsPage
{
    #region "Variables"

    private int boardId = 0;
    private int groupId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current board ID
        boardId = QueryHelper.GetInteger("boardid", 0);

        BoardInfo boardObj = BoardInfoProvider.GetBoardInfo(boardId);
        if (boardObj != null)
        {
            groupId = boardObj.BoardGroupID;

            // Check whether edited board belongs to group
            if (groupId == 0)
            {
                EditedObject = null;
            }
        }

        boardSubscriptions.BoardID = boardId;
        boardSubscriptions.GroupID = groupId;
        boardSubscriptions.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(boardSubscriptions_OnCheckPermissions);
        boardSubscriptions.OnAction += new CommandEventHandler(boardSubscriptions_OnAction);

        // Initialize the master page
        InitializeMasterPage();
    }

    #endregion


    #region "Events"

    private void boardSubscriptions_OnAction(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerCSafe() == "edit")
        {
            // Redirect to edit page with subscription ID specified
            URLHelper.Redirect(UrlResolver.ResolveUrl("Board_Edit_Subscription_Edit.aspx?subscriptionid=" + e.CommandArgument.ToString() + "&boardid=" + boardId +
                               ((groupId > 0) ? "&groupid=" + groupId : "")));
        }
    }


    private void boardSubscriptions_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        BoardInfo bi = BoardInfoProvider.GetBoardInfo(boardId);
        if (bi != null)
        {
            groupId = bi.BoardGroupID;

            // Check whether edited board belongs to any group
            if (groupId == 0)
            {
                EditedObject = null;
            }
        }

        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        HeaderAction action = new HeaderAction();
        action.Text = GetString("board.subscriptions.newitem");
        action.RedirectUrl = "~/CMSModules/Groups/Tools/MessageBoards/Boards/Board_Edit_Subscription_Edit.aspx?boardid=" + boardId.ToString() + "&groupid=" + groupId;
        CurrentMaster.HeaderActions.AddAction(action);
    }

    #endregion
}