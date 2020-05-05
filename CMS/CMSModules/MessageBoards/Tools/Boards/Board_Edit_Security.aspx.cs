using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Security : CMSMessageBoardBoardsPage
{
    #region "Variables"

    protected int boardId = 0;
    protected int groupId = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get parametr from query string
        boardId = QueryHelper.GetInteger("boardid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        // Get board info and chceck whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(boardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        boardSecurity.GroupID = groupId;
        boardSecurity.Board = board;
        boardSecurity.IsLiveSite = false;
    }
}