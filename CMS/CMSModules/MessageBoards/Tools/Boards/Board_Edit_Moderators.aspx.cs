using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Moderators : CMSMessageBoardBoardsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int boardId = QueryHelper.GetInteger("boardid", 0);

        // Get board info and check whether it belongs to current site
        BoardInfo board = BoardInfoProvider.GetBoardInfo(boardId);
        if (board != null)
        {
            CheckMessageBoardSiteID(board.BoardSiteID);
        }

        boardModerators.Board = board;
        boardModerators.IsLiveSite = false;
    }
}