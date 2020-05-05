using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Subscriptions : CMSMessageBoardBoardsPage
{
    // Current board ID
    private int mBoardId;
    private int mGroupId;
    private bool changeMaster = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current board and group ID
        mBoardId = QueryHelper.GetInteger("boardid", 0);
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        // Get board info and check whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(mBoardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        boardSubscriptions.GroupID = mGroupId;
        boardSubscriptions.Board = board;
        boardSubscriptions.ChangeMaster = changeMaster;
        boardSubscriptions.OnAction += boardSubscriptions_OnAction;

        // Initialize the master page
        InitializeMasterPage();
    }


    protected void boardSubscriptions_OnAction(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerCSafe() == "edit")
        {
            // Redirect to edit page with subscription ID specified
            URLHelper.Redirect(UrlResolver.ResolveUrl("Board_Edit_Subscription_Edit.aspx?subscriptionid=" + e.CommandArgument + "&boardid=" + mBoardId + "&changemaster=" + changeMaster
                               + ((mGroupId > 0) ? "&groupid=" + mGroupId : "")));
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        // Setup master page action element
        HeaderAction action = new HeaderAction();
        action.Text = GetString("board.subscriptions.newitem");
        action.RedirectUrl = ResolveUrl("~/CMSModules/MessageBoards/Tools/Boards/Board_Edit_Subscription_Edit.aspx?boardid=" + mBoardId.ToString() + "&changemaster=" + changeMaster);
        CurrentMaster.HeaderActions.AddAction(action);
    }

    #endregion
}